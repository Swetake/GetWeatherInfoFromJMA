using System.Activities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GetWeatherInfoFromJMA.Activities.Properties;
using GetWeatherInfoFromJMA;
using System.Xml;
using System.Xml.Linq;

namespace GetWeatherInfoFromJMA.Activities
{
    [LocalizedDisplayName(nameof(ResourcesActivities.GetXmlDisplayName))]
    [LocalizedDescription(nameof(ResourcesActivities.GetXmlDescription))]
    public class GetXml : AsyncTaskCodeActivity<XDocument[]>
    {
        [LocalizedDisplayName(nameof(ResourcesActivities.GetXmlMeteorologicalOfficeName))]
        [LocalizedDescription(nameof(ResourcesActivities.GetXmlMeteorologicalOfficeDescription))]
        [LocalizedCategory(nameof(ResourcesActivities.Input))]
        [RequiredArgument]
        public InArgument<string> MeteorologicalOffice { get; set; }

        [LocalizedDisplayName(nameof(ResourcesActivities.GetXmlInformationNameDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.GetXmlInformationNameDescription))]
        [LocalizedCategory(nameof(ResourcesActivities.Input))]
        [RequiredArgument]
        public InArgument<string> InformationName { get; set; }

        [LocalizedDisplayName(nameof(ResourcesActivities.GetXmlAtomFeedTypeDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.GetXmlAtomFeedTypeDescription))]
        [LocalizedCategory(nameof(ResourcesActivities.Input))]
        public InArgument<GetWeatherInfoFromJMA.Enums.FeedType> AtomFeedType { get; set; }


        [LocalizedDisplayName(nameof(ResourcesActivities.GetXmlReturnAllMatchesDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.GetXmlReturnAllMatchesDescription))]
        [LocalizedCategory(nameof(ResourcesActivities.Input))]
        public InArgument<bool> ReturnAllMatches { get; set; }

        [LocalizedDisplayName(nameof(ResourcesActivities.GetXmlResultDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.GetXmlResultDescription))]
        [LocalizedCategory(nameof(ResourcesActivities.Output))]
        [RequiredArgument]
        public OutArgument<XDocument[]> Result { get; set; }

        /// <inheritdoc />
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
       }
        
        protected override Task<XDocument[]> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken, Application client)
        {

            var officename = MeteorologicalOffice.Get(context);
            var feedType = AtomFeedType.Get(context);
            var informationName = InformationName.Get(context);
            var isReturnAllMatches = ReturnAllMatches.Get(context);

            if (!client.WaitForReady()) { throw new System.Exception("Timeout Exception"); }
            return client.GetXml(officename, informationName, feedType,isReturnAllMatches);

        }
        

        protected override void OutputResult(AsyncCodeActivityContext context, XDocument[] result)
        {
            Result.Set(context, result);
        }
    }

}
