using System;
using System.Windows;

namespace ImageRecognition.ImageRecognizing
{
    public struct ImageInertiaInfo
    {
        private readonly Point centerOfWeight;
        private readonly Vector mainInerntiaAxisDirection;
        private readonly int weight;

        public ImageInertiaInfo(Point centerOfWeight, Vector mainInertiaAxisDirection, int weight)
        {
            this.centerOfWeight = centerOfWeight;
            this.mainInerntiaAxisDirection = mainInertiaAxisDirection;
            this.weight = weight;
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
