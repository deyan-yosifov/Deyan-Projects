using System;
using System.Windows.Media.Imaging;

namespace ImageRecognition.ImageRecognizing
{
    public class ImagesComparisonInfo
    {
        private readonly double comparisonResult;
        private readonly BitmapSource originalImageIntensities;
        private readonly BitmapSource firstComparisonIntensities;
        private readonly BitmapSource secondComparisonIntensities;

        public ImagesComparisonInfo(double result)
            : this(result, null, null, null)
        {
        }

        public ImagesComparisonInfo(double result, BitmapSource originalImage, BitmapSource firstComparison, BitmapSource secondComarison)
        {
            this.comparisonResult = result;
            this.originalImageIntensities = originalImage;
            this.firstComparisonIntensities = firstComparison;
            this.secondComparisonIntensities = secondComarison;
        }

        public double ComparisonResult
        {
            get
            {
                return this.comparisonResult;
            }
        }

        public BitmapSource OriginalImageIntensities
        {
            get
            {
                return this.originalImageIntensities;
            }
        }

        public BitmapSource FirstComparisonIntensities
        {
            get
            {
                return this.firstComparisonIntensities;
            }
        }

        public BitmapSource SecondComparisonIntensities
        {
            get
            {
                return this.secondComparisonIntensities;
            }
        }
    }
}
