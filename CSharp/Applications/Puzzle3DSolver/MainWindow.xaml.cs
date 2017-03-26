using Deyo.Controls.Controls3D;
using Deyo.Controls.Controls3D.Cameras;
using Deyo.Controls.Controls3D.Shapes;
using Deyo.Controls.Controls3D.Visuals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Puzzle3DSolver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CubeContentProvider cubeBoundsProvider = new CubeContentProvider(Stick3D.StickLength);

        public MainWindow()
        {
            InitializeComponent();
            Sticks3DSet puzzle3DSolution = this.FindPuzzle3DSolution();
            puzzle3DSolution.Explode();
            this.InitializeViewport(puzzle3DSolution);
        }

        private Sticks3DSet FindPuzzle3DSolution()
        {
            // TODO: Implement recursion to find the Sticks3DSet that has no collisions.

            Sticks3DSet cross = new Sticks3DSet();
            Stick3DPosition[] positions = Cross3D.GetStickPositions().ToArray();
            
            foreach (Stick3DRotation rotation in Enum.GetValues(typeof(Stick3DRotation)))
            {
                for (int i = 0; i < positions.Length; i++)
                {
                    Color color = Cross3D.GetStickColor(i + 1);
                    cross.AddStick(new Stick3D(color), positions[i], rotation);
                }
            }

            return cross;
        }

        private void InitializeViewport(Sticks3DSet sticks)
        {
            SceneEditor editor = this.viewport.Editor;
            ZoomToContentsHandler zoomHandler = new ZoomToContentsHandler(editor, this.cubeBoundsProvider);
            this.viewport.PointerHandlersController.Handlers.AddFirst(zoomHandler);

            byte directionIntensity = 250;
            byte ambientIntensity = 125;
            VisualOwner light = editor.AddDirectionalLight(Color.FromRgb(directionIntensity, directionIntensity, directionIntensity), new Vector3D(-1, -3, -5));
            editor.AddAmbientLight(Color.FromRgb(ambientIntensity, ambientIntensity, ambientIntensity));

            this.DrawBoundingCube(true);
            Color previousColor = Colors.Black;
            IDisposable materialRestorer = editor.SaveGraphicProperties();
            Cube cube = editor.ShapeFactory.CreateCube();

            foreach (ColoredCubeBlock block in sticks.ColoredBlocks)
            {
                if (!block.Color.Equals(previousColor))
                {
                    materialRestorer.Dispose();
                    materialRestorer = editor.SaveGraphicProperties();
                    editor.GraphicProperties.MaterialsManager.AddBackDiffuseMaterial(block.Color);
                    editor.GraphicProperties.MaterialsManager.AddFrontDiffuseMaterial(block.Color);
                    previousColor = block.Color;
                    cube = editor.ShapeFactory.CreateCube();
                }

                using (editor.SavePosition())
                {
                    editor.Position.Translate(new Vector3D(block.Position.X, block.Position.Y, block.Position.Z));
                    editor.AddShapeVisual(cube);
                }
            }

            materialRestorer.Dispose();

            editor.Look(new Point3D(25, 25, 25), new Point3D(4, 4, 4));

            this.viewport.StartListeningToMouseEvents();
        }

        private void DrawBoundingCube(bool drawContourPointsOnly)
        {
            SceneEditor editor = this.viewport.Editor;

            using (editor.SaveGraphicProperties())
            {
                editor.GraphicProperties.MaterialsManager.AddBackDiffuseMaterial(Colors.Red);
                editor.GraphicProperties.MaterialsManager.AddFrontDiffuseMaterial(Colors.Red);

                if(!drawContourPointsOnly)
                {                    
                    using (editor.SavePosition())
                    {
                        Cube cube = editor.ShapeFactory.CreateCube();
                        editor.Position.Scale(new Vector3D(8, 8, 8));
                        editor.AddShapeVisual(cube);
                    }
                }

                foreach (Point3D point in this.cubeBoundsProvider.GetContentPoints())
                {
                    editor.AddPointVisual(point);
                }
            }
        }
    }
}
