using Deyo.Core.Common;
using Deyo.Core.Mathematics.Algebra;
using Deyo.Core.Mathematics.Geometry;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Surfaces
{
    public class MeshPatchRotationCache
    {
        private readonly IMeshElementsRelationsProvider elementsProvider;
        private readonly MeshPatchVertexSelectionInfo meshPatch;
        private readonly HashSet<Edge> edges;
        private readonly Dictionary<Vertex, Point3D> vertexToRotatedPositionCache;
        private readonly Vertex center;
        private readonly Vector3D axis;
        private readonly Vector3D boundaryDirection;
        private readonly Vector3D zeroAngleVector;
        private readonly Vector3D positiveAnglesNormal;
        private double previousAngle;
        private Matrix3D currentRotationMatrix;

        public MeshPatchRotationCache(IMeshElementsRelationsProvider elementsProvider, MeshPatchVertexSelectionInfo meshPatch, Vertex rotationCenter, Vector3D rotationAxis, Vector3D boundaryDirection)
        {
            Guard.ThrowExceptionIfNull(elementsProvider, "elementsProvider");
            Guard.ThrowExceptionIfNull(meshPatch, "meshPatch");
            Guard.ThrowExceptionIfTrue(Vector3D.CrossProduct(rotationAxis, boundaryDirection).LengthSquared.IsZero(), "rotationAxis and boundaryDirection vectors cannot be coplanar!");
                        
            this.elementsProvider = elementsProvider;
            this.meshPatch = meshPatch;
            this.center = rotationCenter;
            this.axis = rotationAxis;
            this.axis.Normalize();
            this.boundaryDirection = boundaryDirection;
            this.boundaryDirection.Normalize();

            Point3D planePoint = this.center.Point + boundaryDirection;
            Point3D projectedRotationPlanePoint = IntersectionsHelper.IntersectLineAndPlane(planePoint, rotationAxis, this.center.Point, rotationAxis);
            Vector3D zeroAngleVector = projectedRotationPlanePoint - this.center.Point;
            this.zeroAngleVector = zeroAngleVector;
            this.zeroAngleVector.Normalize();
            this.positiveAnglesNormal = Vector3D.CrossProduct(this.axis, this.zeroAngleVector);

            this.edges = new HashSet<Edge>();

            foreach (Vertex vertex in this.meshPatch.AllPatchVertices)
            {
                foreach (Edge edge in this.elementsProvider.GetEdges(vertex))
                {
                    if (meshPatch.IsVertexFromPatch(edge.Start) && meshPatch.IsVertexFromPatch(edge.End))
                    {
                        this.edges.Add(edge);
                    }
                }
            }

            this.vertexToRotatedPositionCache = new Dictionary<Vertex, Point3D>();

            foreach (Vertex vertex in this.meshPatch.AllPatchVertices)
            {
                this.vertexToRotatedPositionCache[vertex] = vertex.Point;
            }

            this.previousAngle = 0;
            this.currentRotationMatrix = Matrix3D.Identity;
        }

        public Point3D this[Vertex vertex]
        {
            get
            {
                return this.vertexToRotatedPositionCache[vertex];
            }
        }

        public Vertex Center
        {
            get
            {
                return this.center;
            }
        }

        public Vector3D Axis
        {
            get
            {
                return this.axis;
            }
        }

        public Vector3D BoundaryDirection
        {
            get
            {
                return this.boundaryDirection;
            }
        }

        public Vector3D ZeroAngleVector
        {
            get
            {
                return this.zeroAngleVector;
            }
        }

        public MeshPatchVertexSelectionInfo MeshPatch
        {
            get
            {
                return this.meshPatch;
            }
        }

        public double CurrentRotationAngle
        {
            get
            {
                return this.previousAngle;
            }
        }

        public Matrix3D CurrentRotationMatrix
        {
            get
            {
                return this.currentRotationMatrix;
            }
        }

        public IEnumerable<Vertex> AxisVertices
        {
            get
            {
                foreach (Vertex vertex in this.MeshPatch.AllPatchVertices)
                {
                    if ((vertex.Point - this.Center.Point).IsColinear(this.Axis))
                    {
                        yield return vertex;
                    }
                }
            }
        }

        public void PrepareCacheForRotation(Point3D rotationPlanePoint)
        {
            double angleInDegrees = this.CalculateNormalizedAngleInDegrees(rotationPlanePoint);

            this.PrepareCacheForRotation(angleInDegrees);
        }

        public void PrepareCacheForRotation(double angleInDegrees)
        {
            Guard.ThrowExceptionIfNotInRange(angleInDegrees, 0, 360, true, false, "angleInDegrees");

            if (this.previousAngle != angleInDegrees)
            {
                this.CalculateRotationCache(angleInDegrees);
                this.previousAngle = angleInDegrees;
            }
        }

        public double CalculateNormalizedAngleInDegrees(Point3D rotationPlanePoint)
        {
            Vector3D rotationDirection = rotationPlanePoint - this.Center.Point;
            double axisCoordinate = Vector3D.DotProduct(this.Axis, rotationDirection);
            Guard.ThrowExceptionIfTrue(rotationDirection.LengthSquared.IsZero(), "rotationPlanePoint cannot coinside with rotation center!");
            Guard.ThrowExceptionIfFalse(axisCoordinate.IsZero(), "rotationPlanePoint must be in the rotation plane!");

            double angleSignCoordinate = Vector3D.DotProduct(this.positiveAnglesNormal, rotationDirection);
            double angleInDegrees = Vector3D.AngleBetween(this.zeroAngleVector, rotationDirection);

            if (angleSignCoordinate.IsLessThan(0))
            {
                angleInDegrees = 360 - angleInDegrees;
            }

            return angleInDegrees;
        }

        public IEnumerable<Tuple<Point3D, Point3D>> GetRotatedEdges(Point3D rotationPlanePoint)
        {
            this.PrepareCacheForRotation(rotationPlanePoint);

            return this.GetRotatedEdges();
        }

        public IEnumerable<Tuple<Point3D, Point3D>> GetRotatedEdges(double rotationAngle)
        {
            this.PrepareCacheForRotation(rotationAngle);

            return this.GetRotatedEdges();
        }

        private IEnumerable<Tuple<Point3D, Point3D>> GetRotatedEdges()
        {
            foreach (Edge edge in this.edges)
            {
                yield return new Tuple<Point3D, Point3D>(this.vertexToRotatedPositionCache[edge.Start], this.vertexToRotatedPositionCache[edge.End]);
            }
        }

        private void CalculateRotationCache(double angleInDegrees)
        {
            Matrix3D matrix = new Matrix3D();
            matrix.RotateAt(new Quaternion(this.axis, angleInDegrees), this.center.Point);

            foreach (Vertex vertex in this.meshPatch.AllPatchVertices)
            {
                this.vertexToRotatedPositionCache[vertex] = matrix.Transform(vertex.Point);
            }

            this.currentRotationMatrix = matrix;
        }
    }
}
