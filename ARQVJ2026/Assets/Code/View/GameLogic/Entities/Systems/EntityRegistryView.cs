using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.Entities.Events;
using ZooArchitect.View.Mapping;

namespace ZooArchitect.View.Entities
{
    [ViewOf(typeof(EntityRegistry))]
    internal sealed class EntityRegistryView : IService, IDisposable
    {
        public bool IsPersistance => false;

        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

        private Dictionary<uint, EntityView> entities;
        private Dictionary<Type, List<uint>> entityIdsPerType;


        public EntityRegistryView()
        {
            entities = new Dictionary<uint, EntityView>();
            entityIdsPerType = new Dictionary<Type, List<uint>>();
            EventBus.Subscribe<EntityDestroyedEvent>(OnEntityDestroyed);
        }


        internal string RegisterMethodName => nameof(Register);

        private void Register(EntityView entityView)
        {
            entities.Add(entityView.ArchitectureEnitityID, entityView);
            Type currentEntityType = entityView.GetType();
            do
            {
                currentEntityType = currentEntityType == null ? entityView.GetType() : currentEntityType.BaseType;
                if (!entityIdsPerType.ContainsKey(currentEntityType))
                    entityIdsPerType.Add(currentEntityType, new List<uint>());
                entityIdsPerType[currentEntityType].Add(entityView.ArchitectureEnitityID);
            } while (currentEntityType != typeof(EntityView));
        }
        private void OnEntityDestroyed(in EntityDestroyedEvent entityDestroyedEvent)
        {
            EntityView entityView = entities[entityDestroyedEvent.entityID];
            entityView.Dispose();
            entities.Remove(entityDestroyedEvent.entityID);
            UnityEngine.Object.Destroy(entityView.gameObject);
        }

        public EntityType GetAs<EntityType>(uint ID) where EntityType : EntityView
        {
            if (ID == Entity.UNASSIGNED_ENTITY_ID)
            {
                throw new NullReferenceException("Entity id 0 represents a null entity");
            }

            if (!entities.ContainsKey(ID))
            {
                throw new KeyNotFoundException(ID.ToString());
            }

            if (entities[ID] is not EntityType)
            {
                throw new InvalidCastException($"An attempt was made to obtain a type {entities[ID].GetType().Name}"
                                             + $"entity as type {typeof(EntityType).Name} from the EntityRegistry");
            }

            return entities[ID] as EntityType;
        }

        public IEnumerable<EntityView> Entities => FilterEntities<EntityView>();
        public IEnumerable<StrcutureView> Structures => FilterEntities<StrcutureView>();
        public IEnumerable<JailView> Jails => FilterEntities<JailView>();
        public IEnumerable<InfrastructureView> Infrastructures => FilterEntities<InfrastructureView>();
        public IEnumerable<LivingEntityView> LivingEntities => FilterEntities<LivingEntityView>();
        public IEnumerable<AnimalView> Animals => FilterEntities<AnimalView>();
        public IEnumerable<HumanView> Humans => FilterEntities<HumanView>();
        public IEnumerable<WorkerView> Workers => FilterEntities<WorkerView>();
        public IEnumerable<VisitorView> Visitors => FilterEntities<VisitorView>();

        public IEnumerable<EntityType> FilterEntities<EntityType>() where EntityType : EntityView
        {
            if (entityIdsPerType.ContainsKey(typeof(EntityType)))
            {
                foreach (uint ID in entityIdsPerType[typeof(EntityType)])
                {
                    yield return entities[ID] as EntityType;
                }
            }
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<EntityDestroyedEvent>(OnEntityDestroyed);
        }
    }
}
