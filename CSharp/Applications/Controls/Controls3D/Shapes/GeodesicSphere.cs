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
        private class PointIndexWithMeridianAngle
        {
            private readonly int index;
            private readonly double angle;

            public PointIndexWithMeridianAngle(int index, double angle)
            {
                this.index = index;
                this.angle = angle;
            }

            public int Index
            {
                get
                {
                    return index;
                }
            }

            public double Angle
            {
                get
                {
                    return angle;
                }
            }

            public override string ToString()
            {
                return string.Format("<i:{0}, a:{1}>", this.Index, this.Angle);
            }
        }

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

            public override string ToString()
            {
                return string.Format("<{0}, {1}>", this.a, this.b);
            }
        }

        public const double Radius = 0.5;
        private static readonly Point3D Center = new Point3D();
        private static readonly Vector ZeroMeridianDirection = new Vector(1, 0);

        protected GeodesicSphere(int subDevisions, bool isSmooth, Point3D[] initialPoints, int[] initialTriangleIndices)
        {
            List<Point3D> points = new List<Point3D>(initialPoints);
            Queue<int> triangleIndices = new Queue<int>(initialTriangleIndices);

            GeodesicSphere.DoSubDevisions(subDevisions, points, triangleIndices);

            Point[] textureCoordinates;
            GeodesicSphere.SplitTrianglesOnTextureMappingBoundaryAndGetTextureCoordinates(points, triangleIndices, out textureCoordinates);

            this.GeometryModel.Geometry = isSmooth ?
                GeodesicSphere.GenerateSmoothGeometry(points, triangleIndices, textureCoordinates) :
                GeodesicSphere.GenerateSharpGeometry(points, triangleIndices, textureCoordinates);

            // TODO: Delete this commented code and the corresponding method!
            //GeodesicSphere.AddFastSharpTriangles((MeshGeometry3D)this.GeometryModel.Geometry, initialPoints, initialTriangleIndices);

            this.GeometryModel.Geometry.Freeze();
        }

        private static MeshGeometry3D AddFastSharpTriangles(MeshGeometry3D mesh, Point3D[] points, IEnumerable<int> triangleIndices)
        {
            int initialPointsCount = mesh.Positions.Count;
            int i = 0;

            foreach (int index in triangleIndices)
            {
                mesh.Positions.Add(points[index]);
                mesh.TriangleIndices.Add(i++ + initialPointsCount);
            }

            return mesh;
        }

        public abstract SphereType SphereType { get; }

        public ShapeBase Shape
        {
            get
            {
                return this;
            }
        }

        private static void DoSubDevisions(int subDevisions, List<Point3D> points, Queue<int> triangleIndices)
        {
            for (int subDevision = 0; subDevision < subDevisions; subDevision++)
            {
                Dictionary<PointIndicesCouple, int> subDevisionMidPointsCache = new Dictionary<PointIndicesCouple, int>();
                int triangleIndicesBeforeSubDevision = triangleIndices.Count;

                for (int i = 0; i < triangleIndicesBeforeSubDevision; i += 3)
                {
                    int aIndex = triangleIndices.Dequeue();
                    int bIndex = triangleIndices.Dequeue();
                    int cIndex = triangleIndices.Dequeue();
                    int abMidPoint = GeodesicSphere.GetSubDevisionMidPointIndex(aIndex, bIndex, points, subDevisionMidPointsCache);
                    int bcMidPoint = GeodesicSphere.GetSubDevisionMidPointIndex(bIndex, cIndex, points, subDevisionMidPointsCache);
                    int caMidPoint = GeodesicSphere.GetSubDevisionMidPointIndex(cIndex, aIndex, points, subDevisionMidPointsCache);

                    triangleIndices.Enqueue(aIndex);
                    triangleIndices.Enqueue(abMidPoint);
                    triangleIndices.Enqueue(caMidPoint);
                    triangleIndices.Enqueue(abMidPoint);
                    triangleIndices.Enqueue(bIndex);
                    triangleIndices.Enqueue(bcMidPoint);
                    triangleIndices.Enqueue(caMidPoint);
                    triangleIndices.Enqueue(bcMidPoint);
                    triangleIndices.Enqueue(cIndex);
                    triangleIndices.Enqueue(abMidPoint);
                    triangleIndices.Enqueue(bcMidPoint);
                    triangleIndices.Enqueue(caMidPoint);
                }
            }
        }

        private static int GetSubDevisionMidPointIndex(int aIndex, int bIndex, List<Point3D> points, Dictionary<PointIndicesCouple, int> subDevisionMidPointsCache)
        {
            PointIndicesCouple couple = new PointIndicesCouple(aIndex, bIndex);

            int midPointIndex;
            if (!subDevisionMidPointsCache.TryGetValue(couple, out midPointIndex))
            {
                midPointIndex = points.Count;
                subDevisionMidPointsCache.Add(couple, midPointIndex);

                Point3D a = points[aIndex];
                Point3D b = points[bIndex];
                Point3D midPoint = a + 0.5 * (b - a);
                Vector3D midPointVector = midPoint - Center;
                midPointVector = midPointVector * (Radius / midPointVector.Length);
                Point3D subDevisionMidPoint = Center + midPointVector;
                points.Add(subDevisionMidPoint);
            }

            return midPointIndex;
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
            Dictionary<PointIndicesCouple, PointIndexWithMeridianAngle[]> coupleToSplitMidpoints = new Dictionary<PointIndicesCouple, PointIndexWithMeridianAngle[]>();

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
                        int zeroIndex = GeodesicSphere.IndexOfZeroMeridianAngle(currentMeridianAngles);
                        PointIndexWithMeridianAngle[] triangle = new PointIndexWithMeridianAngle[]
                        {
                            new PointIndexWithMeridianAngle(currentTriangleIndices[zeroIndex], currentMeridianAngles[zeroIndex]),
                            new PointIndexWithMeridianAngle(currentTriangleIndices[(zeroIndex + 1) % 3], currentMeridianAngles[(zeroIndex + 1) % 3]),
                            new PointIndexWithMeridianAngle(currentTriangleIndices[(zeroIndex + 2) % 3], currentMeridianAngles[(zeroIndex + 2) % 3]),
                        };
                        int duplicatePointIndex = GeodesicSphere.GetZeroAngleDuplicatePointIndex(triangle[0].Index, points, zeroMeridianVertexToDuplicatedVertex);
                        PointIndexWithMeridianAngle[] midPoints = GeodesicSphere.GetMidpointsIndices(triangle[1].Index, triangle[2].Index, points, coupleToSplitMidpoints, pointIndexToTextureCoordinate);
                        GeodesicSphere.SplitAndEnqueueTriangle(points, triangle, midPoints, duplicatePointIndex, enqueueTriangleIndexWithTexture);
                    }
                    else if (hasPointInNegativeSemiplane)
                    {
                        for (int index = 0; index < 3; index++)
                        {
                            if (currentMeridianAngles[index].IsZero())
                            {
                                int duplicatePointIndex = GeodesicSphere.GetZeroAngleDuplicatePointIndex(currentTriangleIndices[index], points, zeroMeridianVertexToDuplicatedVertex);
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
                // TODO: Delete this commented code!
                //Point texture;
                //if (!pointIndexToTextureCoordinate.TryGetValue(i, out texture))
                //{
                //    texture = new Point();
                //}

                //textureCoordinates[i] = texture;
                textureCoordinates[i] = pointIndexToTextureCoordinate[i];
            }
        }

        private static void SplitAndEnqueueTriangle(List<Point3D> points, PointIndexWithMeridianAngle[] triangle, PointIndexWithMeridianAngle[] midPoints,
            int duplicatePointIndex, Action<int, double> enqueueTriangleIndexWithTexture)
        {
            int[] firstTriangleIndices, secondTriangleIndices;
            double[] firstTriangleMeridianAngles, secondTriangleMeridianAngles;

            if (points[triangle[1].Index].Z > 0)
            {
                firstTriangleIndices = new int[] { triangle[0].Index, triangle[1].Index, midPoints.First().Index };
                secondTriangleIndices = new int[] { midPoints.Last().Index, triangle[2].Index, duplicatePointIndex };
                firstTriangleMeridianAngles = new double[] { 0, triangle[1].Angle, midPoints.First().Angle };
                secondTriangleMeridianAngles = new double[] { midPoints.Last().Angle, triangle[2].Angle, RotationalShape.FullCircleAngleInRadians };
            }
            else
            {
                firstTriangleIndices = new int[] { duplicatePointIndex, triangle[1].Index, midPoints.Last().Index };
                secondTriangleIndices = new int[] { midPoints.First().Index, triangle[2].Index, triangle[0].Index };
                firstTriangleMeridianAngles = new double[] { RotationalShape.FullCircleAngleInRadians, triangle[1].Angle, midPoints.Last().Angle };
                secondTriangleMeridianAngles = new double[] { midPoints.First().Angle, triangle[2].Angle, 0 };
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

        private static PointIndexWithMeridianAngle[] GetMidpointsIndices(int firstNonZeroIndex, int secondNonZeroIndex, List<Point3D> points, Dictionary<PointIndicesCouple, PointIndexWithMeridianAngle[]> coupleToSplitMidpoints, Dictionary<int, Point> pointIndexToTextureCoordinate)
        {
            PointIndicesCouple couple = new PointIndicesCouple(firstNonZeroIndex, secondNonZeroIndex);

            PointIndexWithMeridianAngle[] midPoints;
            if (!coupleToSplitMidpoints.TryGetValue(couple, out midPoints))
            {
                Point3D midPoint = points[firstNonZeroIndex] + 0.5 * (points[secondNonZeroIndex] - points[firstNonZeroIndex]);
                double midPointMeridianAngle = GeodesicSphere.GetMeridianAngle(midPoint);

                if (midPointMeridianAngle.IsZero())
                {
                    midPoints = new PointIndexWithMeridianAngle[] 
                    { 
                        new PointIndexWithMeridianAngle(points.Count, 0), 
                        new PointIndexWithMeridianAngle(points.Count + 1, RotationalShape.FullCircleAngleInRadians) 
                    };
                    pointIndexToTextureCoordinate.Add(midPoints[0].Index, RotationalShape.GetTextureCoordinate(midPoints[0].Angle, midPoint.Z, -Radius, Radius));
                    pointIndexToTextureCoordinate.Add(midPoints[1].Index, RotationalShape.GetTextureCoordinate(midPoints[1].Angle, midPoint.Z, -Radius, Radius));
                    points.Add(midPoint);
                    points.Add(midPoint);
                }
                else
                {
                    midPoints = new PointIndexWithMeridianAngle[] { new PointIndexWithMeridianAngle(points.Count, midPointMeridianAngle) };
                    pointIndexToTextureCoordinate.Add(points.Count, RotationalShape.GetTextureCoordinate(midPointMeridianAngle, midPoint.Z, -Radius, Radius));
                    points.Add(midPoint);                                
                }

                coupleToSplitMidpoints.Add(couple, midPoints);
            }

            return midPoints;
        }
  
        private static int GetZeroAngleDuplicatePointIndex(int zeroAngleIndex, List<Point3D> points, Dictionary<int, int> zeroMeridianVertexToDuplicatedVertex)
        {
            int duplicatePointIndex;
            if (!zeroMeridianVertexToDuplicatedVertex.TryGetValue(zeroAngleIndex, out duplicatePointIndex))
            {
                duplicatePointIndex = points.Count;
                points.Add(points[zeroAngleIndex]);
                zeroMeridianVertexToDuplicatedVertex.Add(zeroAngleIndex, duplicatePointIndex);
            }

            return duplicatePointIndex;
        }

        private static int IndexOfZeroMeridianAngle(double[] currentMeridianAngles)
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

                if (angleInRadians < 0)
                {
                    angleInRadians = RotationalShape.FullCircleAngleInRadians + angleInRadians;
                }
            }

            return angleInRadians;
        }
    }
}
