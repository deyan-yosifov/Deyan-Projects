using Deyo.Controls.Controls3D;
using Deyo.Controls.Controls3D.Shapes;
using Deyo.Controls.Controls3D.Visuals;
using Deyo.Core.Media.Imaging;
using Deyo.Vrml.Core;
using Deyo.Vrml.Editing;
using Deyo.Vrml.FormatProvider;
using Deyo.Vrml.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using TestApplication.Resources;

namespace TestApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ExportFileName = "TestVrmlDocument.wrl";

        public MainWindow()
        {
            InitializeComponent();

            this.InitializeViewport();
        }

        private void BrowseFolderWithWinForms_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show(dialog.SelectedPath);
            }
        }

        private void DeyoBrowseFolderDialog_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Deyo.Controls.Dialogs.Explorer.FolderBrowserDialog();

            if (dialog.ShowDialog() == true)
            {
                MessageBox.Show(dialog.SelectedPath);
            }
        }

        private void ExportVrml_Click(object sender, RoutedEventArgs e)
        {
            VrmlDocument document = MainWindow.CreateDocument();
            document.Title = "My test document";
            document.Background = new VrmlColor(Colors.AliceBlue);

            if (File.Exists(ExportFileName))
            {
                File.Delete(ExportFileName);
            }

            using (Stream fileStream = new FileStream(ExportFileName, FileMode.OpenOrCreate))
            {
                VrmlFormatProvider provider = new VrmlFormatProvider();
                provider.Export(document, fileStream);
            }

            Process.Start(Directory.GetCurrentDirectory());
            Process.Start(ExportFileName);
        }

        private static VrmlDocument CreateDocument()
        {
            VrmlDocumentEditor editor = new VrmlDocumentEditor(new VrmlDocument());

            Rect rect = new Rect(new Point(0, 3), new Point(3, 0));

            editor.GraphicProperties.StrokeThickness = 1;
            editor.GraphicProperties.StrokeColor = new VrmlColor(Colors.GreenYellow);
            editor.DrawPolyline(new Point[] 
            {
                rect.BottomLeft, 
                rect.TopLeft,
                rect.TopRight, 
            });

            editor.GraphicProperties.StrokeThickness = 2;
            editor.GraphicProperties.StrokeColor = new VrmlColor(Colors.Red);
            editor.DrawPoint(rect.BottomLeft);
            editor.GraphicProperties.StrokeColor = new VrmlColor(Colors.Green);
            editor.DrawPoint(rect.TopLeft);
            editor.GraphicProperties.StrokeColor = new VrmlColor(Colors.Blue);
            editor.DrawPoint(rect.TopRight);

            editor.GraphicProperties.StrokeColor = new VrmlColor(Colors.Orange);
            editor.GraphicProperties.StrokeThickness = 0.2;
            DrawViewAndLine(editor, new Point3D(10, 10, 10));
            DrawViewAndLine(editor, new Point3D(0, -10, 10));
            DrawViewAndLine(editor, new Point3D(10, 3, 10));
            DrawViewAndLine(editor, new Point3D(0, 0, 10));
            DrawViewAndLine(editor, new Point3D(0, 0, -10));
            DrawViewAndLine(editor, new Point3D(10, 0, 0));
            DrawViewAndLine(editor, new Point3D(0, 10, 0));

            editor.AddView(new Point3D(10, 5, 20), new Point3D(3, 2, 1));
            editor.DrawLine(Point3D.Add(new Point3D(3, 2, 1), Vector3D.Divide(new Point3D(10, 5, 20) - new Point3D(3, 2, 1), 2)), new Point3D(3, 2, 1));

            return editor.Document;
        }

        private static void DrawViewAndLine(VrmlDocumentEditor editor, Point3D point)
        {
            editor.AddView(point, new Point3D(0, 0, 0));
            editor.DrawLine(new Point3D(), point.Multiply(0.5));
        }

        private void InitializeViewport()
        {
            SceneEditor editor = this.viewport.Editor;

            byte directionIntensity = 250;
            byte ambientIntensity = 125;
            VisualOwner light = editor.AddDirectionalLight(Color.FromRgb(directionIntensity, directionIntensity, directionIntensity), new Vector3D(-1, -3, -5));
            editor.AddAmbientLight(Color.FromRgb(ambientIntensity, ambientIntensity, ambientIntensity));
                       
            editor.GraphicProperties.IsSmooth = true;
            editor.GraphicProperties.ArcResolution = 20;
            //editor.GraphicProperties.MaterialsManager.AddFrontDiffuseMaterial(Colors.Orange); 
            editor.GraphicProperties.MaterialsManager.AddBackDiffuseMaterial(Colors.Green);

            using (editor.SaveGraphicProperties())
            {
                editor.GraphicProperties.MaterialsManager.AddFrontTexture(JpegDecoder.GetBitmapSource(ResourceHelper.GetResourceStream("Resources/earth_map.jpg")));

                editor.GraphicProperties.Thickness = 0.2;
                LineVisual line = editor.AddLineVisual(new Point3D(0, 0, 0), new Point3D(-1, -1, -1));
                editor.AddShapeVisual(editor.ShapeFactory.CreateCylinder(false));

                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(0.5);
                Matrix3D rotation = new Matrix3D();
                rotation.Rotate(new Quaternion(new Vector3D(0, 0, 1), 10));
                light.Visual.Transform = new Transform3DGroup();
                timer.Tick += (s, e) =>
                    {
                        line.MoveTo(line.Start, rotation.Transform(line.End));
                        ((Transform3DGroup)light.Visual.Transform).Children.Add(new MatrixTransform3D(rotation));
                    };
                timer.Start();

                using (editor.SavePosition())
                {
                    editor.Position.Translate(new Vector3D(-1, 1, 0));
                    editor.AddShapeVisual(editor.ShapeFactory.CreateSphere().Shape);

                    editor.GraphicProperties.SphereType = SphereType.IcoSphere;
                    editor.GraphicProperties.SubDevisions = 3;
                    editor.Position.Translate(new Vector3D(0, -1.5, 0));
                    editor.AddShapeVisual(editor.ShapeFactory.CreateSphere().Shape);
                }
            }

            using (editor.SaveGraphicProperties())
            {
                editor.GraphicProperties.MaterialsManager.AddFrontTexture(JpegDecoder.GetBitmapSource(ResourceHelper.GetResourceStream("Resources/TeamBuildingPamporovo.jpg")));
                editor.Position.Scale(new Vector3D(0.5, 0.5, 0.5));

                editor.Position.Translate(new Vector3D(1, 1, 0.5));
                Cube cube = editor.ShapeFactory.CreateCube();
                editor.AddShapeVisual(cube);
                editor.Position.Translate(new Vector3D(1, 1, 0.5));

                using (editor.SavePosition())
                {
                    editor.Position.RotateAt(new Quaternion(new Vector3D(0.5, 0.5, 0.5), 90), new Point3D(0.25, 0.25, 0.25));
                    editor.AddShapeVisual(cube);
                }

                editor.AddShapeVisual(cube);
            }

            editor.Look(new Point3D(3, 3, 3), new Point3D());

            this.viewport.StartListeningToMouseEvents();
        }
    }
}
