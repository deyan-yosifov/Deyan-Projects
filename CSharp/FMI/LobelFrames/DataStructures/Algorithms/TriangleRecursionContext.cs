using Deyo.Core.Common;
using Deyo.Core.Mathematics.Algebra;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Algorithms
{
    internal class TriangleRecursionContext
    {
        private readonly Triangle triangle;
        private readonly OctaTetraApproximationContext context;
        private bool hasEnqueuedSteps;

        public TriangleRecursionContext(Triangle triangle, OctaTetraApproximationContext context)
        {
            this.triangle = triangle;
            this.context = context;
            this.hasEnqueuedSteps = false;
        }

        private Point3D A
        {
            get
            {
                return this.triangle.A.Point;
            }
        }

        private Point3D B
        {
            get
            {
                return this.triangle.B.Point;
            }
        }

        private Point3D C
        {
            get
            {
                return this.triangle.C.Point;
            }
        }

        private UVMeshDescretePosition? AInitialRecursionPosition { get; set; }

        private UVMeshDescretePosition? BInitialRecursionPosition { get; set; }

        private UVMeshDescretePosition? CInitialRecursionPosition { get; set; }
        
        public void Update(UVMeshDescretePosition positionToCheck, Point3D barycentricCoordinates)
        {
            Guard.ThrowExceptionIfTrue(this.hasEnqueuedSteps, "hasEnqueuedSteps");

            if (barycentricCoordinates.X.IsLessThan(0))
            {
                this.AInitialRecursionPosition = positionToCheck;
            }

            if (barycentricCoordinates.Y.IsLessThan(0))
            {
                this.BInitialRecursionPosition = positionToCheck;
            }

            if (barycentricCoordinates.Z.IsLessThan(0))
            {
                this.CInitialRecursionPosition = positionToCheck;
            }
        }

        public void EnqueueRecursionSteps()
        {
            Guard.ThrowExceptionIfTrue(this.hasEnqueuedSteps, "hasEnqueuedSteps");

            this.hasEnqueuedSteps = true;

            foreach (OctaTetraApproximationStep step in this.CalculateNextApproximationSteps())
            {
                this.context.RecursionQueue.Enqueue(step);
            }
        }

        private IEnumerable<OctaTetraApproximationStep> CalculateNextApproximationSteps()
        {
            OctaTetraApproximationStep aStep;
            if(this.TryCalculateApproximationStep(this.AInitialRecursionPosition, 0, out aStep))
            {
                yield return aStep;
            }

            OctaTetraApproximationStep bStep;
            if (this.TryCalculateApproximationStep(this.BInitialRecursionPosition, 1, out bStep))
            {
                yield return bStep;
            }

            OctaTetraApproximationStep cStep;
            if (this.TryCalculateApproximationStep(this.CInitialRecursionPosition, 2, out cStep))
            {
                yield return cStep;
            }
        }

        private bool TryCalculateApproximationStep(UVMeshDescretePosition? recursionPosition, int sideIndex, out OctaTetraApproximationStep step)
        {
            if(recursionPosition.HasValue)
            {
                Triangle[] bundle = this.CreateNonExistingNeigbouringTriangles(sideIndex).ToArray();

                if(bundle.Length != 0)
                {
                    step = new OctaTetraApproximationStep()
                    {
                        InitialRecursionPosition = recursionPosition.Value,
                        TrianglesBundle = bundle
                    };

                    return true;
                }
            }

            step = null;
            return false;
        }

        private IEnumerable<Triangle> CreateNonExistingNeigbouringTriangles(int sideIndex)
        {
            Vertex opositeVertex = this.triangle.GetVertex(sideIndex);
            Vertex edgeStart = this.triangle.GetVertex((sideIndex + 1) % 3);
            Vertex edgeEnd = this.triangle.GetVertex((sideIndex + 2) % 3);
            Vector3D triangleUnitNormal = Vector3D.CrossProduct(this.B - this.A, this.C - this.A);
            triangleUnitNormal.Normalize();

            Point3D triangleCenter = opositeVertex.Point + (1.0 / 3) * ((edgeStart.Point - opositeVertex.Point) + (edgeEnd.Point - opositeVertex.Point));
            Point3D tetrahedronTop = triangleCenter + this.context.TetrahedronHeight * triangleUnitNormal;
            Point3D edgeCenter = edgeStart.Point + 0.5 * (edgeEnd.Point - edgeStart.Point);
            Point3D octahedronPoint = edgeCenter + (edgeCenter - opositeVertex.Point);
            Point3D oppositeTetrahedronTop = edgeCenter + (edgeCenter - tetrahedronTop);

            Triangle tetrahedronTriangle;
            if (this.context.TryCreateNonExistingTriangle(edgeEnd.Point, edgeStart.Point, tetrahedronTop, out tetrahedronTriangle))
            {
                yield return tetrahedronTriangle;
            }

            Triangle octahedronTriangle;
            if (this.context.TryCreateNonExistingTriangle(edgeStart.Point, edgeEnd.Point, octahedronPoint, out octahedronTriangle))
            {
                yield return octahedronTriangle;
            }

            Triangle oppositeTetrahedronTriangle;
            if (this.context.TryCreateNonExistingTriangle(edgeEnd.Point, edgeStart.Point, oppositeTetrahedronTop, out oppositeTetrahedronTriangle))
            {
                yield return oppositeTetrahedronTriangle;
            }
        }
    }
}
