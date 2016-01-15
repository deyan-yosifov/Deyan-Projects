using Deyo.Controls.Controls3D;
using Deyo.Controls.Controls3D.Shapes;
using Deyo.Controls.Controls3D.Visuals;
using Deyo.Controls.Controls3D.Visuals.Overlays2D;
using LobelFrames.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace LobelFrames.DataStructures.Surfaces
{
    public class SceneElementsPool : ISceneElementsManager
    {
        private readonly Scene3D scene;
        private readonly Visual3DPool<MeshVisual> meshPool;
        private readonly Visual3DPool<LineVisual> surfaceLinesPool;
        private readonly Visual2DPool<LineOverlay> lineOverlaysPool;
        private readonly Visual3DPool<PointVisual> controlPointsPool;
        private readonly Dictionary<LineOverlay, Tuple<Point3D, Point3D>> lineOverlayToSegment3D;
        private readonly HashSet<LineOverlay> visibleLineOverlays;

        public SceneElementsPool(Scene3D scene)
        {
            this.scene = scene;
            this.controlPointsPool = new Visual3DPool<PointVisual>(scene);
            this.surfaceLinesPool = new Visual3DPool<LineVisual>(scene);
            this.meshPool = new Visual3DPool<MeshVisual>(scene);
            this.lineOverlaysPool = new Visual2DPool<LineOverlay>();
            this.lineOverlayToSegment3D = new Dictionary<LineOverlay, Tuple<Point3D, Point3D>>();
            this.visibleLineOverlays = new HashSet<LineOverlay>();

            this.SceneEditor.CameraChanged += this.CameraChangedHandler;
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
                    this.SceneEditor.GraphicProperties.Graphics2D.StrokeThickness = SceneConstants.LineOverlaysThickness;
                    this.SceneEditor.GraphicProperties.Graphics2D.Stroke = SceneConstants.LineOverlaysColor;

                    visual = this.SceneEditor.AddLineOverlay();
                }
            }

            lineOverlayToSegment3D.Add(visual, new Tuple<Point3D, Point3D>(fromPoint, toPoint));
            this.MoveLineOverlay(visual, fromPoint, toPoint);

            return visual;
        }

        public LineVisual CreateSurfaceLine(Point3D fromPoint, Point3D toPoint)
        {
            LineVisual visual;
            if (!this.surfaceLinesPool.TryPopElementFromPool(out visual))
            {
                using (this.SceneEditor.SaveGraphicProperties())
                {
                    this.SceneEditor.GraphicProperties.MaterialsManager.AddFrontDiffuseMaterial(SceneConstants.SurfaceLinesColor);
                    this.SceneEditor.GraphicProperties.ArcResolution = SceneConstants.SurfaceLinesArcResolution;
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
            this.lineOverlayToSegment3D.Remove(visual);
            this.lineOverlaysPool.PushElementToPool(visual);
        }

        private void CameraChangedHandler(object sender, EventArgs e)
        {
            foreach (var lineToPoints in this.lineOverlayToSegment3D)
            {
                LineOverlay line = lineToPoints.Key;
                Tuple<Point3D, Point3D> points = lineToPoints.Value;
                this.MoveLineOverlay(line, points.Item1, points.Item2);
            }
        }

        private void MoveLineOverlay(LineOverlay overlay, Point3D start, Point3D end)
        {
            Point startPoint, endPoint;
            if (this.SceneEditor.TryGetPointFromPoint3D(start, out startPoint) && this.SceneEditor.TryGetPointFromPoint3D(end, out endPoint))
            {
                overlay.Start = startPoint;
                overlay.End = endPoint;
                if (this.visibleLineOverlays.Add(overlay))
                {
                    overlay.Visual.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (this.visibleLineOverlays.Remove(overlay))
                {
                    overlay.Visual.Visibility = Visibility.Collapsed;
                }
            }            
        }
    }
}
