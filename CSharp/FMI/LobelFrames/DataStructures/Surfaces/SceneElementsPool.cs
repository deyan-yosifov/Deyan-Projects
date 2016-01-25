using Deyo.Controls.Controls3D;
using Deyo.Controls.Controls3D.Cameras;
using Deyo.Controls.Controls3D.Iteractions;
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
        private readonly Line reusableUnitLineShape;
        private readonly ShapeBase reusableUnitPointShape;
        private readonly Visual3DPool<MeshVisual> meshPool;
        private readonly Visual3DPool<LineVisual> surfaceLinesPool;
        private readonly Visual2DPool<LineOverlay> lineOverlaysPool;
        private readonly Visual3DPool<PointVisual> controlPointsPool;
        private readonly Dictionary<LineOverlay, Tuple<Point3D, Point3D>> lineOverlayToSegment3D;
        private readonly HashSet<LineOverlay> visibleLineOverlays;
        private readonly Dictionary<Visual3D, IteractiveSurface> visual3dToSurfaceOwner;
        private readonly OrbitControl orbitControl;
        private readonly IteractivePointsHandler iteractivePointsHandler;

        public SceneElementsPool(Scene3D scene)
        {
            this.scene = scene;
            this.orbitControl = this.scene.OrbitControl;
            this.iteractivePointsHandler = this.scene.IteractivePointsHandler;
            this.controlPointsPool = new Visual3DPool<PointVisual>(scene);
            this.surfaceLinesPool = new Visual3DPool<LineVisual>(scene);
            this.meshPool = new Visual3DPool<MeshVisual>(scene);
            this.lineOverlaysPool = new Visual2DPool<LineOverlay>();
            this.lineOverlayToSegment3D = new Dictionary<LineOverlay, Tuple<Point3D, Point3D>>();
            this.visual3dToSurfaceOwner = new Dictionary<Visual3D, IteractiveSurface>();
            this.visibleLineOverlays = new HashSet<LineOverlay>();

            this.reusableUnitLineShape = this.CreateReusableLineShape();
            this.reusableUnitPointShape = this.CreateReusablePointShape();
            this.PrepareGraphicStateForDrawingMeshesAndOverlays();

            this.SceneEditor.CameraChanged += this.CameraChangedHandler;

            this.InitializePointerHandlers();
        }

        private void InitializePointerHandlers()
        {
            this.orbitControl.IsEnabled = true;
            this.iteractivePointsHandler.IsEnabled = false;
        }

        private void PrepareGraphicStateForDrawingMeshesAndOverlays()
        {
            this.SceneEditor.GraphicProperties.MaterialsManager.AddFrontDiffuseMaterial(SceneConstants.SurfaceGeometryColor);
            this.SceneEditor.GraphicProperties.MaterialsManager.AddBackDiffuseMaterial(SceneConstants.SurfaceGeometryColor);
            this.SceneEditor.GraphicProperties.Graphics2D.StrokeThickness = SceneConstants.LineOverlaysThickness;
            this.SceneEditor.GraphicProperties.Graphics2D.Stroke = SceneConstants.LineOverlaysColor;
        }

        private ShapeBase CreateReusablePointShape()
        {
            using (this.SceneEditor.SaveGraphicProperties())
            {
                this.SceneEditor.GraphicProperties.ArcResolution = SceneConstants.ControlPointsArcResolution;
                this.SceneEditor.GraphicProperties.MaterialsManager.AddFrontDiffuseMaterial(SceneConstants.ControlPointsColor);

                return this.SceneEditor.ShapeFactory.CreateSphere();
            }
        }

        private Line CreateReusableLineShape()
        {
            using (this.SceneEditor.SaveGraphicProperties())
            {
                this.SceneEditor.GraphicProperties.MaterialsManager.AddFrontDiffuseMaterial(SceneConstants.SurfaceLinesColor);
                this.SceneEditor.GraphicProperties.ArcResolution = SceneConstants.SurfaceLinesArcResolution;

                return this.SceneEditor.ShapeFactory.CreateLine();
            }
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
                visual = this.SceneEditor.AddLineOverlay();
            }

            lineOverlayToSegment3D.Add(visual, new Tuple<Point3D, Point3D>(fromPoint, toPoint));
            this.UpdateLineOverlayPosition(visual, fromPoint, toPoint);

            return visual;
        }

        public PointVisual CreatePoint(Point3D point)
        {
            PointVisual visual;
            if (!this.controlPointsPool.TryPopElementFromPool(out visual))
            {
                this.SceneEditor.GraphicProperties.Thickness = SceneConstants.ControlPointsDiameter;
                visual = this.SceneEditor.AddPointVisual(point, this.reusableUnitPointShape);
            }

            return visual;
        }

        public LineVisual CreateSurfaceLine(IteractiveSurface owner, Point3D fromPoint, Point3D toPoint)
        {
            LineVisual visual;
            if (!this.surfaceLinesPool.TryPopElementFromPool(out visual))
            {
                this.SceneEditor.GraphicProperties.Thickness = SceneConstants.SurfaceLinesDiameter;
                visual = this.SceneEditor.AddLineVisual(fromPoint, toPoint, this.reusableUnitLineShape);
            }

            this.AddVisualOwnerMapping(visual, owner);

            return visual;
        }

        public MeshVisual CreateMesh(IteractiveSurface owner)
        {
            MeshVisual visual;
            if (!this.meshPool.TryPopElementFromPool(out visual))
            {                  
                visual = this.SceneEditor.AddMeshVisual();
            }

            this.AddVisualOwnerMapping(visual, owner);

            return visual;
        }

        public void DeleteLineOverlay(LineOverlay visual)
        {
            this.lineOverlayToSegment3D.Remove(visual);
            this.lineOverlaysPool.PushElementToPool(visual);
            this.visibleLineOverlays.Remove(visual);
        }

        public void DeletePoint(PointVisual visual)
        {
            this.controlPointsPool.PushElementToPool(visual);
        }

        public void DeleteSurfaceLine(LineVisual visual)
        {
            this.surfaceLinesPool.PushElementToPool(visual);
            this.RemoveVisualOwnerMapping(visual);
        }

        public void DeleteMesh(MeshVisual visual)
        {
            this.meshPool.PushElementToPool(visual);
            this.RemoveVisualOwnerMapping(visual);
        }

        public bool TryGetSurfaceFromPoint(Point viewportPosition, out IteractiveSurface surface)
        {
            surface = null;
            Visual3D visual;
            bool success = this.SceneEditor.TryHitVisual3D(viewportPosition, out visual) && 
                this.visual3dToSurfaceOwner.TryGetValue(visual, out surface);

            return success;
        }

        private void CameraChangedHandler(object sender, EventArgs e)
        {
            foreach (var lineToPoints in this.lineOverlayToSegment3D)
            {
                LineOverlay line = lineToPoints.Key;
                Tuple<Point3D, Point3D> points = lineToPoints.Value;
                this.UpdateLineOverlayPosition(line, points.Item1, points.Item2);
            }
        }

        private void UpdateLineOverlayPosition(LineOverlay overlay, Point3D start, Point3D end)
        {
            Point startPoint, endPoint;            
            if (this.SceneEditor.TryGetLineSegmentInVisibleSemiSpace(start, end, out startPoint, out endPoint))
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

        private void AddVisualOwnerMapping(IVisual3DOwner visualOwner, IteractiveSurface surfaceOwner)
        {
            this.visual3dToSurfaceOwner.Add(visualOwner.Visual, surfaceOwner);
        }

        private void RemoveVisualOwnerMapping(IVisual3DOwner visualOwner)
        {
            this.visual3dToSurfaceOwner.Remove(visualOwner.Visual);
        }
    }
}
