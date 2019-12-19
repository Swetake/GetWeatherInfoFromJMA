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
    [LocalizedDisplayName(nameof(ResourcesActivities.GetForecastDisplayName))]
    [LocalizedDescription(nameof(ResourcesActivities.GetForecastDescription))]
    public class GetForecast : AsyncTaskCodeActivity<WeatherInfoContainer>
    {
        [LocalizedDisplayName(nameof(ResourcesActivities.GetForecastAreaDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.GetForecastAreaDescription))]
        [LocalizedCategory(nameof(ResourcesActivities.Input))]
        [RequiredArgument]
        public InArgument<Dictionary<string, List<string>>> Area { get; set; }

        [LocalizedDisplayName(nameof(ResourcesActivities.GetWarningKeyNameSeparatorDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.GetWarningKeyNameSeparatorDescription))]
        [LocalizedCategory(nameof(ResourcesActivities.Input))]
        public InArgument<string> KeyNameSeparator { get; set; }

        [LocalizedDisplayName(nameof(ResourcesActivities.GetForecastResultDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.GetForecastResultDescription))]
        [LocalizedCategory(nameof(ResourcesActivities.Output))]
        [RequiredArgument]
        public OutArgument<Dictionary<string, Dictionary<string, string>>> Result { get; set; }

        /// <inheritdoc />
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
        }
        
        protected override Task<WeatherInfoContainer> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken, Application client)
        {
            var dicArea = new Dictionary<string,List<string>>(Area.Get(context));
            var keySeparator = KeyNameSeparator.Get(context);

            if (!client.WaitForReady()) { throw new System.Exception("Timeout Exception"); }

            return client.GetForecast(dicArea, keySeparator);
        }

        protected override void OutputResult(AsyncCodeActivityContext context, WeatherInfoContainer result)
        {
            Result.Set(context, result.Result[Resources.GetForecast] as Dictionary<string, Dictionary<string, string>>);
        }
    }
}
