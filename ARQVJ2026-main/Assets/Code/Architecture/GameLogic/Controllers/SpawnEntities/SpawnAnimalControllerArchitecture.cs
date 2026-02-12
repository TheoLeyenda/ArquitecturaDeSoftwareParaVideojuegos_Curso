using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Controllers
{
    public sealed class SpawnAnimalControllerArchitecture : IDisposable
    {
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
        private EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();

        public SpawnAnimalControllerArchitecture()
        {
            EventBus.Subscribe<SpawnAnimalRequestEvent>(RequestSpawnAnimal);
        }

        private void RequestSpawnAnimal(in SpawnAnimalRequestEvent spawnAnimalRequestEvent)
        {
            Coordinate tentativeSpawnCoordiante = new Coordinate(spawnAnimalRequestEvent.pointToSpawn);
            
            foreach (Animal animal in EntityRegistry.Animals)
            {
                if (animal.coordinate.Overlaps(tentativeSpawnCoordiante))
                {
                    EventBus.Raise<SpawnAnimalRequestRejectedEvent>
                        (spawnAnimalRequestEvent.blueprintToSpawn, spawnAnimalRequestEvent.pointToSpawn);
                    return;
                }
            }

            foreach (Jail jail in EntityRegistry.Jails)
            {
                if (jail.coordinate.IsInInner(tentativeSpawnCoordiante))
                {
                    EventBus.Raise<SpawnAnimalRequestAcceptedEvent>
                        (spawnAnimalRequestEvent.blueprintToSpawn, spawnAnimalRequestEvent.pointToSpawn);
                    return;
                }
            }

            EventBus.Raise<SpawnAnimalRequestRejectedEvent>
                (spawnAnimalRequestEvent.blueprintToSpawn, spawnAnimalRequestEvent.pointToSpawn);
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<SpawnAnimalRequestEvent>(RequestSpawnAnimal);
        }
    }
}
