using System;
using System.IO;
using System.Windows.Media.Media3D;
using Deyo.Vrml.Geometries;
using Deyo.Vrml.Model.Shapes;

namespace Deyo.Vrml.FormatProvider
{
    public class ExtrusionImporter
    {
        private enum ImportState
        {
            Face, Normal, Spine, Invalid
        }

        private ImportState state;
        private ExtrusionGeometry extrusion;

        public static ExtrusionGeometry ImportFromText(string text)
        {
            return new ExtrusionImporter().Import(text);
        }

        private Face Face
        {
            get
            {
                return this.extrusion.Face;
            }
        }

        private Polyline Polyline
        {
            get
            {
                return this.extrusion.Polyline;
            }
        }

        public ExtrusionGeometry Import(string text)
        {
            this.Initialize();

            string[] lines = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                if (!this.TryChangeState(line))
                {
                    this.ParsePoint(line);
                }
            }

            return this.extrusion;
        }

        private void Initialize()
        {
            this.state = ImportState.Invalid;
            this.extrusion = new ExtrusionGeometry();
        }

        private void ParsePoint(string line)
        {
            string[] coordinates = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            double x = double.Parse(coordinates[0]);
            double y = double.Parse(coordinates[1]);
            double z = double.Parse(coordinates[2]);

            Point3D point = new Point3D(x, y, z);

            switch (this.state)
            {
                case ImportState.Face:
                    this.Face.Points.Add(point);
                    break;
                case ImportState.Normal:
                    this.Face.NormalVector = new Vector3D(point.X, point.Y, point.Z);
                    break;
                case ImportState.Spine:
                    this.Polyline.Points.Add(point);
                    break;
                default:
                    throw new NotSupportedException(string.Format("Invalid parse state {0}!", this.state.ToString()));
            }
        }

        private bool TryChangeState(string line)
        {
            switch (line)
            {
                case "face":
                    this.state = ImportState.Face;
                    break;
                case "normal":
                    this.state = ImportState.Normal;
                    break;
                case "spine":
                    this.state = ImportState.Spine;
                    break;
                default:
                    return false;
            }

            return true;
        }
    }
}
