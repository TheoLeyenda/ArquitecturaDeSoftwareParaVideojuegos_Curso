using ImageCampus.ToolBox.Scheduling;
using ImageCampus.ToolBox.Services;
using System.Collections.Generic;
using ZooArchitect.Architecture.GameLogic;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Entities
{
    public abstract class LivingEntity : Entity
    {
        private TaskScheduler TaskScheduler => ServiceProvider.Instance.GetService<TaskScheduler>();


		private Pathfinding pathfinding;

        private float stepDelay = 0.2f;
		public float StepDelay  => stepDelay;

        protected LivingEntity(uint ID, Coordinate coordinate) : base(ID, coordinate)
        {
            pathfinding = new Pathfinding();
        }

        protected void Travel(in Point destination) 
        {
            List<Point> path = pathfinding.FindPath(coordinate.Origin, destination);
            int currentIndex = 0;
            if (path == null)
                return;

            PerformStep();

            void PerformStep() 
            {
				if (path.Count > currentIndex)
				{
                    TaskScheduler.Schedule(() =>
                    {
                        Teleport(path[currentIndex]);
                        currentIndex++;
                        PerformStep();
                    },
                    stepDelay);
				}
            }
        }
    }
}
