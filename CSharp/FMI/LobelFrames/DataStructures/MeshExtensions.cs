using Deyo.Core.Mathematics.Algebra;
using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures
{
    public static class MeshExtensions
    {
        public static Vertex GetFirstVertex(this Edge edge, Vector3D direction)
        {
            Vector3D edgeDirection = edge.End.Point - edge.Start.Point;

            if (edgeDirection.IsSameSemiSpaceDirection(direction))
            {
                return edge.Start;
            }
            else
            {
                return edge.End;
            }
        }

        public static Vertex GetLastVertex(this Edge edge, Vector3D direction)
        {
            Vector3D edgeDirection = edge.End.Point - edge.Start.Point;

            if (edgeDirection.IsSameSemiSpaceDirection(direction))
            {
                return edge.End;
            }
            else
            {
                return edge.Start;
            }
        }
    }
}
