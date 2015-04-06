using System;
using Deyo.Vrml.Model.Animations;

namespace Deyo.Vrml.FormatProvider.ElementWriters
{
    internal class InterpolatorWriter : ElementWriterBase
    {
        private const string Key = "key";
        private const string KeyValue = "keyValue";

        public override void WriteOverride<T>(T element, Writer writer)
        {
            OrientationInterpolator orientationInterpolator = element as OrientationInterpolator;
            if (orientationInterpolator != null)
            {
                writer.WriteArrayCollection(orientationInterpolator.Keys, Key);
                writer.WriteArrayCollection(orientationInterpolator.Values, KeyValue);
                return;
            }

            PositionInterpolator positionInterpolator = element as PositionInterpolator;
            if (positionInterpolator != null)
            {
                writer.WriteArrayCollection(positionInterpolator.Keys, Key);
                writer.WriteArrayCollection(positionInterpolator.Values, KeyValue);
                return;
            }

            throw new NotSupportedException("Parameter element must be of some supported Interpolator type!");
        }
    }
}
