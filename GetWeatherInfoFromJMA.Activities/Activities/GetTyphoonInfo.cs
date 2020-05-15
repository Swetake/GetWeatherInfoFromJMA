using System.Activities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GetWeatherInfoFromJMA.Activities.Properties;
using GetWeatherInfoFromJMA;
using System.Xml;
using System.Xml.Linq;
using System.ServiceModel.Syndication;


namespace GetWeatherInfoFromJMA.Activities
{
    [LocalizedDisplayName(nameof(ResourcesActivities.GetTyphoonInfoDisplayName))]
    [LocalizedDescription(nameof(ResourcesActivities.GetTyphoonInfoDescription))]
    public class GetTyphoonInfo : AsyncTaskCodeActivity<WeatherInfoContainer>
    {
        [LocalizedDisplayName(nameof(ResourcesActivities.GetTyphoonInfoNumberListDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.GetTyphoonInfoNumberListDescription))]
        [LocalizedCategory(nameof(ResourcesActivities.Input))]
        public InArgument<List<int>> NumberList { get; set; }


        [LocalizedDisplayName(nameof(ResourcesActivities.GetTyphoonInfoIgnoreIdListDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.GetTyphoonInfoIgnoreIdListDescription))]
        [LocalizedCategory(nameof(ResourcesActivities.Input))]
        public InArgument<List<string>> IgnoreIdList { get; set; }


        [LocalizedDisplayName(nameof(ResourcesActivities.GetTyphoonInfoProcessedIdListDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.GetTyphoonInfoProcessedIdListDescription))]
        [LocalizedCategory(nameof(ResourcesActivities.Output))]
        public OutArgument<List<string>> ProcessedIdList { get; set; }


        [LocalizedDisplayName(nameof(ResourcesActivities.GetTyphoonInfoResultDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.GetTyphoonInfoResultDescription))]
        [LocalizedCategory(nameof(ResourcesActivities.Output))]
        public OutArgument<Dictionary<string, Dictionary<string, string>>> Result { get; set; }

        /// <inheritdoc />
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
        }

        protected override Task<WeatherInfoContainer> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken, Application client)
        {
            var numberList = NumberList.Get(context);
            var ignoreList = IgnoreIdList.Get(context);

            if (ignoreList == null) { ignoreList = new List<string>(); }
            if (numberList == null) { numberList = new List<int>(); }
            if (!client.WaitForReady()) { throw new System.Exception("Timeout Exception"); }

            return client.GetTyphoonInfo(numberList, ignoreList);
        }

        protected override void OutputResult(AsyncCodeActivityContext context, WeatherInfoContainer result)
        {
            Result.Set(context, result.Result[Resources.GetTyphoonInfo] as Dictionary<string, Dictionary<string, string>>);
            ProcessedIdList.Set(context, result.ProcessedID[Resources.GetTyphoonInfo]);
        }
    }
}
