using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;
using Deyo.Core.Mathematics;
using Deyo.Core.Mathematics.Algebra;
using System.Collections.Generic;

namespace Deyo.Controls.Controls3D.Shapes
{
    public abstract class GeodesicSphere : ShapeBase, ISphereShape
    {
        private class PointIndicesCouple
        {
            private readonly int a;
            private readonly int b;

            public PointIndicesCouple(int a, int b)
            {
                this.a = a;
                this.b = b;
            }

            public override bool Equals(object obj)
            {
                PointIndicesCouple other = obj as PointIndicesCouple;

                if (other == null)
                {
                    return false;
                }

                return (this.a == other.a && this.b == other.b) || (this.a == other.b && this.b == other.a);
            }

            public override int GetHashCode()
            {
                return this.a ^ this.b;
            }
        }

        public const double Radius = 0.5;
        private static readonly Point3D Center = new Point3D();
        private static readonly Vector ZeroMeridianDirection = new Vector(1, 0);

        protected GeodesicSphere(int subDevisions, bool isSmooth, Point3D[] initialPoints, int[] initialTriangleIndices)
        {
            List<Point3D> points = new List<Point3D>(initialPoints);
            Queue<int> triangleIndices = new Queue<int>(initialTriangleIndices);

            for (int i = 0; i < subDevisions; i++)
            {
                // TODO: Recursion
            }

            Point[] textureCoordinates;
            GeodesicSphere.SplitTrianglesOnTextureMappingBoundaryAndGetTextureCoordinates(points, triangleIndices, out textureCoordinates);

            this.GeometryModel.Geometry = isSmooth ?
                GeodesicSphere.GenerateSmoothGeometry(points, triangleIndices, textureCoordinates) :
                GeodesicSphere.GenerateSharpGeometry(points, triangleIndices, textureCoordinates);
            this.GeometryModel.Geometry.Freeze();
        }

        public abstract SphereType SphereType { get; }

        public ShapeBase Shape
        {
            get
            {
                return this;
            }
        }

        private static MeshGeometry3D GenerateSmoothGeometry(List<Point3D> points, Queue<int> triangleIndices, Point[] textureCoordinates)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            for (int i = 0; i < points.Count; i++)
            {
                Point3D point = points[i];
                Point texturePoint = textureCoordinates[i];
                mesh.Positions.Add(point);
                mesh.TextureCoordinates.Add(texturePoint);
                Vector3D normal = point - Center;
                normal.Normalize();
                mesh.Normals.Add(normal);
            }

            while (triangleIndices.Count > 0)
            {
                mesh.TriangleIndices.Add(triangleIndices.Dequeue());
            }

            return mesh;
        }

        private static MeshGeometry3D GenerateSharpGeometry(List<Point3D> points, Queue<int> triangleIndices, Point[] textureCoordinates)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            while (triangleIndices.Count > 0)
            {
                int index = triangleIndices.Dequeue();
                Point3D point = points[index];
                Point texturePoint = textureCoordinates[index];
                mesh.Positions.Add(point);
                mesh.TextureCoordinates.Add(texturePoint);
            }

            for (int i = 0; i < mesh.Positions.Count; i++)
            {
                mesh.TriangleIndices.Add(i);
            }

            return mesh;
        }

        private static void SplitTrianglesOnTextureMappingBoundaryAndGetTextureCoordinates(List<Point3D> points, Queue<int> triangleIndices, out Point[] textureCoordinates)
        {
            int initialTrianglesIndicesCount = triangleIndices.Count;
            Dictionary<int, Point> pointIndexToTextureCoordinate = new Dictionary<int, Point>();
            Dictionary<int, int> zeroMeridianVertexToDuplicatedVertex = new Dictionary<int, int>();
            Dictionary<PointIndicesCouple, int[]> coupleToSplitMidpoints = new Dictionary<PointIndicesCouple, int[]>();

            Action<int, double> enqueueTriangleIndexWithTexture = (pointIndex, meridianAngle) =>
                {
                    triangleIndices.Enqueue(pointIndex);

                    if (!pointIndexToTextureCoordinate.ContainsKey(pointIndex))
                    {
                        Point texture = RotationalShape.GetTextureCoordinate(meridianAngle, points[pointIndex].Z, -Radius, Radius);
                        pointIndexToTextureCoordinate.Add(pointIndex, texture);
                    }
                };

            for (int i = 0; i < initialTrianglesIndicesCount; i+=3)
            {
                int[] currentTriangleIndices = { triangleIndices.Dequeue(), triangleIndices.Dequeue(), triangleIndices.Dequeue() };
                Point3D[] currentTrianglePoints = { points[currentTriangleIndices[0]], points[currentTriangleIndices[1]], points[currentTriangleIndices[2]] };
                double[] currentMeridianAngles = { GetMeridianAngle(currentTrianglePoints[0]), GetMeridianAngle(currentTrianglePoints[1]), GetMeridianAngle(currentTrianglePoints[2]) };

                bool hasModifiedCurrentTriangle = false;
                bool hasBorderMeridian = GeodesicSphere.HasBorderMeridian(currentMeridianAngles);

                if (hasBorderMeridian)
                {
                    bool hasPointInNegativeSemiplane = GeodesicSphere.HasPointInNegativeSemiplane(currentTrianglePoints);
                    bool hasPointInPositiveSemiplane = GeodesicSphere.HasPointInPositiveSemiplane(currentTrianglePoints);
                    bool shouldSplitTriangleTextures = hasPointInNegativeSemiplane && hasPointInPositiveSemiplane;

                    if (shouldSplitTriangleTextures)
                    {
                        int zeroAngleIndex = GeodesicSphere.FindZeroAngleIndex(currentMeridianAngles);
                        int duplicatePointIndex = GeodesicSphere.GetZeroAngleDuplicatePointIndex(zeroAngleIndex, currentTriangleIndices, zeroMeridianVertexToDuplicatedVertex, points, currentTrianglePoints);
                        int firstNonZeroIndex = (zeroAngleIndex + 1) % 3;
                        int secondNonZeroIndex = (zeroAngleIndex + 2) % 3;
                        int[] midPointIndices = GeodesicSphere.GetMidpointsIndices(firstNonZeroIndex, secondNonZeroIndex, coupleToSplitMidpoints, currentTrianglePoints, points, pointIndexToTextureCoordinate);
                        GeodesicSphere.EnqueueSplitTriangles(midPointIndices, points, currentTrianglePoints, firstNonZeroIndex, zeroAngleIndex, secondNonZeroIndex, duplicatePointIndex, currentMeridianAngles, enqueueTriangleIndexWithTexture);
                    }
                    else if (hasPointInNegativeSemiplane)
                    {
                        for (int index = 0; index < 3; index++)
                        {
                            if (currentMeridianAngles[index].IsZero())
                            {
                                int duplicatePointIndex = GeodesicSphere.GetZeroAngleDuplicatePointIndex(index, currentTriangleIndices, zeroMeridianVertexToDuplicatedVertex, points, currentTrianglePoints);
                                enqueueTriangleIndexWithTexture(duplicatePointIndex, RotationalShape.FullCircleAngleInRadians);
                            }
                            else
                            {
                                enqueueTriangleIndexWithTexture(currentTriangleIndices[index], currentMeridianAngles[index]);
                            }
                        }
                    }

                    hasModifiedCurrentTriangle = shouldSplitTriangleTextures || hasPointInNegativeSemiplane;
                }

                if (!hasModifiedCurrentTriangle)
                {
                    GeodesicSphere.AddTriangleWithTextures(currentTriangleIndices, currentMeridianAngles, enqueueTriangleIndexWithTexture);
                }
            }

            textureCoordinates = new Point[points.Count];

            for (int i = 0; i < points.Count; i++)
            {
                textureCoordinates[i] = pointIndexToTextureCoordinate[i];
            }
        }
  
        private static void EnqueueSplitTriangles(int[] midPointIndices, List<Point3D> points, Point3D[] currentTrianglePoints, int firstNonZeroIndex, int zeroAngleIndex, int secondNonZeroIndex, int duplicatePointIndex, double[] currentMeridianAngles, Action<int, double> enqueueTriangleIndexWithTexture)
        {
            double[] midPointMeridianAngles;
            if (midPointIndices.Length == 1)
            {
                midPointMeridianAngles = new double[] { GeodesicSphere.GetMeridianAngle(points[midPointIndices[0]]) };
            }
            else
            {
                midPointMeridianAngles = new double[] { 0, RotationalShape.FullCircleAngleInRadians };
            }

            int[] firstTriangleIndices, secondTriangleIndices;
            double[] firstTriangleMeridianAngles, secondTriangleMeridianAngles;

            if (currentTrianglePoints[firstNonZeroIndex].Z > 0)
            {
                firstTriangleIndices = new int[] { zeroAngleIndex, firstNonZeroIndex, midPointIndices.First() };
                secondTriangleIndices = new int[] { midPointIndices.Last(), secondNonZeroIndex, duplicatePointIndex };
                firstTriangleMeridianAngles = new double[] { 0, currentMeridianAngles[firstNonZeroIndex], midPointMeridianAngles.First() };
                secondTriangleMeridianAngles = new double[] { midPointMeridianAngles.Last(), currentMeridianAngles[secondNonZeroIndex], RotationalShape.FullCircleAngleInRadians };
            }
            else
            {
                firstTriangleIndices = new int[] { duplicatePointIndex, firstNonZeroIndex, midPointIndices.Last() };
                secondTriangleIndices = new int[] { midPointIndices.First(), secondNonZeroIndex, zeroAngleIndex };
                firstTriangleMeridianAngles = new double[] { RotationalShape.FullCircleAngleInRadians, currentMeridianAngles[firstNonZeroIndex], midPointMeridianAngles.Last() };
                secondTriangleMeridianAngles = new double[] { midPointMeridianAngles.First(), currentMeridianAngles[secondNonZeroIndex], 0 };
            }

            GeodesicSphere.AddTriangleWithTextures(firstTriangleIndices, firstTriangleMeridianAngles, enqueueTriangleIndexWithTexture);
            GeodesicSphere.AddTriangleWithTextures(secondTriangleIndices, secondTriangleMeridianAngles, enqueueTriangleIndexWithTexture);
        }
  
        private static void AddTriangleWithTextures(int[] currentTriangleIndices, double[] currentMeridianAngles, Action<int, double> enqueueTriangleIndexWithTexture)
        {
            for (int index = 0; index < 3; index++)
            {
                enqueueTriangleIndexWithTexture(currentTriangleIndices[index], currentMeridianAngles[index]);
            }
        }
  
        private static int[] GetMidpointsIndices(int firstNonZeroIndex, int secondNonZeroIndex, Dictionary<PointIndicesCouple, int[]> coupleToSplitMidpoints, Point3D[] currentTrianglePoints, List<Point3D> points, Dictionary<int, Point> pointIndexToTextureCoordinate)
        {
            PointIndicesCouple couple = new PointIndicesCouple(firstNonZeroIndex, secondNonZeroIndex);

            int[] midPoints;
            if (!coupleToSplitMidpoints.TryGetValue(couple, out midPoints))
            {
                Point3D midPoint = currentTrianglePoints[firstNonZeroIndex] + 0.5 * (currentTrianglePoints[secondNonZeroIndex] - currentTrianglePoints[firstNonZeroIndex]);
                double midPointMeridianAngle = GeodesicSphere.GetMeridianAngle(midPoint);

                if (midPointMeridianAngle.IsZero())
                {
                    midPoints = new int[] { points.Count, points.Count + 1 };
                    pointIndexToTextureCoordinate.Add(points.Count, RotationalShape.GetTextureCoordinate(0, midPoint.Z, -Radius, Radius));
                    pointIndexToTextureCoordinate.Add(points.Count + 1, RotationalShape.GetTextureCoordinate(RotationalShape.FullCircleAngleInRadians, midPoint.Z, -Radius, Radius));
                    points.Add(midPoint);
                    points.Add(midPoint);
                }
                else
                {
                    midPoints = new int[] { points.Count };
                    pointIndexToTextureCoordinate.Add(points.Count, RotationalShape.GetTextureCoordinate(midPointMeridianAngle, midPoint.Z, -Radius, Radius));
                    points.Add(midPoint);                                
                }

                coupleToSplitMidpoints.Add(couple, midPoints);
            }

            return midPoints;
        }
  
        private static int GetZeroAngleDuplicatePointIndex(int zeroAngleIndex, int[] currentTriangleIndices, Dictionary<int, int> zeroMeridianVertexToDuplicatedVertex, List<Point3D> points, Point3D[] currentTrianglePoints)
        {
            int duplicatePointIndex;
            if (!zeroMeridianVertexToDuplicatedVertex.TryGetValue(currentTriangleIndices[zeroAngleIndex], out duplicatePointIndex))
            {
                duplicatePointIndex = points.Count;
                points.Add(currentTrianglePoints[zeroAngleIndex]);
                zeroMeridianVertexToDuplicatedVertex.Add(currentTriangleIndices[zeroAngleIndex], duplicatePointIndex);
            }
            return duplicatePointIndex;
        }

        private static int FindZeroAngleIndex(double[] currentMeridianAngles)
        {
            for (int i = 0; i < currentMeridianAngles.Length; i++)
            {
                if (currentMeridianAngles[i].IsZero())
                {
                    return i;
                }
            }

            throw new InvalidOperationException("There is no zero meridian angle!");
        }

        private static bool HasPointInNegativeSemiplane(IEnumerable<Point3D> points)
        {
            foreach (Point3D point in points)
            {
                if (point.Y.IsLessThan(0))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasPointInPositiveSemiplane(IEnumerable<Point3D> points)
        {
            foreach (Point3D point in points)
            {
                if (point.Y.IsGreaterThan(0))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasBorderMeridian(IEnumerable<double> currentMeridianAngles)
        {
            foreach (double angle in currentMeridianAngles)
            {
                if (angle.IsZero())
                {
                    return true;
                }
            }

            return false;
        }

        private static double GetMeridianAngle(Point3D point)
        {
            Vector projection = new Vector(point.X, point.Y);
            double angleInRadians;

            if (projection.LengthSquared.IsZero())
            {
                angleInRadians = 0;
            }
            else
            {
                double angleInDegrees = Vector.AngleBetween(ZeroMeridianDirection, projection);
                angleInRadians = angleInDegrees.DegreesToRadians();
            }

            return angleInRadians;
        }
    }
}
