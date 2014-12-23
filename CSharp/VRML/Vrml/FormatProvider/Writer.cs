using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using Vrml.Core;
using Vrml.Model;

namespace Vrml.FormatProvider
{
    internal class Writer
    {
        public const char Space = ' ';
        public const char LeftBracket = '{';
        public const char RightBracket = '}';
        public const char OffsetSymbol = '\t';
        private readonly ExportContext context;
        private readonly StringBuilder builder;

        public Writer()
        {
            this.context = new ExportContext();
            this.builder = new StringBuilder();
        }

        public ExportContext Context
        {
            get
            {
                return this.context;
            }
        }

        private string Offset
        {
            get
            {
                return new string(OffsetSymbol, this.Context.WriteOffset);
            }
        }

        public void WriteLine()
        {
            this.builder.AppendLine();
        }

        public void WriteLine(char symbol)
        {
            this.WriteLine(symbol.ToString());
        }

        public void WriteLine(string text, params object[] parameters)
        {
            this.WriteOffset();
            this.Write(text, parameters);
            this.WriteLine();
        }

        public void Write(char symbol)
        {
            this.Write(symbol.ToString());
        }

        public void Write(string text, params object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                this.builder.Append(text);
            }
            else
            {
                this.builder.AppendFormat(text, parameters);
            }
        }

        public void WriteOffset()
        {
            this.Write(this.Offset);
        }

        public void Write(Point3D point)
        {
            this.Write("{0} {1} {2}", point.X, point.Y, point.Z);
        }

        public void Write(Vector3D vector)
        {
            this.Write("{0} {1} {2}", vector.X, vector.Y, vector.Z);
        }

        public void Write(Point point)
        {
            this.Write("{0} {1}", point.X, point.Y);
        }

        public void Write(Size size)
        {
            this.Write("{0} {1}", size.Width, size.Height);
        }

        public void Write(Orientation orientation)
        {
            this.Write(orientation.Vector);
            this.Write(string.Format(" {0}", orientation.Angle));
        }

        public void WriteLine(Point point)
        {
            this.WriteOffset();
            this.Write(point);
            this.WriteLine();
        }

        public void WriteLine(Point3D point)
        {
            this.WriteOffset();
            this.Write(point);
            this.WriteLine();
        }

        public void WriteLine(Vector3D vector)
        {
            this.WriteOffset();
            this.Write(vector);
            this.WriteLine();
        }

        public void WriteLine(Orientation orientation)
        {
            this.WriteOffset();
            this.Write(orientation);
            this.WriteLine();
        }

        public void WriteLine(Size size)
        {
            this.WriteOffset();
            this.Write(size);
            this.WriteLine();
        }

        public void WriteArrayCollection<T>(Collection<T> collection, string collectionName, Action<T, Writer> writeElement)
        {
            this.WriteLine("{0} [", collectionName);
            this.MoveIn();

            for (int i = 0; i < collection.Count - 1; i++)
            {
                T element = collection[i];
                this.WriteOffset();
                writeElement(element, this);
                this.Write(",");
                this.WriteLine();
            }

            T lastElement = collection[collection.Count - 1];
            this.WriteOffset();
            writeElement(lastElement, this);
            this.WriteLine();

            this.MoveOut();
            this.WriteLine("]");
        }

        public void MoveIn()
        {
            this.Context.WriteOffset++;
        }

        public void MoveOut()
        {
            this.Context.WriteOffset--;
        }

        internal void Initialize()
        {
            this.Context.WriteOffset = 0;
            this.builder.Clear();
        }

        public override string ToString()
        {
            return this.builder.ToString();
        }
    }
}
