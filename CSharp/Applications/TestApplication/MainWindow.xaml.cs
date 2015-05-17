using Deyo.Controls.Contols3D.Shapes;
using Deyo.Controls.Controls3D;
using Deyo.Vrml.Core;
using Deyo.Vrml.Editing;
using Deyo.Vrml.FormatProvider;
using Deyo.Vrml.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

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
            editor.AddDirectionalLight(Color.FromRgb(directionIntensity, directionIntensity, directionIntensity), new Vector3D(-1, -3, -5));
            editor.AddAmbientLight(Color.FromRgb(ambientIntensity, ambientIntensity, ambientIntensity));

            Cube cube = new Cube();
            cube.AddDiffuseMaterial(Colors.Red);
            cube.AddBackDiffuseMaterial(Colors.Blue);
            editor.AddShapeVisual(cube);
            
            editor.Position.Translate(new Vector3D(0.5, 0.5, 0.5));

            using (editor.SavePosition())
            {
                editor.Position.Rotate(new Quaternion(new Vector3D(1, 1, 1), 90));
                editor.AddShapeVisual(cube);
            }

            editor.AddShapeVisual(cube);

            editor.Look(new Point3D(3, 3, 3), new Point3D());

            this.viewport.OrbitControl.Start();
        }
    }
}
