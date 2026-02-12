using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.Logs;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Controllers
{
    public sealed class SpawnJailControllerArchitecture : IDisposable
    {
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
        private EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();
        public SpawnJailControllerArchitecture()
        {
            EventBus.Subscribe<SpawnJailRequestEvent>(RequestSpawnJail);
        }

        private void RequestSpawnJail(in SpawnJailRequestEvent spawnJailRequestEvent)
        {
            Coordinate tentativeNewCoordinate = new Coordinate(spawnJailRequestEvent.origin, spawnJailRequestEvent.end);

            foreach (Structure structure in EntityRegistry.Structures)
            {
                if (structure.coordinate.Overlaps(tentativeNewCoordinate))
                {
                    EventBus.Raise<SpawnJainRequestRejectedEvent>(spawnJailRequestEvent.origin,
                        spawnJailRequestEvent.end, spawnJailRequestEvent.blueprintName);
                    return;
                }
            }

            foreach (Point _ in tentativeNewCoordinate.Inner)
            {
                EventBus.Raise<SpawnJailRequestAcceptedEvent>(spawnJailRequestEvent.origin,
                    spawnJailRequestEvent.end, spawnJailRequestEvent.blueprintName);
                return;
            }
            EventBus.Raise<SpawnJainRequestRejectedEvent>(spawnJailRequestEvent.origin,
                spawnJailRequestEvent.end, spawnJailRequestEvent.blueprintName);
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<SpawnJailRequestEvent>(RequestSpawnJail);
        }
    }

}
