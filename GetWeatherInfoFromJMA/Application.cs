using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GetWeatherInfoFromJMA
{
    /// <summary>
    /// Core class for GetWeatherInfoFromXML
    /// </summary>
    /// 
    public partial class Application : IAsyncInitialization
    {

        #region Properties
        private Dictionary<string, string> AtomFeedUrl { get; }
        public bool HasRead { get; set; }

        private WeatherInfoContainer weatherInfoContainer; 

        #endregion


        #region Constructors

        public Application() { }

        public Application(Dictionary<string, string> dicFeeds)
        {
            HasRead = false;
            weatherInfoContainer = new WeatherInfoContainer();
            AtomFeedUrl = dicFeeds;
            var task = this.Initialization;
        }

        // Allows Initialization (the step right after constructor runs) to be asynchronous
        public Task Initialization => InitializeAsync();

        // Asynchronously init
        private async Task InitializeAsync()
        {
            KeyValuePair<string, SyndicationFeed>[] listSyndicationFeed;
            List<Task<KeyValuePair<string, SyndicationFeed>>> tasks = new List<Task<KeyValuePair<string, SyndicationFeed>>>();
            foreach (string item in AtomFeedUrl.Keys)
            {
                if (!String.IsNullOrEmpty(item))
                {
                    var task = Task.Run(() => GetAtom(item, AtomFeedUrl[item]));
                    tasks.Add(task);
                }
            }
            listSyndicationFeed = await Task.WhenAll(tasks).ConfigureAwait(false);
            weatherInfoContainer.AtomFeeds = listSyndicationFeed.ToDictionary(x => x.Key, x => x.Value);
            HasRead = true;
        }
        #endregion




        #region Info Calls
        private async Task<KeyValuePair<string, SyndicationFeed>> GetAtom(string key, string URL)
        {
            XDocument xdoc = XDocument.Load(URL);
            var feed = new SyndicationFeed();
            using (var reader = xdoc.CreateReader())
            {
                feed = SyndicationFeed.Load(reader);
                feed.Items = feed.Items.OrderByDescending(r => r.PublishDate);
            }
            KeyValuePair<string, SyndicationFeed> kvFeed = new KeyValuePair<string, SyndicationFeed>(key, feed);
            xdoc = null;
            return await Task.FromResult(kvFeed);

        }

        #endregion


        #region Action Calls
        public bool WaitForReady()
        {
            int i = 0;
            while (this.HasRead == false)
            {
                Thread.Sleep(100);
                i++;
                if (i > Int32.Parse(Resources.WaitTimeSec)*10)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Get Weather Warning from XML 
        /// </summary>
        /// <param name="area">Area which you want to get weather warning</param>
        /// <param name="keySeparator">Key separtor which concatnate Meteorological office name and area name</param>
        /// <param name="ignoreIdList">These IDs will not be processed</param>
        /// <returns></returns>
        public Task<WeatherInfoContainer> GetWarnings(Dictionary<string, List<string>> area, string keySeparator, List<string> ignoreIdList)
        {
            SyndicationFeed feed = weatherInfoContainer.AtomFeeds[Resources.EXTRA];
            IEnumerable<SyndicationItem> feedItemList = feed.Items;
            List<string> processedIdList = new List<string>();
            Dictionary<string, List<Dictionary<string, String>>> result = new Dictionary<string, List<Dictionary<string, String>>>();

            foreach (SyndicationItem item in feedItemList)
            {
                string id = item.Id;
                string authorName = item.Authors.First().Name;
                if (item.Title.Text == Resources.WARNING_TITLE && area.Keys.Contains(authorName) && !ignoreIdList.Contains(id))
                {
                    string url = item.Links.First().Uri.ToString();
                    XDocument xdoc = XDocument.Load(url);

                    IEnumerable<XElement> xTopElements = xdoc.Root.Elements();

                    // Get Body element and Head element
                    #region GetBodyElementandHeadElement
                    XElement xBody = null;
                    XElement xHead = null;
                    XNamespace xns = null;
                    foreach (XElement elementItem in xTopElements)
                    {
                        xns = elementItem.Name.Namespace;
                        if (elementItem.Name == xns + "Body") { xBody = elementItem; }
                        if (elementItem.Name == xns + "Head") { xHead = elementItem; }
                    }
                    if (xBody == null || xHead == null)
                    {
                        throw new System.FormatException("Cannot find Body or Header Element(s).");
                    } 
                    #endregion


                    //Get Item Elements based on Area dictionary
                    Dictionary<string, List<XElement>> dicAreaItem = GetItemsFromArea(area, xBody, authorName);

                    //Get ReportDate
                    string strReportDateTime = GetReportDateTime(xHead);

                    //Create result Dictionary
                    foreach (string areaName in dicAreaItem.Keys)
                    {
                        XElement xElemItemArea = dicAreaItem[areaName][0];
                        xns = xElemItemArea.Name.Namespace;
                        IEnumerable<XElement> ieXElemKind = xElemItemArea.Elements(xns + "Kind");
                        var listDic = new List<Dictionary<string, string>>();

                        // Set the value of Name,Status and Addition for each Warning to dictionary list.
                        foreach (XElement xKind in ieXElemKind)
                        {
                            var dicResultItem = new Dictionary<string, string>()
                            {
                                { "ReportDateTime" , strReportDateTime }
                            };

                            if (xKind.Elements(xns + "Name").Count() > 0)
                            {
                                dicResultItem["Name"] = xKind.Element(xns + "Name").Value;
                            }

                            if (xKind.Elements(xns + "Status").Count() > 0)
                            {
                                dicResultItem["Status"] = xKind.Element(xns + "Status").Value;
                            }

                            if (xKind.Elements(xns + "Addition").Count() > 0)
                            {
                                IEnumerable<XElement> ieXadd = xKind.Element(xns + "Addition").Elements(xns + "Note");
                                string notes = "";
                                foreach (XElement xAdd in ieXadd)
                                {
                                    notes += (xAdd.Value + " ");
                                }
                                if (!String.IsNullOrEmpty(notes.Trim())) { dicResultItem["Addition"] = notes.Trim(); }
                            }
                            listDic.Add(dicResultItem);
                        }

                        result[item.Authors.First().Name + keySeparator + areaName] = listDic;
                    }
                    xdoc = null;
                    processedIdList.Add(id);
                    area.Remove(authorName);
                    if (area.Count == 0) { break; }
                }

            }
            weatherInfoContainer.Result[Resources.GetWarning] = result;
            weatherInfoContainer.ProcessedID[Resources.GetWarning] = processedIdList;

            return Task.FromResult(this.weatherInfoContainer);
        }


        /// <summary>
        /// Get Weather Forecast from XML
        /// </summary>
        /// <param name="area">Area which you want to get weather forecast</param>
        /// <param name="keySeparator">Key separtor which concatnate Meteorological office name and area name</param>
        /// <returns></returns>
        ///         public Task<Dictionary<string, Dictionary<string, string>>> GetForecast(Dictionary<string, List<string>> area, string keySeparator)
        public Task<WeatherInfoContainer> GetForecast(Dictionary<string, List<string>> area, string keySeparator)
        {
            SyndicationFeed feed = weatherInfoContainer.AtomFeeds[Resources.REGULAR];
            IEnumerable<SyndicationItem> itemList = feed.Items;
            var result = new Dictionary<string, Dictionary<string, String>>();

            // Iterate ATOM Feed Items
            foreach (SyndicationItem item in itemList)
            {
                string id = item.Id;
                string authorName = item.Authors.First().Name;
                if (item.Title.Text == Resources.FORECAST_TITLE && area.Keys.Contains(authorName))
                {
                    string url = item.Links.First().Uri.ToString();
                    XDocument xdoc = XDocument.Load(url);

                    IEnumerable<XElement> xTopElements = xdoc.Root.Elements();

                    // Get Body element and Header element
                    XElement xBody = null;
                    XElement xHead = null;
                    XNamespace xns = null;
                    foreach (XElement topElem in xTopElements)
                    {
                        xns = topElem.Name.Namespace;
                        if (topElem.Name == xns + "Body") { xBody = topElem; }
                        if (topElem.Name == xns + "Head") { xHead = topElem; }
                    }
                    if (xBody == null || xHead == null)
                    {
                        throw new System.FormatException("Cannot find Body or Header Element(s).");
                    }

                    //Get Item Element based on area dictionary
                    Dictionary<string, List<XElement>> dicAreaItem = GetItemsFromArea(area, xBody, authorName);

                    //Get ReportDate
                    string strReportDateTime = GetReportDateTime(xHead);

                    //Iterate each area
                    foreach (string areaName in dicAreaItem.Keys)
                    {
                        XElement xe = null;

                        foreach (XElement xeitem in dicAreaItem[areaName])
                        {
                            var lxns = xeitem.Name.Namespace;
                            string itemType = xeitem.Element(lxns + "Kind").Element(lxns + "Property").Element(lxns + "Type").Value;
                            if (itemType == "天気")
                            {
                                xe = xeitem;
                                break;
                            }
                        }
                        if (xe == null) { throw new FormatException("Cannot find item Tenki"); }
                        xns = xe.Name.Namespace;


                        //Get defenition of TimeDef
                        XElement xTimeDefines = (XElement)xe.PreviousNode;
                        IEnumerable<XElement> xeTimeDef = xTimeDefines.Elements(xns + "TimeDefine");
                        var dicTimeDef = new Dictionary<string, string>();
                        foreach (XElement xElemTimeDef in xeTimeDef)
                        {
                            string timeIdValue = xElemTimeDef.Attribute("timeId").Value;
                            if (timeIdValue != null)
                            {
                                dicTimeDef[timeIdValue] = xElemTimeDef.Element(xns + "Name").Value;
                            }
                        }

                        //Iterate Weather Forecast Part Elements
                        IEnumerable<XElement> ieXElemWFPart = xe.Elements(xns + "Kind").First().Descendants(xns + "WeatherForecastPart");

                        var dicContent = new Dictionary<string, string>()
                        {
                            { "ReportDateTime", strReportDateTime }
                        };
                    
                        foreach (XElement xElemWFPart in ieXElemWFPart)
                        {

                            var attr = xElemWFPart.Attribute("refID");
                            if (attr != null && dicTimeDef.ContainsKey(attr.Value))
                            {
                                XNamespace sxns = xElemWFPart.Name.Namespace;
                                dicContent[dicTimeDef[attr.Value]] = xElemWFPart.Element(sxns + "Sentence").Value;
                            }
                            else
                            {
                                throw new FormatException("Cannot find attr.");
                            }     
                        }
                        result[item.Authors.First().Name + keySeparator + areaName] = dicContent;
                    }
                    xdoc = null;
                    area.Remove(authorName);
                    if (area.Count == 0) { break; }
                }
            }
            weatherInfoContainer.Result[Resources.GetForecast]= result;
            return Task.FromResult(weatherInfoContainer);
        }

        /// <summary>
        /// Get each XML Data
        /// </summary>
        /// <param name="meteororlogicaloffice"></param>
        /// <param name="informationName"></param>
        /// <param name="feedType"></param>
        /// <param name="returnAllMatches"></param>
        /// <returns></returns>
        public async Task<XDocument[]> GetXml(string meteororlogicaloffice, string informationName, GetWeatherInfoFromJMA.Enums.FeedType feedType,bool returnAllMatches)
        {
            var siList = new List<SyndicationItem>();
            var isRestrictFeedType = true;
            var listUrl = new List<string>();
            var tasks = new List<Task<XDocument>>();

            if (feedType == Enums.FeedType.All)
            {
                isRestrictFeedType = false;
            }

            foreach (string item in weatherInfoContainer.AtomFeeds.Keys)
            {
                if (feedType.ToString() == item || isRestrictFeedType == false)
                {
                    SyndicationFeed sf = weatherInfoContainer.AtomFeeds[feedType.ToString()];
                    siList.AddRange(sf.Items);
                }
            }
            foreach (SyndicationItem item in siList)
            {
                if (item.Title.Text == informationName)
                {
                    listUrl.Add(item.Links.First().Uri.ToString());
                    if (!returnAllMatches)
                    {
                        break;
                    }
                }
            }


            foreach (string url in listUrl)
            {
                var task = Task.Run(() => XDocument.Load(url));
                tasks.Add(task);

            }
            return  await Task.WhenAll(tasks).ConfigureAwait(false);
          
            #endregion
        }

        /// <summary>
        ///  Get Atom Feed
        /// </summary>
        /// <returns></returns>
        public Task<Dictionary<string, SyndicationFeed>> GetAtomFeeds()
        {
            return Task.FromResult(this.weatherInfoContainer.AtomFeeds);
        }


        /// <summary>
        /// GetItemsFromArea
        /// </summary>
        /// <param name="area"></param>
        /// <param name="xBody"></param>
        /// <param name="authorName"></param>
        /// <returns></returns>
        private Dictionary<string, List<XElement>> GetItemsFromArea(Dictionary<string, List<string>> area,XElement xBody, string authorName)
        {

            var dicAreaItem = new Dictionary<string, List<XElement>>();

            XNamespace xns = xBody.Name.Namespace;

            IEnumerable<XElement> xAreas = xBody.Descendants(xns + "Area");
            var listXElm = new List<XElement>();
            foreach (XElement sArea in xAreas)
            {
                //Get 
                XElement xAreaName = sArea.Descendants(xns + "Name").First();
                string areaName = xAreaName.Value;

                foreach (string targetArea in area[authorName])
                {
                    if (targetArea == areaName)
                    {
                        listXElm.Add(xAreaName.Ancestors(xns + "Item").First());
                        dicAreaItem[targetArea] = listXElm;
                    }
                }
            }

            return dicAreaItem;

        }

        /// <summary>
        /// GetReportDateTime
        /// </summary>
        /// <param name="xHead"></param>
        /// <returns></returns>
        private string GetReportDateTime(XElement xHead)
        {
            XNamespace xns = xHead.Name.Namespace;
            XElement xReportDateTime = xHead.Element(xns+"ReportDateTime");
            if (xReportDateTime!=null)
            {
                return xReportDateTime.Value;
            }
            return "";
        }
    }
}
