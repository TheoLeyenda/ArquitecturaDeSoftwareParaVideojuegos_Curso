using System.Collections.Generic;

namespace ZooArchitect.Architecture.Math
{
    public struct Coordinate
    {
        private Point[] points;

        public bool IsSingleCoordinate => points.Length == 1;

        public IEnumerable<Point> Points => points;

        public Coordinate(params Point[] points)
        {
            if (points == null || points.Length == 0)
            {
                throw new System.Exception();
            }
            this.points = points;
        }
    }
}
