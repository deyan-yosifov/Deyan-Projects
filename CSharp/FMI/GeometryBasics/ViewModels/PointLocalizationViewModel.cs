using Deyo.Controls.Charts;
using Deyo.Controls.Common;
using GeometryBasics.Algorithms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace GeometryBasics.ViewModels
{
    public class PointLocalizationViewModel : CartesianPlaneViewModelBase
    {
        private ObservableCollection<Point> point;
        private ObservableCollection<Point> polygon;
        private string result;
        private ICommand selectPointCommand;
        private ICommand selectPolygonCommand;
        private bool isSelectingSinglePoint;
        private bool isSelectingPolygon;
        Line unfinishedLine;
        Ellipse lastPoint;
        private NonIntersectingPolygonValidator validator;

        public PointLocalizationViewModel(CartesianPlane cartesianPlane)
            : base(cartesianPlane)
        {
        }

        public ObservableCollection<Point> Point
        {
            get
            {
                return this.point;
            }
        }

        public ObservableCollection<Point> Polygon
        {
            get
            {
                return this.polygon;
            }
        }

        public bool IsSelectingSinglePoint
        {
            get
            {
                return this.isSelectingSinglePoint;
            }
            set
            {
                this.SetProperty(ref this.isSelectingSinglePoint, value);
            }
        }

        public bool IsSelectingPolygon
        {
            get
            {
                return this.isSelectingPolygon;
            }
            set
            {
                this.SetProperty(ref this.isSelectingPolygon, value);
            }
        }

        public ICommand SelectPointCommand
        {
            get
            {
                return this.selectPointCommand;
            }
            set
            {
                this.SetProperty(ref this.selectPointCommand, value);
            }
        }

        public ICommand SelectPolygonCommand
        {
            get
            {
                return this.selectPolygonCommand;
            }
            set
            {
                this.SetProperty(ref this.selectPolygonCommand, value);
            }
        }

        public string Result
        {
            get
            {
                return this.result;
            }
            set
            {
                this.SetProperty(ref this.result, value);
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
                return new ViewportInfo(new Point(6, 3.5), 18);
            }
        }

        protected override ICartesianPlaneAlgorithm StartAlgorithm()
        {
            return new PointLocalizationAlgorithm(this.CartesianPlane, this.Polygon, this.Point.First(), (text) => { this.Result = text; });
        }

        protected override void RenderInputDataOverride()
        {
            this.RenderPolygonData();
            this.RenderPointData();
        }

        protected override void InitializeFieldsOverride()
        {
            this.point = new ObservableCollection<Point>();
            this.polygon = new ObservableCollection<Point>();

            this.SelectPointCommand = new DelegateCommand((parameter) =>
                {
                    this.IsSelectingSinglePoint = true;
                    this.IsSelectingPolygon = false;
                    base.StartSelectionCommand.Execute(parameter);
                });

            this.selectPolygonCommand = new DelegateCommand((parameter) =>
                {
                    this.IsSelectingPolygon = true;
                    this.IsSelectingSinglePoint = false;
                    base.StartSelectionCommand.Execute(parameter);
                });

            this.GenerateSampleData();
        }

        protected override void OnPointSelectedOverride(Point point, bool isFirstPointSelection)
        {
            if (isFirstPointSelection)
            {
                this.CartesianPlane.ClearAllElements();
                this.lastPoint = null;
                this.unfinishedLine = null;
            }

            if (this.IsSelectingSinglePoint)
            {
                if (isFirstPointSelection)
                {
                    this.RenderPolygonData();
                }

                this.Point.Clear();
                this.Point.Add(point);
                if (this.lastPoint != null)
                {
                    this.CartesianPlane.RemoveElement(this.lastPoint);
                }
                this.lastPoint = this.RenderPointData();
            }
            else if (this.IsSelectingPolygon)
            {
                if (isFirstPointSelection)
                {
                    this.Polygon.Clear();
                    this.CartesianPlane.ClearAllElements();
                    this.RenderPointData();
                    base.DrawLinesInContext(() =>
                    {
                        this.unfinishedLine = this.CartesianPlane.AddLine(point, point);
                    });
                    this.validator = this.CreateValidator();
                    this.validator.TryAddPoint(point);
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
                    }
                    else
                    {
                        MessageBox.Show("Точките трябва да образуват несамопресичащ се многоъгълник!", "Невалидна операция");
                    }
                }
            }
        }

        protected override void OnSelectionMoveOverride(Point point)
        {
            if (this.IsSelectingPolygon && this.unfinishedLine != null)
            {
                this.unfinishedLine.X2 = point.X;
                this.unfinishedLine.Y2 = point.Y;
            }
        }

        protected override void OnSelectionCanceledOverride()
        {
            this.lastPoint = null;
            this.unfinishedLine = null;
            this.IsSelectingSinglePoint = false;
            this.IsSelectingPolygon = false;
            if (validator != null && !this.validator.TryClosePolygon())
            {
                MessageBox.Show("Някои точки бяха изтрити поради невъзможност да бъде затворен многостена без самопресичане.", "Невалидна операция");
            }
            this.validator = null;
        }

        private void GenerateSampleData()
        {
            Point[] points = { new Point(1, 1), new Point(3, 5), new Point(5, 4), new Point(9, 7), new Point(11, 3), new Point(11, 1), new Point(8, 2) };

            NonIntersectingPolygonValidator validator = this.CreateValidator();
            foreach (Point point in points)
            {
                validator.TryAddPoint(point);
            }
            validator.TryClosePolygon();

            if (this.Polygon.Count < points.Length)
            {
                MessageBox.Show("Точките трябва да образуват несамопресичащ се многоъгълник!", "Невалидна операция");
            }

            this.Point.Add(new Point(4, 2));
        }

        private NonIntersectingPolygonValidator CreateValidator()
        {
            return new NonIntersectingPolygonValidator((point) => { this.Polygon.Add(point); }, () => { this.Polygon.RemoveAt(this.Polygon.Count - 1); });
        }

        private void RenderPolygonData()
        {
            base.DrawLinesInContext(() =>
            {
                int count = this.Polygon.Count;

                for (int i = 0; i < count; i++)
                {
                    this.CartesianPlane.AddLine(this.Polygon[i], this.Polygon[(i + 1) % count]);
                }
            });
        }

        private Ellipse RenderPointData()
        {
            Ellipse pointData = null;

            base.DrawPointsInContext(() =>
            {
                pointData = this.CartesianPlane.AddPoint(this.Point.First());
            });

            return pointData;
        }
    }
}
