using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.Data;
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
            EventBus.Subscribe<SpawnRequestEvent<Infrastructure>>(RequestSpawnInfrastructure);
		}

		private void RequestSpawnInfrastructure(in SpawnRequestEvent<Infrastructure> spawnInfrastructureRequestEvent)
		{

            foreach (Structure structure in EntityRegistry.Structures)
            {
                if (structure.coordinate.Overlaps(spawnInfrastructureRequestEvent.coordinateToSpawn))
                {
                    EventBus.Raise<SpawnRequestRejectedEvent<Infrastructure>>
                        (spawnInfrastructureRequestEvent.blueprintToSpawn, spawnInfrastructureRequestEvent.coordinateToSpawn);
                    return;
                }
            }

            EventBus.Raise<SpawnRequestAcceptedEvent<Infrastructure>>
                   (spawnInfrastructureRequestEvent.blueprintToSpawn, spawnInfrastructureRequestEvent.coordinateToSpawn, 
                   TableNames.INFTRASTRUCTURE_TABLE_NAME);
        }

		public void Dispose()
		{
            EventBus.Unsubscribe<SpawnRequestEvent<Infrastructure>>(RequestSpawnInfrastructure);
        }
    }

}
