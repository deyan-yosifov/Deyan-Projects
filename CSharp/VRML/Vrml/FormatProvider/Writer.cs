using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Vrml.Core;
using Vrml.Model;
using Vrml.Model.Animations;

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

        public void Write(double number)
        {
            this.Write(number.ToString());
        }

        public void Write(Point point)
        {
            this.Write("{0} {1}", point.X, point.Y);
        }

        public void Write(Size size)
        {
            this.Write("{0} {1}", size.Width, size.Height);
        }

        public void Write(IVrmlSimpleType simpleType)
        {
            this.Write(simpleType.VrmlText);
        }

        public void Writeline(Route route)
        {
            this.WriteLine("ROUTE {0}.{1} TO {2}.{3}",
                route.ElementOut.DefinitionName,
                route.EventOut,
                route.ElementIn.DefinitionName,
                route.EventIn);
        }

        public void WriteArrayCollection(IEnumerable<IVrmlSimpleType> simpleTypes, string collectionName)
        {
            this.WriteArrayCollection(new Collection<IVrmlSimpleType>(simpleTypes), collectionName, (simpleType, wr) => { wr.Write(simpleType); });
        }

        public void WriteArrayCollection(IEnumerable<double> numbers, string collectionName)
        {
            this.WriteArrayCollection(new Collection<double>(numbers), collectionName, (num, wr) => { wr.Write(num); });
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
