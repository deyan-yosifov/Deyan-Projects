//* Read in MSDN about the keyword event in C# and how to publish events.
//Re-implement the above (problem 7) using .NET events and following the best practices.

using System;
using System.Threading;

namespace ChimpmunkTetris
{
    class Timer
    {
        private bool isRunning = false;
        private Thread thread;

        public event EventHandler TimerChanged;

        protected void OnTimerChanged()
        {
            if (TimerChanged != null)
            {
                TimerChanged(this, new EventArgs());
            }
        }

        public bool IsRunning
        {
            get { return this.isRunning; }
        }
        public int Interval { get; set; }
        public int RepetitionsLeft { get; set; }
        public bool IsInfinite { get; set; }

        public Timer(int interval)
        {
            this.Interval = interval;
            this.RepetitionsLeft = 0;
            this.IsInfinite = true;
        }

        public Timer(int interval, int repetitions)
            : this(interval)
        {
            this.RepetitionsLeft = repetitions;
            this.IsInfinite = false;
        }

        public void Play()
        {
            if (!this.isRunning)
            {
                this.isRunning = true;
                this.thread = new Thread(new ThreadStart(this.Play));
                this.thread.Start();
            }
            else
            {
                while (this.RepetitionsLeft > 0 || this.IsInfinite)
                {
                    if (!this.isRunning) break;
                    Thread.Sleep(this.Interval);
                    if (!this.IsInfinite) this.RepetitionsLeft--;
                    this.OnTimerChanged();
                }
            }
        }

        public void Pause()
        {
            this.isRunning = false;
            this.thread.Abort();
        }

        public void PauseOrPlay()
        {
            if (this.isRunning) this.Pause();
            else this.Play();
        }

        public override string ToString()
        {
            return String.Format("TimerInfo:\r\nRepetitionsLeft: {0}\r\nInterval: {1}ms\r\nIsInfinite: {2}",
                this.RepetitionsLeft, this.Interval, this.IsInfinite);
        }

    }
}
