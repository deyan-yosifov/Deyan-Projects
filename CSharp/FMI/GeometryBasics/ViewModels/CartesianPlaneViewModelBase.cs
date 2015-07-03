using Deyo.Controls.Charts;
using Deyo.Controls.Common;
using GeometryBasics.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace GeometryBasics.ViewModels
{
    public abstract class CartesianPlaneViewModelBase : ViewModelBase, IReleasable
    {
        private CartesianPlane cartesianPlane;
        private DispatcherTimer timer;
        private bool areFieldInitialized;
        private bool isSelectingPoints;
        private bool isAnimating;
        private bool isFirstPointSelection;

        public CartesianPlaneViewModelBase(CartesianPlane cartesianPlane)
        {
            this.areFieldInitialized = false;
            this.InitializeFields(cartesianPlane);

            this.Initialize();
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

        public bool IsFirstPointSelection
        {
            get
            {
                return this.isFirstPointSelection;
            }
            set
            {
                this.SetProperty(ref this.isFirstPointSelection, value);
            }
        }

        protected abstract void InitializeFieldsOverride();

        protected abstract void RenderSampleDataOverride();

        protected abstract void AnimationTickOverride();

        protected abstract void OnPointSelectedOverride(Point point);

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

        public void Initialize()
        {
            this.IsAnimating = false;
            this.IsSelectingPoints = false;
            this.IsFirstPointSelection = false;

            this.RenderSampleData();
        }

        public void Release()
        {
            this.IsAnimating = false;
            this.IsSelectingPoints = false;
            this.IsFirstPointSelection = false;

            using (this.cartesianPlane.SuspendLayoutUpdate())
            {
                this.cartesianPlane.ClearAllElements();
            }
        }
    }
}
