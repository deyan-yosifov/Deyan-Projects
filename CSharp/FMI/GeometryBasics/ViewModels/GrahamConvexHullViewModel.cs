using Deyo.Controls.Charts;
using GeometryBasics.Algorithms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GeometryBasics.ViewModels
{
    public class GrahamConvexHullViewModel : CartesianPlaneViewModelBase
    {
        private ObservableCollection<Point> points;

        public GrahamConvexHullViewModel(CartesianPlane cartesianPlane)
            : base(cartesianPlane)
        {
        }

        public ObservableCollection<Point> Points
        {
            get
            {
                return this.points;
            }
        }

        protected override ICartesianPlaneAlgorithm StartAlgorithm()
        {
            return new GrahamConvexHullAlgorithm(this.CartesianPlane, this.Points);
        }

        protected override void InitializeFieldsOverride()
        {
            this.points = new ObservableCollection<Point>();

            this.GenerateSamplePoints();
        }

        private void GenerateSamplePoints()
        {
            this.Points.Add(new Point(1, 4));
            this.Points.Add(new Point(4, 2));
            this.Points.Add(new Point(6, 10));
            this.Points.Add(new Point(16, 5));
            this.Points.Add(new Point(18, 7));
            this.Points.Add(new Point(12, 16));
            this.Points.Add(new Point(11, 3));
        }

        protected override void RenderInputDataOverride()
        {
            base.DrawPointsInContext(() =>
                {
                    foreach (Point point in this.Points)
                    {
                        this.CartesianPlane.AddPoint(point);
                    }
                });
        }

        protected override void OnPointSelectedOverride(Point point, bool isFirstPointSelection)
        {
            if (isFirstPointSelection)
            {
                this.Points.Clear();
                this.CartesianPlane.ClearAllElements();
            }

            this.DrawPointsInContext(() =>
            {
                this.CartesianPlane.AddPoint(point);
                this.Points.Add(point);
            });
        }

        protected override void OnSelectionCanceledOverride()
        {
        }
    }
}
