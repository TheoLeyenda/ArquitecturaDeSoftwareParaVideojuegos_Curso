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

        [BlueprintParameter("Food needed per day")] private long foodNeededPerDay;
        public long FoodNeededPerDay => foodNeededPerDay;

        public object ServiceProvier { get; private set; }

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
