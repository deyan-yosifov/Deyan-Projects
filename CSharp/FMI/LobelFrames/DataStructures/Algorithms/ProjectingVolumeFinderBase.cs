﻿using Deyo.Core.Mathematics.Geometry.Algorithms;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal abstract class ProjectingVolumeFinderBase : TriangleIterationHandlerBase
    {
        private readonly HashSet<UVMeshDescretePosition> verticesFromIntersectingTriangles;
        private double totalOrientedVolume;
        private double totalCommonArea;

        public ProjectingVolumeFinderBase(OctaTetraApproximationContext approximationContext, Triangle lobelMeshTriangle)
            : base(approximationContext, lobelMeshTriangle)
        {
            this.verticesFromIntersectingTriangles = new HashSet<UVMeshDescretePosition>();
            this.totalOrientedVolume = 0;
            this.totalCommonArea = 0;
        }

        public double ResultAbsoluteVolume
        {
            get
            {
                return this.verticesFromIntersectingTriangles.Count > 0 ? Math.Abs(this.totalOrientedVolume) : double.MaxValue;
            }
        }

        public double ResultCommonArea
        {
            get
            {
                return this.totalCommonArea;
            }
        }

        public IEnumerable<UVMeshDescretePosition> VerticesFromIntersectingMeshTriangles
        {
            get
            {
                return this.verticesFromIntersectingTriangles;
            }
        }

        protected abstract void GetProjectionInfo(UVMeshTriangleInfo uvMeshTriangle,
            out Point3D a, out Point3D b, out Point3D c, out TriangleProjectionContext projectionContext);

        protected sealed override TriangleIterationResult HandleNextInterationTriangleOverride(UVMeshTriangleInfo uvMeshTriangle)
        {
            Point3D a, b, c;
            TriangleProjectionContext projectionContext;
            this.GetProjectionInfo(uvMeshTriangle, out a, out b, out c, out projectionContext);
            bool addNeigboursToRecursion = false;

            double orientedVolume, commonArea;
            if (ProjectionIntersections.TryFindCommonProjectionVolume(projectionContext, a, b, c, out orientedVolume, out commonArea))
            {
                this.totalOrientedVolume += orientedVolume;
                this.totalCommonArea += commonArea;
                this.verticesFromIntersectingTriangles.Add(uvMeshTriangle.A);
                this.verticesFromIntersectingTriangles.Add(uvMeshTriangle.B);
                this.verticesFromIntersectingTriangles.Add(uvMeshTriangle.C);
                addNeigboursToRecursion = true;
            }

            return new TriangleIterationResult(false, addNeigboursToRecursion);
        }
    }
}
