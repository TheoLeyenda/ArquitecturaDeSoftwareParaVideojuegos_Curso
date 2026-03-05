using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Rules;
using ImageCampus.ToolBox.Services;
using System;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.Data;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.GameLogic;
using ZooArchitect.Architecture.GameLogic.Events;

namespace ZooArchitect.Architecture.Controllers
{
    public sealed class SpawnAnimalControllerArchitecture : IDisposable
    {
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
        private EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();
        private BlueprintRegistry BlueprintRegistry => ServiceProvider.Instance.GetService<BlueprintRegistry>();
        private Wallet Wallet => ServiceProvider.Instance.GetService<Wallet>();
        private RuleFactory RuleFactory => ServiceProvider.Instance.GetService<RuleFactory>();
        private GameplayLogic GameplayLogic => ServiceProvider.Instance.GetService<GameplayLogic>();

        private Scene Scene => ServiceProvider.Instance.GetService<Scene>();

        public SpawnAnimalControllerArchitecture()
        {
            EventBus.Subscribe<SpawnRequestEvent<Animal>>(RequestSpawnAnimal);
        }

        private void RequestSpawnAnimal(in SpawnRequestEvent<Animal> spawnAnimalRequestEvent)
        {

            string canBuyRuleName = BlueprintRegistry[TableNames.ANIMALS_TABLE_NAME,
                spawnAnimalRequestEvent.blueprintToSpawn, Animal.CAN_BE_PURCHASED_RULE_KEY];

            Rule canBuyAnimalRule = RuleFactory.GetRule(canBuyRuleName);

            string resourcePurchaseKey = BlueprintRegistry[TableNames.ANIMALS_TABLE_NAME,
                                                    spawnAnimalRequestEvent.blueprintToSpawn, Animal.PRICE_RESOURCE_KEY];

            long price = Convert.ToInt64(BlueprintRegistry[TableNames.ANIMALS_TABLE_NAME,
                                            spawnAnimalRequestEvent.blueprintToSpawn, Animal.PRICE_KEY]);

            if (!canBuyAnimalRule.Evaluate(spawnAnimalRequestEvent.blueprintToSpawn, spawnAnimalRequestEvent.blueprintToSpawn))
            {
                EventBus.Raise<SpawnRequestRejectedEvent<Animal>>
                        (spawnAnimalRequestEvent.blueprintToSpawn, spawnAnimalRequestEvent.coordinateToSpawn,
                        $"{spawnAnimalRequestEvent.blueprintToSpawn} price: {price} - " +
                        $"Money in Wallet: {Wallet.GetResourceAmount(resourcePurchaseKey)} " +
                        $"So much expensive! ");
                return;
            }

            int animalTier = Convert.ToInt32(BlueprintRegistry[TableNames.ANIMALS_TABLE_NAME,
                                            spawnAnimalRequestEvent.blueprintToSpawn, Animal.TIER_KEY]);

			if (animalTier > GameplayLogic.ZooTier)
			{
                EventBus.Raise<SpawnRequestRejectedEvent<Animal>>
                        (spawnAnimalRequestEvent.blueprintToSpawn, spawnAnimalRequestEvent.coordinateToSpawn,
                        $"{spawnAnimalRequestEvent.blueprintToSpawn} is tier: {animalTier} - " +
                        $"But the tier of the zoo is {GameplayLogic.ZooTier}");
                return;
            }

            foreach (uint entityId in Scene.GetEntitiesIn(spawnAnimalRequestEvent.coordinateToSpawn))
            {
                if (EntityRegistry[entityId] is Animal)
                {
                    EventBus.Raise<SpawnRequestRejectedEvent<Animal>>
                        (spawnAnimalRequestEvent.blueprintToSpawn, spawnAnimalRequestEvent.coordinateToSpawn,
                        $"New {spawnAnimalRequestEvent.blueprintToSpawn} overlaps whit {EntityRegistry[entityId].ToString()} " +
                        $"in coordinate {spawnAnimalRequestEvent.coordinateToSpawn.ToString()}");
                    return;
                }
            }


            foreach (uint entiyID in Scene.GetEntitiesIn(spawnAnimalRequestEvent.coordinateToSpawn))
            {
                if (EntityRegistry[entiyID] is Jail)
                {
                    if (EntityRegistry[entiyID].coordinate.IsInInner(spawnAnimalRequestEvent.coordinateToSpawn))
                    {
                        foreach (Math.Point innerPoint in EntityRegistry[entiyID].coordinate.Inner)
                        {
                            foreach (uint entitiesInJailCoordinate in Scene.GetEntitiesIn(new Math.Coordinate(innerPoint)))
                            {
                                if (EntityRegistry[entitiesInJailCoordinate] is Animal)
                                {
                                    if ((EntityRegistry[entitiesInJailCoordinate] as Animal).IncompatibleInHabitatAnimals.Contains(spawnAnimalRequestEvent.blueprintToSpawn))
                                    {
                                        EventBus.Raise<SpawnRequestRejectedEvent<Animal>>
                                                (spawnAnimalRequestEvent.blueprintToSpawn, spawnAnimalRequestEvent.coordinateToSpawn,
                                                $"A new {spawnAnimalRequestEvent.blueprintToSpawn} cannot coexist in a habitat with the animals that are currently in the Jail");
                                        return;
                                    }
                                }
                            }
                        }
                        
                        EventBus.Raise<RemoveResourceToWalletEvent>(resourcePurchaseKey, price);
                        EventBus.Raise<SpawnRequestAcceptedEvent<Animal>>
                            (spawnAnimalRequestEvent.blueprintToSpawn, spawnAnimalRequestEvent.coordinateToSpawn, TableNames.ANIMALS_TABLE_NAME);
                        return;
                    }
                }
            }

            EventBus.Raise<SpawnRequestRejectedEvent<Animal>>
                (spawnAnimalRequestEvent.blueprintToSpawn, spawnAnimalRequestEvent.coordinateToSpawn,
                $"{spawnAnimalRequestEvent.blueprintToSpawn} outside a valid Jail");
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<SpawnRequestEvent<Animal>>(RequestSpawnAnimal);
        }
    }
}
