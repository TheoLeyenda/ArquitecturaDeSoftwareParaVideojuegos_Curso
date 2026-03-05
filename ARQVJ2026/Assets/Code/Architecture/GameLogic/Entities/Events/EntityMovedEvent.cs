using ImageCampus.ToolBox.Events;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Entities.Events
{
    public struct EntityMovedEvent : IEvent
    {
        public uint movedEntityId;
        public Coordinate oldCoodinate;

        public void Assign(params object[] parameters)
        {
            movedEntityId = (uint)parameters[0];
            oldCoodinate = (Coordinate)parameters[1];
        }

        public void Reset()
        {
            movedEntityId = Entity.UNASSIGNED_ENTITY_ID;
            oldCoodinate = default(Coordinate);
        }
    }
}