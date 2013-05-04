namespace MatrixTasks
{
    public class DirectionVector
    {
        private int currentIndex;

        public static readonly int[] DirectionsDx = { 1, 1, 1, 0, -1, -1, -1, 0 };
        public static readonly int[] DirectionsDy = { 1, 0, -1, -1, -1, 0, 1, 1 };
        public const int DirectionsCount = 8;

        public int Dx { get; private set; }
        public int Dy { get; private set; }

        public DirectionVector()
        {
            this.Reset();
        }

        public void Next()
        {
            this.currentIndex++;
            if (currentIndex >= DirectionsCount)
            {
                this.currentIndex = 0;
            }
            this.RecalculateDirection();
        }

        public void Reset()
        {
            this.currentIndex = 0;
            this.RecalculateDirection();
        }

        private void RecalculateDirection()
        {
            this.Dx = DirectionsDx[this.currentIndex];
            this.Dy = DirectionsDy[this.currentIndex];
        }
    }
}