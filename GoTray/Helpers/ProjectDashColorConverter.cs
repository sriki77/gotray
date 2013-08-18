using System;
using GoTrayFeed;
using Windows.UI.Xaml.Data;

namespace GoTray.Helpers
{
    public class ProjectDashColorConverter : IValueConverter
    {
        private const String FailureColor = "#5d0104"; 
        private const String PassColor = "#035b06";
        private const String InProgressColor = "#8b7703";
        private const String GrayColor = "LightGray"; 

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch ((GoProject.ProjectStatus) value)
            {
                case GoProject.ProjectStatus.Building:
                    return InProgressColor;
                case GoProject.ProjectStatus.Failure:
                    return FailureColor;
                case GoProject.ProjectStatus.Success:
                    return PassColor;
            }
            return GrayColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}