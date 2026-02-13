using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Cast;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.Data;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.GameLogic;
using ZooArchitect.Architecture.GameLogic.Events;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Controllers
{
    public sealed class SpawnAnimalControllerArchitecture : IDisposable
    {
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
        private EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();

        private BlueprintRegistry BlueprintRegistry => ServiceProvider.Instance.GetService<BlueprintRegistry>();

        private Wallet Wallet => ServiceProvider.Instance.GetService<Wallet>();

        public SpawnAnimalControllerArchitecture()
        {
            EventBus.Subscribe<SpawnAnimalRequestEvent>(RequestSpawnAnimal);
        }

        private void RequestSpawnAnimal(in SpawnAnimalRequestEvent spawnAnimalRequestEvent)
        {
            Coordinate tentativeSpawnCoordiante = new Coordinate(spawnAnimalRequestEvent.pointToSpawn);

            long price = (long)StringCast.Convert
                               (BlueprintRegistry[TableNames.ANIMALS_TABLE_NAME,
                               spawnAnimalRequestEvent.blueprintToSpawn,
                               "Price"],
                               typeof(long));

            if (!Wallet.HasResourceAmount("Money", price))
            {
                EventBus.Raise<SpawnAnimalRequestRejectedEvent>
                        (spawnAnimalRequestEvent.blueprintToSpawn, spawnAnimalRequestEvent.pointToSpawn,
                        $"So much expensive! " +
                        $"{spawnAnimalRequestEvent.blueprintToSpawn} price: {price} - " +
                        $"Money in wallet: {Wallet.GetResourceAmount("Money")}");
                return;
            }

            foreach (Animal animal in EntityRegistry.Animals)
            {
                if (animal.coordinate.Overlaps(tentativeSpawnCoordiante))
                {
                    EventBus.Raise<SpawnAnimalRequestRejectedEvent>
                        (spawnAnimalRequestEvent.blueprintToSpawn, spawnAnimalRequestEvent.pointToSpawn,
                        $"New {spawnAnimalRequestEvent.blueprintToSpawn} overlaps whit {animal.ToString()} " +
                        $"in coordinate {tentativeSpawnCoordiante.ToString()}");
                    return;
                }
            }

            foreach (Jail jail in EntityRegistry.Jails)
            {
                if (jail.coordinate.IsInInner(tentativeSpawnCoordiante))
                {
                    EventBus.Raise<RemoveResourceToWalletEvent>("Money", price);
                    EventBus.Raise<SpawnAnimalRequestAcceptedEvent>
                        (spawnAnimalRequestEvent.blueprintToSpawn, spawnAnimalRequestEvent.pointToSpawn);
                    return;
                }
            }

            EventBus.Raise<SpawnAnimalRequestRejectedEvent>
                (spawnAnimalRequestEvent.blueprintToSpawn, spawnAnimalRequestEvent.pointToSpawn,
                $"{spawnAnimalRequestEvent.blueprintToSpawn} outside a valid Jail");
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<SpawnAnimalRequestEvent>(RequestSpawnAnimal);
        }
    }
}
