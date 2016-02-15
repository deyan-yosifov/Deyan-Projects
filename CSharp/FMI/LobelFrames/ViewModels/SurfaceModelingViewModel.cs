using Deyo.Controls.Common;
using Deyo.Controls.Controls3D;
using Deyo.Controls.Controls3D.Iteractions;
using Deyo.Controls.Controls3D.Visuals;
using Deyo.Controls.Controls3D.Visuals.Overlays2D;
using LobelFrames.DataStructures;
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
            this.elementsPool = new SceneElementsPool(scene);
            this.context = new SurfaceModelingContext(this.elementsPool);
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
            this.Context.CommandContext.BeginCommand(CommandType.SelectMesh);
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
            this.Context.CommandContext.BeginCommand(CommandType.MoveMesh);
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
            this.Context.HistoryManager.PushUndoableAction(new SelectSurfaceAction(e.Surface, this.context));
            this.Context.CommandContext.EndCommand();
            this.DisableSurfacePointerHandler();
            this.HintManager.Hint = Hints.Default;
        }

        private void HandlePointClicked(object sender, PointClickEventArgs e)
        {
            CommandContext commandContext = this.Context.CommandContext;

            switch (commandContext.Type)
            {
                case CommandType.MoveMesh:
                    this.HandleMoveCommandPointClick(e);
                    break;
                default:
                    throw new NotSupportedException(string.Format("Not supported command type: {0}!", commandContext.Type));
            }
        }

        private void HandlePointMove(object sender, PointEventArgs e)
        {
            CommandContext commandContext = this.Context.CommandContext;

            switch (commandContext.Type)
            {
                case CommandType.MoveMesh:
                    this.HandleMoveCommandPointMove(e);
                    break;
                default:
                    throw new NotSupportedException(string.Format("Not supported command type: {0}!", commandContext.Type));
            }
        }

        private void HandleMoveCommandPointClick(PointClickEventArgs e)
        {
            IteractionRestrictor restrictor = this.SurfacePointerHandler.PointHandler.Restrictor;
            PointVisual pointVisual;

            if (restrictor.IsInIteraction)
            {
                restrictor.EndIteraction();
                this.DisableSurfacePointerHandler();
                Vector3D moveDirection = e.Point - this.Context.CommandContext.Points[0].Position;
                this.Context.HistoryManager.PushUndoableAction(new MoveSurfaceAction(this.Context.SelectedSurface, moveDirection));
                this.Context.CommandContext.EndCommand();
                this.HintManager.Hint = Hints.Default;
            }
            else if(e.TryGetVisual(out pointVisual))
            {
                this.Context.CommandContext.MovingLine = this.ElementsPool.BeginMovingLineOverlay(pointVisual.Position);
                restrictor.BeginIteraction(pointVisual.Position);
                this.Context.CommandContext.Points.Add(pointVisual);
                this.Context.CommandContext.Edges.AddRange(this.Context.SelectedSurface.GetContour());

                foreach (Edge edge in this.Context.CommandContext.Edges)
                {
                    this.Context.CommandContext.Lines.Add(this.ElementsPool.CreateLineOverlay(edge.Start.Point, edge.End.Point));
                }
            }
        }

        private void HandleMoveCommandPointMove(PointEventArgs e)
        {
            this.ElementsPool.MoveLineOverlay(this.Context.CommandContext.MovingLine, e.Point);
            Vector3D moveDirection = e.Point - this.Context.CommandContext.Points[0].Position;

            for (int i = 0; i < this.Context.CommandContext.Edges.Count; i++)
            {
                Edge edge = this.Context.CommandContext.Edges[i];
                LineOverlay line = this.Context.CommandContext.Lines[i];
                this.ElementsPool.MoveLineOverlay(line, edge.Start.Point + moveDirection, edge.End.Point + moveDirection);
            }
        }

        private void EnableSurfacePointerHandler(IteractionHandlingType iteractionType)
        {
            this.surfacePointerHandler.IteractionType = iteractionType;
            this.surfacePointerHandler.IsEnabled = true;
            this.scene.PointerHandlersController.HandleMoveWhenNoHandlerIsCaptured = true;
        }

        private void DisableSurfacePointerHandler()
        {
            this.surfacePointerHandler.IsEnabled = false;
            this.scene.PointerHandlersController.HandleMoveWhenNoHandlerIsCaptured = false;
        }
    }
}
