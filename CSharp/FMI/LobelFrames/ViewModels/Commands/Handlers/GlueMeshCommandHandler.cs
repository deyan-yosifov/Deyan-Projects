using Deyo.Controls.Controls3D.Visuals;
using Deyo.Core.Common;
using LobelFrames.DataStructures;
using LobelFrames.DataStructures.Surfaces;
using LobelFrames.IteractionHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LobelFrames.ViewModels.Commands.Handlers
{
    public class GlueMeshCommandHandler : LobelEditingCommandHandler
    {
        private int? currentGlueRows;

        public GlueMeshCommandHandler(ILobelSceneEditor editor, ISceneElementsManager elementsManager)
            : base(editor, elementsManager)
        {
        }

        public override CommandType Type
        {
            get
            {
                return CommandType.GlueMesh;
            }
        }

        public override void BeginCommand()
        {
            base.BeginCommand();

            this.currentGlueRows = null;
            this.Editor.EnableSurfacePointerHandler(IteractionHandlingType.PointIteraction);
            this.UpdateInputLabel();
        }

        public override void EndCommand()
        {
            base.EndCommand();

            this.currentGlueRows = null;
        }

        public override void HandlePointClicked(PointClickEventArgs e)
        {
            PointVisual point;
            if (this.Points.Count < 3 && !this.IsSamePointClick(e) && e.TryGetVisual(out point))
            {
                if (this.Points.Count == 1 && !this.TryValidateColinearEdgesConnection(point))
                {
                    return;
                }

                if (this.Points.Count == 2 && !this.TryValidatePointIsNotColinearWithPreviousPoints(point))
                {
                    return;
                }

                this.Points.Add(point);
                this.HandleValidPointSelectionChange();
                this.UpdateInputLabel();
            }
        }

        public override void HandleCancelInputed(CancelInputedEventArgs e)
        {
            if (this.Points.Count > 0)
            {
                this.Points.PopLast();
                this.HandleValidPointSelectionChange();
                this.UpdateInputLabel();
            }
            else
            {
                this.Editor.CloseCommandContext();
            }
        }

        public override void HandleParameterInputed(ParameterInputedEventArgs e)
        {
            if (this.Points.Count == 3)
            {
                int rows;
                if (int.TryParse(e.Parameter, out rows) && rows > 0)
                {
                    if (rows == this.currentGlueRows.Value)
                    {
                        this.EndGlueMeshCommand();
                    }
                    else
                    {
                        this.currentGlueRows = rows;
                        this.ClearLinesOverlays();
                        this.DrawGlueTrianglesOverlays();
                        e.Handled = true;
                        e.ClearHandledParameterValue = false;
                    }
                }
                else
                {
                    this.Editor.ShowHint(Hints.InvalidGlueRowsParameterValue, HintType.Warning);
                }
            }
        }

        protected override void UpdateInputLabel()
        {
            string hint = null;
            this.Editor.InputManager.HandleEmptyParameterInput = false;

            switch (this.Points.Count)
            {
                case 0:
                    hint = Hints.SelectFirstGluePoint;
                    this.Editor.InputManager.Start(Labels.PressEscapeToCancel, string.Empty, true);
                    break;
                case 1:
                    hint = Hints.SelectSecondGluePoint;
                    this.Editor.InputManager.Start(Labels.PressEscapeToStepBack, string.Empty, true);
                    break;
                case 2:
                    hint = Hints.SelectGlueDirectionPoint;
                    this.Editor.InputManager.Start(Labels.PressEscapeToStepBack, string.Empty, true);
                    break;
                case 3:
                    hint = Hints.InputNumberOfGlueRowsToAddAndPressEnter;
                    this.Editor.InputManager.Start(Labels.PressEnterToGlue, this.currentGlueRows.ToString(), false);
                    break;
                default:
                    throw new NotSupportedException(string.Format("Not supported points count: {0}", this.Points.Count));
            }

            this.Editor.ShowHint(hint, HintType.Info);
        }

        private void HandleValidPointSelectionChange()
        {
            switch (this.Points.Count)
            {
                case 0:
                    this.EndPointMoveIteraction();
                    break;
                case 1:
                    if (this.MovingLine == null)
                    {
                        this.BeginPointMoveIteraction(this.Points[0].Position);
                    }
                    else
                    {
                        this.ClearLinesOverlays();
                    }
                    break;
                case 2:
                    if (this.MovingLine == null)
                    {
                        this.ClearLinesOverlays();
                        this.BeginPointMoveIteraction(this.Points[0].Position);
                    }

                    this.Lines.Add(this.ElementsManager.CreateLineOverlay(this.Points[0].Position, this.Points[1].Position));
                    break;
                case 3:
                    this.EndPointMoveIteraction();
                    this.ClearLinesOverlays();
                    int rowHeights = (int)Math.Round(Vector3D.DotProduct(this.CalculateRowDirection(), this.Points[2].Position - this.Points[0].Position) / this.GetTriangleHeight());
                    this.currentGlueRows = Math.Max(1, rowHeights);                    
                    this.DrawGlueTrianglesOverlays();
                    break;
            }
        }

        private Vector3D CalculateRowDirection()
        {
            Vector3D columnDirection = this.Points[1].Position - this.Points[0].Position;
            columnDirection.Normalize();
            Vector3D planeNormal = Vector3D.CrossProduct(columnDirection, this.Points[2].Position - this.Points[0].Position);
            Vector3D rowDirection = Vector3D.CrossProduct(planeNormal, columnDirection);
            rowDirection.Normalize();

            return rowDirection;
        }

        private double GetTriangleHeight()
        {
            double height = this.Surface.MeshEditor.SideSize * (Math.Sqrt(3) / 2);

            return height;
        }

        private void DrawGlueTrianglesOverlays()
        {
            foreach (LightTriangle triangle in this.EnumerateGlueTriangles())
            {
                this.Lines.Add(this.ElementsManager.CreateLineOverlay(triangle.A, triangle.C));
                this.Lines.Add(this.ElementsManager.CreateLineOverlay(triangle.B, triangle.C));
                this.Lines.Add(this.ElementsManager.CreateLineOverlay(triangle.A, triangle.B));
            }
        }

        private IEnumerable<LightTriangle> EnumerateGlueTriangles()
        {
            Vector3D rowDirection = this.CalculateRowDirection();
            Vertex previous = this.Surface.GetVertexFromPointVisual(this.Points[0]);
            Vertex next = this.Surface.GetVertexFromPointVisual(this.Points[1]);
            VertexConnectionInfo connectionInfo;
            if (!this.Surface.MeshEditor.TryConnectVerticesWithColinearEdges(previous, next, out connectionInfo))
            {
                throw new InvalidOperationException("First and second glue points must be connected with colinear edged!");
            }

            Vector3D heightVector = this.GetTriangleHeight() * rowDirection;
            Point3D[] firstLinePoints = connectionInfo.ConnectingVertices.Select(v => v.Point).ToArray();
            Point3D[] secondLinePoints = this.EnumerateSecondLinePoints(firstLinePoints, heightVector).ToArray();
            Vector3D planeNormal = Vector3D.CrossProduct(firstLinePoints[1] - firstLinePoints[0], secondLinePoints[0] - firstLinePoints[0]);
            double dotProduct = Vector3D.DotProduct(planeNormal, connectionInfo.FirstPlaneNormal);
            bool flipNormal = dotProduct < 0;

            for (int lineIndex = 0; lineIndex <= this.currentGlueRows.Value; lineIndex++)
            {
                bool isLongLine = (lineIndex & 1) == 0;
                int heightsOffset = (lineIndex / 2) * 2;
                Vector3D offset = heightsOffset * heightVector;

                if (!isLongLine)
                {
                    foreach (LightTriangle triangle in this.EnumerateRowTriangles(firstLinePoints, secondLinePoints, offset, offset, flipNormal))
                    {
                        yield return triangle;
                    }

                    if (lineIndex < this.currentGlueRows.Value)
                    {
                        Vector3D firstLineOffset = offset + 2 * heightVector;
                        foreach (LightTriangle triangle in this.EnumerateRowTriangles(firstLinePoints, secondLinePoints, firstLineOffset, offset, !flipNormal))
                        {
                            yield return triangle;
                        }
                    }
                }
            }
        }

        private IEnumerable<LightTriangle> EnumerateRowTriangles(Point3D[] firstLinePoints, Point3D[] secondLinePoints, Vector3D firstLineOffset, Vector3D secondLineOffset, bool flipNormal)
        {
            for (int i = 0; i < secondLinePoints.Length; i++)
            {
                Point3D c = secondLinePoints[i] + secondLineOffset;
                Point3D a = firstLinePoints[i] + firstLineOffset;
                Point3D b = firstLinePoints[i + 1] + firstLineOffset;
                yield return flipNormal ? new LightTriangle(c, b, a) : new LightTriangle(a, b, c);

                if (i < secondLinePoints.Length - 1)
                {
                    Point3D d = secondLinePoints[i + 1] + secondLineOffset;
                    yield return flipNormal ? new LightTriangle(d, b, c) : new LightTriangle(c, b, d);
                }
            }
        }

        private IEnumerable<Point3D> EnumerateSecondLinePoints(Point3D[] firstLinePoints, Vector3D heightVector)
        {
            Vector3D halfSideVector = 0.5 * (firstLinePoints[1] - firstLinePoints[0]);
            int secondLinePointsCount = firstLinePoints.Length - 1;

            for (int i = 0; i < secondLinePointsCount; i++)
            {
                Point3D secondLinePoint = firstLinePoints[i] + halfSideVector + heightVector;
                yield return secondLinePoint;
            }
        }

        private void EndGlueMeshCommand()
        {
            // TODO:
            //GlueMeshPatchAction glueAction = new GlueMeshPatchAction(this.Surface, foldingInfo);
            //this.Editor.DoAction(glueAction);
            this.Editor.CloseCommandContext();
        }

        private bool IsSamePointClick(PointClickEventArgs e)
        {
            bool isSame = this.Points.Count > 0 && this.Points.PeekLast().Position.Equals(e.Point);

            return isSame;
        }
    }
}
