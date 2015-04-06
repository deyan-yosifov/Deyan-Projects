using System;
using System.Collections.Generic;
using Deyo.Vrml.Model;
using Deyo.Vrml.Model.Animations;
using Deyo.Vrml.Model.Shapes;

namespace Deyo.Vrml.FormatProvider.ElementWriters
{
    internal static class Writers
    {
        private static readonly Dictionary<Type, ElementWriterBase> writers;

        static Writers()
        {
            writers = new Dictionary<Type, ElementWriterBase>();

            InterpolatorWriter interpolatorWriter = new InterpolatorWriter();

            RegisterElementWriter(typeof(Viewpoint), new ViewpointWriter());
            RegisterElementWriter(typeof(NavigationInfo), new NavigationInfoWriter());
            RegisterElementWriter(typeof(Transformation), new TransformationWriter());
            RegisterElementWriter(typeof(Extrusion), new ExtrusionWriter());
            RegisterElementWriter(typeof(Sphere), new SphereWriter());
            RegisterElementWriter(typeof(Appearance), new AppearanceWriter());
            RegisterElementWriter(typeof(IndexedLineSet), new IndexedLineSetWriter());
            RegisterElementWriter(typeof(OrientationInterpolator), interpolatorWriter);
            RegisterElementWriter(typeof(PositionInterpolator), interpolatorWriter);
            RegisterElementWriter(typeof(TimeSensor), new TimeSensorWriter());
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
