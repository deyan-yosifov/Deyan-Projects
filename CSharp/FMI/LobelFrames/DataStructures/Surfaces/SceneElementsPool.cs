using Deyo.Controls.Controls3D;
using Deyo.Controls.Controls3D.Shapes;
using Deyo.Controls.Controls3D.Visuals;
using Deyo.Controls.Controls3D.Visuals.Overlays2D;
using LobelFrames.ViewModels;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Surfaces
{
    public class SceneElementsPool : ISceneElementsManager
    {
        private readonly Scene3D scene;
        private readonly Visual3DPool<PointVisual> controlPointsPool;
        private readonly Visual3DPool<LineVisual> surfaceLinesPool;
        private readonly Visual3DPool<MeshVisual> meshPool;
        private readonly Visual2DPool<LineOverlay> lineOverlaysPool;

        public SceneElementsPool(Scene3D scene)
        {
            this.scene = scene;
            this.controlPointsPool = new Visual3DPool<PointVisual>(scene);
            this.surfaceLinesPool = new Visual3DPool<LineVisual>(scene);
            this.meshPool = new Visual3DPool<MeshVisual>(scene);
            this.lineOverlaysPool = new Visual2DPool<LineOverlay>();
        }

        private SceneEditor SceneEditor
        {
            get
            {
                return this.scene.Editor;
            }
        }

        public LineOverlay CreateLineOverlay(Point3D fromPoint, Point3D toPoint)
        {
            LineOverlay visual;
            if (!lineOverlaysPool.TryPopElementFromPool(out visual))
            {
                using (this.SceneEditor.SaveGraphicProperties())
                {
                    this.SceneEditor.GraphicProperties.Graphics2D.Stroke = SceneConstants.LineOverlaysColor;
                    this.SceneEditor.GraphicProperties.Graphics2D.StrokeThickness = SceneConstants.LineOverlaysThickness;

                    this.SceneEditor.AddLineOverlay(fromPoint, toPoint);
                }
            }

            return visual;
        }

        public LineVisual CreateSurfaceLine(Point3D fromPoint, Point3D toPoint)
        {
            LineVisual visual;
            if (!this.surfaceLinesPool.TryPopElementFromPool(out visual))
            {
                using (this.SceneEditor.SaveGraphicProperties())
                {
                    this.SceneEditor.GraphicProperties.ArcResolution = SceneConstants.SurfaceLinesArcResolution;
                    this.SceneEditor.GraphicProperties.MaterialsManager.AddFrontDiffuseMaterial(SceneConstants.SurfaceLinesColor);
                    this.SceneEditor.GraphicProperties.Thickness = SceneConstants.SurfaceLinesDiameter;

                    this.SceneEditor.AddLineVisual(fromPoint, toPoint);
                }
            }

            return visual;
        }

        public MeshVisual CreateMesh()
        {
            MeshVisual visual;
            if (!this.meshPool.TryPopElementFromPool(out visual))
            {
                using (this.SceneEditor.SaveGraphicProperties())
                {
                    this.SceneEditor.GraphicProperties.MaterialsManager.AddFrontDiffuseMaterial(SceneConstants.SurfaceGeometryColor);
                    this.SceneEditor.GraphicProperties.MaterialsManager.AddBackDiffuseMaterial(SceneConstants.SurfaceGeometryColor);                    
                    visual = this.SceneEditor.AddMeshVisual();
                }
            }

            return visual;
        }

        public void DeleteLine(LineVisual visual)
        {
            this.surfaceLinesPool.PushElementToPool(visual);
        }

        public void DeleteMesh(MeshVisual visual)
        {
            this.meshPool.PushElementToPool(visual);
        }

        public void DeleteLine(LineOverlay visual)
        {
            this.lineOverlaysPool.PushElementToPool(visual);
        }
    }
}
