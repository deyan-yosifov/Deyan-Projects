using System;
using Vrml.Core;
using Vrml.Model.Animations;

namespace Vrml.FormatProvider.ElementWriters
{
    internal class TimeSensorWriter : ElementWriterBase
    {
        public override void WriteOverride<T>(T element, Writer writer)
        {
            TimeSensor timeSensor = element as TimeSensor;
            this.Write(timeSensor, writer);   
        }

        public void Write(TimeSensor timeSensor, Writer writer)
        {
            Guard.ThrowExceptionIfNull(timeSensor, "timeSensor");

            writer.WriteLine("cycleInterval {0}", timeSensor.CycleInterval);
            writer.WriteLine("loop {0}", timeSensor.Loop.ToString().ToUpper());
            writer.WriteLine("startTime {0}", timeSensor.StartTime);
            writer.WriteLine("stopTime {0}", timeSensor.StopTime);
        }
    }
}
