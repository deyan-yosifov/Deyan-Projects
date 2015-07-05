using GeometryBasics.Models;
using System;
using System.Windows.Data;

namespace GeometryBasics.Common
{
    public class LineSegmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            LineSegment line = (LineSegment)value;

            return string.Format("({0}, {1})\r\n({2}, {3})", PointConverter.Round(line.Start.X), PointConverter.Round(line.Start.Y), PointConverter.Round(line.End.X), PointConverter.Round(line.End.Y));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
