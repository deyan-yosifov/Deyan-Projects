using Deyo.Controls.Charts;
using Deyo.Controls.Common;
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
        private CartesianPlane cartesianPlane;
        private DispatcherTimer timer;
        private int previousMoveTimestamp;
        private bool areFieldInitialized;
        private bool isSelectingPoints;
        private bool isAnimating;
        private bool isFirstPointSelection;
        private bool isAttachedToMouseEvents;

        public CartesianPlaneViewModelBase(CartesianPlane cartesianPlane)
        {
            this.isAttachedToMouseEvents = false;
            this.areFieldInitialized = false;
            this.InitializeFields(cartesianPlane);
            this.previousMoveTimestamp = 0;
            this.cartesianPlane.ZoomPanControl.ZoomWidthSpeed = 3;

            this.Initialize();
            this.AttachToMouseEvents();
        }

        protected virtual double AnimationTickSeconds
        {
            get
            {
                return 0.1;
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

        public bool IsAnimating
        {
            get
            {
                return this.isAnimating;
            }
            set
            {
                if (this.SetProperty(ref this.isAnimating, value))
                {
                    if (value)
                    {
                        this.StartAnimation();
                    }
                    else
                    {
                        this.FinishAnimation();
                        this.RenderSampleData();
                    }
                }
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

        protected virtual bool HandleSelectionMove
        {
            get
            {
                return false;
            }
        }

        protected abstract void InitializeFieldsOverride();

        protected abstract void RenderSampleDataOverride();

        protected abstract void AnimationTickOverride();

        protected abstract void OnPointSelectedOverride(Point point);

        protected abstract void OnPointSelectedOverride(Point point, bool isFirstPointSelection);

        protected abstract void OnSelectionMoveOverride(Point point);

        private void StartAnimation()
        {
            if (!this.timer.IsEnabled)
            {
                using (this.cartesianPlane.SuspendLayoutUpdate())
                {
                    this.cartesianPlane.ClearAllElements();
                }

                this.timer.Start();
            }
        }

        protected void FinishAnimation()
        {
            if (this.timer.IsEnabled)
            {
                this.timer.Stop();
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            using(this.cartesianPlane.SuspendLayoutUpdate())
            {
                this.AnimationTickOverride();
            }
        }

        private void RenderSampleData()
        {
            using (this.cartesianPlane.SuspendLayoutUpdate())
            {
                this.cartesianPlane.ClearAllElements();
                this.RenderSampleDataOverride();
            }
        }

        private void InitializeFields(CartesianPlane cartesianPlane)
        {
            if (this.areFieldInitialized)
            {
                throw new InvalidOperationException("Cannot initialize fields more than once!");
            }

            this.areFieldInitialized = true;

            this.cartesianPlane = cartesianPlane;
            this.cartesianPlane.ViewportInfo = this.ViewportInfo;
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(this.AnimationTickSeconds);
            this.timer.Tick += TimerTick;

            this.InitializeFieldsOverride();
        }

        private void AttachToMouseEvents()
        {
            if (!this.isAttachedToMouseEvents)
            {
                this.isAttachedToMouseEvents = true;

                this.cartesianPlane.MouseDown += CartesianPlane_MouseDown;
                this.cartesianPlane.MouseMove += CartesianPlane_MouseMove;
            }
        }

        private void DetachFromMouseEvents()
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
                }
            }
        }

        public void Initialize()
        {
            this.IsAnimating = false;
            this.IsSelectingPoints = false;
            this.isFirstPointSelection = false;
            this.cartesianPlane.StartListeningToMouseEvents();

            this.RenderSampleData();
        }

        public void Release()
        {
            this.IsAnimating = false;
            this.IsSelectingPoints = false;
            this.isFirstPointSelection = false;
            this.cartesianPlane.StopListeningToMouseEvents();

            using (this.cartesianPlane.SuspendLayoutUpdate())
            {
                this.cartesianPlane.ClearAllElements();
            }
        }
    }
}
