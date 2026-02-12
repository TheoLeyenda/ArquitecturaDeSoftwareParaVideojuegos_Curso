using ImageCampus.ToolBox.Events;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Entities.Events
{
    public struct EntityCreatedEvent<EntityType> : IEvent
        where EntityType : Entity
    {
        public string blueprintId;
        public string blueprintTable;
        public uint entityCreatedId;
        public Point origin;
        public Point end;

        public void Assign(params object[] parameters)
        {
            blueprintId = (string)parameters[0];
            blueprintTable = (string)parameters[1];
            entityCreatedId = (uint)parameters[2];
            origin = (Point)parameters[3];
            end = (Point)parameters[4];
        }

        public void Reset()
        {
            blueprintId = default(string);
            blueprintTable = default(string);
            entityCreatedId = Entity.UNASSIGNED_ENTITY_ID;
            origin = default(Point);
            end = default(Point);
        }
    }
}