using System;
using Deyo.Vrml.Core;
using Deyo.Vrml.Model;

namespace Deyo.Vrml.FormatProvider.ElementWriters
{
    internal class NavigationInfoWriter : ElementWriterBase
    {
        public void Write(NavigationInfo navigationInfo, Writer writer)
        {
            Guard.ThrowExceptionIfNull(navigationInfo, "navigationInfo");

            writer.WriteLine("type [ \"EXAMINE\", \"ANY\" ]");
            writer.WriteLine("headlight TRUE");
        }

        public override void WriteOverride<T>(T element, Writer writer)
        {
            NavigationInfo navigationInfo = element as NavigationInfo;
            this.Write(navigationInfo, writer);
        }
    }
}
