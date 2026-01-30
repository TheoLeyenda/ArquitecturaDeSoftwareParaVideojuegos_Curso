using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Dataflow;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Scheduling;
using ImageCampus.ToolBox.Services;
using System;
using ZooArchitect.Architecture.GameLogic;

namespace ZooArchitect.Architecture
{
    public sealed class Gameplay : IInitable, ITickable, IDisposable
    {
        private TaskScheduler TaskScheduler => ServiceProvider.Instance.GetService<TaskScheduler>();
        private Time Time => ServiceProvider.Instance.GetService<Time>();

        private Scene scene;

        public Gameplay(string blueprintsPath)
        {
            ServiceProvider.Instance.AddService<EventBus>(new EventBus());
            ServiceProvider.Instance.AddService<BlueprintRegistry>(new BlueprintRegistry(blueprintsPath));
            ServiceProvider.Instance.AddService<BlueprintBinder>(new BlueprintBinder());
            ServiceProvider.Instance.AddService<TaskScheduler>(new TaskScheduler());
            scene = new Scene();
        }

        public void Init()
        {
            scene.Init();
        }

        public void LateInit()
        {
            scene.LateInit();
        }

        public void Tick(float deltaTime)
        {
            Time.Tick(deltaTime);
            TaskScheduler.Tick(Time.LogicDeltaTime);
            scene.Tick(Time.LogicDeltaTime);
        }

        public void Dispose()
        {
            scene.Dispose();
        }
    }
}