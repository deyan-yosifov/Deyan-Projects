using System;
using System.Windows;

namespace ImageRecognition.ImageRecognizing
{
    public struct Axis
    {
        private readonly Point startPoint;
        private readonly Vector directionVector;

        public Axis(Point startPoint, Vector directionVector)
        {
            this.startPoint = startPoint;
            this.directionVector = directionVector;
        }

        public Point StartPoint
        {
            get
            {
                return this.startPoint;
            }
        }

        public Vector DirectionVector
        {
            get
            {
                return this.directionVector;
            }
        }
    }
}
