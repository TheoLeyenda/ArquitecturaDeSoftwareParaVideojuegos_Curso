using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Controllers
{
	public sealed class SpawnInfrastructureControllerArchitecture : IDisposable
	{
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
        private EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();

        public SpawnInfrastructureControllerArchitecture()
		{
            EventBus.Subscribe<SpawnInfrastructureRequestEvent>(RequestSpawnInfrastructure);
		}

		private void RequestSpawnInfrastructure(in SpawnInfrastructureRequestEvent spawnInfrastructureRequestEvent)
		{
            Coordinate tentativeSpawnCoordiante = new Coordinate(spawnInfrastructureRequestEvent.pointToSpawn);

            foreach (Structure structure in EntityRegistry.Structures)
            {
                if (structure.coordinate.Overlaps(tentativeSpawnCoordiante))
                {
                    EventBus.Raise<SpawnInfrastructureRequestRejectedEvent>
                        (spawnInfrastructureRequestEvent.blueprintToSpawn, spawnInfrastructureRequestEvent.pointToSpawn);
                    return;
                }
            }

            EventBus.Raise<SpawnInfrastructureRequestAcceptedEvent>
                   (spawnInfrastructureRequestEvent.blueprintToSpawn, spawnInfrastructureRequestEvent.pointToSpawn);
        }

		public void Dispose()
		{
            EventBus.Unsubscribe<SpawnInfrastructureRequestEvent>(RequestSpawnInfrastructure);
        }
    }

}
