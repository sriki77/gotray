using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace GoTrayUtils
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public BooleanToVisibilityConverter()
        {
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        public Visibility FalseValue { get; set; }
        public Visibility TrueValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return FalseValue;
            }
            var isVisible = (bool) value;
            return isVisible ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var visiblity = (Visibility) value;
            return visiblity == Visibility.Visible;
        }
    }
}