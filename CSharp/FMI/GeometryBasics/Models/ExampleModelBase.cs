using GeometryBasics.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GeometryBasics.Models
{
    public abstract class ExampleModelBase
    {
        private readonly Func<ExampleUserControl> createView;
        private ExampleUserControl view;

        public ExampleModelBase(string name, string description, Func<ExampleUserControl> createView)
        {
            this.Name = name;
            this.Description = description;
            this.CanCreateView = false;
            this.createView = createView;
        }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public bool CanCreateView { get; set; }

        public ExampleUserControl View
        {
            get
            {
                if (this.view == null && this.CanCreateView)
                {
                    this.view = this.createView();
                }

                return this.view;
            }
        }
    }
}
