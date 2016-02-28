using Deyo.Controls.Common;
using Deyo.Controls.Controls3D;
using Deyo.Controls.Controls3D.Iteractions;
using Deyo.Controls.Controls3D.Visuals;
using Deyo.Controls.Controls3D.Visuals.Overlays2D;
using Deyo.Core.Common.History;
using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using LobelFrames.FormatProviders;
using LobelFrames.IteractionHandling;
using LobelFrames.ViewModels.Commands;
using LobelFrames.ViewModels.Commands.Handlers;
using LobelFrames.ViewModels.Commands.History;
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
        private readonly CommandDescriptors commandDescriptors;
        private readonly SceneElementsPool elementsPool;
        private readonly SurfaceModelingContext context;
        private readonly SurfaceModelingPointerHandler surfacePointerHandler;

        public SurfaceModelingViewModel(Scene3D scene)
        {
            this.scene = scene;
            this.hintManager = new HintManager();
            this.inputManager = new InputManager();
            this.elementsPool = new SceneElementsPool(scene);
            this.context = new SurfaceModelingContext(CommandHandlersFactory.CreateCommandHandlers(this, this.elementsPool));
            this.commandDescriptors = new CommandDescriptors(this);
            this.surfacePointerHandler = new SurfaceModelingPointerHandler(this.elementsPool, scene.Editor);
            this.scene.PointerHandlersController.Handlers.AddFirst(this.surfacePointerHandler);

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

        public void AddLobelMesh()
        {
            using (this.HistoryManager.BeginUndoGroup())
            {
                LobelSurface surface = new LobelSurface(this.ElementsPool, 7, 5, 3);
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

        public void ShowHint(string hint)
        {
            this.HintManager.Hint = hint;
        }

        public void DoAction(IUndoRedoAction action)
        {
            this.HistoryManager.PushUndoableAction(action);
        }

        public void CloseCommandContext()
        {
            this.CommandContext.EndCommand();
            this.DisableSurfacePointerHandler();
            this.ShowHint(Hints.Default);
            this.InputManager.Stop();
        }

        public LobelScene SaveScene()
        {
            throw new NotImplementedException();
        }

        public void LoadScene(LobelScene scene)
        {
            throw new NotImplementedException();
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
            MessageBox.Show(string.Format("Parameter inputed: {0}", e.Parameter));
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
