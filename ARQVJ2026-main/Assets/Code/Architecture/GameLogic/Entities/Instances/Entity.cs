using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Dataflow;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using ZooArchitect.Architecture.Entities.Events;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Entities
{
    public abstract class Entity : IInitable, ITickable
    {
        public const uint UNASSIGNED_ENTITY_ID = 0;

        protected EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

        public uint ID;
        public Coordinate coordinate;

        protected Entity(uint ID, Coordinate coordinate) 
        {
            this.ID = ID;
            this.coordinate = coordinate;
        }

        public virtual void Init() {}

        public virtual void LateInit() { }

        public virtual void Tick(float deltaTime) { }

        public void Move(Point offset) 
        {
            coordinate.Move(offset);
            EventBus.Raise<EntityMovedEvent>(ID);
        }
    }
}
