using Deyo.Controls.Controls3D.Visuals;
using Deyo.Controls.Controls3D.Visuals.Overlays2D;
using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using LobelFrames.IteractionHandling;
using System;
using System.Collections.Generic;

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

        public abstract CommandType Type { get; }

        public abstract void BeginCommand();

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
            throw new NotImplementedException();
        }

        public void EndCommand()
        {
            this.ClearElements();
        }

        private void ClearElements()
        {
            if (this.MovingLine != null)
            {
                this.elementsManager.DeleteMovingLineOverlay(this.MovingLine);
                this.MovingLine = null;
            }

            foreach (LineOverlay line in this.lines)
            {
                this.elementsManager.DeleteLineOverlay(line);
            }

            this.lines.Clear();
            this.points.Clear();
            this.edges.Clear();
        }
    }
}
