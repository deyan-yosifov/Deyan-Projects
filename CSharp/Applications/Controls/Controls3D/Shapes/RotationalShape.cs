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
        public RotationalShape(Point[][] sectionsInXZPlane, int meridiansCount, bool isSmooth)
        {
            base.GeometryModel.Geometry = this.GenerateGeometry(sectionsInXZPlane, meridiansCount, isSmooth);
        }
        
        private Geometry3D GenerateGeometry(Point[][] sectionsInXZPlane, int meridiansCount, bool isSmooth)
        {
            double minZ, maxZ;
            Point3D[][] sectionsPoints;
            CalculateSectionsPoints(sectionsInXZPlane, out sectionsPoints, out minZ, out maxZ);
            
            MeshGeometry3D geometry = isSmooth ? GenerateSmoothEdgesGeometry(sectionsPoints, minZ, maxZ, meridiansCount) : GenerateSharpEdgesGeometry(sectionsPoints, minZ, maxZ, meridiansCount);
            
            return geometry;
        }

        private MeshGeometry3D GenerateSharpEdgesGeometry(Point3D[][] sectionsPoints, double minZ, double maxZ, int meridiansCount)
        {
            MeshGeometry3D geometry = new MeshGeometry3D();

            for (int section = 0; section < sectionsPoints.Length; section++)
            {
                this.GenerateSharpEdgesGeometry(geometry, sectionsPoints[section], minZ, maxZ, meridiansCount);
            }

            return geometry;
        }

        private void GenerateSharpEdgesGeometry(MeshGeometry3D geometry, Point3D[] sectionPoints, double minZ, double maxZ, int meridiansCount)
        {
            Func<int, bool> isOnAxis = (parallelIndex) => { return sectionPoints[parallelIndex].X == 0; };

            Point3D[] firstPoints = sectionPoints.ToArray();
            Point3D[] secondPoints = new Point3D[firstPoints.Length];

            for (int meridian = 0; meridian < meridiansCount; meridian++)
            {
                double angle = GetMeridianRotationInRadians(meridian + 1, meridiansCount);

                for (int parallel = 0; parallel < sectionPoints.Length; parallel++)
                {
                    secondPoints[parallel] = RotateSectionPoint(sectionPoints[parallel], angle, meridian + 1, meridiansCount);
                }

                Func<Point3D, int, int> addPointToGeometry = (point, meridianIndex) => { return AddPointToGeometry(geometry, point, meridianIndex, meridiansCount, minZ, maxZ); };
                Action<Point3D, Point3D, Point3D> addPointsToGeomety = (first, second, third) =>
                {
                    AddTriangle(geometry, addPointToGeometry(first, meridian), addPointToGeometry(second, meridian), addPointToGeometry(third, meridian + 1));
                };

                for (int parallel = 0; parallel < sectionPoints.Length - 1; parallel++)
                {
                    if (isOnAxis(parallel))
                    {
                        if (!isOnAxis(parallel + 1))
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
                        AddTriangle(geometry, geometry.Positions.Count - 3, geometry.Positions.Count - 1, addPointToGeometry(secondPoints[parallel], meridian + 1));
                    }
                }

                Point3D[] swap = firstPoints;
                firstPoints = secondPoints;
                secondPoints = swap;
            }
        }

        private MeshGeometry3D GenerateSmoothEdgesGeometry(Point3D[][] sectionsPoints, double minZ, double maxZ, int meridiansCount)
        {
            MeshGeometry3D geometry = new MeshGeometry3D();

            for (int section = 0; section < sectionsPoints.Length; section++)
            {
                this.GenerateSmoothEdgesGeometry(geometry, sectionsPoints[section], minZ, maxZ, meridiansCount);
            }

            return geometry;
        }

        private void GenerateSmoothEdgesGeometry(MeshGeometry3D geometry, Point3D[] sectionPoints, double minZ, double maxZ, int meridiansCount)
        {
            Func<int, bool> isOnAxis = (parallelIndex) => { return sectionPoints[parallelIndex].X == 0; };

            int[] firstPoints = new int[sectionPoints.Length];
            int[] secondPoints = new int[firstPoints.Length];
            for (int i = 0; i < firstPoints.Length; i++)
            {
                firstPoints[i] = AddPointToGeometry(geometry, sectionPoints[i], 0, meridiansCount, minZ, maxZ);
            }

            for (int meridian = 0; meridian < meridiansCount; meridian++)
            {
                double angle = GetMeridianRotationInRadians(meridian + 1, meridiansCount);

                for (int parallel = 0; parallel < sectionPoints.Length; parallel++)
                {
                    Point3D rotatedPoint = RotateSectionPoint(sectionPoints[parallel], angle, meridian + 1, meridiansCount);
                    secondPoints[parallel] = AddPointToGeometry(geometry, rotatedPoint, meridian + 1, meridiansCount, minZ, maxZ);
                }

                for (int parallel = 0; parallel < sectionPoints.Length - 1; parallel++)
                {
                    if (isOnAxis(parallel))
                    {
                        if (!isOnAxis(parallel + 1))
                        {
                            AddTriangle(geometry, firstPoints[parallel], firstPoints[parallel + 1], secondPoints[parallel + 1]);
                        }
                    }
                    else if (isOnAxis(parallel + 1))
                    {
                        AddTriangle(geometry, firstPoints[parallel], firstPoints[parallel + 1], secondPoints[parallel]);
                    }
                    else
                    {
                        AddTriangle(geometry, firstPoints[parallel], firstPoints[parallel + 1], secondPoints[parallel + 1]);
                        AddTriangle(geometry, firstPoints[parallel], secondPoints[parallel + 1], secondPoints[parallel]);
                    }
                }

                int[] swap = firstPoints;
                firstPoints = secondPoints;
                secondPoints = swap;
            }
        }

        private static void AddTriangle(MeshGeometry3D geometry, int first, int second, int third)
        {
            geometry.TriangleIndices.Add(first);
            geometry.TriangleIndices.Add(second);
            geometry.TriangleIndices.Add(third);
        }

        private static Point3D RotateSectionPoint(Point3D sectionPoint, double angle, int meridian, int meridiansCount)
        {
            if (sectionPoint.X == 0 || meridian == meridiansCount)
            {
                return sectionPoint;
            }
            else
            {
                return new Point3D(sectionPoint.X * Math.Cos(angle), sectionPoint.X * Math.Sin(angle), sectionPoint.Z);
            }
        }

        private static int AddPointToGeometry(MeshGeometry3D geometry, Point3D point, int meridian, int meridiansCount, double minZ, double maxZ)
        {
            geometry.Positions.Add(point);
            geometry.TextureCoordinates.Add(GetTextureCoordinate(meridian, meridiansCount, point.Z, minZ, maxZ));

            return geometry.Positions.Count - 1;
        }

        private static void CalculateSectionsPoints(Point[][] sectionsInXZPlane, out Point3D[][] sectionsPoints, out double minZ, out double maxZ)
        {
            minZ = double.MaxValue;
            maxZ = double.MinValue;
            sectionsPoints = new Point3D[sectionsInXZPlane.Length][];

            for(int section = 0; section < sectionsPoints.Length; section++)
            {
                Point[] sectionInXZPlane = sectionsInXZPlane[section];
                Point3D[] sectionPoints = new Point3D[sectionInXZPlane.Length];
                sectionsPoints[section] = sectionPoints;

                for (int i = 0; i < sectionPoints.Length; i++)
                {
                    Point point = sectionInXZPlane[i];
                    sectionPoints[i] = new Point3D(point.X, 0, point.Y);
                    minZ = Math.Min(minZ, point.Y);
                    maxZ = Math.Max(maxZ, point.Y);
                }
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
