using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vrml.Core;
using Vrml.Model.Shapes;

namespace Vrml.Model
{
    public class Transformation : IVrmlElement
    {
        private readonly Collection<IShape> children;

        public Transformation()
        {
            this.children = new Collection<IShape>();
        }

        public string Name { get; set; }

        public Collection<IShape> Children
        {
            get
            {
                return this.children;
            }
        }
        
    }
}
