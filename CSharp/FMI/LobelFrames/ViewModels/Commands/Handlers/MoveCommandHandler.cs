﻿using Deyo.Controls.Controls3D.Iteractions;
using Deyo.Controls.Controls3D.Visuals;
using Deyo.Controls.Controls3D.Visuals.Overlays2D;
using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using LobelFrames.IteractionHandling;
using LobelFrames.ViewModels.Commands.History;
using System;
using System.Windows.Media.Media3D;

namespace LobelFrames.ViewModels.Commands.Handlers
{
    public class MoveCommandHandler : CommandHandlerBase
    {
        public MoveCommandHandler(ILobelSceneEditor editor, ISceneElementsManager elementsManager)
            : base(editor, elementsManager)
        {
        }

        public override CommandType Type
        {
            get
            {
                return CommandType.MoveMesh;
            }
        }

        public override void BeginCommand()
        {
            base.Editor.EnableSurfacePointerHandler(IteractionHandlingType.PointIteraction);
            base.Editor.ShowHint(Hints.SelectFirstMovePoint);
        }

        public override void HandlePointClicked(PointClickEventArgs e)
        {
            IteractionRestrictor restrictor = base.Editor.SurfacePointerHandler.PointHandler.Restrictor;
            PointVisual pointVisual;

            if (restrictor.IsInIteraction)
            {
                restrictor.EndIteraction();
                Vector3D moveDirection = e.Point - base.Points[0].Position;
                base.Editor.DoAction(new MoveSurfaceAction(base.Editor.Context.SelectedSurface, moveDirection));
                base.Editor.CloseCommandContext();
            }
            else if (e.TryGetVisual(out pointVisual))
            {
                base.Editor.InputManager.Start(Labels.InputMoveDistance, 0.ToString());
                base.MovingLine = base.ElementsManager.BeginMovingLineOverlay(pointVisual.Position);
                restrictor.BeginIteraction(pointVisual.Position);
                base.Points.Add(pointVisual);
                base.Edges.AddRange(base.Editor.Context.SelectedSurface.GetContour());

                foreach (Edge edge in base.Edges)
                {
                    base.Lines.Add(base.ElementsManager.CreateLineOverlay(edge.Start.Point, edge.End.Point));
                }

                base.Editor.ShowHint(Hints.SelectSecondMovePoint);
            }
        }

        public override void HandlePointMove(PointEventArgs e)
        {
            base.ElementsManager.MoveLineOverlay(base.MovingLine, e.Point);
            Vector3D moveDirection = e.Point - base.Points[0].Position;
            base.Editor.InputManager.InputValue = Labels.GetDecimalNumberValue(moveDirection.Length);

            for (int i = 0; i < base.Edges.Count; i++)
            {
                Edge edge = base.Edges[i];
                LineOverlay line = base.Lines[i];
                base.ElementsManager.MoveLineOverlay(line, edge.Start.Point + moveDirection, edge.End.Point + moveDirection);
            }
        }
    }
}