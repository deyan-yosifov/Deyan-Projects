using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Fractals
{
    public class FractalTreeGenerator2D
    {
        private readonly Queue<LineSegment2D> lineSegments;
        public const double InitialLineSegmentLength = 1;
        public const double InitialLineSegmentThickness = 0.15;
        public const double ScaleBetweenLevels = 0.7;

        public FractalTreeGenerator2D()
        {
            this.lineSegments = new Queue<LineSegment2D>();
            this.CurrentLevel = -1;
        }

        public int CurrentLevel { get; private set; }

        public IEnumerable<LineSegment2D> CurrentLevelLineSegments
        {
            get
            {
                foreach (LineSegment2D segment in this.lineSegments)
                {
                    yield return segment;
                }
            }
        }

        public double CurrentSegmentLength { get; private set; }
        public double CurrentSegmentThickness { get; private set; }

        public void MoveToNextLevel()
        {
            this.CurrentLevel++;

            if (this.CurrentLevel == 0)
            {
                this.CurrentSegmentLength = InitialLineSegmentLength;
                this.CurrentSegmentThickness = InitialLineSegmentThickness;
                this.lineSegments.Enqueue(new LineSegment2D(new Point(0, 0), new Point(0, this.CurrentSegmentLength), this.CurrentSegmentThickness));
            }
            else
            {
                int parentsCount = this.lineSegments.Count;
                this.CurrentSegmentLength *= ScaleBetweenLevels;
                this.CurrentSegmentThickness *= ScaleBetweenLevels;

                for (int i = 0; i < parentsCount; i++)
                {
                    LineSegment2D parent = this.lineSegments.Dequeue();
                    this.lineSegments.Enqueue(this.CalculateLeftChild(parent));
                    this.lineSegments.Enqueue(this.CalculateRightChild(parent));
                }
            }
        }

        private LineSegment2D CalculateLeftChild(LineSegment2D parent)
        {
            return this.CalculateChild(parent, 1 * this.CurrentSegmentThickness, 90);
        }

        private LineSegment2D CalculateRightChild(LineSegment2D parent)
        {
            return this.CalculateChild(parent, 2 * this.CurrentSegmentThickness, -90);
        }

        private LineSegment2D CalculateChild(LineSegment2D parent, double topDisplacement, double rotationAngleInDegrees)
        {
            Vector parentVector = parent.End - parent.Start;
            Vector displacement = (parentVector / parentVector.Length) * (parentVector.Length - topDisplacement);

            Matrix matrix = new Matrix();
            matrix.ScaleAt(ScaleBetweenLevels, ScaleBetweenLevels, parent.Start.X, parent.Start.Y);
            matrix.RotateAt(rotationAngleInDegrees, parent.Start.X, parent.Start.Y);
            matrix.Translate(displacement.X, displacement.Y);

            LineSegment2D childSegment = new LineSegment2D(matrix.Transform(parent.Start), matrix.Transform(parent.End), this.CurrentSegmentThickness);

            return childSegment;
        }
    }
}
