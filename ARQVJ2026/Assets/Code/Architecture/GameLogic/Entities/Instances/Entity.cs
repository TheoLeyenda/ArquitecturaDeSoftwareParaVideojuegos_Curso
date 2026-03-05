using ImageCampus.ToolBox.Dataflow;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Rules;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using ZooArchitect.Architecture.Entities.Events;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Entities
{
    public abstract class Entity : IInitable, ITickable
    {
        public const uint UNASSIGNED_ENTITY_ID = 0;

        public const string TIER_KEY = "Tier";

        protected RuleFactory RuleFactory => ServiceProvider.Instance.GetService<RuleFactory>();
        protected EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
        private EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();

        public uint ID;
        public Coordinate coordinate;

        protected Entity(uint ID, Coordinate coordinate)
        {
            this.ID = ID;
            this.coordinate = coordinate;
        }

        public ICollection<string> PerformableActions => PerfomableMethods.Keys;

        internal virtual Dictionary<string, Action> PerfomableMethods { get; }
        internal virtual Dictionary<string, Func<bool>> ChechForPerfomableActions { get; }

        public virtual void Init() { }

        public virtual void LateInit() { }

        public virtual void Tick(float deltaTime) { }


        public void Move(Point offset)
        {
            Coordinate oldCoordinate = coordinate;
            coordinate = coordinate.Move(offset);
            EventBus.Raise<EntityMovedEvent>(ID, oldCoordinate);
        }

        public void Teleport(Point position) 
        {
            Coordinate oldCoordinate = coordinate;
            coordinate = new Coordinate(position);
            EventBus.Raise<EntityMovedEvent>(ID, oldCoordinate);
        }

        protected void Destroy()
        {
            EntityRegistry.Unregister(this);
        }
    }
}
