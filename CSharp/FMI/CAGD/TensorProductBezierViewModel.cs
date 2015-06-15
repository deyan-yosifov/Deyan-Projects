﻿using Deyo.Controls.Common;
using Deyo.Controls.Controls3D;
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
        private int degreeInDirectionU;
        private int degreeInDirectionV;
        private int devisionsInDirectionU;
        private int devisionsInDirectionV;

        public TensorProductBezierViewModel(Scene3D scene)
        {
            this.scene = scene;
            this.geometryManager = new TensorProductBezierGeometryManager(scene);

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
                    // TODO:
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
                    // TODO:
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

        private void InitializeScene()
        {
            byte directionIntensity = 250;
            byte ambientIntensity = 125;
            this.SceneEditor.AddDirectionalLight(Color.FromRgb(directionIntensity, directionIntensity, directionIntensity), new Vector3D(-1, -3, -5));
            this.SceneEditor.AddAmbientLight(Color.FromRgb(ambientIntensity, ambientIntensity, ambientIntensity));
            this.SceneEditor.Look(new Point3D(25, 25, 35), new Point3D());

            this.geometryManager.GenerateGeometry(new Point3D[,] { /* TODO: */ });
        }
    }
}
