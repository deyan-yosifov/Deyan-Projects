using Deyo.Controls.Charts;
using GeometryBasics.Algorithms;
using GeometryBasics.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace GeometryBasics.ViewModels
{
    public class LineSegmentsIntersectionViewModel : CartesianPlaneViewModelBase
    {
        private ObservableCollection<LineSegment> lineSegments;
        private Line unfinishedLine;

        public LineSegmentsIntersectionViewModel(CartesianPlane cartesianPlane)
            : base(cartesianPlane)
        {
        }

        public ObservableCollection<LineSegment> LineSegments
        {
            get
            {
                return this.lineSegments;
            }
        }

        protected override bool HandleSelectionMove
        {
            get
            {
                return true;
            }
        }

        protected override ViewportInfo ViewportInfo
        {
            get
            {
                return new ViewportInfo(new Point(10, 5), 22);
            }
        }

        protected override ICartesianPlaneAlgorithm StartAlgorithm()
        {
            return new LineSegmentsIntersectionAlgorithm(this.CartesianPlane, this.LineSegments);
        }

        protected override void RenderInputDataOverride()
        {
            base.DrawLinesInContext(() =>
                {
                    foreach (LineSegment line in this.LineSegments)
                    {
                        this.CartesianPlane.AddLine(line.Start, line.End);
                    }
                });
        }

        protected override void InitializeFieldsOverride()
        {
            this.lineSegments = new ObservableCollection<LineSegment>();
            this.unfinishedLine = null;

            this.GenerateSampleLines();
        }

        protected override void OnPointSelectedOverride(Point point, bool isFirstPointSelection)
        {
            if (isFirstPointSelection)
            {
                this.LineSegments.Clear();
                this.CartesianPlane.ClearAllElements();
            }

            if (this.unfinishedLine == null)
            {
                this.DrawLinesInContext(() =>
                {
                    this.unfinishedLine = this.CartesianPlane.AddLine(point, point);
                });
            }
            else
            {
                this.unfinishedLine.X2 = point.X;
                this.unfinishedLine.Y2 = point.Y;

                this.LineSegments.Add(new LineSegment(new Point(this.unfinishedLine.X1, this.unfinishedLine.Y1), point));
                this.unfinishedLine = null;
            }
        }

        protected override void OnSelectionMoveOverride(Point point)
        {
            if (this.unfinishedLine != null)
            {
                this.unfinishedLine.X2 = point.X;
                this.unfinishedLine.Y2 = point.Y;
            }
        }

        private void GenerateSampleLines()
        {
            this.LineSegments.Add(new LineSegment(new Point(3, 6), new Point(10, 4)));
            this.LineSegments.Add(new LineSegment(new Point(16, 10), new Point(16, 5)));
            this.LineSegments.Add(new LineSegment(new Point(1, 4), new Point(4, 2)));
            this.LineSegments.Add(new LineSegment(new Point(8, 1), new Point(18, 7)));
            this.LineSegments.Add(new LineSegment(new Point(12, 6), new Point(14, 3)));
        }

        protected override void OnSelectionCanceledOverride()
        {
            this.unfinishedLine = null;
        }
    }
}
