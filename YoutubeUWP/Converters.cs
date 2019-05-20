
using System;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace YoutubeUWP
{
    public class InvertedBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value == true)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Convert(!(bool)value, targetType, parameter, language);
        }
    }

    public class BooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value == true)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Convert(!(bool)value, targetType, parameter, language);
        }
    }

    public class UriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string path = value as String;
            BitmapImage bi = new BitmapImage();
            bi.UriSource = new Uri(path);
            return bi;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ViewCountColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int viewCount = (int)value;

            if (viewCount < 1000)
                return new SolidColorBrush(Color.FromArgb(0x80, 0x00, 0x80, 0x00));
            if (viewCount < 1000000)
                return new SolidColorBrush(Color.FromArgb(0x80, Colors.DarkOrange.R, Colors.DarkOrange.G, Colors.DarkOrange.B));

            return new SolidColorBrush(Color.FromArgb(0x80, 0xFF, 0x00, 0x00));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ViewCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int viewCount = (int)value;

            if (viewCount < 1000)
                return viewCount.ToString();
            if (viewCount < 1000000)
                return (viewCount / 1000).ToString() + "K";
            if (viewCount < 10000000)
                return string.Format(CultureInfo.InvariantCulture, "{0:0.0}M", viewCount / 1000000.0);

            return string.Format("{0}M", viewCount / 1000000);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var date = (DateTime)value;

            var resultDT = DateTime.Now - date;

            if (resultDT.TotalDays >= 1)
                return ((int)resultDT.TotalDays).ToString() + " days ago";
            if (resultDT.TotalHours >= 1)
                return ((int)resultDT.TotalHours).ToString() + " hours ago";
            if (resultDT.TotalMinutes >= 1)
                return ((int)resultDT.TotalMinutes).ToString() + " minutes ago";
            if (resultDT.TotalSeconds >= 1)
                return ((int)resultDT.TotalSeconds).ToString() + " seconds ago";

            return "???";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}