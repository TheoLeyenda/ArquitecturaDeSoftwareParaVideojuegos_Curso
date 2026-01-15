
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using ImageCampus.ToolBox.Updateable;
using ImageCampus.ToolBox.Scheduling;

namespace ZooArchitect.Architecture
{
    public sealed class Gameplay : IUpdateable
    {
        public EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
        public TaskScheduler TaskScheduler => ServiceProvider.Instance.GetService<TaskScheduler>();
        public Gameplay()
        {
            ServiceProvider.Instance.AddService<EventBus>(new EventBus());
            ServiceProvider.Instance.AddService<TaskScheduler>(new TaskScheduler());
        }

        public void Update(float deltaTime)
        {
            TaskScheduler.Update(deltaTime);
        }
    }
}