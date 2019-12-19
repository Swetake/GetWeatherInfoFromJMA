using System.Windows;
using System.Windows.Controls;

namespace GetWeatherInfoFromJMA.Activities.Design
{
    public class ActivityDecoratorControl : ContentControl
    {
        static ActivityDecoratorControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ActivityDecoratorControl), new FrameworkPropertyMetadata(typeof(ActivityDecoratorControl)));
        }
    }
}