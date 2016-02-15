using Deyo.Controls.Controls3D.Visuals;
using Deyo.Controls.Controls3D.Visuals.Overlays2D;
using Deyo.Core.Common;
using Deyo.Core.Common.History;
using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using System;
using System.Collections.Generic;

namespace LobelFrames.ViewModels.Commands
{
    public class CommandContext
    {
        private readonly ISceneElementsManager elementsManager;
        private readonly HistoryManager historyManager;
        private readonly List<LineOverlay> lines;
        private readonly List<PointVisual> points;
        private readonly List<Edge> edges;
        private bool isStarted;
        private CommandType type;
        private LineOverlay movingLine;
        private IDisposable endHistoryAction;

        internal CommandContext(ISceneElementsManager elementsManager, HistoryManager history)
        {
            this.lines = new List<LineOverlay>();
            this.points = new List<PointVisual>();
            this.edges = new List<Edge>();
            this.elementsManager = elementsManager;
            this.historyManager = history;
            this.isStarted = false;
        }

        public bool IsStarted
        {
            get
            {
                return this.isStarted;
            }
        }

        public CommandType Type
        {
            get
            {
                this.EnsureIsStarted();
                return this.type;
            }
        }

        public LineOverlay MovingLine
        {
            get
            {
                this.EnsureIsStarted();
                return this.movingLine;
            }
            set
            {
                this.EnsureIsStarted();
                this.movingLine = value;
            }
        }

        public List<LineOverlay> Lines
        {
            get
            {
                this.EnsureIsStarted();
                return this.lines;
            }
        }

        public List<PointVisual> Points
        {
            get
            {
                this.EnsureIsStarted();
                return this.points;
            }
        }

        public List<Edge> Edges
        {
            get
            {
                this.EnsureIsStarted();
                return this.edges;
            }
        }

        public void BeginCommand(CommandType commandType)
        {
            Guard.ThrowExceptionIfTrue(this.isStarted, "isStarted");

            this.isStarted = true;
            this.type = commandType;
            this.endHistoryAction = this.historyManager.BeginUndoGroup();
        }

        public void EndCommand()
        {
            this.EnsureIsStarted();

            this.endHistoryAction.Dispose();
            this.endHistoryAction = null;
            this.isStarted = false;
            this.ClearElements();
        }

        private void ClearElements()
        {
            if (this.movingLine != null)
            {
                this.elementsManager.DeleteMovingLineOverlay(this.movingLine);
                this.movingLine = null;
            }

            foreach (LineOverlay line in this.lines)
            {
                this.elementsManager.DeleteLineOverlay(line);
            }

            this.lines.Clear();
            this.points.Clear();
            this.edges.Clear();
        }

        private void EnsureIsStarted()
        {
            Guard.ThrowExceptionIfFalse(this.IsStarted, "IsStarted");
        }
    }
}
