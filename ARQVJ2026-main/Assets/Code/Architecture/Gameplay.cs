using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Dataflow;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Rules;
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

        private Scene Scene => ServiceProvider.Instance.GetService<Scene>();

        public Gameplay(string blueprintsPath)
        {
            ServiceProvider.Instance.AddService<EventBus>(new EventBus());
            ServiceProvider.Instance.AddService<BlueprintRegistry>(new BlueprintRegistry(blueprintsPath));
            ServiceProvider.Instance.AddService<BlueprintBinder>(new BlueprintBinder());
            ServiceProvider.Instance.AddService<TaskScheduler>(new TaskScheduler());
            ServiceProvider.Instance.AddService<RuleEvaluator>(new RuleEvaluator());
            ServiceProvider.Instance.AddService<RuleFactory>(new RuleFactory());
            ServiceProvider.Instance.AddService<Scene>(new Scene());
        }

        public void Init()
        {
            Scene.Init();
        }

        public void LateInit()
        {
            Scene.LateInit();
        }

        public void Tick(float deltaTime)
        {
            Time.Tick(deltaTime);
            TaskScheduler.Tick(Time.LogicDeltaTime);
            Scene.Tick(Time.LogicDeltaTime);
        }

        public void Dispose()
        {
            Scene.Dispose();
        }
    }
}