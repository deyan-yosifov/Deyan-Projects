using System;
using Deyo.Vrml.Core;
using Deyo.Vrml.Model.Shapes;

namespace Deyo.Vrml.FormatProvider.ElementWriters
{
    internal class IndexedLineSetWriter : ShapeWriterBase
    {
        public void WriteGeometry(IndexedLineSet lineSet, Writer writer)
        {
            Guard.ThrowExceptionIfNull(lineSet, "lineSet");

            writer.WriteLine("geometry IndexedLineSet {0}", Writer.LeftBracket);
            writer.MoveIn();

            if (!lineSet.Points.IsEmpty)
            {
                writer.WriteLine("coord Coordinate {0}", Writer.LeftBracket);
                writer.MoveIn();

                writer.WriteArrayCollection(lineSet.Points, "point", (point, wr) => { wr.Write(point); });

                writer.MoveOut();
                writer.WriteLine(Writer.RightBracket);
            }

            if (!lineSet.Indexes.IsEmpty)
            {
                writer.WriteArrayCollection(lineSet.Indexes, "coordIndex", (index, wr) => { wr.Write(index.ToString()); });
            }

            writer.MoveOut();
            writer.WriteLine(Writer.RightBracket);
        }

        public override void WriteGeometry(IShape element, Writer writer)
        {
            IndexedLineSet lineSet = element as IndexedLineSet;
            this.WriteGeometry(lineSet, writer);
        }
    }
}
