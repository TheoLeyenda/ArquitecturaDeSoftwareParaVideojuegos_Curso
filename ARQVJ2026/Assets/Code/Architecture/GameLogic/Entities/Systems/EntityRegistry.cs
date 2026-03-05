using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using ZooArchitect.Architecture.Entities.Events;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Entities
{
    public sealed class EntityRegistry : IService
    {
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

        public bool IsPersistance => false;

        private Dictionary<uint, Entity> entities;
        private Dictionary<Type, List<uint>> entityIdsPerType;

        public EntityRegistry()
        {
            entities = new Dictionary<uint, Entity>();
            entityIdsPerType = new Dictionary<Type, List<uint>>();
        }

        public Entity this[uint ID] => entities[ID];

        internal string RegisterMethodName => nameof(Register);

        private void Register(Entity entity)
        {
            entities.Add(entity.ID, entity);
            Type currentEntityType = null;
            do
            {
                currentEntityType = currentEntityType == null ? entity.GetType() : currentEntityType.BaseType;
                if (!entityIdsPerType.ContainsKey(currentEntityType))
                    entityIdsPerType.Add(currentEntityType, new List<uint>());
                entityIdsPerType[currentEntityType].Add(entity.ID);
            } while (currentEntityType != typeof(Entity));
        }

        public void Unregister(Entity entity)
        {
            if (entities.ContainsKey(entity.ID))
            {
                EventBus.Raise<EntityDestroyedEvent>(entity.ID);
                entities.Remove(entity.ID);

                Type currentEntityType = null;

                do
                {
                    currentEntityType = currentEntityType == null ? entity.GetType() : currentEntityType.BaseType;
                    if (!entityIdsPerType.ContainsKey(currentEntityType))
                        entityIdsPerType.Add(currentEntityType, new List<uint>());
                    entityIdsPerType[currentEntityType].Remove(entity.ID);
                } while (currentEntityType != typeof(Entity));
            }
        }

        public EntityType GetAs<EntityType>(uint ID) where EntityType : Entity
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

        public IEnumerable<Entity> Entities => FilterEntities<Entity>();
        public IEnumerable<Structure> Structures => FilterEntities<Structure>();
        public IEnumerable<Jail> Jails => FilterEntities<Jail>();
        public IEnumerable<Infrastructure> Infrastructures => FilterEntities<Infrastructure>();
        public IEnumerable<LivingEntity> LivingEntities => FilterEntities<LivingEntity>();
        public IEnumerable<Animal> Animals => FilterEntities<Animal>();
        public IEnumerable<Human> Humans => FilterEntities<Human>();
        public IEnumerable<Worker> Workers => FilterEntities<Worker>();
        public IEnumerable<Visitor> Visitors => FilterEntities<Visitor>();

        public IEnumerable<EntityType> FilterEntities<EntityType>() where EntityType : Entity
        {
            if (entityIdsPerType.ContainsKey(typeof(EntityType)))
            {
                foreach (uint ID in entityIdsPerType[typeof(EntityType)])
                {
                    yield return entities[ID] as EntityType;
                }
            }
        }
    }
}
