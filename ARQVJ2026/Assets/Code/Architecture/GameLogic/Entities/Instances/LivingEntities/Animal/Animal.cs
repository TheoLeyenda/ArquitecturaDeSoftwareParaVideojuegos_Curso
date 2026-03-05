using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Rules;
using ImageCampus.ToolBox.Scheduling;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using ZooArchitect.Architecture.Entities.Events;
using ZooArchitect.Architecture.GameLogic;
using ZooArchitect.Architecture.GameLogic.Events;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Entities
{
    public sealed class Animal : LivingEntity
    {
        private static Random random;

        public const string PRICE_KEY = "Price";
        public const string PRICE_RESOURCE_KEY = "Price Reource Key";
        public const string CAN_BE_PURCHASED_RULE_KEY = "Can be purchased rule";

        private Scene Scene => ServiceProvider.Instance.GetService<Scene>();
        private TaskScheduler TaskScheduler => ServiceProvider.Instance.GetService<TaskScheduler>();
        private EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();

        [BlueprintParameter("Name")] private string name;

        [BlueprintParameter("Can be feeded rule")] private string canAnimalBeFeededRuleName;

        private Rule canAnimalBeFeeded;

        [BlueprintParameter("Food needed per day")] private int foodNeededPerDay;
        public int FoodNeededPerDay => foodNeededPerDay;

        [BlueprintParameter("Food resource key")] private string foodResourceKey;
        private string FoodResourceKey => foodResourceKey;

        [BlueprintParameter("Weight")] private int weight;
        public int Weight => weight;

        [BlueprintParameter("Sleep start hour")] private int sleepStartHour;
        public int SleepStartHour => sleepStartHour;

        [BlueprintParameter("Sleep end hour")] private int sleepEndHour;
        public int SleepEndHour => sleepEndHour;

        [BlueprintParameter(PRICE_KEY)] private int price;
        public int Price => price;

        [BlueprintParameter(PRICE_RESOURCE_KEY)] private string priceResourceKey;
        private string PriceResourceKey => priceResourceKey;

        [BlueprintParameter(CAN_BE_PURCHASED_RULE_KEY)] private string canBePurchasedRuleName;
        private string CanBePurchasedRuleName => canBePurchasedRuleName;

        [BlueprintParameter("Incompatible in habitat animals")] private List<string> incompatibleInHabitatAnimals;
        public List<string> IncompatibleInHabitatAnimals => incompatibleInHabitatAnimals;


        [BlueprintParameter(TIER_KEY)] private int tier;

        [BlueprintParameter("Time between movements")] private float timeBetweenMovements;
        [BlueprintParameter("Go to sleep time")] private string goToSleepHour;
        [BlueprintParameter("Wake up time")] private string wakeUpHour;

        public string GoToSleepHour => goToSleepHour;
        public string WakeUpHour => wakeUpHour;

        private bool isSleeping;

        private Point previousMovementTo;

        private Animal(uint ID, Coordinate coordinate) : base(ID, coordinate)
        {
            if (random == null)
                random = new Random();
        }

        public override void Init()
        {
            previousMovementTo = Point.Zero;
            base.Init();
        }

        public override void LateInit()
        {
            canAnimalBeFeeded = RuleFactory.GetRule(canAnimalBeFeededRuleName);
            isSleeping = false;
            Wander();
            base.LateInit();
        }

        public override void Tick(float deltaTime)
        {
        }

        internal void Feed()
        {
            if (canAnimalBeFeeded.Evaluate(this))
            {
                EventBus.Raise<RemoveResourceToWalletEvent>(foodResourceKey, foodNeededPerDay);
                EventBus.Raise<OnAnimalFeedSucsess>(ID);
            }
            else
            {
                EventBus.Raise<OnAnimalFeedFail>(ID);
            }
        }

        public void Wander()
        {
            List<Point> validMovements = new List<Point>();
            foreach (Point direction in Pathfinding.Directions)
            {
                TileData tileData = Scene.GetTileDataOf(coordinate.Origin + direction);

                if (tileData.isAnimalHabitat && !tileData.isAnimalHabitatWall && direction != -previousMovementTo)
                {
                    validMovements.Add(direction);
                }
            }

            if (validMovements.Count > 0)
            {
                int index = random.Next(0, validMovements.Count);
                Move(validMovements[index]);
                previousMovementTo = validMovements[index];
            }
            else if(previousMovementTo != Point.Zero)
            {
                Move(-previousMovementTo);
                previousMovementTo = -previousMovementTo;
            }

            TaskScheduler.Schedule(() =>
            {
                if (!isSleeping)
                    Wander();
            }
            ,timeBetweenMovements);
        }

        internal void GoToSleep() 
        {
            isSleeping = true;
        }

        internal void WakeUp()
        {
            isSleeping = false;
            Wander();
        }

        internal bool CanAttackTo(uint entityID)
        {
            if (isSleeping)
                return false;

            if (EntityRegistry[entityID] is not LivingEntity)
                return false;

            if (EntityRegistry[entityID] is Animal)
            {
                if (EntityRegistry.GetAs<Animal>(entityID).incompatibleInHabitatAnimals.Contains(name))
                    return true;

                return EntityRegistry.GetAs<Animal>(entityID).weight < weight;
            }
            
            return true;
        }
    }
}
