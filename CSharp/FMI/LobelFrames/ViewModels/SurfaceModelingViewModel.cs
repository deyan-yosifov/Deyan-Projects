using Deyo.Controls.Common;
using Deyo.Controls.Controls3D;
using Deyo.Controls.Controls3D.Cameras;
using Deyo.Controls.Controls3D.Iteractions;
using Deyo.Controls.Controls3D.Visuals;
using Deyo.Controls.Controls3D.Visuals.Overlays2D;
using Deyo.Core.Common;
using Deyo.Core.Common.History;
using Deyo.Core.Mathematics.Geometry;
using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using LobelFrames.FormatProviders;
using LobelFrames.IteractionHandling;
using LobelFrames.ViewModels.Commands;
using LobelFrames.ViewModels.Commands.Handlers;
using LobelFrames.ViewModels.Commands.History;
using LobelFrames.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace LobelFrames.ViewModels
{
    public class SurfaceModelingViewModel : ViewModelBase, ILobelSceneEditor
    {
        private readonly Scene3D scene;
        private readonly HintManager hintManager;
        private readonly InputManager inputManager;
        private readonly SettingsViewModel settings;
        private readonly CommandDescriptors commandDescriptors;
        private readonly SceneElementsPool elementsPool;
        private readonly SurfaceModelingContext context;
        private readonly SurfaceModelingPointerHandler surfacePointerHandler;
        private readonly ZoomToContentsPointerHandler zoomToContentsPointerHandler;

        public SurfaceModelingViewModel(Scene3D scene)
        {
            this.scene = scene;
            this.hintManager = new HintManager();
            this.inputManager = new InputManager();
            this.elementsPool = new SceneElementsPool(scene);
            this.context = new SurfaceModelingContext(CommandHandlersFactory.CreateCommandHandlers(this, this.elementsPool));
            this.settings = new SettingsViewModel(this.context.HistoryManager);
            this.commandDescriptors = new CommandDescriptors(this);
            this.surfacePointerHandler = new SurfaceModelingPointerHandler(this.elementsPool, scene.Editor);
            this.zoomToContentsPointerHandler = new ZoomToContentsPointerHandler(this.ZoomToContents);
            this.scene.PointerHandlersController.Handlers.AddFirst(this.surfacePointerHandler);
            this.scene.PointerHandlersController.Handlers.AddFirst(this.zoomToContentsPointerHandler);

            this.AttachToEvents();
            this.InitializeScene();
        }

        public SceneElementsPool ElementsPool
        {
            get
            {
                return this.elementsPool;
            }
        }

        public CommandDescriptors CommandDescriptors
        {
            get
            {
                return this.commandDescriptors;
            }
        }

        public HintManager HintManager
        {
            get
            {
                return this.hintManager;
            }
        }

        public InputManager InputManager
        {
            get
            {
                return this.inputManager;
            }
        }

        public SettingsViewModel Settings
        {
            get
            {
                return this.settings;
            }
        }

        public ILobelSceneContext Context
        {
            get
            {
                return this.context;
            }
        }

        public ISurfaceModelingPointerHandler SurfacePointerHandler
        {
            get
            {
                return this.surfacePointerHandler;
            }
        }

        private HistoryManager HistoryManager
        {
            get
            {
                return this.context.HistoryManager;
            }
        }

        private CommandContext CommandContext
        {
            get
            {
                return this.context.CommandContext;
            }
        }

        public void Save()
        {
            this.CommandContext.BeginCommand(CommandType.Save);
        }

        public void Open()
        {
            this.CommandContext.BeginCommand(CommandType.Open);
        }

        public void Undo()
        {
            this.HistoryManager.Undo();
        }

        public void Redo()
        {
            this.HistoryManager.Redo();
        }

        public void ChangeGeneralSettings()
        {
            this.Settings.GeneralSettings.IsOpen = !this.Settings.GeneralSettings.IsOpen;
        }

        public void ChangeLobelSettings()
        {
            this.Settings.LobelSettings.IsOpen = !this.Settings.LobelSettings.IsOpen;
        }

        public void ChangeBezierSettings()
        {
            this.Settings.BezierSettings.IsOpen = !this.Settings.BezierSettings.IsOpen;
        }

        public void BeforeCommandExecuted(CommandType type)
        {
            if (type != CommandType.Settings)
            {
                this.Settings.GeneralSettings.IsOpen = false;
            }

            if (type != CommandType.LobelSettings)
            {
                this.Settings.LobelSettings.IsOpen = false;
            }

            if (type != CommandType.BezierSettings)
            {
                this.Settings.BezierSettings.IsOpen = false;
            }
        }

        public void AddLobelMesh()
        {
            using (this.HistoryManager.BeginUndoGroup())
            {
                LobelSurface surface = new LobelSurface(this.ElementsPool, this.Settings.LobelSettings.MeshRows, this.Settings.LobelSettings.MeshColumns, 3);
                this.DoAction(new AddSurfaceAction(surface, this.Context));
                this.DoAction(new SelectSurfaceAction(surface, this.Context));
            }
        }

        public void SelectMesh()
        {
            this.CommandContext.BeginCommand(CommandType.SelectMesh);
        }

        public void Deselect()
        {
            this.DoAction(new DeselectSurfaceAction(this.Context));
        }

        public void DeleteMesh()
        {
            this.DoAction(new DeleteSurfaceAction(this.Context));
        }

        public void MoveMesh()
        {
            this.CommandContext.BeginCommand(CommandType.MoveMesh);
        }

        public void CutMesh()
        {
            this.CommandContext.BeginCommand(CommandType.CutMesh);
        }

        public void EnableSurfacePointerHandler(IteractionHandlingType iteractionType)
        {
            this.surfacePointerHandler.IteractionType = iteractionType;
            this.surfacePointerHandler.IsEnabled = true;
            this.scene.PointerHandlersController.HandleMoveWhenNoHandlerIsCaptured = true;
        }

        public void DisableSurfacePointerHandler()
        {
            this.surfacePointerHandler.IsEnabled = false;
            this.scene.PointerHandlersController.HandleMoveWhenNoHandlerIsCaptured = false;
        }

        public void ShowHint(string hint, HintType hintType)
        {
            this.HintManager.Hint = hint;
            this.HintManager.HintType = hintType;
        }

        public void DoAction(IUndoRedoAction action)
        {
            this.HistoryManager.PushUndoableAction(action);
        }

        public void CloseCommandContext()
        {
            this.CommandContext.EndCommand();
            this.DisableSurfacePointerHandler();
            this.ShowHint(Hints.Default, HintType.Info);
            this.InputManager.Stop();
        }

        public LobelScene SaveScene()
        {
            LobelScene scene = new LobelScene();

            scene.Camera = new CameraModel();
            this.scene.Editor.DoActionOnCamera(
                (perspectiveCamera) =>
                {
                    scene.Camera.LookDirection = perspectiveCamera.LookDirection;
                    scene.Camera.Position = perspectiveCamera.Position;
                    scene.Camera.UpDirection = perspectiveCamera.UpDirection;
                },  (orthographicCamera) => Guard.ThrowNotSupportedCameraException());

            scene.AddSurfaces(LobelSceneFormatProviderBase.GetSurfaceModels(this.Context));

            return scene;
        }

        public void LoadScene(LobelScene scene)
        {
            this.Context.Clear();

            foreach (SurfaceModel model in scene.Surfaces)
            {
                IteractiveSurface surface = LobelSceneFormatProviderBase.CreateIteractiveSurface(this.ElementsPool, model);
                this.Context.AddSurface(surface);

                if (model.IsSelected)
                {
                    this.Context.SelectedSurface = surface;
                }
            }

            if (scene.Camera == null)
            {
                this.ZoomToContents();
            }
            else
            {
                this.scene.Editor.DoActionOnCamera(
                    (perspectiveCamera) =>
                    {
                        perspectiveCamera.LookDirection = scene.Camera.LookDirection;
                        perspectiveCamera.Position = scene.Camera.Position;
                        perspectiveCamera.UpDirection = scene.Camera.UpDirection;
                    }, (orthographicCamera) => { Guard.ThrowNotSupportedCameraException(); });
            }
        }

        private void ZoomToContents()
        {
            this.scene.Editor.DoActionOnCamera((perspectiveCamera) =>
            {
                IEnumerable<Point3D> contentPoints = this.GetSceneContextPoints();
                Point3D fromPoint = CameraHelper.GetZoomToContentsCameraPosition(perspectiveCamera, this.scene.Editor.ViewportSize, contentPoints);
                Rect3D boundingRect = GeometryHelper.GetBoundingRectangle(contentPoints);
                Point3D boundingCenter = boundingRect.Location + new Vector3D(boundingRect.SizeX, boundingRect.SizeY, boundingRect.SizeZ);
                Vector3D lookDirection = perspectiveCamera.LookDirection;
                lookDirection.Normalize();
                double projectedCoordinate = Vector3D.DotProduct(lookDirection, boundingCenter - fromPoint);
                Point3D projectedCenter = fromPoint + projectedCoordinate * lookDirection;
                this.scene.Editor.Look(fromPoint, projectedCenter);
            }, (orthographicCamera) => Guard.ThrowNotSupportedCameraException());
        }
        
        private IEnumerable<Point3D> GetSceneContextPoints()
        {
            if (this.Context.SelectedSurface == null)
            {
                foreach (IteractiveSurface surface in this.Context.Surfaces)
                {
                    foreach (Vertex vertex in surface.ElementsProvider.Vertices)
                    {
                        yield return vertex.Point;
                    }
                }
            }
            else
            {
                foreach (Vertex vertex in this.Context.SelectedSurface.ElementsProvider.Vertices)
                {
                    yield return vertex.Point;
                }
            }
        }

        private void InitializeScene()
        {
            byte directionIntensity = 250;
            byte ambientIntensity = 125;
            this.scene.Editor.AddDirectionalLight(Color.FromRgb(directionIntensity, directionIntensity, directionIntensity), new Vector3D(-1, -3, -5));
            this.scene.Editor.AddAmbientLight(Color.FromRgb(ambientIntensity, ambientIntensity, ambientIntensity));
            this.scene.Editor.Look(new Point3D(25, 25, 35), new Point3D());

            this.scene.StartListeningToMouseEvents();
        }

        private void AttachToEvents()
        {
            this.InputManager.ParameterInputed += this.HandleInputManagerParameterInputed;
            this.InputManager.CancelInputed += this.HandleInputManagerCancelInputed;
            this.HistoryManager.HistoryChanged += this.HandleHistoryChanges;
            this.SurfacePointerHandler.SurfaceHandler.SurfaceSelected += this.HandleSurfaceSelected;
            this.SurfacePointerHandler.PointHandler.PointClicked += HandlePointClicked;
            this.SurfacePointerHandler.PointHandler.PointMove += HandlePointMove;
        }

        private void HandleHistoryChanges(object sender, EventArgs e)
        {
            this.CommandDescriptors.UpdateCommandStates();
        }

        private void HandleInputManagerParameterInputed(object sender, ParameterInputedEventArgs e)
        {
            this.CommandContext.CommandHandler.HandleParameterInputed(e);
        }

        private void HandleInputManagerCancelInputed(object sender, EventArgs e)
        {
            this.CommandContext.CommandHandler.HandleCancelInputed();
        }

        private void HandleSurfaceSelected(object sender, SurfaceSelectedEventArgs e)
        {
            this.CommandContext.CommandHandler.HandleSurfaceSelected(e);
        }

        private void HandlePointClicked(object sender, PointClickEventArgs e)
        {
            this.CommandContext.CommandHandler.HandlePointClicked(e);
        }

        private void HandlePointMove(object sender, PointEventArgs e)
        {
            this.CommandContext.CommandHandler.HandlePointMove(e);
        }
    }
}
