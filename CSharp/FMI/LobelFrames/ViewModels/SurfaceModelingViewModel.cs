using Deyo.Controls.Common;
using Deyo.Controls.Controls3D;
using LobelFrames.DataStructures.Surfaces;
using LobelFrames.DataStructures.Surfaces.IteractionHandling;
using LobelFrames.ViewModels.Commands;
using LobelFrames.ViewModels.Commands.History;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace LobelFrames.ViewModels
{
    public class SurfaceModelingViewModel : ViewModelBase
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
            this.context = new SurfaceModelingContext();
            this.elementsPool = new SceneElementsPool(scene);
            this.commandDescriptors = new CommandDescriptors(this);
            this.surfacePointerHandler = new SurfaceModelingPointerHandler(this.elementsPool);
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

        internal SurfaceModelingContext Context
        {
            get
            {
                return this.context;
            }
        }

        private SurfaceModelingPointerHandler SurfacePointerHandler
        {
            get
            {
                return this.surfacePointerHandler;
            }
        }

        public void AddLobelMesh()
        {
            using (this.Context.HistoryManager.BeginUndoGroup())
            {
                LobelSurface surface = new LobelSurface(this.ElementsPool, 7, 5, 3);
                this.Context.HistoryManager.PushUndoableAction(new AddSurfaceAction(surface, this.Context));
                this.Context.HistoryManager.PushUndoableAction(new SelectSurfaceAction(surface, this.Context));
            }
        }

        public void Undo()
        {
            this.Context.HistoryManager.Undo();
        }

        public void Redo()
        {
            this.Context.HistoryManager.Redo();
        }

        public void SelectMesh()
        {
            this.context.BeginCommandContext(CommandType.SelectMesh);
            this.EnableSurfacePointerHandler(IteractionHandlingType.SurfaceIteraction);
            this.HintManager.Hint = Hints.SelectMesh;
        }

        public void Deselect()
        {
            this.Context.HistoryManager.PushUndoableAction(new DeselectSurfaceAction(this.Context));
        }

        public void DeleteMesh()
        {
            this.Context.HistoryManager.PushUndoableAction(new DeleteSurfaceAction(this.Context));
        }

        public void MoveMesh()
        {
            this.Context.BeginCommandContext(CommandType.MoveMesh);
            this.EnableSurfacePointerHandler(IteractionHandlingType.PointIteraction);
            this.HintManager.Hint = Hints.SelectFirstMovePoint;
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
            this.Context.HistoryManager.HistoryChanged += this.HandleHistoryChanges;
            this.surfacePointerHandler.SurfaceHandler.SurfaceSelected += this.HandleSurfaceSelected;
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
            this.context.HistoryManager.PushUndoableAction(new SelectSurfaceAction(e.Surface, this.context));
            this.context.EndCommandContext();
            this.DisableSurfacePointerHandler();
            this.HintManager.Hint = Hints.Default;
        }

        private void EnableSurfacePointerHandler(IteractionHandlingType iteractionType)
        {
            this.surfacePointerHandler.IteractionType = iteractionType;
            this.surfacePointerHandler.IsEnabled = true;
        }

        private void DisableSurfacePointerHandler()
        {
            this.surfacePointerHandler.IsEnabled = false;
        }
    }
}
