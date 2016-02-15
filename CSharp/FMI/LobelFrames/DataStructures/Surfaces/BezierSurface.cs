using Deyo.Controls.Controls3D.Visuals;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Surfaces
{
    public class BezierSurface : IteractiveSurface
    {
        public BezierSurface(ISceneElementsManager sceneManager)
            : base(sceneManager)
        {
        }

        public override SurfaceType Type
        {
            get
            {
                return SurfaceType.Bezier;
            }
        }

        protected override IMeshElementsProvider ElementsProvider
        {
            get { throw new NotImplementedException(); }
        }

        public override void Select()
        {
            throw new NotImplementedException();
        }

        public override void Deselect()
        {
            throw new NotImplementedException();
        }

        public override void Move(Vector3D direction)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Edge> GetContour()
        {
            throw new NotImplementedException();
        }
    }
}
