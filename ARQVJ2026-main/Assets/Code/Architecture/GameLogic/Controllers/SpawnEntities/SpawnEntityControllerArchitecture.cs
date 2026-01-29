using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.Entities;

namespace ZooArchitect.Architecture.Controllers
{
    public sealed class SpawnEntityControllerArchitecture : IDisposable
    {
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
        private EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();

        public SpawnEntityControllerArchitecture()
        {
            EventBus.Subscribe<SpawnEntityRequestEvent>(RequestSpawnEntity);
        }

        private void RequestSpawnEntity(in SpawnEntityRequestEvent spawnEntityRequestEvent)
        {
            bool collides = false;
            foreach (Animal animal in EntityRegistry.Animals)
            {
                if (animal.coordinate.Origin == spawnEntityRequestEvent.coordinateToSpawn.Origin)
                {
                    collides = true;
                    break;
                }
            }

            if (collides)
            {
                EventBus.Raise<SpawnEntityRequestRejectedEvent>
                    (spawnEntityRequestEvent.blueprintToSpawn, spawnEntityRequestEvent.coordinateToSpawn);
            }
            else
            {
                EventBus.Raise<SpawnEntityRequestAceptedEvent>
                    (spawnEntityRequestEvent.blueprintToSpawn, spawnEntityRequestEvent.coordinateToSpawn);
            }
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<SpawnEntityRequestEvent>(RequestSpawnEntity);
        }
    }
}
