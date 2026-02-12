using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Services;
using ZooArchitect.Architecture.GameLogic;
using ZooArchitect.Architecture.GameLogic.Events;
using ZooArchitect.Architecture.Math;
using ZooArchitect.Architecture.Entities.Events;

namespace ZooArchitect.Architecture.Entities
{
    public sealed class Animal : LivingEntity
    {
        private Wallet Wallet => ServiceProvider.Instance.GetService<Wallet>();

        [BlueprintParameter("Food needed per day")] private int foodNeededPerDay;
        public int FoodNeededPerDay => foodNeededPerDay;

        [BlueprintParameter("Weight")] private int weight;
        public int Weight => weight;

        [BlueprintParameter("Sleep start hour")] private int sleepStartHour;
        public int SleepStartHour => sleepStartHour;

        [BlueprintParameter("Sleep end hour")] private int sleepEndHour;
        public int SleepEndHour => sleepEndHour;

        [BlueprintParameter("Price")] private int price;
        public int Price => price;

        [BlueprintParameter("Incompatible in habitat animals")] private string[] incompatibleInHabitatAnimals;
        public string[] IncompatibleInHabitatAnimals => incompatibleInHabitatAnimals;

        private Animal(uint ID, Coordinate coordinate) : base(ID, coordinate)
        {

        }

        public override void Tick(float deltaTime)
        {
        }

        internal void Feed()
        {
            if (Wallet.HasResourceAmount("Comida de Animales", foodNeededPerDay))
            {
                EventBus.Raise<RemoveResourceToWlletEvent>("Comida de Animales", foodNeededPerDay);
                EventBus.Raise<OnAnimalFeedSucsess>(ID);
            }
            else
            {
                EventBus.Raise<OnAnimalFeedFail>(ID);
            }
        }
    }
}
