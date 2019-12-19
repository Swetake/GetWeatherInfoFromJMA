using System.Activities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GetWeatherInfoFromJMA.Activities.Properties;
using GetWeatherInfoFromJMA;

namespace GetWeatherInfoFromJMA.Activities
{
    [LocalizedDisplayName(nameof(ResourcesActivities.GetWarningDisplayName))]
    [LocalizedDescription(nameof(ResourcesActivities.GetWarningDescription))]
    public class GetWarning : AsyncTaskCodeActivity<WeatherInfoContainer>
    {
        [LocalizedDisplayName(nameof(ResourcesActivities.GetWarningAreaDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.GetWarningAreaDescription))]
        [LocalizedCategory(nameof(ResourcesActivities.Input))]
        [RequiredArgument]
        public InArgument<Dictionary<string,List<string>>> Area { get; set; }

        [LocalizedDisplayName(nameof(ResourcesActivities.GetWarningKeyNameSeparatorDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.GetWarningKeyNameSeparatorDescription))]
        [LocalizedCategory(nameof(ResourcesActivities.Input))]
        public InArgument<string> KeyNameSeparator { get; set; }


        [LocalizedDisplayName(nameof(ResourcesActivities.GetWarningIgnoreIdListDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.GetWarningIgnoreIdListDescription))]
        [LocalizedCategory(nameof(ResourcesActivities.Input))]
        public InArgument<List<string>> IgnoreIdList { get; set; }


        [LocalizedDisplayName(nameof(ResourcesActivities.GetWarningProcessedIdListDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.GetWarningProcessedIdListDescription))]
        [LocalizedCategory(nameof(ResourcesActivities.Output))]
        public OutArgument<List<string>> ProcessedIdList { get; set; }


        [LocalizedDisplayName(nameof(ResourcesActivities.GetWarningResultDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.GetWarningResultDescription))]
        [LocalizedCategory(nameof(ResourcesActivities.Output))]
        [RequiredArgument]
        public OutArgument<Dictionary<string,List<Dictionary<string,string>>>> Result { get; set; }

        /// <inheritdoc />
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);

       }
        
        protected override Task<WeatherInfoContainer> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken, Application client)
        {


            var area = new Dictionary<string, List<string>>( Area.Get(context));
            var keySeparator = KeyNameSeparator.Get(context);
            var ignoreList = IgnoreIdList.Get(context);

            if (keySeparator == null) { keySeparator = ""; }
            if (ignoreList == null) { ignoreList = new List<string>(); }

            if (!client.WaitForReady()) { throw new System.Exception("Timeout Exception"); }

            return client.GetWarnings(area, keySeparator, ignoreList);
        }
        

        protected override void OutputResult(AsyncCodeActivityContext context, WeatherInfoContainer result)
        {
            Result.Set(context, result.Result[Resources.GetWarning] as Dictionary<string, List<Dictionary<string, string>>>);
            ProcessedIdList.Set(context, result.ProcessedID[Resources.GetWarning]);
        }
    }

}
