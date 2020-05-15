using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.ComponentModel.Design;
using GetWeatherInfoFromJMA.Activities.Design.Properties;

namespace GetWeatherInfoFromJMA.Activities.Design
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            var builder = new AttributeTableBuilder();
            builder.ValidateTable();

            var categoryAttribute =  new CategoryAttribute($"{Resources.Category}");
            var categoryAttributeExtra = new CategoryAttribute($"{Resources.Category}.{Resources.Extra}");
            var categoryAttributeRegular = new CategoryAttribute($"{Resources.Category}.{Resources.Regular}");
            var categoryAttributeEqVol = new CategoryAttribute($"{Resources.Category}.{Resources.EqVol}");
            var categoryAttributeOther = new CategoryAttribute($"{Resources.Category}.{Resources.Other}");

            //Common
            builder.AddCustomAttributes(typeof(WeatherInfoScope), categoryAttribute);
            builder.AddCustomAttributes(typeof(WeatherInfoScope), new DesignerAttribute(typeof(WeatherInfoScopeDesigner)));
            builder.AddCustomAttributes(typeof(WeatherInfoScope), new HelpKeywordAttribute("https://go.uipath.com"));


            builder.AddCustomAttributes(typeof(GetXml), categoryAttribute);
            builder.AddCustomAttributes(typeof(GetXml), new DesignerAttribute(typeof(GetXmlDesigner)));
            builder.AddCustomAttributes(typeof(GetXml), new HelpKeywordAttribute("https://go.uipath.com"));

            builder.AddCustomAttributes(typeof(GetAtomFeed), categoryAttribute);
            builder.AddCustomAttributes(typeof(GetAtomFeed), new DesignerAttribute(typeof(GetAtomFeedDesigner)));
            builder.AddCustomAttributes(typeof(GetAtomFeed), new HelpKeywordAttribute("https://go.uipath.com"));

            // Extra Feed
            builder.AddCustomAttributes(typeof(GetWarning), categoryAttributeExtra);
            builder.AddCustomAttributes(typeof(GetWarning), new DesignerAttribute(typeof(GetWarningDesigner)));
            builder.AddCustomAttributes(typeof(GetWarning), new HelpKeywordAttribute("https://go.uipath.com"));

            builder.AddCustomAttributes(typeof(GetTyphoonInfo), categoryAttributeExtra);
            builder.AddCustomAttributes(typeof(GetTyphoonInfo), new DesignerAttribute(typeof(GetTyphoonInfoDesigner)));
            builder.AddCustomAttributes(typeof(GetTyphoonInfo), new HelpKeywordAttribute("https://go.uipath.com"));

            // Regular Feed
            builder.AddCustomAttributes(typeof(GetForecast), categoryAttributeRegular);
            builder.AddCustomAttributes(typeof(GetForecast), new DesignerAttribute(typeof(GetForecastDesigner)));
            builder.AddCustomAttributes(typeof(GetForecast), new HelpKeywordAttribute("https://go.uipath.com"));

            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
