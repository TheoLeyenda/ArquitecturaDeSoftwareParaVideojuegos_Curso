using ImageCampus.ToolBox.Events;

namespace ZooArchitect.Architecture.Entities.Events
{
    public struct EntityCreatedEvent<EntityType> : IEvent
        where EntityType : Entity
    {
        public uint entityCreatedId;

        public void Assign(params object[] parameters)
        {
            entityCreatedId = (uint)parameters[0];
        }

        public void Reset()
        {
            entityCreatedId = Entity.UNASSIGNED_ENTITY_ID;
        }
    }
}