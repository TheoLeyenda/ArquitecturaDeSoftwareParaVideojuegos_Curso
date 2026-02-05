namespace ZooArchitect.Architecture.Math
{
    public struct Point
    {
        public static Point Right => new Point(1, 0);
        public static Point Left => new Point(-1, 0);
        public static Point Up => new Point(0, 1);
        public static Point Down => new Point(0, -1);

        private int x;
        private int y;
        public int X => x;
        public int Y => y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static bool operator ==(Point left, Point right)
        {
            return left.x == right.x && left.y == right.y;
        }

        public static bool operator !=(Point left, Point right) 
        {
            return !(left == right);
        }

        public static Point operator + (Point thisPoint, Point other)
        {
            return new Point(thisPoint.x + other.x, thisPoint.y + other.y);
        }
    }
}
