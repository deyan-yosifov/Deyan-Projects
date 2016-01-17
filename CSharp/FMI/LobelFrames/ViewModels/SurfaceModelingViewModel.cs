using Deyo.Controls.Common;
using Deyo.Controls.Controls3D;
using LobelFrames.DataStructures.Surfaces;
using LobelFrames.ViewModels.Commands;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace LobelFrames.ViewModels
{
    public class SurfaceModelingViewModel : ViewModelBase
    {
        private readonly Scene3D scene;
        private readonly HintManager hintManager;
        private readonly InputManager inputManager;
        private readonly CommandDescriptors commandDescriptors;
        private readonly SceneElementsPool elementsPool;
        private readonly SurfaceModelingContext context;

        public SurfaceModelingViewModel(Scene3D scene)
        {
            this.scene = scene;
            this.hintManager = new HintManager();
            this.inputManager = new InputManager();
            this.context = new SurfaceModelingContext();
            this.elementsPool = new SceneElementsPool(scene);
            this.commandDescriptors = new CommandDescriptors(this);

            this.AttachToEvents();
            this.InitializeScene();
        }

        public SceneElementsPool ElementsPool
        {
            get
            {
                return this.elementsPool;
            }
        }

        public CommandDescriptors CommandDescriptors
        {
            get
            {
                return this.commandDescriptors;
            }
        }

        public HintManager HintManager
        {
            get
            {
                return this.hintManager;
            }
        }

        public InputManager InputManager
        {
            get
            {
                return this.inputManager;
            }
        }

        private SurfaceModelingContext Context
        {
            get
            {
                return this.context;
            }
        }

        public void AddLobelMesh()
        {
            LobelSurface surface = new LobelSurface(this.ElementsPool, 7, 5, 3);
            this.Context.Surfaces.Add(surface);
            this.Context.SelectedSurface = surface;
        }

        private void InitializeScene()
        {
            byte directionIntensity = 250;
            byte ambientIntensity = 125;
            this.scene.Editor.AddDirectionalLight(Color.FromRgb(directionIntensity, directionIntensity, directionIntensity), new Vector3D(-1, -3, -5));
            this.scene.Editor.AddAmbientLight(Color.FromRgb(ambientIntensity, ambientIntensity, ambientIntensity));
            this.scene.Editor.Look(new Point3D(25, 25, 35), new Point3D());

            this.scene.StartListeningToMouseEvents();
        }

        private void AttachToEvents()
        {
            this.InputManager.ParameterInputed += this.HandleInputManagerParameterInputed;
            this.Context.HistoryManager.HistoryChanged += this.HandleHistoryChanges;
        }

        private void HandleHistoryChanges(object sender, EventArgs e)
        {
            this.CommandDescriptors[CommandType.Undo].IsEnabled = this.Context.HistoryManager.CanUndo;
            this.CommandDescriptors[CommandType.Undo].IsVisible = this.Context.HistoryManager.CanUndo;
            this.CommandDescriptors[CommandType.Redo].IsEnabled = this.Context.HistoryManager.CanRedo;
            this.CommandDescriptors[CommandType.Redo].IsVisible = this.Context.HistoryManager.CanRedo;
        }

        private void HandleInputManagerParameterInputed(object sender, ParameterInputedEventArgs e)
        {
            MessageBox.Show(string.Format("Parameter inputed: {0}", e.Parameter));
        }
    }
}
