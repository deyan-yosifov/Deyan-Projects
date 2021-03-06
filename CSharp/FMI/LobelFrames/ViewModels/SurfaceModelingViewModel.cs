﻿using Deyo.Controls.Common;
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
    public class SurfaceModelingViewModel : ViewModelBase, ILobelSceneEditor, IContentProvider
    {
        private readonly Scene3D scene;
        private readonly HintManager hintManager;
        private readonly InputManager inputManager;
        private readonly SettingsViewModel settings;
        private readonly PopupViewModel help;
        private readonly CommandDescriptors commandDescriptors;
        private readonly SceneElementsPool elementsPool;
        private readonly SurfaceModelingContext context;
        private readonly SurfaceModelingPointerHandler surfacePointerHandler;
        private readonly ZoomToContentsHandler zoomToContentsPointerHandler;

        public SurfaceModelingViewModel(Scene3D scene)
        {
            this.scene = scene;
            this.hintManager = new HintManager();
            this.inputManager = new InputManager();
            this.elementsPool = new SceneElementsPool(scene);
            this.settings = new SettingsViewModel();
            this.help = new PopupViewModel();
            this.context = new SurfaceModelingContext(this.settings, CommandHandlersFactory.CreateCommandHandlers(this, this.elementsPool));
            this.commandDescriptors = new CommandDescriptors(this);
            this.surfacePointerHandler = new SurfaceModelingPointerHandler(this.elementsPool, scene.Editor);
            this.zoomToContentsPointerHandler = new ZoomToContentsHandler(this.scene.Editor, this);
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

        public SettingsViewModel SettingsViewModel
        {
            get
            {
                return this.settings;
            }
        }

        public PopupViewModel HelpViewModel
        {
            get
            {
                return this.help;
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
            this.SettingsViewModel.GeneralSettingsViewModel.IsOpen = !this.SettingsViewModel.GeneralSettingsViewModel.IsOpen;
        }

        public void ChangeLobelSettings()
        {
            this.SettingsViewModel.LobelSettingsViewModel.IsOpen = !this.SettingsViewModel.LobelSettingsViewModel.IsOpen;
        }

        public void ChangeBezierSettings()
        {
            this.SettingsViewModel.BezierSettingsViewModel.IsOpen = !this.SettingsViewModel.BezierSettingsViewModel.IsOpen;
        }

        public void ChangeHelpAppearance()
        {
            this.HelpViewModel.IsOpen = !this.HelpViewModel.IsOpen;

            if (this.HelpViewModel.IsOpen)
            {
                foreach (CommandType command in Enum.GetValues(typeof(CommandType)))
                {
                    if (command != CommandType.Help)
                    {
                        this.CommandDescriptors[command].IsEnabled = false;
                    }
                }
            }
            else
            {
                this.CommandDescriptors.UpdateCommandStates();
            }
        }

        public void BeforeCommandExecuted(CommandType type)
        {
            if (type != CommandType.Settings)
            {
                this.SettingsViewModel.GeneralSettingsViewModel.IsOpen = false;
            }

            if (type != CommandType.LobelSettings)
            {
                this.SettingsViewModel.LobelSettingsViewModel.IsOpen = false;
            }

            if (type != CommandType.BezierSettings)
            {
                this.SettingsViewModel.BezierSettingsViewModel.IsOpen = false;
            }

            if (type != CommandType.Help)
            {
                this.HelpViewModel.IsOpen = false;
            }
        }

        public void AddLobelMesh()
        {
            LobelSurface surface = new LobelSurface(
                this.ElementsPool, 
                this.Context.Settings.LobelSettings.MeshRows, 
                this.Context.Settings.LobelSettings.MeshColumns, 
                this.Context.Settings.LobelSettings.MeshTriangleSide);
            this.AddIteractiveSurface(surface);
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

        public void FoldMesh()
        {
            this.CommandContext.BeginCommand(CommandType.FoldMesh);
        }

        public void GlueMesh()
        {
            this.CommandContext.BeginCommand(CommandType.GlueMesh);
        }

        public void AddBezierSurface()
        {
            BezierSurface surface = new BezierSurface(
                this.ElementsPool,
                this,
                this.Context.Settings.BezierSettings.UDevisions,
                this.Context.Settings.BezierSettings.VDevisions,
                this.Context.Settings.BezierSettings.UDegree,
                this.Context.Settings.BezierSettings.VDegree,
                this.Context.Settings.BezierSettings.InitialWidth,
                this.Context.Settings.BezierSettings.InitialHeight);
            this.AddIteractiveSurface(surface);
        }

        public void ApproximateWithLobelMesh()
        {
            this.CommandContext.BeginCommand(CommandType.ApproximateWithLobelMesh);
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
                IteractiveSurface surface = LobelSceneFormatProviderBase.CreateIteractiveSurface(this.ElementsPool, this, model);
                this.Context.AddSurface(surface);

                if (model.IsSelected)
                {
                    this.Context.SelectedSurface = surface;
                }
            }

            if (scene.Camera == null)
            {
                this.zoomToContentsPointerHandler.ZoomToContents();
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

        public IEnumerable<Point3D> GetContentPoints()
        {
            if (this.Context.SelectedSurface == null)
            {
                foreach (IteractiveSurface surface in this.Context.Surfaces)
                {
                    foreach (Vertex vertex in surface.BoundingVertices)
                    {
                        yield return vertex.Point;
                    }
                }
            }
            else
            {
                foreach (Vertex vertex in this.Context.SelectedSurface.BoundingVertices)
                {
                    yield return vertex.Point;
                }
            }
        }

        private void InitializeScene()
        {
            byte movingIntensity = 125;
            byte directionIntensity = 175;
            byte ambientIntensity = 75;
            this.scene.Editor.AddDirectionalLight(Color.FromRgb(directionIntensity, directionIntensity, directionIntensity), new Vector3D(-1, -3, -5));
            this.scene.Editor.AddAmbientLight(Color.FromRgb(ambientIntensity, ambientIntensity, ambientIntensity));
            VisualOwner owner = this.scene.Editor.AddDirectionalLight(Color.FromRgb(movingIntensity, movingIntensity, movingIntensity), new Vector3D());
            DirectionalLight movingLight = (DirectionalLight)((ModelVisual3D)owner.Visual).Content;
            ProjectionCamera initialCamera = null;
            this.scene.Editor.DoActionOnCamera((perspective) => { initialCamera = perspective; }, (orthographic) => { initialCamera = orthographic; });
            this.scene.Editor.CameraChanged += (s, e) =>
                {
                    movingLight.Direction = initialCamera.LookDirection;
                };
            this.scene.Editor.Look(new Point3D(25, 25, 35), new Point3D());

            this.scene.StartListeningToMouseEvents();
        }

        private void AttachToEvents()
        {
            this.InputManager.ParameterInputed += this.HandleInputManagerParameterInputed;
            this.InputManager.CancelInputed += this.HandleInputManagerCancelInputed;
            this.HistoryManager.HistoryChanged += this.HandleHistoryChanges;
            this.SurfacePointerHandler.SurfaceHandler.SurfaceSelected += this.HandleSurfaceSelected;
            this.SurfacePointerHandler.PointHandler.PointClicked += this.HandlePointClicked;
            this.SurfacePointerHandler.PointHandler.PointMove += this.HandlePointMove;
        }

        private void HandleHistoryChanges(object sender, EventArgs e)
        {
            this.CommandDescriptors.UpdateCommandStates();
        }

        private void HandleInputManagerParameterInputed(object sender, ParameterInputedEventArgs e)
        {
            this.CommandContext.CommandHandler.HandleParameterInputed(e);
        }

        private void HandleInputManagerCancelInputed(object sender, CancelInputedEventArgs e)
        {
            this.CommandContext.CommandHandler.HandleCancelInputed(e);
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

        private void AddIteractiveSurface(IteractiveSurface surface)
        {
            using (this.HistoryManager.BeginUndoGroup())
            {
                this.DoAction(new AddSurfaceAction(surface, this.Context));
                this.DoAction(new SelectSurfaceAction(surface, this.Context));
            }
        }
    }
}
