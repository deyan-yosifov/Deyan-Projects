using Deyo.Controls.Charts;
using GeometryBasics.Algorithms;
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
    public class RotatingCalipersViewModel : CartesianPlaneViewModelBase
    {
        private ObservableCollection<Point> convexPolygon;
        Line unfinishedLine;
        ConvexPolygonValidator validator;

        public RotatingCalipersViewModel(CartesianPlane cartesianPlane)
            : base(cartesianPlane)
        {            
        }

        public ObservableCollection<Point> ConvexPolygon
        {
            get
            {
                return this.convexPolygon;
            }
        }

        protected override ViewportInfo ViewportInfo
        {
            get
            {
                return new ViewportInfo(new Point(5, 7), 18);
            }
        }

        protected override bool HandleSelectionMove
        {
            get
            {
                return true;
            }
        }

        protected override ICartesianPlaneAlgorithm StartAlgorithm()
        {
            return new RotatingCalipersAlgorithm(this.CartesianPlane, this.ConvexPolygon);
        }

        protected override void RenderInputDataOverride()
        {
            base.DrawLinesInContext(() =>
            {
                int count = this.ConvexPolygon.Count;

                for (int i = 0; i < count; i++)
                {
                    this.CartesianPlane.AddLine(this.ConvexPolygon[i], this.ConvexPolygon[(i + 1) % count]);
                }
            });
        }

        protected override void InitializeFieldsOverride()
        {
            this.convexPolygon = new ObservableCollection<Point>();

            this.GenerateSamplePolygon();
        }

        private void GenerateSamplePolygon()
        {
            this.ConvexPolygon.Add(new Point(7, 4));
            this.ConvexPolygon.Add(new Point(10, 7));
            this.ConvexPolygon.Add(new Point(8, 10));
            this.ConvexPolygon.Add(new Point(4, 11));
            this.ConvexPolygon.Add(new Point(1, 5));
            this.ConvexPolygon.Add(new Point(2, 2));

            ConvexPolygonValidator validator = new ConvexPolygonValidator();
            int invalidIndex = -1;
            for (int i = 0; i < this.ConvexPolygon.Count; i++)
            {
                if (!validator.TryAddPoint(this.ConvexPolygon[i]))
                {
                    invalidIndex = i;
                    break;
                }
            }

            if (invalidIndex > -1)
            {
                while (this.ConvexPolygon.Count > invalidIndex)
                {
                    this.ConvexPolygon.RemoveAt(this.ConvexPolygon.Count - 1);
                }

                MessageBox.Show("Примерните данни за входния многоъгълник са невалидни. Многоъгълникът трябва да бъде изпъкнал!", "Грешни данни.");
            }
        }

        protected override void OnPointSelectedOverride(Point point, bool isFirstPointSelection)
        {
            if (isFirstPointSelection)
            {
                this.ConvexPolygon.Clear();
                this.CartesianPlane.ClearAllElements();
                base.DrawLinesInContext(() =>
                    {
                        this.unfinishedLine = this.CartesianPlane.AddLine(point, point);
                    });
                this.validator = new ConvexPolygonValidator();
                this.validator.TryAddPoint(point);
                this.ConvexPolygon.Add(point);
            }
            else
            {
                if (this.validator.TryAddPoint(point))
                {
                    this.unfinishedLine.X2 = point.X;
                    this.unfinishedLine.Y2 = point.Y;
                    base.DrawLinesInContext(() =>
                    {
                        this.unfinishedLine = this.CartesianPlane.AddLine(point, point);
                    });
                    this.ConvexPolygon.Add(point);
                }
                else
                {
                    MessageBox.Show("Точките трябва да образуват изпъкнал многоъгълник!", "Невалидна операция");
                }
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

        protected override void OnSelectionCanceledOverride()
        {
            this.unfinishedLine = null;
            this.validator = null;
        }
    }
}
