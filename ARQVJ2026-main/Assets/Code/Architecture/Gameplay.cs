using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Dataflow;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Scheduling;
using ImageCampus.ToolBox.Services;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.GameLogic;

namespace ZooArchitect.Architecture
{
    public sealed class Gameplay : IInitable, ITickable
    {
        private TaskScheduler TaskScheduler => ServiceProvider.Instance.GetService<TaskScheduler>();
        private Time Time => ServiceProvider.Instance.GetService<Time>();

        public Gameplay(string blueprintsPath)
        {
            ServiceProvider.Instance.AddService<EventBus>(new EventBus());
            ServiceProvider.Instance.AddService<BlueprintRegistry>(new BlueprintRegistry(blueprintsPath));
            ServiceProvider.Instance.AddService<BlueprintBinder>(new BlueprintBinder());
            ServiceProvider.Instance.AddService<TaskScheduler>(new TaskScheduler());
        }

        public void Init()
        {
            ServiceProvider.Instance.AddService<Time>(new Time());
            ServiceProvider.Instance.AddService<DayNightCycle>(new DayNightCycle());
            ServiceProvider.Instance.AddService<Wallet>(new Wallet());
            ServiceProvider.Instance.AddService<EntityRegistry>(new EntityRegistry());
            ServiceProvider.Instance.AddService<EntityFactory>(new EntityFactory());
        }

        public void LateInit()
        {
            new Map(10, 10);
        }

        public void Tick(float deltaTime)
        {
            Time.Tick(deltaTime);
            TaskScheduler.Tick(Time.LogicDeltaTime);
        }

    }
}