using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using ZooArchitect.Architecture.Entities.Events;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Entities
{
    public abstract class Structure : Entity
    {
        protected static Random random;
        [BlueprintParameter("Max maintenance")] protected uint maxMaintenance;
        [BlueprintParameter("Daily maintenance decrease")] protected uint dailyMaintenanceDecrease;
        protected uint currentMaintenance;

        [BlueprintParameter("Chance to drop maintenance to 0 under")] private uint chanceToDropMaintenanceToZeroUnderPercentaje;
        [BlueprintParameter("Chance to drop maintenance to 0")] private uint chanceToDropMaintenanceToZeroPercentaje;

        [BlueprintParameter("Percentaje under to notify for maintenance")] private uint percentajeUnderToNotifyForMaintenance;


        private EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();

        protected Structure(uint ID, Coordinate coordinate) : base(ID, coordinate)
        {
            if (random != null)
                random = new Random();
        }

        public override void Init()
        {
            currentMaintenance = maxMaintenance;
            base.Init();
        }

        internal override Dictionary<string, Action> PerfomableMethods
        {
            get
            {
                Dictionary<string, Action> performableActions = new Dictionary<string, Action>();
                performableActions.Add(nameof(RequestMantenance), RequestMantenance);
                return performableActions;
            }
        }

        private void RequestMantenance()
        {
            uint abiableWorkerId = Entity.UNASSIGNED_ENTITY_ID;
            foreach (Worker worker in EntityRegistry.Workers)
            {
                if (worker.IsAbiable() && worker.CanDoMaintenance)
                {
                    abiableWorkerId = worker.ID;
                    break;
                }
            }

            if (abiableWorkerId != Entity.UNASSIGNED_ENTITY_ID)
            {
                EventBus.Raise<StructureMaintenanceEvent>(ID, abiableWorkerId);
            }
        }

        internal virtual void DecreaseDailyMaintenance()
        {
            currentMaintenance = System.Math.Clamp(currentMaintenance - dailyMaintenanceDecrease, 0, maxMaintenance);

            if (currentMaintenance < chanceToDropMaintenanceToZeroUnderPercentaje)
                if (random.Next(0, 100) < chanceToDropMaintenanceToZeroPercentaje)
                    currentMaintenance = 0;

            if (currentMaintenance <= percentajeUnderToNotifyForMaintenance)
            {
                //NOTIFY!
            }
        }

        internal void Maintain()
        {
            currentMaintenance = maxMaintenance;
        }
    }
}
