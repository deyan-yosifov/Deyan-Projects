using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace VrmlSceneCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {     
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CombineShapes_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string vrmlScene = CombineShapesFromFiles(ofd.FileNames);

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "VRML Scene|*.wrl";
                sfd.FileName = "CrossBow-DeyanYosifov-M24906.wrl";

                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    using (Stream fileStream = sfd.OpenFile())
                    {
                        StreamWriter writer = new StreamWriter(fileStream);//, Encoding.UTF8);
                        writer.Write(vrmlScene);
                        writer.Flush();
                    }
                }
            }
        }

        private static string CombineShapesFromFiles(string[] fileNames)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(Constants.SceneStart);

            foreach (string fileName in fileNames)
            {
                StreamReader reader = new StreamReader(fileName);//, Encoding.UTF8);
                string shape = reader.ReadToEnd();
                string shapeOffset = GetShapeOffset(shape);
                string shapeName = Path.GetFileNameWithoutExtension(fileName);

                builder.AppendLine(String.Format("{0}# {1} starts", shapeOffset, shapeName));
                builder.AppendLine(shape);
                builder.AppendLine(String.Format("{0}# {1} ends", shapeOffset, shapeName));
                builder.AppendLine();
            }

            builder.AppendLine(Constants.SceneEnd);

            return builder.ToString();
        }

        private static string GetShapeOffset(string shape)
        {
            StringBuilder builder = new StringBuilder();

            foreach (char symbol in shape)
            {
                if (char.IsWhiteSpace(symbol))
                {
                    builder.Append(symbol);
                }
                else
                {
                    break;
                }
            }

            return builder.ToString();
        }
    }
}
