using Deyo.Controls.Controls3D.Visuals;
using System;

namespace LobelFrames.DataStructures.Surfaces
{
    public class BezierSurface : IteractiveSurface
    {
        public BezierSurface(ISceneElementsManager sceneManager)
            : base(sceneManager)
        {
        }

        // TODO:
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
    }
}
