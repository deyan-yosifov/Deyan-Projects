using Deyo.Controls.Common;
using Deyo.Controls.Controls3D;
using Deyo.Controls.Controls3D.Iteractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace CAGD
{
    public class TensorProductBezierViewModel : ViewModelBase
    {
        private readonly Scene3D scene;
        private readonly TensorProductBezierGeometryManager geometryManager;
        private readonly IteractivePointsHandler iteractivePointsHandler;
        private int degreeInDirectionU;
        private int degreeInDirectionV;
        private int devisionsInDirectionU;
        private int devisionsInDirectionV;
        private bool showControlPoints;
        private bool showControlLines;
        private bool showSurfaceLines;
        private bool showSurfaceGeometry;

        public TensorProductBezierViewModel(Scene3D scene)
        {
            this.scene = scene;
            this.iteractivePointsHandler = this.scene.IteractivePointsHandler;
            this.geometryManager = new TensorProductBezierGeometryManager(scene);
            this.degreeInDirectionU = 3;
            this.degreeInDirectionV = 5;
            this.devisionsInDirectionU = 10;
            this.devisionsInDirectionV = 10;
            this.showControlPoints = true;
            this.showControlLines = true;
            this.showSurfaceLines = true;
            this.showSurfaceGeometry = true;

            this.scene.StartListeningToMouseEvents();
            this.InitializeScene();
        }
        
        public SceneEditor SceneEditor
        {
            get
            {
                return this.scene.Editor;
            }
        }

        public int DegreeInDirectionU
        {
            get
            {
                return this.degreeInDirectionU;
            }
            set
            {
                if (this.SetProperty(ref this.degreeInDirectionU, value))
                {
                    this.RecalculateGeometry();
                }
            }
        }

        public int DegreeInDirectionV
        {
            get
            {
                return this.degreeInDirectionV;
            }
            set
            {
                if (this.SetProperty(ref this.degreeInDirectionV, value))
                {
                    this.RecalculateGeometry();
                }
            }
        }

        public int DevisionsInDirectionU
        {
            get
            {
                return this.devisionsInDirectionU;
            }
            set
            {
                if (this.SetProperty(ref this.devisionsInDirectionU, value))
                {
                    // TODO:
                }
            }
        }

        public int DevisionsInDirectionV
        {
            get
            {
                return this.devisionsInDirectionV;
            }
            set
            {
                if (this.SetProperty(ref this.devisionsInDirectionV, value))
                {
                    // TODO:
                }
            }
        }

        public bool ShowControlPoints
        {
            get
            {
                return this.showControlPoints;
            }
            set
            {
                if (this.SetProperty(ref this.showControlPoints, value))
                {
                    if (value)
                    {
                        this.geometryManager.ShowControlPoints();
                    }
                    else
                    {
                        this.geometryManager.HideControlPoints();
                    }
                }
            }
        }

        public bool ShowControlLines
        {
            get
            {
                return this.showControlLines;
            }
            set
            {
                if (this.SetProperty(ref this.showControlLines, value))
                {
                    // TODO:
                }
            }
        }

        public bool ShowSurfaceLines
        {
            get
            {
                return this.showSurfaceLines;
            }
            set
            {
                if (this.SetProperty(ref this.showSurfaceLines, value))
                {
                    // TODO:
                }
            }
        }

        public bool ShowSurfaceGeometry
        {
            get
            {
                return this.showSurfaceGeometry;
            }
            set
            {
                if (this.SetProperty(ref this.showSurfaceGeometry, value))
                {
                    // TODO:
                }
            }
        }

        public bool CanMoveOnXAxis
        {
            get
            {
                return this.iteractivePointsHandler.CanMoveOnXAxis;
            }
            set
            {
                if (this.iteractivePointsHandler.CanMoveOnXAxis != value)
                {
                    this.iteractivePointsHandler.CanMoveOnXAxis = value;
                    this.OnPropertyChanged();
                    // TODO:
                }
            }
        }

        public bool CanMoveOnYAxis
        {
            get
            {
                return this.iteractivePointsHandler.CanMoveOnYAxis;
            }
            set
            {
                if (this.iteractivePointsHandler.CanMoveOnYAxis != value)
                {
                    this.iteractivePointsHandler.CanMoveOnYAxis = value;
                    this.OnPropertyChanged();
                    // TODO:
                }
            }
        }

        public bool CanMoveOnZAxis
        {
            get
            {
                return this.iteractivePointsHandler.CanMoveOnZAxis;
            }
            set
            {
                if (this.iteractivePointsHandler.CanMoveOnZAxis != value)
                {
                    this.iteractivePointsHandler.CanMoveOnZAxis = value;
                    this.OnPropertyChanged();
                    // TODO:
                }
            }
        }

        public TensorProductBezierGeometryContext GeometryContext
        {
            get
            {
                return new TensorProductBezierGeometryContext()
                {
                    DevisionsInDirectionU = this.DevisionsInDirectionU,
                    DevisionsInDirectionV = this.DevisionsInDirectionV,
                    ShowControlLines = this.ShowControlLines,
                    ShowControlPoints = this.ShowControlPoints,
                    ShowSurfaceGeometry = this.ShowSurfaceGeometry,
                    ShowSurfaceLines = this.ShowSurfaceLines
                };
            }
        }

        private void InitializeScene()
        {
            byte directionIntensity = 250;
            byte ambientIntensity = 125;
            this.SceneEditor.AddDirectionalLight(Color.FromRgb(directionIntensity, directionIntensity, directionIntensity), new Vector3D(-1, -3, -5));
            this.SceneEditor.AddAmbientLight(Color.FromRgb(ambientIntensity, ambientIntensity, ambientIntensity));
            this.SceneEditor.Look(new Point3D(25, 25, 35), new Point3D());

            this.RecalculateGeometry();
        }

        private void RecalculateGeometry()
        {
            this.geometryManager.GenerateGeometry(this.CalculateControlPoints(), this.GeometryContext);
        }

        private Point3D[,] CalculateControlPoints()
        {
            int uPoints = this.DegreeInDirectionU + 1;
            int vPoints = this.DegreeInDirectionV + 1;
            Point3D[,] points = new Point3D[uPoints, vPoints];
            double squareSize = 15;
            double startX = -squareSize / 2;
            double deltaX = squareSize / this.DegreeInDirectionU;
            double startY = -squareSize / 2;
            double deltaY = squareSize / this.DegreeInDirectionV;

            for (int u = 0; u < uPoints; u++)
            {
                for (int v = 0; v < vPoints; v++)
                {
                    double x = startX + u * deltaX;
                    double y = startY + v * deltaY;

                    points[u, v] = new Point3D(x, y, 0);
                }
            }

            return points;
        }
    }
}
