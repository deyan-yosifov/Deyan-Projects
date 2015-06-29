using ImageRecognition.Common;
using ImageRecognition.Database;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageRecognition.ImageRecognizing
{
    public static class ImagesComparer
    {
        public static ImagesComparisonInfo GetComparisonInfo(NormalizedImageInfo originalImage, NormalizedImageInfo imageToCompare)
        {
            return ImagesComparer.GetComparisonInfo(originalImage, imageToCompare, true);
        }

        public static double CompareImages(NormalizedImageInfo originalImage, NormalizedImageInfo imageToCompare)
        {
            return ImagesComparer.GetComparisonInfo(originalImage, imageToCompare, false).ComparisonResult;
        }

        public static ImageInertiaInfo CalculateInertiaInfo(BitmapSource bitmapSource)
        {
            byte?[,] intensities = bitmapSource.GetPixelsIntensity();
            int height = intensities.GetLength(0);
            int width = intensities.GetLength(1);
            double xFirstMoment = 0;
            double yFirstMoment = 0;
            double xSecondMoment = 0;
            double ySecondMoment = 0;
            double xySecondMoment = 0;            
            int weight = 0;
            int area = 0;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    byte? intensity = intensities[i, j];

                    if (intensity.HasValue)
                    {
                        area++;
                        double x = j + 0.5;
                        double y = i + 0.5;
                        byte pixelIntensity = intensity.Value;
                        weight += pixelIntensity;
                        xFirstMoment += y * pixelIntensity;
                        yFirstMoment += x * pixelIntensity;
                        xSecondMoment += y * y * pixelIntensity;
                        ySecondMoment += x * x * pixelIntensity;
                        xySecondMoment += x * y * pixelIntensity;
                    }
                }
            }

            return ImagesComparer.CalculateInertiaInfo(intensities, xFirstMoment, yFirstMoment, xSecondMoment, ySecondMoment, xySecondMoment, weight, area);
        }

        private static ImageInertiaInfo CalculateInertiaInfo(byte?[,] intensities, double xFirstMoment, double yFirstMoment, double xSecondMoment, double ySecondMoment, double xySecondMoment, int weight, int area)
        {
            if (area == 0)
            {
                return new ImageInertiaInfo(new Point(), new Vector(), 0, 0);
            }
            else
            {
                Point centerOfWeight = new Point(yFirstMoment / weight, xFirstMoment / weight);
                xSecondMoment -= centerOfWeight.Y * centerOfWeight.Y * weight;
                ySecondMoment -= centerOfWeight.X * centerOfWeight.X * weight;
                xySecondMoment -= centerOfWeight.X * centerOfWeight.Y * weight;

                if (xSecondMoment == ySecondMoment)
                {
                    return ImagesComparer.CalculateSymetricImageInertia(intensities, centerOfWeight, weight, area);
                }
                else
                {
                    return ImagesComparer.CalculateMaximumInertiaInfo(xSecondMoment, ySecondMoment, xySecondMoment, centerOfWeight, weight, area);
                }
            } 
        }

        private static ImageInertiaInfo CalculateSymetricImageInertia(byte?[,] imageIntensities, Point centerOfWeight, int weight, int area)
        {
            double maxDistance = double.MinValue;
            double xMaxDistance = 0;
            double yMaxDistance = 0;
            int height = imageIntensities.GetLength(0);
            int width = imageIntensities.GetLength(1);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (imageIntensities[i, j].HasValue)
                    {
                        double x = j + 0.5;
                        double y = i + 0.5;

                        double distance = new Vector(centerOfWeight.X - x, centerOfWeight.Y - y).LengthSquared;

                        if (distance > maxDistance)
                        {
                            xMaxDistance = x;
                            yMaxDistance = y;
                            maxDistance = distance;
                        }
                    }
                }
            }

            Vector direction = new Vector(xMaxDistance - centerOfWeight.X, yMaxDistance - centerOfWeight.Y);
            direction.Normalize();

            return new ImageInertiaInfo(centerOfWeight, direction, area, weight);
        }

        private static ImageInertiaInfo CalculateMaximumInertiaInfo(double xSecondMoment, double ySecondMoment, double xySecondMoment, Point centerOfWeight, int weight, int area)
        {
            double extremumAngle = Math.Atan(2 * xySecondMoment / (ySecondMoment - xSecondMoment));
            double cos = Math.Cos(extremumAngle);
            double sin = Math.Sin(extremumAngle);

            double uSecondMoment = (xSecondMoment + ySecondMoment) / 2 + (xSecondMoment - ySecondMoment) * cos / 2 - xySecondMoment * sin;
            double axisAngle = extremumAngle / 2;

            if (2 * uSecondMoment < xSecondMoment + ySecondMoment)
            {
                axisAngle += Math.PI / 2;
            }

            Matrix matrix = new Matrix();
            matrix.Rotate(axisAngle.RadiansToDegrees());
            Vector vector = matrix.Transform(new Vector(1, 0));

            return new ImageInertiaInfo(centerOfWeight, vector, area, weight);
        }

        private static ImagesComparisonInfo GetComparisonInfo(NormalizedImageInfo originalImage, NormalizedImageInfo imageToCompare, bool getImageIntensities)
        {
            double comparison;
            if (ImagesComparer.TryCompareImagesWithZeroArea(originalImage, imageToCompare, out comparison))
            {
                BitmapSource blankImage = getImageIntensities ? originalImage.ImageSource : null;

                return new ImagesComparisonInfo(comparison, blankImage, blankImage, blankImage);
            }

            return ImagesComparer.CompareImagesWithNonZeroArea(originalImage, imageToCompare, getImageIntensities);
        }

        private static ImagesComparisonInfo CompareImagesWithNonZeroArea(NormalizedImageInfo originalImage, NormalizedImageInfo imageToCompare, bool getImageIntensities)
        {
            byte?[,] originalPixels = originalImage.ImageSource.GetPixelsIntensity();
            byte?[,] imageToComparePixels = imageToCompare.ImageSource.GetPixelsIntensity();
            int height = originalPixels.GetLength(0);
            int width = originalPixels.GetLength(1);

            Matrix firstMatrix = ImagesComparer.CalculateSameDirectionVectorMatrix(originalImage.InertiaInfo, imageToCompare.InertiaInfo);
            Matrix secondMatrix = ImagesComparer.CalculateOpositeDirectionVectorMatrix(originalImage.InertiaInfo, imageToCompare.InertiaInfo);
            int firstDifference = 0;
            int secondDifference = 0;
            byte maxIntensity = 0;
            byte?[,] firstComparisonImage = getImageIntensities ? new byte?[height, width] : null;
            byte?[,] secondComparisonImage = getImageIntensities ? new byte?[height, width] : null;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Point originalPoint = new Point(j + 0.5, i + 0.5);
                    byte? originalIntensity = originalPixels[i, j];

                    if (originalIntensity.HasValue)
                    {
                        maxIntensity = Math.Max(maxIntensity, originalIntensity.Value);
                        Point firstPoint = firstMatrix.Transform(originalPoint);
                        byte? firstPixel = ImagesComparer.GetImagePixelFromPoint(imageToComparePixels, firstPoint);
                        if (getImageIntensities)
                        {
                            firstComparisonImage[i, j] = firstPixel;
                        }
                        firstDifference += ImagesComparer.GetDifference(originalIntensity.Value, firstPixel);

                        Point secondPoint = secondMatrix.Transform(originalPoint);
                        byte? secondPixel = ImagesComparer.GetImagePixelFromPoint(imageToComparePixels, secondPoint);
                        if (getImageIntensities)
                        {
                            secondComparisonImage[i, j] = secondPixel;
                        }
                        secondDifference += ImagesComparer.GetDifference(originalIntensity.Value, secondPixel);
                    }
                }
            }
            double maxDifference = originalImage.InertiaInfo.Area * 255;

            double difference = Math.Min(firstDifference, secondDifference) / maxDifference;
            double comparison = 1 - difference;

            if (getImageIntensities)
            {
                BitmapSource originalImageSource = ImageExtensions.CreateImageFromGrayPixels(originalPixels);
                BitmapSource firstComparisonImageSource = ImageExtensions.CreateImageFromGrayPixels(firstComparisonImage);
                BitmapSource secondComparisonImageSource = ImageExtensions.CreateImageFromGrayPixels(secondComparisonImage);

                return new ImagesComparisonInfo(comparison, originalImageSource, firstComparisonImageSource, secondComparisonImageSource);
            }
            else
            {
                return new ImagesComparisonInfo(comparison);
            }
        }

        private static int GetDifference(int firstPixel, byte? secondPixel)
        {
            if (secondPixel.HasValue)
            {
                return Math.Abs(firstPixel - secondPixel.Value);
            }

            return 255;
        }

        private static byte? GetImagePixelFromPoint(byte?[,] pixels, Point point)
        {
            int j = (int)point.X;
            int i = (int)point.Y;

            if (0 <= i && i < pixels.GetLength(0) && 0 <= j && j < pixels.GetLength(1))
            {
                return pixels[i, j];
            }

            return null;
        }

        private static Matrix CalculateSameDirectionVectorMatrix(ImageInertiaInfo originalInertia, ImageInertiaInfo imageToCompareInertia)
        {
            double scale = Math.Sqrt(imageToCompareInertia.Area / (double)originalInertia.Area);
            double angle = Vector.AngleBetween(originalInertia.MainInertiaAxisDirection, imageToCompareInertia.MainInertiaAxisDirection);
            double centerX = imageToCompareInertia.CenterOfWeight.X;
            double centerY = imageToCompareInertia.CenterOfWeight.Y;
            Vector translation = imageToCompareInertia.CenterOfWeight - originalInertia.CenterOfWeight;

            Matrix matrix = new Matrix();
            matrix.Translate(translation.X, translation.Y);
            matrix.RotateAt(angle, centerX, centerY);
            matrix.ScaleAt(scale, scale, centerX, centerY);

            return matrix;
        }

        private static Matrix CalculateOpositeDirectionVectorMatrix(ImageInertiaInfo originalInertia, ImageInertiaInfo imageToCompareInertia)
        {
            Point center = imageToCompareInertia.CenterOfWeight;
            Vector direction = -imageToCompareInertia.MainInertiaAxisDirection;
            int area = imageToCompareInertia.Area;
            int weight = imageToCompareInertia.Weight;
            ImageInertiaInfo opositeDirectionInertia = new ImageInertiaInfo(center, direction, area, weight);

            return ImagesComparer.CalculateSameDirectionVectorMatrix(originalInertia, opositeDirectionInertia);
        }

        private static bool TryCompareImagesWithZeroArea(NormalizedImageInfo originalImage, NormalizedImageInfo imageToCompare, out double comparison)
        {
            comparison = 0;
            double firstArea = originalImage.InertiaInfo.Area;
            double secondArea = imageToCompare.InertiaInfo.Area;

            if (firstArea == 0 && secondArea == 0)
            {
                comparison = 1;
                return true;
            }
            else if (firstArea == 0 || secondArea == 0)
            {
                return true;
            }

            return false;
        }
    }
}
