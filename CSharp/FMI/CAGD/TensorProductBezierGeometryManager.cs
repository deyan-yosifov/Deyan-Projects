using Deyo.Controls.Controls3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace CAGD
{
    public class TensorProductBezierGeometryManager
    {
        private readonly Scene3D scene;

        public TensorProductBezierGeometryManager(Scene3D scene)
        {
            this.scene = scene;
        }

        public SceneEditor SceneEditor
        {
            get
            {
                return this.scene.Editor;
            }
        }

        public void GenerateGeometry(Point3D[,] controlPoints)
        {
            // TODO:
            this.SceneEditor.GraphicProperties.ArcResolution = 6;
            this.SceneEditor.GraphicProperties.MaterialsManager.AddFrontDiffuseMaterial(Color.FromRgb(160, 160, 160));

            this.scene.IteractivePointsHandler.RegisterIteractivePoint(this.SceneEditor.AddCubePointVisual(new Point3D()));
        }
    }
}
