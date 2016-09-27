using Deyo.Controls.Controls3D.Visuals;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Surfaces
{
    public class BezierSurface : IteractiveSurface
    {
        private readonly BezierMesh mesh;

        public BezierSurface(ISceneElementsManager sceneManager, int uDevisions, int vDevisions, int uDegree, int vDegree, double width, double height)
            : base(sceneManager)
        {
            this.mesh = new BezierMesh(uDevisions, vDevisions, uDegree, vDegree, width, height);
        }

        public override SurfaceType Type
        {
            get
            {
                return SurfaceType.Bezier;
            }
        }

        public override IMeshElementsProvider ElementsProvider
        {
            get
            {
                return this.mesh;
            }
        }

        protected override IEnumerable<Vertex> SurfaceVerticesToRender
        {
            get
            {
                for (int u = 0; u <= this.mesh.UDegree; u++)
                {
                    for (int v = 0; v <= this.mesh.VDegree; v++)
                    {
                        yield return new Vertex(this.mesh[u, v]);
                    }
                }
            }
        }

        protected override void MoveMeshVertices(Vector3D moveDirection)
        {
            this.mesh.MoveMeshVertices(moveDirection);
        }

        public override IEnumerable<Edge> GetContour()
        {
            return this.mesh.Contour;
        }
    }
}
