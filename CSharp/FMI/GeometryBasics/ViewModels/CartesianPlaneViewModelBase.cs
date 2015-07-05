using Deyo.Controls.Charts;
using Deyo.Controls.Common;
using GeometryBasics.Algorithms;
using GeometryBasics.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace GeometryBasics.ViewModels
{
    public abstract class CartesianPlaneViewModelBase : ViewModelBase, IReleasable
    {
        private const int MoveDeltaTime = 20;
        private readonly CartesianPlane cartesianPlane;
        private readonly DispatcherTimer timer;
        private readonly CartesianPlaneRenderer renderer;
        private int previousMoveTimestamp;
        private bool isSelectingPoints;
        private bool isAnimating;
        private bool isFirstPointSelection;
        private bool isAttachedToMouseEvents;
        private ICartesianPlaneAlgorithm algorithm;
        private ICommand startAnimationCommand;
        private ICommand stopAnimationCommand;
        private ICommand startSelectionCommand;
        private ICommand stopSelectionCommand;

        public CartesianPlaneViewModelBase(CartesianPlane cartesianPlane)
        {
            this.isAttachedToMouseEvents = false;
            this.previousMoveTimestamp = 0;
            
            this.cartesianPlane = cartesianPlane;
            this.renderer = new CartesianPlaneRenderer(cartesianPlane);
            this.cartesianPlane.ViewportInfo = this.ViewportInfo;
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(this.AnimationTickSeconds);
            this.timer.Tick += TimerTick;

            this.StartAnimationCommand = new DelegateCommand((parameter) => { this.DoOnAnimationStart(); });
            this.StopAnimationCommand = new DelegateCommand((parameter) => { this.DoOnAnimationEnd(); });
            this.StartSelectionCommand = new DelegateCommand((parameter) => { this.DoOnSelectionStart(); });
            this.StopSelectionCommand = new DelegateCommand((parameter) => { this.DoOnSelectionEnd(); });

            this.InitializeFieldsOverride();
        }

        public ICommand StartSelectionCommand
        {
            get
            {
                return this.startSelectionCommand;
            }
            set
            {
                this.SetProperty(ref this.startSelectionCommand, value);
            }
        }

        public ICommand StartAnimationCommand
        {
            get
            {
                return this.startAnimationCommand;
            }
            set
            {
                this.SetProperty(ref this.startAnimationCommand, value);
            }
        }

        public ICommand StopSelectionCommand
        {
            get
            {
                return this.stopSelectionCommand;
            }
            set
            {
                this.SetProperty(ref this.stopSelectionCommand, value);
            }
        }

        public ICommand StopAnimationCommand
        {
            get
            {
                return this.stopAnimationCommand;
            }
            set
            {
                this.SetProperty(ref this.stopAnimationCommand, value);
            }
        }

        public bool IsAnimating
        {
            get
            {
                return this.isAnimating;
            }
            set
            {
                this.SetProperty(ref this.isAnimating, value);
            }
        }

        public bool IsSelectingPoints
        {
            get
            {
                return this.isSelectingPoints;
            }
            set
            {
                this.SetProperty(ref this.isSelectingPoints, value);
            }
        }

        protected virtual double AnimationTickSeconds
        {
            get
            {
                return 0.2;
            }
        }

        protected virtual ViewportInfo ViewportInfo
        {
            get
            {
                return new ViewportInfo(new Point(10, 10), 22);
            }
        }

        protected CartesianPlane CartesianPlane
        {
            get
            {
                return this.cartesianPlane;
            }
        }

        protected virtual bool HandleSelectionMove
        {
            get
            {
                return false;
            }
        }

        public void Initialize()
        {
            this.IsAnimating = false;
            this.IsSelectingPoints = false;
            this.isFirstPointSelection = false;
            this.CartesianPlane.ZoomPanControl.HandleLeftButtonDown = true;
            this.cartesianPlane.StartListeningToMouseEvents();

            this.ClearCartesianPlane();
            this.RenderInputData();
        }

        public void Release()
        {
            this.IsAnimating = false;
            this.IsSelectingPoints = false;
            this.isFirstPointSelection = false;
            this.CartesianPlane.ZoomPanControl.HandleLeftButtonDown = true;
            this.cartesianPlane.StopListeningToMouseEvents();
            this.DetachFromMouseSelectionEvents();
            this.StopAnimationTimer();

            this.ClearCartesianPlane();
        }

        protected abstract void InitializeFieldsOverride();

        protected abstract void RenderInputDataOverride();

        protected abstract ICartesianPlaneAlgorithm StartAlgorithm();

        protected abstract void OnPointSelectedOverride(Point point, bool isFirstPointSelection);

        protected abstract void OnSelectionMoveOverride(Point point);

        protected void DrawPointsInContext(Action pointsDrawingAction)
        {
            this.renderer.RenderPointsInContext(pointsDrawingAction);
        }

        protected void DrawLinesInContext(Action linesDrawingAction)
        {
            this.renderer.RenderLinesInContext(linesDrawingAction);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            using(this.cartesianPlane.SuspendLayoutUpdate())
            {
                this.algorithm.DrawNextStep();
            }

            if (algorithm.HasEnded)
            {
                this.StopAnimationTimer();
            }
        }

        private void DoOnAnimationStart()
        {
            this.ClearCartesianPlane();
            this.IsAnimating = true;
            this.IsSelectingPoints = false;
            this.algorithm = this.StartAlgorithm();

            this.StartAnimationTimer();
        }

        private void DoOnAnimationEnd()
        {
            this.StopAnimationTimer();
            this.ClearCartesianPlane();
            this.IsAnimating = false;
            this.IsSelectingPoints = false;
            this.algorithm = null;

            this.RenderInputData();
        }

        private void DoOnSelectionStart()
        {
            this.IsAnimating = false;
            this.IsSelectingPoints = true;
            this.isFirstPointSelection = true;
            this.CartesianPlane.ZoomPanControl.HandleLeftButtonDown = false;
            this.AttachToMouseSelectionEvents();
        }

        private void DoOnSelectionEnd()
        {
            this.DetachFromMouseSelectionEvents();
            this.CartesianPlane.ZoomPanControl.HandleLeftButtonDown = true;
            this.isFirstPointSelection = false;
            this.IsAnimating = false;
            this.IsSelectingPoints = false;
        }

        private void RenderInputData()
        {
            using (this.cartesianPlane.SuspendLayoutUpdate())
            {
                this.RenderInputDataOverride();
            }
        }

        private void AttachToMouseSelectionEvents()
        {
            if (!this.isAttachedToMouseEvents)
            {
                this.isAttachedToMouseEvents = true;

                this.cartesianPlane.MouseDown += CartesianPlane_MouseDown;
                this.cartesianPlane.MouseMove += CartesianPlane_MouseMove;
            }
        }

        private void DetachFromMouseSelectionEvents()
        {
            if (this.isAttachedToMouseEvents)
            {
                this.isAttachedToMouseEvents = false;

                this.cartesianPlane.MouseDown -= CartesianPlane_MouseDown;
                this.cartesianPlane.MouseMove -= CartesianPlane_MouseMove;
            }
        }

        private void CartesianPlane_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.HandleSelectionMove && (e.Timestamp - this.previousMoveTimestamp > CartesianPlaneViewModelBase.MoveDeltaTime))
            {
                this.previousMoveTimestamp = e.Timestamp;

                this.OnSelectionMoveOverride(this.CartesianPlane.GetCartesianPointFromMousePosition(e));
            }
        }

        private void CartesianPlane_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                using (this.CartesianPlane.SuspendLayoutUpdate())
                {
                    this.OnPointSelectedOverride(this.CartesianPlane.GetCartesianPointFromMousePosition(e), this.isFirstPointSelection);
                    this.isFirstPointSelection = false;
                }
            }
        }

        private void StartAnimationTimer()
        {
            if (!this.timer.IsEnabled)
            {
                this.timer.Start();
            }
        }

        private void StopAnimationTimer()
        {
            if (this.timer.IsEnabled)
            {
                this.timer.Stop();
            }
        }

        private void ClearCartesianPlane()
        {
            using (this.cartesianPlane.SuspendLayoutUpdate())
            {
                this.cartesianPlane.ClearAllElements();
            }
        }
    }
}
