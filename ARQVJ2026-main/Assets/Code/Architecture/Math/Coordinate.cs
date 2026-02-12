using System.Collections.Generic;

namespace ZooArchitect.Architecture.Math
{
    public struct Coordinate
    {
        private Point[] points;
        public IEnumerable<Point> Points => points;

        private List<int> innerIndexes;
        private List<int> perimeterIndexes;

        public IEnumerable<Point> Inner => Filter(innerIndexes);
        public IEnumerable<Point> Perimeter => Filter(perimeterIndexes);

        public bool IsSingleCoordinate => points.Length == 1;
        public Point Origin => points[0];
        public Point End => points[^1];

        public int Width => (End.X - Origin.X) + 1;
        public int Height => (End.Y - Origin.Y) + 1;

        public Coordinate(Point a, Point b)
        {
            innerIndexes = new List<int>();
            perimeterIndexes = new List<int>();

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
                for (int y = minY; y <= maxY; y++, index++)
                {
                    points[index] = new Point(x, y);

                    if (x > minX && x < maxX && y > minY && y < maxY)
                        innerIndexes.Add(index);
                    else
                        perimeterIndexes.Add(index);
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

        private IEnumerable<Point> Filter(IEnumerable<int> indexes)
        {
            foreach (int index in indexes)
            {
                yield return points[index];
            }
        }

        public bool Overlaps(Coordinate other)
        {
            return End.X >= other.Origin.X &&
                   Origin.X <= other.End.X &&
                   End.Y >= other.Origin.Y &&
                   Origin.Y <= other.End.Y;
        }

        public bool IsInPerimeter(Coordinate other)
        {
            if (!Overlaps(other))
                return false;

            return End.X == other.Origin.X ||
                   Origin.X == other.End.X ||
                   End.Y == other.Origin.Y ||
                   Origin.Y == other.End.Y;
        }

        public bool IsInInner(Coordinate other)
        {
            return Origin.X < other.Origin.X &&
                   End.X > other.End.X &&
                   Origin.Y < other.Origin.Y &&
                   End.Y > other.End.Y;
        }
    }
}
