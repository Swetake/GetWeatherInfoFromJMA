using System;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.PropertyEditing;
using System.ComponentModel;
using System.Activities.Statements;
using System.Collections.Generic;
using GetWeatherInfoFromJMA.Activities.Properties;


namespace GetWeatherInfoFromJMA.Activities
{

    [LocalizedDescription(nameof(ResourcesActivities.WeatherInfoScopeDescription))]
    [LocalizedDisplayName(nameof(ResourcesActivities.WeatherInfoScopeDisplayName))]
    public class WeatherInfoScope : NativeActivity
    {
        #region Properties

        [Browsable(false)]
        public ActivityAction<Application> Body { get; set; }

        // Category for RegularFeed
        [LocalizedCategory(nameof(ResourcesActivities.RegularFeed))]
        [LocalizedDisplayName(nameof(ResourcesActivities.WeatherInfoScopeDefaultHighFreqRegularDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.WeatherInfoScopeDefaultHighFreqRegularDescription))]
        public InArgument<Boolean> DefaultHighFreqRegular { get; set; }

        [LocalizedCategory(nameof(ResourcesActivities.RegularFeed))]
        [LocalizedDisplayName(nameof(ResourcesActivities.WeatherInfoScopeDefaultLongTermPeriodicDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.WeatherInfoScopeDefaultLongTermPeriodicDescription))]
        public InArgument<Boolean> DefaultLongTermRegular { get; set; }

        [LocalizedCategory(nameof(ResourcesActivities.RegularFeed))]
        [LocalizedDisplayName(nameof(ResourcesActivities.WeatherInfoScopeURLRegularDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.WeatherInfoScopeURLRegularDescription))]
        public InArgument<string> UrlRegular { get; set; }




        // Category for Extra
        [LocalizedCategory(nameof(ResourcesActivities.ExtraFeed))]
        [LocalizedDisplayName(nameof(ResourcesActivities.WeatherInfoScopeDefaultHighFreqExtraDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.WeatherInfoScopeDefaultHighFreqExtraDescription))]
        public InArgument<Boolean> DefaultHighFreqExtra { get; set; }

        [LocalizedCategory(nameof(ResourcesActivities.ExtraFeed))]
        [LocalizedDisplayName(nameof(ResourcesActivities.WeatherInfoScopeDefaultLongTermExtraDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.WeatherInfoScopeDefaultLongTermExtraDescription))]
        public InArgument<Boolean> DefaultLongTermExtra { get; set; }

        [LocalizedCategory(nameof(ResourcesActivities.ExtraFeed))]
        [LocalizedDisplayName(nameof(ResourcesActivities.WeatherInfoScopeURLExtraDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.WeatherInfoScopeURLExtraDescription))]
        public InArgument<string> UrlExtra { get; set; }


        // Category for EqVol
        [LocalizedCategory(nameof(ResourcesActivities.EqVolFeed))]
        [LocalizedDisplayName(nameof(ResourcesActivities.WeatherInfoScopeDefaultHighFreqEqVolDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.WeatherInfoScopeDefaultHighFreqEqVolDescription))]
        public InArgument<Boolean> DefaultHighFreqEqVol { get; set; }

        [LocalizedCategory(nameof(ResourcesActivities.EqVolFeed))]
        [LocalizedDisplayName(nameof(ResourcesActivities.WeatherInfoScopeDefaultLongTermEqVolDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.WeatherInfoScopeDefaultLongTermEqVolDescription))]
        public InArgument<Boolean> DefaultLongTermEqVol { get; set; }

        [LocalizedCategory(nameof(ResourcesActivities.EqVolFeed))]
        [LocalizedDisplayName(nameof(ResourcesActivities.WeatherInfoScopeURLEqVolDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.WeatherInfoScopeURLEqVolDescription))]
        public InArgument<string> UrlEqVol { get; set; }



        // Category for Other
        [LocalizedCategory(nameof(ResourcesActivities.OtherFeed))]
        [LocalizedDisplayName(nameof(ResourcesActivities.WeatherInfoScopeDefaultHighFreqOtherDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.WeatherInfoScopeDefaultHighFreqOtherDescription))]
        public InArgument<Boolean> DefaultHighFreqOther { get; set; }

        [LocalizedCategory(nameof(ResourcesActivities.OtherFeed))]
        [LocalizedDisplayName(nameof(ResourcesActivities.WeatherInfoScopeDefaultLongTermOtherDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.WeatherInfoScopeDefaultLongTermOtherDescription))]
        public InArgument<Boolean> DefaultLongTermOther { get; set; }

        [LocalizedCategory(nameof(ResourcesActivities.OtherFeed))]
        [LocalizedDisplayName(nameof(ResourcesActivities.WeatherInfoScopeURLOtherDisplayName))]
        [LocalizedDescription(nameof(ResourcesActivities.WeatherInfoScopeURLOtherDescription))]
        public InArgument<string> UrlOther{ get; set; }

       

        internal static string ParentContainerPropertyTag => "WeatherInfoScope";

        Application application;

        #endregion


        #region Constructors

        public WeatherInfoScope()
        {
            Body = new ActivityAction<Application>
            {
                Argument = new DelegateInArgument<Application>(ParentContainerPropertyTag),
                Handler = new Sequence { DisplayName = "Do" }
            };
        }

        #endregion


        #region Private Methods

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);

        }

        protected override void Execute(NativeActivityContext context)
        {
            var dicInputs = new Dictionary<string, string>();

            //Periodic
            var isDefaultHighFreqRegular = DefaultHighFreqRegular.Get(context);
            var isDefaultLongTermRegular = DefaultLongTermRegular.Get(context);
            var strUrlRegular = UrlRegular.Get(context);
            if (isDefaultHighFreqRegular)
            {
                dicInputs[Resources.REGULAR] = ResourcesActivities.UrlDefaultHighFreqRegular;
            }
            else
            {
                if (isDefaultLongTermRegular)
                {
                    dicInputs[Resources.REGULAR] = ResourcesActivities.UrlDefaultLongTermRegular;
                }
                else
                {
                    if (!String.IsNullOrEmpty(strUrlRegular))
                    {
                        dicInputs[Resources.REGULAR] = strUrlRegular;
                    }
                }
            }

            //Extra
            var isDefaultHighFreqExtra = DefaultHighFreqExtra.Get(context);
            var isDefaultLongTermAsNeeded = DefaultLongTermExtra.Get(context);
            var strUrlExtra = UrlExtra.Get(context);
            if (isDefaultHighFreqExtra)
            {
                dicInputs[Resources.EXTRA] = ResourcesActivities.UrlDefaultHighFreqExtra;
            }
            else
            {
                if (isDefaultLongTermAsNeeded)
                {
                    dicInputs[Resources.EXTRA] = ResourcesActivities.UrlDefaultLongTermExtra;
                }
                else
                {
                    if (!String.IsNullOrEmpty(strUrlExtra))
                    {
                        dicInputs[Resources.EXTRA] = strUrlExtra;
                    }
                }
            }

            //EqVol
            var isDefaultHighFreqEqVol = DefaultHighFreqEqVol.Get(context);
            var isDefaultLongTermEqVol = DefaultLongTermEqVol.Get(context);
            var strUrlEqVol = UrlEqVol.Get(context);
            if (isDefaultHighFreqEqVol)
            {
                dicInputs[Resources.EQVOL] = ResourcesActivities.UrlDefaultHighFreqEqVol;
            }
            else
            {
                if (isDefaultLongTermEqVol)
                {
                    dicInputs[Resources.EQVOL] = ResourcesActivities.UrlDefaultLongTermEqVol;
                }
                else
                {
                    if (!String.IsNullOrEmpty(strUrlEqVol))
                    {
                        dicInputs[Resources.EQVOL] = strUrlEqVol;
                    }
                }
            }

            //Other
            var isDefaultHighFreqOther = DefaultHighFreqOther.Get(context);
            var isDefaultLongTermOther = DefaultLongTermOther.Get(context);
            var strUrlOther = UrlOther.Get(context);
            if (isDefaultHighFreqEqVol)
            {
                dicInputs[Resources.OTHER] = ResourcesActivities.UrlDefaultHighFreqOther;
            }
            else
            {
                if (isDefaultLongTermOther)
                {
                    dicInputs[Resources.OTHER] = ResourcesActivities.UrlDefaultLongTermOther;
                }
                else
                {
                    if (!String.IsNullOrEmpty(strUrlOther))
                    {
                        dicInputs[Resources.OTHER] = strUrlOther;
                    }
                }
            }

            application = new Application(dicInputs);
            
            if (Body != null)
            {
                context.ScheduleAction<Application>(Body, application, OnCompleted, OnFaulted);
            }
        }

        private void OnFaulted(NativeActivityFaultContext faultContext, Exception propagatedException, ActivityInstance propagatedFrom)
        {
            application.Dispose();
        }

        private void OnCompleted(NativeActivityContext context, ActivityInstance completedInstance)
        {
            application.Dispose();
        }

        #endregion


        #region Helpers
        
        #endregion
    }
}
