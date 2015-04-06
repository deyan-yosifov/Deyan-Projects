using System;
using Deyo.Vrml.Core;

namespace Deyo.Vrml.Model.Animations
{
    public class Route
    {
        private readonly IVrmlElement elementOut;
        private readonly string eventOut;
        private readonly IVrmlElement elementIn;
        private readonly string eventIn;

        public Route(IVrmlElement elementOut, string eventOut, IVrmlElement elementIn, string eventIn)
        {
            Guard.ThrowExceptionIfNullOrEmpty(elementOut.DefinitionName, "elementOut.DefinitionName");
            Guard.ThrowExceptionIfNullOrEmpty(elementIn.DefinitionName, "elementIn.DefinitionName");

            this.elementOut = elementOut;
            this.eventOut = eventOut;
            this.elementIn = elementIn;
            this.eventIn = eventIn;
        }

        public IVrmlElement ElementOut
        {
            get
            {
                return this.elementOut;
            }
        }

        public string EventOut
        {
            get
            {
                return this.eventOut;
            }
        }

        public IVrmlElement ElementIn
        {
            get
            {
                return this.elementIn;
            }
        }

        public string EventIn
        {
            get
            {
                return this.eventIn;
            }
        }
    }
}
