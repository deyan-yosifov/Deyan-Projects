using System;
using System.Collections.Generic;
using Vrml.Model;
using Vrml.Model.Shapes;

namespace Vrml.FormatProvider.ElementWriters
{
    internal static class Writers
    {
        private static Dictionary<Type, ElementWriterBase> writers;

        static Writers()
        {
            writers = new Dictionary<Type, ElementWriterBase>();

            RegisterElementWriter(typeof(VrmlDocument), new VrmlDocumentWriter());
            RegisterElementWriter(typeof(Viewpoint), new ViewpointWriter());
            RegisterElementWriter(typeof(NavigationInfo), new NavigationInfoWriter());
            RegisterElementWriter(typeof(Transformation), new TransformationWriter());
            RegisterElementWriter(typeof(Extrusion), new ExtrusionWriter());
            RegisterElementWriter(typeof(Appearance), new AppearanceWriter());
            RegisterElementWriter(typeof(IndexedLineSet), new IndexedLineSetWriter());
        }

        public static void Write(IVrmlElement element, Writer writer)
        {
            if (element != null)
            {
                ElementWriterBase elementWriter = GetElementWriter(element.GetType());
                elementWriter.Write(element, writer);
            }
        }

        public static ElementWriterBase GetElementWriter(Type type)
        {
            return writers[type];
        }

        private static void RegisterElementWriter(Type type, ElementWriterBase writer)
        {
            writers[type] = writer;
        }
    }
}
