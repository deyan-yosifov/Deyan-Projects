using Deyo.Controls.Controls3D;
using Deyo.Controls.Controls3D.Cameras;
using Deyo.Controls.Controls3D.Shapes;
using Deyo.Controls.Controls3D.Visuals;
using Deyo.Core.Common;
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

            Stick3D[] puzzleSticks = Cross3D.InitializePuzzleSticks();
            Sticks3DSet puzzle3DSolution = this.FindPuzzle3DSolution(puzzleSticks);

            if (puzzle3DSolution == null)
            {
                MessageBox.Show("No puzzle solution found!");
            }
            else
            {
                puzzle3DSolution.Explode();
                this.InitializeViewport(puzzle3DSolution);
            }
        }

        private Sticks3DSet FindPuzzle3DSolution(Stick3D[] puzzleSticks)
        {
            Guard.ThrowExceptionIfNotEqual(puzzleSticks.Length, 6, "puzzleSticks.Length");
            
            List<Stick3DPosition> positions = new List<Stick3DPosition>();
            List<Stick3DRotation> rotations = new List<Stick3DRotation>();

            Sticks3DSet cross = this.FindPuzzle3DSolution(0, new Sticks3DSet(), puzzleSticks, positions, rotations);

            return cross;
        }

        private Sticks3DSet FindPuzzle3DSolution(int stickIndex, Sticks3DSet set, Stick3D[] puzzleSticks, List<Stick3DPosition> positions, List<Stick3DRotation> rotations)
        {
            if (set.HasCollisions)
            {
                return null;
            }

            if (stickIndex == puzzleSticks.Length)
            {
                return set;
            }

            foreach (Stick3DPosition position in Cross3D.GetStickPositions())
            {
                positions.Add(position);

                foreach (Stick3DRotation rotation in Enum.GetValues(typeof(Stick3DRotation)))
                {
                    rotations.Add(rotation);

                    set = new Sticks3DSet();
                    for (int i = 0; i <= stickIndex; i++)
                    {
                        set.AddStick(puzzleSticks[i], positions[i], rotations[i]);
                    }

                    set = this.FindPuzzle3DSolution(stickIndex + 1, set, puzzleSticks, positions, rotations);

                    if (set != null)
                    {
                        return set;
                    }

                    rotations.PopLast();
                }

                positions.PopLast();
            }

            return null;
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
