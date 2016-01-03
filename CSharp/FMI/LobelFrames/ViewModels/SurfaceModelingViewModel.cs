using Deyo.Controls.Common;
using Deyo.Controls.Controls3D;
using LobelFrames.DataStructures.Surfaces;
using LobelFrames.ViewModels.Commands;
using System;
using System.Windows;

namespace LobelFrames.ViewModels
{
    public class SurfaceModelingViewModel : ViewModelBase
    {
        private readonly Scene3D scene;
        private readonly HintManager hintManager;
        private readonly InputManager inputManager;
        private readonly CommandDescriptors commandDescriptors;
        private readonly SceneElementsPool elementsPool;

        public SurfaceModelingViewModel(Scene3D scene)
        {
            this.scene = scene;
            this.hintManager = new HintManager();
            this.inputManager = new InputManager();
            this.commandDescriptors = new CommandDescriptors(this);
            this.elementsPool = new SceneElementsPool(scene);
            this.inputManager.ParameterInputed += InputManager_ParameterInputed;
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

        private void InputManager_ParameterInputed(object sender, ParameterInputedEventArgs e)
        {
            MessageBox.Show(string.Format("Parameter inputed: {0}", e.Parameter));
        }

        
    }
}
