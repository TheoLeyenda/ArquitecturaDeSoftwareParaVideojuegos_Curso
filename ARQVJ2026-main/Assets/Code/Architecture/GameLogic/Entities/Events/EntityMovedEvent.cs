using ImageCampus.ToolBox.Events;

namespace ZooArchitect.Architecture.Entities.Events
{
    public struct EntityMovedEvent : IEvent
    {
        public uint movedEntityId;

        public void Assign(params object[] parameters)
        {
            movedEntityId = (uint)parameters[0];
        }

        public void Reset()
        {
            movedEntityId = Entity.UNASSIGNED_ENTITY_ID;
        }
    }
}