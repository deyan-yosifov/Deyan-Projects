using System;
using System.Windows.Media.Media3D;
using Deyo.Vrml.Core;

namespace Deyo.Vrml.Model
{
    public class Transformation : IVrmlElement
    {
        private readonly Collection<IVrmlElement> children;

        public Transformation()
        {
            this.children = new Collection<IVrmlElement>();
        }

        public string Comment { get; set; }
        public string DefinitionName { get; set; }
        public Position Center { get; set; }
        public Orientation Rotation { get; set; }
        public Position Scale { get; set; }
        public Position ScaleOrientation { get; set; }
        public Position Translation { get; set; }

        public Collection<IVrmlElement> Children
        {
            get
            {
                return this.children;
            }
        }

        public string Name
        {
            get
            {
                return ElementNames.Transform;
            }
        }

        public static class EventsIn
        {
            public const string SetRotation = "set_rotation";
            public const string SetScale = "set_scale";
            public const string SetTranslation = "set_translation";
            public const string SetCenter = "set_center";
            public const string SetScaleOrientation = "set_scaleOrientation";
        }
    }
}
