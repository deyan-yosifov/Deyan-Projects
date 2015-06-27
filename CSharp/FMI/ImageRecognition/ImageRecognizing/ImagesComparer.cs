using ImageRecognition.Common;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageRecognition.ImageRecognizing
{
    public static class ImagesComparer
    {
        public static ImageInertiaInfo CalculateInertiaInfo(BitmapSource bitmapSource)
        {
            byte?[,] intensities = bitmapSource.GetPixelsIntensity();
            int width = intensities.GetLength(0);
            int height = intensities.GetLength(1);
            double xFirstMoment = 0;
            double yFirstMoment = 0;
            double xSecondMoment = 0;
            double ySecondMoment = 0;
            double xySecondMoment = 0;            
            int weight = 0;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    byte? intensity = intensities[i, j];

                    if (intensity.HasValue)
                    {
                        double x = i + 0.5;
                        double y = j + 0.5;
                        weight += intensity.Value;
                        xFirstMoment += y * intensity.Value;
                        yFirstMoment += x * intensity.Value;
                        xSecondMoment += y * y * intensity.Value;
                        ySecondMoment += x * x * intensity.Value;
                        xySecondMoment += x * y * intensity.Value;
                    }
                }
            }

            return ImagesComparer.CalculateInertiaInfo(intensities, xFirstMoment, yFirstMoment, xSecondMoment, ySecondMoment, xySecondMoment, weight);
        }

        private static ImageInertiaInfo CalculateInertiaInfo(byte?[,] intensities, double xFirstMoment, double yFirstMoment, double xSecondMoment, double ySecondMoment, double xySecondMoment, int weight)
        {
            if (weight == 0)
            {
                return new ImageInertiaInfo(new Point(), new Vector(), 0);
            }
            else
            {
                Point centerOfWeight = new Point(yFirstMoment / weight, xFirstMoment / weight);
                xSecondMoment -= centerOfWeight.Y * centerOfWeight.Y * weight;
                ySecondMoment -= centerOfWeight.X * centerOfWeight.X * weight;
                xySecondMoment -= centerOfWeight.X * centerOfWeight.Y * weight;

                if (xSecondMoment == ySecondMoment)
                {
                    return ImagesComparer.CalculateSymetricImageInertia(intensities, centerOfWeight, weight);
                }
                else
                {
                    return ImagesComparer.CalculateMaximumInertiaInfo(xSecondMoment, ySecondMoment, xySecondMoment, centerOfWeight, weight);
                }
            } 
        }

        private static ImageInertiaInfo CalculateSymetricImageInertia(byte?[,] imageIntensities, Point centerOfWeight, int weight)
        {
            double maxDistance = double.MinValue;
            double xMaxDistance = 0;
            double yMaxDistance = 0;
            int width = imageIntensities.GetLength(0);
            int height = imageIntensities.GetLength(1);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (imageIntensities[i, j].HasValue)
                    {
                        double x = i + 0.5;
                        double y = j + 0.5;

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

            return new ImageInertiaInfo(centerOfWeight, direction, weight);
        }

        private static ImageInertiaInfo CalculateMaximumInertiaInfo(double xSecondMoment, double ySecondMoment, double xySecondMoment, Point centerOfWeight, int weight)
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
            matrix.Rotate(axisAngle);
            Vector vector = matrix.Transform(new Vector(1, 0));

            return new ImageInertiaInfo(centerOfWeight, vector, weight);
        }
    }
}
