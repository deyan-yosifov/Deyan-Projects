using Deyo.Controls.Controls3D;
using Deyo.Controls.Controls3D.Cameras;
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

namespace Deyo.Controls.Controls3D
{
    /// <summary>
    /// Interaction logic for Scene3D.xaml
    /// </summary>
    public partial class Scene3D : UserControl
    {
        private readonly SceneEditor editor;
        private readonly OrbitControl orbitControl;
        private readonly Canvas viewport2D;
        private readonly Viewport3D viewport3D;

        public Scene3D()
        {
            InitializeComponent();

            this.viewport2D = new Canvas() { IsHitTestVisible = true, Background = new SolidColorBrush(Colors.Transparent) };
            this.viewport3D = new Viewport3D() { IsHitTestVisible = false };
            this.editor = new SceneEditor(this.viewport3D);
            this.orbitControl = new OrbitControl(this);

            this._container.Children.Add(this.viewport3D);
            this._container.Children.Add(this.viewport2D);
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
