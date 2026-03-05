using ImageCampus.ToolBox.Events;

namespace ZooArchitect.Architecture.Entities.Events
{
    public struct WorkerStartWorkEvent : IEvent
    {
        public uint workerId;

        public void Assign(params object[] parameters)
        {
            workerId = (uint)parameters[0];
        }

        public void Reset()
        {
            workerId = Entity.UNASSIGNED_ENTITY_ID;
        }
    }
}
