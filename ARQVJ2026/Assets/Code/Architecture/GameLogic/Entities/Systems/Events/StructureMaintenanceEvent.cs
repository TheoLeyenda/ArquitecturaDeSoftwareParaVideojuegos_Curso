using ImageCampus.ToolBox.Events;

namespace ZooArchitect.Architecture.Entities.Events
{
    public struct StructureMaintenanceEvent : IEvent
    {
        public uint structureId;
        public uint workerId;
        public void Assign(params object[] parameters)
        {
            structureId = (uint)parameters[0];
            workerId = (uint)parameters[1];
        }

        public void Reset()
        {
            structureId = Entity.UNASSIGNED_ENTITY_ID;
            workerId = Entity.UNASSIGNED_ENTITY_ID;
        }
    }
}
