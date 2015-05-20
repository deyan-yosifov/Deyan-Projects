using Deyo.Controls.Contols3D.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Deyo.Controls.Controls3D.Shapes
{
    public class RotationalShape : ShapeBase
    {
        public static readonly Vector3D RotationAxis = new Vector3D(0, 0, 1);

        public RotationalShape(Point[] sectionInXZPlane, int meridiansCount, bool isSmooth)
        {
            base.GeometryModel.Geometry = this.GenerateGeometry(sectionInXZPlane, meridiansCount, isSmooth);
        }

        private Geometry3D GenerateGeometry(Point[] sectionInXZPlane, int meridiansCount, bool isSmooth)
        {
            double minZ, maxZ;
            Point3D[] sectionPoints;
            CalculateSectionPoints(sectionInXZPlane, out sectionPoints, out minZ, out maxZ);
            
            MeshGeometry3D geometry = isSmooth ? GenerateSmoothEdgesGeometry(sectionPoints, minZ, maxZ, meridiansCount) : GenerateSharpEdgesGeometry(sectionPoints, minZ, maxZ, meridiansCount);
            
            return geometry;
        }

        private MeshGeometry3D GenerateSharpEdgesGeometry(Point3D[] sectionPoints, double minZ, double maxZ, int meridiansCount)
        {
            MeshGeometry3D geometry = new MeshGeometry3D();

            Func<int, bool> isOnAxis = (parallelIndex) => { return sectionPoints[parallelIndex].X == 0; };

            Point3D[] firstPoints = sectionPoints.ToArray();
            Point3D[] secondPoints = new Point3D[firstPoints.Length];
            
            for (int meridian = 0; meridian < meridiansCount; meridian++)
            {
                double angle = GetMeridianRotationInRadians(meridian + 1, meridiansCount);

                for (int parallel = 0; parallel < sectionPoints.Length; parallel++)
                {
                    Point3D sectionPoint = sectionPoints[parallel];
                    Point3D rotatedPoint = (isOnAxis(parallel) || (meridian == meridiansCount - 1))
                        ? sectionPoint 
                        : new Point3D(sectionPoint.X * Math.Cos(angle), sectionPoint.X * Math.Sin(angle), sectionPoint.Z);

                    secondPoints[parallel] = rotatedPoint;
                }

                Func<Point3D, int, int> addPointToGeometry = (point, meridianIndex) => { return AddPointToGeometry(geometry, point, meridianIndex, meridiansCount, minZ, maxZ); };
                Action<Point3D, Point3D, Point3D> addPointsToGeomety = (first, second, third) =>
                    {
                        geometry.TriangleIndices.Add(addPointToGeometry(first, meridian));
                        geometry.TriangleIndices.Add(addPointToGeometry(second, meridian));
                        geometry.TriangleIndices.Add(addPointToGeometry(third, meridian + 1));
                    };

                for (int parallel = 0; parallel < sectionPoints.Length - 1; parallel++)
                {
                    if (isOnAxis(parallel))
                    {
                        if(!isOnAxis(parallel + 1))
                        {
                            addPointsToGeomety(firstPoints[parallel], firstPoints[parallel + 1], secondPoints[parallel + 1]);
                        }
                    }
                    else if (isOnAxis(parallel + 1))
                    {
                        addPointsToGeomety(firstPoints[parallel], firstPoints[parallel + 1], secondPoints[parallel]);
                    }
                    else
                    {
                        addPointsToGeomety(firstPoints[parallel], firstPoints[parallel + 1], secondPoints[parallel + 1]);
                        geometry.TriangleIndices.Add(geometry.Positions.Count - 3);
                        geometry.TriangleIndices.Add(geometry.Positions.Count - 1);
                        geometry.TriangleIndices.Add(addPointToGeometry(secondPoints[parallel], meridian + 1));
                    }
                }

                Point3D[] swap = firstPoints;
                firstPoints = secondPoints;
                secondPoints = swap;
            }

            return geometry;
        }

        private MeshGeometry3D GenerateSmoothEdgesGeometry(Point3D[] sectionPoints, double minZ, double maxZ, int meridiansCount)
        {
            throw new NotImplementedException();
        }

        private static int AddPointToGeometry(MeshGeometry3D geometry, Point3D point, int meridian, int meridiansCount, double minZ, double maxZ)
        {
            geometry.Positions.Add(point);
            geometry.TextureCoordinates.Add(GetTextureCoordinate(meridian, meridiansCount, point.Z, minZ, maxZ));

            return geometry.Positions.Count - 1;
        }

        private static void CalculateSectionPoints(Point[] sectionInXZPlane, out Point3D[] sectionPoints, out double minZ, out double maxZ)
        {
            minZ = double.MaxValue;
            maxZ = double.MinValue;
            sectionPoints = new Point3D[sectionInXZPlane.Length];

            for(int i = 0; i < sectionPoints.Length; i++)
            {
                Point point = sectionInXZPlane[i];
                sectionPoints[i] = new Point3D(point.X, 0, point.Y);
                minZ = Math.Min(minZ, point.Y);
                maxZ = Math.Max(maxZ, point.Y);
            }
        }

        private static Point GetTextureCoordinate(int meridian, int meridiansCount, double parallelZ, double minZ, double maxZ)
        {
            double u = meridian / ((double) meridiansCount);
            double v = (maxZ - parallelZ) / (maxZ - minZ);

            return new Point(u, v);
        }

        private static double GetMeridianRotationInRadians(int meridian, int meridiansCount)
        {
            return meridian * (2 * Math.PI / meridiansCount);
        }
    }
}
