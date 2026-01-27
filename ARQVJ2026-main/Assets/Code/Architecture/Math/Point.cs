namespace ZooArchitect.Architecture.Math
{
    public struct Point 
    {
        private int x;
        private int y;
        public int X => x; 
        public int Y => y; 

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
