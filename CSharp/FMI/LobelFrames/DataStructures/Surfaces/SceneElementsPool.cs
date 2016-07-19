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
        private readonly Visual2DPool<LineOverlay> movingLineOverlaysPool;
        private readonly Visual3DPool<PointVisual> controlPointsPool;
        private readonly Dictionary<LineOverlay, Tuple<Point3D, Point3D>> lineOverlayToSegment3D;
        private readonly HashSet<LineOverlay> visibleLineOverlays;
        private readonly Dictionary<Visual3D, IteractiveSurface> visual3dToSurfaceOwner;
        private readonly Dictionary<Visual3D, PointVisual> visual3dToPointOwner;
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
            this.movingLineOverlaysPool = new Visual2DPool<LineOverlay>();
            this.lineOverlayToSegment3D = new Dictionary<LineOverlay, Tuple<Point3D, Point3D>>();
            this.visual3dToSurfaceOwner = new Dictionary<Visual3D, IteractiveSurface>();
            this.visual3dToPointOwner = new Dictionary<Visual3D, PointVisual>();
            this.visibleLineOverlays = new HashSet<LineOverlay>();

            this.reusableUnitLineShape = this.CreateReusableLineShape();
            this.reusableUnitPointShape = this.CreateReusablePointShape();
            this.PrepareGraphicStateForDrawingMeshesAndOverlays();

            this.SceneEditor.CameraChanged += this.CameraChangedHandler;
            scene.SizeChanged += this.SceneSizeChanged;

            this.InitializePointerHandlers();
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
            return this.CreateLineOverlay(this.lineOverlaysPool, null, fromPoint, toPoint);
        }

        public LineOverlay BeginMovingLineOverlay(Point3D startPoint)
        {
            return this.CreateLineOverlay(this.movingLineOverlaysPool, SceneConstants.StrokeDashArray, startPoint, startPoint);
        }

        public PointVisual CreatePoint(Point3D point)
        {
            PointVisual visual;
            if (this.controlPointsPool.TryPopElementFromPool(out visual))
            {
                visual.Position = point;
            }
            else
            {
                this.SceneEditor.GraphicProperties.Thickness = SceneConstants.ControlPointsDiameter;
                visual = this.SceneEditor.AddPointVisual(point, this.reusableUnitPointShape);
            }

            this.AddPointVisualMapping(visual);

            return visual;
        }

        public LineVisual CreateSurfaceLine(IteractiveSurface owner, Point3D fromPoint, Point3D toPoint)
        {
            LineVisual visual;
            if (this.surfaceLinesPool.TryPopElementFromPool(out visual))
            {
                visual.MoveTo(fromPoint, toPoint);
            }
            else
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
            this.DeleteLineOverlay(this.lineOverlaysPool, visual);
        }

        public void DeleteMovingLineOverlay(LineOverlay visual)
        {
            this.DeleteLineOverlay(this.movingLineOverlaysPool, visual);
        }

        public void DeletePoint(PointVisual visual)
        {
            this.controlPointsPool.PushElementToPool(visual);
            this.RemovePointVisualMapping(visual);
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

        public bool TryGetSurfaceFromViewPoint(Point viewportPosition, out IteractiveSurface surface)
        {
            return this.TryGetElementFromViewPoint(this.visual3dToSurfaceOwner, viewportPosition, out surface);
        }

        public bool TryGetPointFromViewPoint(Point viewportPosition, out PointVisual point)
        {
            return this.TryGetElementFromViewPoint(this.visual3dToPointOwner, viewportPosition, out point);
        }

        public void MoveLineOverlay(LineOverlay line, Point3D endPoint)
        {
            Tuple<Point3D, Point3D> oldPosition = this.lineOverlayToSegment3D[line];
            this.MoveLineOverlay(line, oldPosition.Item1, endPoint);
        }

        public void MoveLineOverlay(LineOverlay line, Point3D startPoint, Point3D endPoint)
        {
            Tuple<Point3D, Point3D> newPosition = new Tuple<Point3D, Point3D>(startPoint, endPoint);
            this.lineOverlayToSegment3D[line] = newPosition;
            this.UpdateLineOverlayPosition(line, newPosition.Item1, newPosition.Item2);
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
                this.SceneEditor.GraphicProperties.SphereType = SceneConstants.ControlPointsSphereType;
                this.SceneEditor.GraphicProperties.SubDevisions = SceneConstants.ControlPointsSubDevisions;
                this.SceneEditor.GraphicProperties.MaterialsManager.AddFrontDiffuseMaterial(SceneConstants.ControlPointsColor);

                return this.SceneEditor.ShapeFactory.CreateSphere().Shape;
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

        private void CameraChangedHandler(object sender, EventArgs e)
        {
            this.UpdateLineOverlays();
        }

        private void SceneSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateLineOverlays();
        }

        private void UpdateLineOverlays()
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

        private void AddPointVisualMapping(PointVisual point)
        {
            this.visual3dToPointOwner.Add(point.Visual, point);
        }

        private void RemovePointVisualMapping(PointVisual point)
        {
            this.visual3dToPointOwner.Remove(point.Visual);
        }

        private void AddVisualOwnerMapping(IVisual3DOwner visualOwner, IteractiveSurface surfaceOwner)
        {
            this.visual3dToSurfaceOwner.Add(visualOwner.Visual, surfaceOwner);
        }

        private void RemoveVisualOwnerMapping(IVisual3DOwner visualOwner)
        {
            this.visual3dToSurfaceOwner.Remove(visualOwner.Visual);
        }

        private bool TryGetElementFromViewPoint<T>(Dictionary<Visual3D, T> visual3dToElementOwnerMapping, Point viewportPosition, out T element)
        {
            element = default(T);
            Visual3D visual;
            bool success = this.SceneEditor.TryHitVisual3D(viewportPosition, out visual) &&
                visual3dToElementOwnerMapping.TryGetValue(visual, out element);

            return success;
        }

        private LineOverlay CreateLineOverlay(Visual2DPool<LineOverlay> pool, double[] strokeDashArray, Point3D fromPoint, Point3D toPoint)
        {
            LineOverlay visual;
            if (!pool.TryPopElementFromPool(out visual))
            {
                this.SceneEditor.GraphicProperties.Graphics2D.StrokeDashArray = strokeDashArray;
                visual = this.SceneEditor.AddLineOverlay();
            }

            this.lineOverlayToSegment3D.Add(visual, new Tuple<Point3D, Point3D>(fromPoint, toPoint));
            this.UpdateLineOverlayPosition(visual, fromPoint, toPoint);

            return visual;
        }

        private void DeleteLineOverlay(Visual2DPool<LineOverlay> pool, LineOverlay visual)
        {
            pool.PushElementToPool(visual);
            this.lineOverlayToSegment3D.Remove(visual);
            this.visibleLineOverlays.Remove(visual);
        }        
    }
}
