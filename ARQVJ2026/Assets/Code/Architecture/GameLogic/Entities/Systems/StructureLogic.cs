using ImageCampus.ToolBox.Dataflow;
using ImageCampus.ToolBox.Services;
using System;

namespace ZooArchitect.Architecture.Entities
{
    public sealed class StructureLogic : ITickable, IDisposable
    {
        private EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();
        public StructureLogic()
        {
        }
        public void Tick(float deltaTime)
        {
        }

        internal void DecreaseDailyMaintenance() 
        {
            foreach (Structure structure in EntityRegistry.Structures)
            {
                structure.DecreaseDailyMaintenance();
            }
        }

        public void Dispose()
        {
        }
    }
}
