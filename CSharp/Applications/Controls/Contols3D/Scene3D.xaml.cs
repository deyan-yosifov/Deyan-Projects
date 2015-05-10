using Deyo.Controls.Controls3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Deyo.Controls.Contols3D
{
    /// <summary>
    /// Interaction logic for Scene3D.xaml
    /// </summary>
    public partial class Scene3D : UserControl
    {
        private readonly SceneEditor editor;
        private readonly OrbitControl orbitControl;

        public Scene3D()
        {
            InitializeComponent();

            this.editor = new SceneEditor(this.viewport3D);
            this.orbitControl = new OrbitControl(this);
        }

        public SceneEditor Editor
        {
            get
            {
                return editor;
            }
        }

        public Canvas Viewport2D
        {
            get
            {
                return this.viewport2D;
            }
        }

        public OrbitControl OrbitControl
        {
            get
            {
                return this.orbitControl;
            }
        }
    }
}
