using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vrml.Model;

namespace VrmlSceneCreator
{
    public class VrmlExportContext
    {
        private readonly VrmlDocument document;

        public VrmlExportContext()
        {
            this.document = new VrmlDocument();
        }

        public VrmlDocument Document
        {
            get
            {
                return this.document;
            }
        }
    }
}
