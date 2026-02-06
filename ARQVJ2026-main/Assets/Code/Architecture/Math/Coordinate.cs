using System.Collections.Generic;

namespace ZooArchitect.Architecture.Math
{
    public struct Coordinate
    {
        private Point[] points;
        public IEnumerable<Point> Points => points;

        public bool IsSingleCoordinate => points.Length == 1;
        public Point Origin => points[0];
        public Point End => points[^1];

        public Coordinate(Point a, Point b)
        {
            int minX = a.X < b.X ? a.X : b.X;
            int maxX = a.X > b.X ? a.X : b.X;
            int minY = a.Y < b.Y ? a.Y : b.Y;
            int maxY = a.Y > b.Y ? a.Y : b.Y;

            int width = maxX - minX + 1;
            int height = maxY - minY + 1;

            points = new Point[width * height];

            int index = 0;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    points[index++] = new Point(x, y);
                }
            }
        }

        public Coordinate(int x, int y)
        {
            this = new Coordinate(new Point(x, y), new Point(x, y));
        }

        public Coordinate(Point point)
        {
            this = new Coordinate(point, point);
        }

        public void Move(Point offset)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i] += offset;
            }
        }
    }
}
