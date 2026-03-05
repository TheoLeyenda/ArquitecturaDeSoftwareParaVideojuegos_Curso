using ImageCampus.ToolBox.Dataflow;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Scheduling;
using ImageCampus.ToolBox.Services;
using System;
using ZooArchitect.Architecture.Entities.Events;

namespace ZooArchitect.Architecture.Entities
{
    public sealed class WorkersLogic : ITickable, IDisposable
    {
        private EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

        private TaskScheduler TaskScheduler => ServiceProvider.Instance.GetService<TaskScheduler>();
        public WorkersLogic()
        {
            EventBus.Subscribe<StructureMaintenanceEvent>(OnStructureMaintenance);
        }

        private void OnStructureMaintenance(in StructureMaintenanceEvent structureMaintenanceEvent)
        {
            uint structureId = structureMaintenanceEvent.structureId;
            uint workerId = structureMaintenanceEvent.workerId;

            Structure Structure() => EntityRegistry.GetAs<Structure>(structureId);
            Worker Worker() => EntityRegistry.GetAs<Worker>(workerId);

            Worker().SetAsWorking();
            Worker().Teleport(Structure().coordinate.Origin);

            TaskScheduler.Schedule(() => 
            {
                Structure().Maintain();
                Worker().EndWork();
            }, 
            Worker().WorkTime);
        }

        public void Tick(float deltaTime)
        {
        }
        internal void PaidWorkers()
        {
            foreach (Worker worker in EntityRegistry.Workers)
            {
                worker.GetPaid();
            }
        }
        public void Dispose()
        {
            EventBus.Unsubscribe<StructureMaintenanceEvent>(OnStructureMaintenance);
        }

    }
}