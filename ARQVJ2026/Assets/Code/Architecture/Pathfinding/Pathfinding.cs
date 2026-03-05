using ImageCampus.ToolBox.Services;
using System.Collections.Generic;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.GameLogic
{
	public sealed class Pathfinding
	{
		private Scene Scene => ServiceProvider.Instance.GetService<Scene>();

		internal static readonly Point[] Directions =
		{
			Point.Up,
			Point.Right,
			Point.Down,
			Point.Left
		};

		public List<Point> FindPath(Point start, Point goal)
		{
			int widht = Scene.MapCoordinate.maxX;
			int height = Scene.MapCoordinate.maxY;

			Queue<Point> openQueue = new Queue<Point>();
			bool[,] visited = new bool[widht, height];
			Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();

			openQueue.Enqueue(start);
			visited[start.x, start.y] = true;

			while (openQueue.Count > 0)
			{
				Point current = openQueue.Dequeue();

				if (current == goal)
				{
					return ReconstructPath(cameFrom, start, goal);
				}

				foreach (Point direction in Directions)
				{
					Point newDirection = new Point(current.x + direction.x, current.y + direction.y);

					if (!IsInside(newDirection))
						continue;

					if (!Scene.GetTileDataOf(newDirection).isWalkable)
						continue;

					if (visited[newDirection.x, newDirection.y])
						continue;

					visited[newDirection.x, newDirection.y] = true;

					openQueue.Enqueue(newDirection);
					cameFrom[newDirection] = current;
				}
			}

			return null;

			bool IsInside(in Point point)
			{
				return point.x >= 0 && point.y >= 0 && point.x < widht && point.y < height;
			}
		}

		private List<Point> ReconstructPath(Dictionary<Point,Point> cameFrom, Point start, Point goal) 
		{
			List<Point> path = new List<Point>();
			Point current = goal;

			while (current != start)
			{
				path.Add(current);
				current = cameFrom[current];
			}

			path.Add(start);
			path.Reverse();
			return path;
		}
	}
}
