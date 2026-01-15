
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using ImageCampus.ToolBox.Updateable;
using ImageCampus.ToolBox.Scheduling;
using ZooArchitect.Architecture.GameLogic;

namespace ZooArchitect.Architecture
{
    public sealed class Gameplay : IUpdateable
    {
        private TaskScheduler TaskScheduler => ServiceProvider.Instance.GetService<TaskScheduler>();
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
        private Time Time => ServiceProvider.Instance.GetService<Time>();


        public Gameplay()
        {
            ServiceProvider.Instance.AddService<EventBus>(new EventBus());
            ServiceProvider.Instance.AddService<TaskScheduler>(new TaskScheduler());
            ServiceProvider.Instance.AddService<Time>(new Time());
            ServiceProvider.Instance.AddService<DayNightCycle>(new DayNightCycle());
        }

        public void Update(float deltaTime)
        {
            Time.Update(deltaTime);
            TaskScheduler.Update(Time.LogicDeltaTime);
        }
    }
}