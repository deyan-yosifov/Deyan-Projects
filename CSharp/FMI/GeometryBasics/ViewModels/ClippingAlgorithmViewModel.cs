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
    public class ClippingAlgorithmViewModel : CartesianPlaneViewModelBase
    {
        private ObservableCollection<Point> clipPoints;
        private ObservableCollection<Point> polygon;
        private bool isSelectingPolygon;
        private bool isSelectingClip;
        private ICommand selectPolygonCommand;
        private ICommand selectClipCommand;
        private Line unfinishedLine;
        private ConvexPolygonValidator validator;

        public ClippingAlgorithmViewModel(CartesianPlane cartesianPlane)
            : base(cartesianPlane)
        {
        }

        public ObservableCollection<Point> ClipPoints
        {
            get
            {
                return this.clipPoints;
            }
        }

        public ObservableCollection<Point> Polygon
        {
            get
            {
                return this.polygon;
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

        public bool IsSelectingClip
        {
            get
            {
                return this.isSelectingClip;
            }
            set
            {
                this.SetProperty(ref this.isSelectingClip, value);
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

        public ICommand SelectClipCommand
        {
            get
            {
                return this.selectClipCommand;
            }
            set
            {
                this.SetProperty(ref this.selectClipCommand, value);
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
                return new ViewportInfo(new Point(9, 9), 22);
            }
        }

        protected override void InitializeFieldsOverride()
        {
            this.SelectClipCommand = new DelegateCommand(((parameter) =>
                {
                    this.IsSelectingClip = true;
                    this.isSelectingPolygon = false;
                    base.StartSelectionCommand.Execute(parameter);
                }));

            this.SelectPolygonCommand = new DelegateCommand(((parameter) =>
            {
                this.IsSelectingClip = false;
                this.isSelectingPolygon = true;
                base.StartSelectionCommand.Execute(parameter);
            }));

            this.IsSelectingPolygon = false;
            this.IsSelectingClip = false;
            this.polygon = new ObservableCollection<Point>();
            this.clipPoints = new ObservableCollection<Point>();
            this.unfinishedLine = null;
            this.validator = null;

            this.GenerateSampleData();
        }

        protected override void RenderInputDataOverride()
        {
            this.RenderPolygonData();
            this.RenderClipData();
        }

        protected override void OnPointSelectedOverride(Point point, bool isFirstPointSelection)
        {
            if (isFirstPointSelection)
            {
                this.CartesianPlane.ClearAllElements();

                if (this.IsSelectingClip)
                {
                    this.ClipPoints.Clear();
                    this.RenderPolygonData();
                    this.validator = new ConvexPolygonValidator();
                    this.validator.TryAddPoint(point);
                    this.ClipPoints.Add(point);
                    this.unfinishedLine = AlgorithmHelper.CreateRedLine(this.CartesianPlane, point, point);
                }
                else if (this.IsSelectingPolygon)
                {
                    this.Polygon.Clear();
                    this.RenderClipData();
                    this.Polygon.Add(point);
                    base.DrawLinesInContext(() =>
                    {
                        this.unfinishedLine = this.CartesianPlane.AddLine(point, point);
                    });
                }
            }
            else
            {
                if (this.IsSelectingClip)
                {
                    if (this.validator.TryAddPoint(point))
                    {
                        this.unfinishedLine.X2 = point.X;
                        this.unfinishedLine.Y2 = point.Y;
                        this.unfinishedLine = AlgorithmHelper.CreateRedLine(this.CartesianPlane, point, point);
                        this.ClipPoints.Add(point);
                    }
                }
                else if (this.IsSelectingPolygon)
                {
                    this.unfinishedLine.X2 = point.X;
                    this.unfinishedLine.Y2 = point.Y;
                    base.DrawLinesInContext(() =>
                    {
                        this.unfinishedLine = this.CartesianPlane.AddLine(point, point);
                    });
                    this.Polygon.Add(point);
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
            this.IsSelectingPolygon = false;
            this.IsSelectingClip = false;
        }

        protected override ICartesianPlaneAlgorithm StartAlgorithm()
        {
            return new ClippingAlgorithm(this.CartesianPlane, this.ClipPoints, this.Polygon);
        }

        private void GenerateSampleData()
        {
            Point[] polygon = { new Point(4, 1), new Point(7, 3), new Point(9, 15), new Point(15, 5), new Point(10, 6), new Point(7, 5) };
            Point[] clip = { new Point(3, 3), new Point(6, 1), new Point(11, 1), new Point(13.5, 3), new Point(8.5, 8.5) };

            foreach (Point point in polygon)
            {
                this.Polygon.Add(point);
            }

            foreach (Point point in clip)
            {
                this.ClipPoints.Add(point);
            }

            this.ValidateClip();
        }

        private void ValidateClip()
        {
            ConvexPolygonValidator validator = new ConvexPolygonValidator();
            int invalidIndex = -1;
            for (int i = 0; i < this.ClipPoints.Count; i++)
            {
                if (!validator.TryAddPoint(this.ClipPoints[i]))
                {
                    invalidIndex = i;
                    break;
                }
            }

            if (invalidIndex > -1)
            {
                while (this.ClipPoints.Count > invalidIndex)
                {
                    this.ClipPoints.RemoveAt(this.ClipPoints.Count - 1);
                }

                MessageBox.Show("Примерните данни за входния клипиращ многоъгълник са невалидни. Многоъгълникът трябва да бъде изпъкнал!", "Грешни данни.");
            }
        }

        private void RenderClipData()
        {
            int count = this.ClipPoints.Count;

            for (int i = 0; i < count; i++)
            {
                AlgorithmHelper.CreateRedLine(this.CartesianPlane, this.ClipPoints[i], this.ClipPoints[(i + 1) % count]);
            }
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
    }
}
