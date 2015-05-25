using Deyo.Core.Mathematics.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Fractals
{
    public class FractalTreeGenerator3D
    {
        private readonly Queue<LineSegment3D> lineSegments;
        public const double InitialLineSegmentLength = 1;
        public const double InitialLineSegmentThickness = 0.15;
        public const double ScaleBetweenLevels = 0.7;

        public FractalTreeGenerator3D()
        {
            this.lineSegments = new Queue<LineSegment3D>();
            this.CurrentLevel = -1;            
        }

        public int CurrentLevel { get; private set; }
        public double CurrentSegmentLength { get; private set; }

        public IEnumerable<LineSegment3D> CurrentLevelLineSegments
        {
            get
            {
                foreach (LineSegment3D segment in this.lineSegments)
                {
                    yield return segment;
                }
            }
        }

        public void MoveToNextLevel()
        {
            this.CurrentLevel++;

            if (this.CurrentLevel == 0)
            {
                this.CurrentSegmentLength = InitialLineSegmentLength;
                this.lineSegments.Enqueue(new LineSegment3D(new Point3D(0, 0, 0), new Point3D(0, 0, this.CurrentSegmentLength), InitialLineSegmentThickness));
            }
            else
            {
                this.CurrentSegmentLength *= ScaleBetweenLevels;
                int parentsCount = this.lineSegments.Count;

                for (int parentIndex = 0; parentIndex < parentsCount; parentIndex++)
                {
                    LineSegment3D parent = this.lineSegments.Dequeue();
                    Vector3D i, j;
                    GetNormalPlaneVectors(parent, out i, out j);
                    this.lineSegments.Enqueue(CalculateChild(parent, 1 * parent.Thickness * ScaleBetweenLevels, i));
                    this.lineSegments.Enqueue(CalculateChild(parent, 2 * parent.Thickness * ScaleBetweenLevels, j));
                    this.lineSegments.Enqueue(CalculateChild(parent, 3 * parent.Thickness * ScaleBetweenLevels, -i));
                    this.lineSegments.Enqueue(CalculateChild(parent, 4 * parent.Thickness * ScaleBetweenLevels, -j));
                }
            }
        }

        private LineSegment3D CalculateChild(LineSegment3D parent, double topDisplacement, Vector3D childDirection)
        {
            Vector3D parentVector = parent.End - parent.Start;
            Vector3D displacement = (parentVector / parentVector.Length) * (parentVector.Length - topDisplacement);

            Point3D childStart = parent.Start + displacement;
            Point3D childEnd = childStart + (childDirection * this.CurrentSegmentLength);
            double childThickness = parent.Thickness * ScaleBetweenLevels;

            LineSegment3D childSegment = new LineSegment3D(childStart, childEnd, childThickness);

            return childSegment;
        }

        private static void GetNormalPlaneVectors(LineSegment3D segment, out Vector3D i, out Vector3D j)
        {
            Vector3D direction = segment.End - segment.Start;
            direction.Normalize();

            if (Math.Abs(direction.X).IsEqualTo(1))
            {
                GetXNormalPlaneVectors(direction.X > 0, out i, out j);
            }
            else if (Math.Abs(direction.Y).IsEqualTo(1))
            {
                GetYNormalPlaneVectors(direction.Y > 0, out i, out j);
            }
            else if (Math.Abs(direction.Z).IsEqualTo(1))
            {
                GetZNormalPlaneVectors(direction.Z > 0, out i, out j);
            }
            else
            {
                throw new NotSupportedException("Cannot calculate normal vectors of line segment that is not parallel to some of the coordinate axises!");
            }
        }

        private static void GetXNormalPlaneVectors(bool isPositiveDirection, out Vector3D i, out Vector3D j)
        {
            int sign = isPositiveDirection ? 1 : -1;
            i = new Vector3D(0, sign, 0);
            j = new Vector3D(0, 0, sign);
        }

        private static void GetYNormalPlaneVectors(bool isPositiveDirection, out Vector3D i, out Vector3D j)
        {
            int sign = isPositiveDirection ? 1 : -1;
            i = new Vector3D(0, 0, sign);
            j = new Vector3D(sign, 0, 0);
        }

        private static void GetZNormalPlaneVectors(bool isPositiveDirection, out Vector3D i, out Vector3D j)
        {
            int sign = isPositiveDirection ? 1 : -1;
            i = new Vector3D(sign, 0, 0);
            j = new Vector3D(0, sign, 0);
        }
    }
}
