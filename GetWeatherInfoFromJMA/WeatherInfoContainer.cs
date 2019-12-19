using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;

namespace GetWeatherInfoFromJMA
{
    public class WeatherInfoContainer
    {
        //Internal
        public Dictionary<string, SyndicationFeed> AtomFeeds { get; set; }


        //Output
        public Dictionary<string, object> Result { get; set; }
        public Dictionary<string,List<string>> ProcessedID { get; set; }

        public WeatherInfoContainer()
        {
            Result = new Dictionary<string, object>();
            ProcessedID = new Dictionary<string, List<string>>();
        }
    }
}
