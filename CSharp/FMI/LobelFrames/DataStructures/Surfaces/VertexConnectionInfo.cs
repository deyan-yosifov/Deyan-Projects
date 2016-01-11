using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Surfaces
{
    public class VertexConnectionInfo
    {
        public VertexConnectionInfo(Edge[] connectingEdges, bool hasEdgesOnBothSides, Vector3D firstPlaneNormal)
        {
            this.ConnectingEdges = connectingEdges;
            this.HasEdgesOnBothSides = hasEdgesOnBothSides;
            this.FirstPlaneNormal = firstPlaneNormal;
        }

        public Edge[] ConnectingEdges { get; private set; }
        public bool HasEdgesOnBothSides { get; private set; }
        public Vector3D FirstPlaneNormal { get; private set; }
    }
}
