using Deyo.Controls.Controls3D.Visuals;
using Deyo.Core.Common;
using Deyo.Core.Mathematics.Algebra;
using Deyo.Core.Mathematics.Geometry;
using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LobelFrames.ViewModels.Commands.Handlers
{
    internal class FoldMeshCalculator
    {
        private readonly LobelSurface surface;

        public FoldMeshCalculator(LobelSurface surface)
        {
            this.surface = surface;
        }

        public LobelSurface Surface
        {
            get
            {
                return this.surface;
            }
        }

        public MeshPatchRotationCache CalculateRotationCache(PointVisual firstAxisPoint, PointVisual secondAxisPoint, PointVisual rotationPlanePoint)
        {
            Vertex center = this.Surface.GetVertexFromPointVisual(firstAxisPoint);
            Vertex axis = this.Surface.GetVertexFromPointVisual(secondAxisPoint);
            Vertex plane = this.Surface.GetVertexFromPointVisual(rotationPlanePoint);

            Vector3D axisVector = axis.Point - center.Point;
            Vector3D boundaryVector = plane.Point - center.Point;
            Vector3D normal = Vector3D.CrossProduct(-axisVector, boundaryVector);

            MeshPatchVertexSelectionInfo patch = this.Surface.MeshEditor.GetMeshPatchVertexSelection(new Vertex[] { axis, center, plane }, normal);
            MeshPatchRotationCache rotationCache =
                new MeshPatchRotationCache(this.Surface.MeshEditor.ElementsProvider, patch, center, axisVector, boundaryVector);

            return rotationCache;
        }

        public Vertex CalculateExtendedRayEndVertex(PointVisual fromPoint, PointVisual toPoint)
        {
            Vertex first = this.Surface.GetVertexFromPointVisual(fromPoint);
            Vertex second = this.Surface.GetVertexFromPointVisual(toPoint);
            Vertex extendedEnd = this.Surface.MeshEditor.FindEndOfEdgesRayInPlane(first, second);

            return extendedEnd;
        }

        public bool ArePatchesIntersectingInMoreThanOnePoint(MeshPatchVertexSelectionInfo firstPatch, MeshPatchVertexSelectionInfo secondPatch)
        {
            VerticesSet setToCheck, setToEnumerate;

            if (firstPatch.AllPatchVertices.Count > secondPatch.AllPatchVertices.Count)
            {
                setToCheck = firstPatch.AllPatchVertices;
                setToEnumerate = secondPatch.AllPatchVertices;
            }
            else
            {
                setToCheck = secondPatch.AllPatchVertices;
                setToEnumerate = firstPatch.AllPatchVertices;
            }

            int coinsideCount = 0;
            foreach (Vertex vertex in setToEnumerate)
            {
                if (setToCheck.Contains(vertex))
                {
                    coinsideCount++;

                    if (coinsideCount > 1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public double NormalizeRotationAngle(double rotationAngle)
        {
            int multiple = (int)(rotationAngle / 360);
            rotationAngle = rotationAngle - multiple * 360;

            if (rotationAngle < 0)
            {
                rotationAngle += 360;
            }

            return rotationAngle;
        }

        public IEnumerable<Tuple<double, double>> CalculatePossibleFoldAngles
            (MeshPatchRotationCache firstRotationInfo, MeshPatchRotationCache secondRotationInfo)
        {
            TwoPatchesFoldingContext c = new TwoPatchesFoldingContext(firstRotationInfo, secondRotationInfo);

            if (c.FirstCenterProjectionDistanceSquared.IsEqualTo(c.FirstRadiusSquared))
            {
                if (c.IsPointEquidistantWithSecondCircleRadius(c.FirstCenterProjection))
                {
                    yield return this.CalculateRotationAngles(c.FirstCenterProjection, c);
                }
            }
            else if (c.FirstCenterProjectionDistanceSquared < c.FirstRadiusSquared)
            {
                double intersectionProjectionDistance = Math.Sqrt(c.FirstRadiusSquared - c.FirstCenterProjectionDistanceSquared);
                Point3D firstCircleIntersection = c.FirstCenterProjection + intersectionProjectionDistance * c.IntersectionLineUnitVector;
                Point3D secondCircleIntersection = c.FirstCenterProjection - intersectionProjectionDistance * c.IntersectionLineUnitVector;

                if (c.IsPointEquidistantWithSecondCircleRadius(firstCircleIntersection))
                {
                    yield return this.CalculateRotationAngles(firstCircleIntersection, c);
                }

                if (c.IsPointEquidistantWithSecondCircleRadius(secondCircleIntersection))
                {
                    yield return this.CalculateRotationAngles(secondCircleIntersection, c);
                }
            }
        }

        private Tuple<double, double> CalculateRotationAngles(Point3D circlesIntersection, TwoPatchesFoldingContext c)
        {
            double firstRotationAngle = c.GetFirstRotationAngle(circlesIntersection);
            double secondRotationAngle = c.GetSecondRotationAngle(circlesIntersection);

            return new Tuple<double, double>(firstRotationAngle, secondRotationAngle);
        }

        private class TwoPatchesFoldingContext
        {
            public readonly Point3D FirstCenterProjection;
            public readonly Vector3D IntersectionLineUnitVector;
            public readonly double FirstRadiusSquared;
            public readonly double FirstCenterProjectionDistanceSquared;
            private readonly double secondRadiusSquared;
            private readonly Point3D secondCircleCenter;
            private readonly MeshPatchRotationCache firstRotationInfo;
            private readonly MeshPatchRotationCache secondRotationInfo;

            public TwoPatchesFoldingContext(MeshPatchRotationCache firstRotationInfo, MeshPatchRotationCache secondRotationInfo)
            {
                Guard.ThrowExceptionIfNull(firstRotationInfo, "firstRotationInfo");
                Guard.ThrowExceptionIfNull(secondRotationInfo, "secondRotationInfo");
                Guard.ThrowExceptionInNotEqual(firstRotationInfo.Center, secondRotationInfo.Center, "Both rotation must have the same center!");

                this.firstRotationInfo = firstRotationInfo;
                this.secondRotationInfo = secondRotationInfo;

                Point3D commonCenter = firstRotationInfo.Center.Point;
                Vector3D firstAxisDirection = firstRotationInfo.Axis;
                Vector3D secondAxisDirection = secondRotationInfo.Axis;
                Vector3D firstSideDirection = firstRotationInfo.BoundaryDirection;
                Vector3D secondSideDirection = secondRotationInfo.BoundaryDirection;

                Point3D firstCircleCenter = commonCenter + firstAxisDirection * Vector3D.DotProduct(firstAxisDirection, firstSideDirection);
                this.secondCircleCenter = commonCenter + secondAxisDirection * Vector3D.DotProduct(secondAxisDirection, secondSideDirection);

                Vector3D intersectionLineDirection = Vector3D.CrossProduct(firstAxisDirection, secondAxisDirection);
                intersectionLineDirection.Normalize();
                this.IntersectionLineUnitVector = intersectionLineDirection;
                Vector3D firstNormalToIntersectionLine = Vector3D.CrossProduct(this.IntersectionLineUnitVector, firstAxisDirection);
                this.FirstCenterProjection =
                    IntersectionsHelper.IntersectLineAndPlane(firstCircleCenter, firstNormalToIntersectionLine, secondCircleCenter, secondAxisDirection);
                this.FirstRadiusSquared = ((commonCenter + firstSideDirection) - firstCircleCenter).LengthSquared;
                this.secondRadiusSquared = ((commonCenter + secondSideDirection) - secondCircleCenter).LengthSquared;
                this.FirstCenterProjectionDistanceSquared = (this.FirstCenterProjection - firstCircleCenter).LengthSquared;
            }

            public bool IsPointEquidistantWithSecondCircleRadius(Point3D point)
            {
                double secondCenterProjectionDistanceSquared = (point - this.secondCircleCenter).LengthSquared;
                bool isEquidistantWithSecondRadius = secondCenterProjectionDistanceSquared.IsEqualTo(this.secondRadiusSquared);

                return isEquidistantWithSecondRadius;
            }

            public double GetFirstRotationAngle(Point3D point)
            {
                return TwoPatchesFoldingContext.GetRotationAngle(point, this.firstRotationInfo);
            }

            public double GetSecondRotationAngle(Point3D point)
            {
                return TwoPatchesFoldingContext.GetRotationAngle(point, this.secondRotationInfo);
            }

            private static double GetRotationAngle(Point3D point, MeshPatchRotationCache rotationInfo)
            {
                Point3D projectedPoint = IntersectionsHelper.IntersectLineAndPlane(point, rotationInfo.Axis, rotationInfo.Center.Point, rotationInfo.Axis);
                double angle = rotationInfo.CalculateNormalizedAngleInDegrees(projectedPoint);

                return angle;
            }
        }
    }
}
