using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace ImageRecognition.ImageRecognizing
{
    public class ImagesComparisonInfo
    {
        public ImagesComparisonInfo(double result)
            : this(result, null, null, null)
        {
        }

        public ImagesComparisonInfo(double result, BitmapSource originalImage, BitmapSource firstComparison, BitmapSource secondComarison)
        {
            this.ComparisonResult = result;
            this.OriginalImageIntensities = originalImage;
            this.FirstComparisonIntensities = firstComparison;
            this.SecondComparisonIntensities = secondComarison;
        }

        public double ComparisonResult { get; private set; }
        public BitmapSource OriginalImageIntensities { get; private set; }
        public BitmapSource FirstComparisonIntensities { get; private set; }
        public BitmapSource SecondComparisonIntensities { get; private set; }
    }
}
