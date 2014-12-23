using System;
using Vrml.Model;

namespace Vrml.FormatProvider.ElementWriters
{
    internal class NavigationInfoWriter : ElementWriterBase
    {
        public void Write(NavigationInfo element, Writer writer)
        {
            writer.WriteLine("NavigationInfo {0}", Writer.LeftBracket);

            writer.MoveIn();
            writer.WriteLine("type [ \"EXAMINE\", \"ANY\" ]");
            writer.WriteLine("headlight TRUE");
            writer.MoveOut();

            writer.WriteLine(Writer.RightBracket);
            writer.WriteLine();
        }

        public override void Write<T>(T element, Writer writer)
        {
            NavigationInfo navigationInfo = element as NavigationInfo;

            if (navigationInfo != null)
            {
                this.Write(navigationInfo, writer);
            }
        }
    }
}
