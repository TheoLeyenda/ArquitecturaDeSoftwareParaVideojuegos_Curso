using ImageCampus.ToolBox.Dataflow;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.Entities.Events;
using ZooArchitect.View.Mapping;

namespace ZooArchitect.View.Entities
{
    [ViewOf(typeof(WorkersLogic))]
    internal sealed class WorkersLogicView : IInitable, ITickable, IDisposable
    {
        private EntityRegistryView EntityRegistryView => ServiceProvider.Instance.GetService<EntityRegistryView>();
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
        public void Init()
        {
            EventBus.Subscribe<WorkerStartWorkEvent>(OnWorkStart);
            EventBus.Subscribe<WorkerEndWorkEvent>(OnWorkEnd);
        }


        private void OnWorkStart(in WorkerStartWorkEvent workerStartWorkEvent)
        {
            EntityRegistryView.GetAs<WorkerView>(workerStartWorkEvent.workerId).OnStartWorking();

        }
        private void OnWorkEnd(in WorkerEndWorkEvent workerEndWorkEvent)
        {
            EntityRegistryView.GetAs<WorkerView>(workerEndWorkEvent.workerId).OnEndWorking();

        }

        public void LateInit()
        {
        }

        public void Tick(float deltaTime)
        {
        }
        public void Dispose()
        {
            EventBus.Unsubscribe<WorkerStartWorkEvent>(OnWorkStart);
            EventBus.Unsubscribe<WorkerEndWorkEvent>(OnWorkEnd);
        }

    }
}
