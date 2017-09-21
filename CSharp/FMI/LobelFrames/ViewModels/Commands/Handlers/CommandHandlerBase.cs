using Deyo.Controls.Controls3D.Iteractions;
using Deyo.Controls.Controls3D.Visuals;
using Deyo.Controls.Controls3D.Visuals.Overlays2D;
using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using LobelFrames.IteractionHandling;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LobelFrames.ViewModels.Commands.Handlers
{
    public abstract class CommandHandlerBase : ICommandHandler
    {
        private readonly ISceneElementsManager elementsManager;
        private readonly ILobelSceneEditor editor;
        private readonly List<PointVisual> points;
        private readonly List<LineOverlay> lines;
        private readonly List<Edge> edges;

        protected CommandHandlerBase(ILobelSceneEditor editor, ISceneElementsManager elementsManager)
        {
            this.editor = editor;
            this.elementsManager = elementsManager;
            this.lines = new List<LineOverlay>();
            this.points = new List<PointVisual>();
            this.edges = new List<Edge>();
            this.MovingLine = null;
        }

        protected ILobelSceneEditor Editor
        {
            get
            {
                return this.editor;
            }
        }

        protected ISceneElementsManager ElementsManager
        {
            get
            {
                return this.elementsManager;
            }
        }

        protected LineOverlay MovingLine { get; set; }

        protected List<LineOverlay> Lines
        {
            get
            {
                return this.lines;
            }
        }

        protected List<PointVisual> Points
        {
            get
            {
                return this.points;
            }
        }

        protected List<Edge> Edges
        {
            get
            {
                return this.edges;
            }
        }

        protected bool IsInPointMoveIteraction
        {
            get
            {
                return this.Restrictor.IsInIteraction;
            }
        }

        protected IteractionRestrictor Restrictor
        {
            get
            {
                return this.Editor.SurfacePointerHandler.PointHandler.Restrictor;
            }
        }

        public abstract CommandType Type { get; }

        public virtual void BeginCommand()
        {
            this.Editor.InputManager.Start(Labels.PressEscapeToCancel, string.Empty, true);
        }

        public virtual void HandleSurfaceSelected(SurfaceSelectedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public virtual void HandlePointClicked(PointClickEventArgs e)
        {
            throw new NotImplementedException();
        }

        public virtual void HandlePointMove(PointEventArgs e)
        {
            if (this.MovingLine != null)
            {
                this.ElementsManager.MoveLineOverlay(this.MovingLine, e.Point);
            }
        }

        public virtual void BeginPointMoveIteraction(Point3D point)
        {
            this.MovingLine = this.ElementsManager.BeginMovingLineOverlay(point);
            this.Restrictor.BeginIteraction(point);
        }

        public virtual void EndPointMoveIteraction()
        {
            this.Restrictor.EndIteraction();
            this.ClearMovingLine();
        }

        public virtual void EndCommand()
        {
            this.Editor.InputManager.Reset();
            this.ClearElements();
        }

        public virtual void HandleParameterInputed(ParameterInputedEventArgs e)
        {
            // Do nothing...
        }


        public virtual void HandleCancelInputed(CancelInputedEventArgs e)
        {
            // Do nothing
        }

        protected void ClearLinesOverlays()
        {
            foreach (LineOverlay line in this.lines)
            {
                this.elementsManager.DeleteLineOverlay(line);
            }

            this.lines.Clear();
        }

        protected void ClearMovingLine()
        {
            if (this.MovingLine != null)
            {
                this.elementsManager.DeleteMovingLineOverlay(this.MovingLine);
                this.MovingLine = null;
            }
        }

        private void ClearElements()
        {
            this.ClearMovingLine();
            this.ClearLinesOverlays();

            this.points.Clear();
            this.edges.Clear();
        }
    }
}
