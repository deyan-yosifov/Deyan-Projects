using System;

namespace Vrml.Model.Animations
{
    public class TimeSensor : IVrmlElement
    {
        public string Name
        {
            get
            {
                return ElementNames.TimeSensor;
            }
        }

        public string DefinitionName { get; set; }

        public string Comment { get; set; }

        public double CycleInterval { get; set; }
        public double StartTime { get; set; }
        public double StopTime { get; set; }
        public bool Loop { get; set; }

        public static class EventsOut
        {
            public const string FractionChanged = "fraction_changed";
        }
    }
}
