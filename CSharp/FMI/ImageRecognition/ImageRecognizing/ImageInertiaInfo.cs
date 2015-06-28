using System;
using System.Windows;

namespace ImageRecognition.ImageRecognizing
{
    public class ImageInertiaInfo
    {
        private readonly Point centerOfWeight;
        private readonly Vector mainInerntiaAxisDirection;
        private readonly int weight;
        private readonly int area;

        public ImageInertiaInfo(Point centerOfWeight, Vector mainInertiaAxisDirection, int area, int weight)
        {
            this.centerOfWeight = centerOfWeight;
            this.mainInerntiaAxisDirection = mainInertiaAxisDirection;
            this.area = area;
            this.weight = weight;
        }

        public int Area
        {
            get
            {
                return this.area;
            }
        }

        public int Weight
        {
            get
            {
                return this.weight;
            }
        }

        public Point CenterOfWeight
        {
            get
            {
                return this.centerOfWeight;
            }
        }

        public Vector MainInertiaAxisDirection
        {
            get
            {
                return this.mainInerntiaAxisDirection;
            }
        }
    }
}
