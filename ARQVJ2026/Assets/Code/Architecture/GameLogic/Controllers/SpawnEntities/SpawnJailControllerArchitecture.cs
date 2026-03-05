using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.Data;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.Logs;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Controllers
{
    public sealed class SpawnJailControllerArchitecture : IDisposable
    {
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
        private EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();
        private Scene Scene => ServiceProvider.Instance.GetService<Scene>();

        public SpawnJailControllerArchitecture()
        {
            EventBus.Subscribe<SpawnRequestEvent<Jail>>(RequestSpawnJail);
        }

        private void RequestSpawnJail(in SpawnRequestEvent<Jail> spawnJailRequestEvent)
        {
            foreach (Structure structure in EntityRegistry.Structures)
            {
                if (structure.coordinate.Overlaps(spawnJailRequestEvent.coordinateToSpawn))
                {
                    EventBus.Raise<SpawnRequestRejectedEvent<Jail>>(spawnJailRequestEvent.blueprintToSpawn,
                        spawnJailRequestEvent.coordinateToSpawn);
                    return;
                }
            }

            if (Scene.MapCoordinate.minX > spawnJailRequestEvent.coordinateToSpawn.minX ||
                Scene.MapCoordinate.minY > spawnJailRequestEvent.coordinateToSpawn.minY ||
                Scene.MapCoordinate.maxX < spawnJailRequestEvent.coordinateToSpawn.maxX ||
                Scene.MapCoordinate.maxY < spawnJailRequestEvent.coordinateToSpawn.maxY)
            {
                EventBus.Raise<SpawnRequestRejectedEvent<Jail>>(spawnJailRequestEvent.blueprintToSpawn,
                spawnJailRequestEvent.coordinateToSpawn);
            }

            foreach (Point _ in spawnJailRequestEvent.coordinateToSpawn.Inner)
            {
                EventBus.Raise<SpawnRequestAcceptedEvent<Jail>>(spawnJailRequestEvent.blueprintToSpawn,
                    spawnJailRequestEvent.coordinateToSpawn, TableNames.JAILS_TABLE_NAME);
                return;
            }

            EventBus.Raise<SpawnRequestRejectedEvent<Jail>>(spawnJailRequestEvent.blueprintToSpawn, 
                spawnJailRequestEvent.coordinateToSpawn);
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<SpawnRequestEvent<Jail>>(RequestSpawnJail);
        }
    }

}
