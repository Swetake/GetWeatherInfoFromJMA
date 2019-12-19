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
    [LocalizedDisplayName(nameof(ResourcesActivities.GetAtomFeedDisplayName))]
    [LocalizedDescription(nameof(ResourcesActivities.GetAtomFeedDescription))]
    public class GetAtomFeed : AsyncTaskCodeActivity<Dictionary<string, SyndicationFeed>>
    {
        [LocalizedDisplayName(nameof(ResourcesActivities.GetAtomFeedResultDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.GetAtomFeedResultDescription))]
        [LocalizedCategory(nameof(ResourcesActivities.Output))]
        [RequiredArgument]
        public OutArgument<Dictionary<string, SyndicationFeed>> Result { get; set; }

        /// <inheritdoc />
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
        }

        protected override Task<Dictionary<string, SyndicationFeed>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken, Application client)
        {
            if (!client.WaitForReady()) { throw new System.Exception("Timeout Exception"); }
            return client.GetAtomFeeds();
        }

        protected override void OutputResult(AsyncCodeActivityContext context, Dictionary<string, SyndicationFeed> result)
        {
            Result.Set(context, result);
        }
    }
}
