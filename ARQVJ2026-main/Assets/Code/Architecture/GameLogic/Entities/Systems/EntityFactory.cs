using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using System.Reflection;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.Data;
using ZooArchitect.Architecture.Entities.Events;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Entities
{
    public sealed class EntityFactory : IService, IDisposable
    {
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
        private EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();

        private BlueprintBinder BlueprintBinder => ServiceProvider.Instance.GetService<BlueprintBinder>();

        private uint lastAssignedEntityId;
        public bool IsPersistance => false;

        private Dictionary<Type, ConstructorInfo> entityConstructors;
        private MethodInfo registerEntityMethod;
        private MethodInfo raiseEntityCreatedMethod;

        public EntityFactory()
        {
            this.lastAssignedEntityId = Entity.UNASSIGNED_ENTITY_ID;
            entityConstructors = new Dictionary<Type, ConstructorInfo>();

            registerEntityMethod = EntityRegistry.GetType().GetMethod(EntityRegistry.RegisterMethodName,
                BindingFlags.NonPublic | BindingFlags.Instance);

            raiseEntityCreatedMethod = GetType().GetMethod(nameof(RaiseEntityCreated), BindingFlags.NonPublic | BindingFlags.Instance);

            RegisterEntityMethods();

            EventBus.Subscribe<SpawnAnimalRequestAcceptedEvent>(SpawnAnimal);
            EventBus.Subscribe<SpawnJailRequestAcceptedEvent>(SpawnJail);
            EventBus.Subscribe<SpawnInfrastructureRequestAcceptedEvent>(SpawnInfrastrcture);
        }

        private void SpawnInfrastrcture(in SpawnInfrastructureRequestAcceptedEvent spawnInfrastructureRequestAcceptedEvent)
        {
            CreateInstance<Infrastructure>(spawnInfrastructureRequestAcceptedEvent.blueprintToSpawn,
                new Coordinate(spawnInfrastructureRequestAcceptedEvent.pointToSpawn),
                TableNames.INFTRASTRUCTURE_TABLE_NAME);
        }

        private void SpawnJail(in SpawnJailRequestAcceptedEvent spawnJailRequestAcceptedEvent)
        {
            CreateInstance<Jail>(spawnJailRequestAcceptedEvent.blueprintName,
                new Coordinate(spawnJailRequestAcceptedEvent.origin, spawnJailRequestAcceptedEvent.end),
                TableNames.JAILS_TABLE_NAME);
        }

        private void SpawnAnimal(in SpawnAnimalRequestAcceptedEvent spawnAnimalRequestAceptedEvent)
        {
            CreateInstance<Animal>(spawnAnimalRequestAceptedEvent.blueprintToSpawn,
                new Coordinate(spawnAnimalRequestAceptedEvent.pointToSpawn),
                TableNames.ANIMALS_TABLE_NAME);
        }

        public void CreateInstance<EntityType>(string blueprintId, Coordinate coordinate, string tableName) where EntityType : Entity
        {
            lastAssignedEntityId++;
            uint newEntityId = lastAssignedEntityId;

            if (!entityConstructors.ContainsKey(typeof(EntityType)))
                throw new MissingMethodException($"Missing constructor for {typeof(EntityType).Name}");

            object newEntity = entityConstructors[typeof(EntityType)].Invoke(new object[] { newEntityId, coordinate });

            BlueprintBinder.Apply(ref newEntity, tableName, blueprintId);

            (newEntity as EntityType).Init();

            if (registerEntityMethod == null)
                throw new MissingMethodException($"Missing EntityRegistry register method");

            registerEntityMethod.Invoke(EntityRegistry, new object[] { newEntity });

            List<Type> entityTypes = new List<Type>();
            Type currentType = null;

            do
            {
                currentType = currentType == null ? newEntity.GetType() : currentType.BaseType;
                entityTypes.Add(currentType);
            } while (currentType != typeof(Entity));

            for (int i = entityTypes.Count - 1; i >= 0; i--)
            {
                raiseEntityCreatedMethod.MakeGenericMethod(entityTypes[i]).Invoke(this, new object[] { blueprintId, tableName, newEntity });
            }

            (newEntity as EntityType).LateInit();
        }

        private void RaiseEntityCreated<EntityType>(string blueprintId, string blueprintTable, EntityType newEntity) where EntityType : Entity
        {
            EventBus.Raise<EntityCreatedEvent<EntityType>>(blueprintId, blueprintTable, newEntity.ID,
                newEntity.coordinate.Origin, newEntity.coordinate.End);
        }

        private void RegisterEntityMethods()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsClass && !type.IsAbstract)
                {
                    if (typeof(Entity).IsAssignableFrom(type))
                    {
                        RegisterEntity(type);
                    }
                }
            }

            void RegisterEntity(Type entityType)
            {
                foreach (ConstructorInfo constructor in entityType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    ParameterInfo[] parameters = constructor.GetParameters();
                    if (parameters.Length == 2 &&
                        parameters[0].ParameterType == typeof(uint) &&
                        parameters[1].ParameterType == typeof(Coordinate))
                    {
                        entityConstructors.Add(entityType, constructor);
                        break;
                    }
                }
            }
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<SpawnAnimalRequestAcceptedEvent>(SpawnAnimal);
            EventBus.Unsubscribe<SpawnJailRequestAcceptedEvent>(SpawnJail);
            EventBus.Unsubscribe<SpawnInfrastructureRequestAcceptedEvent>(SpawnInfrastrcture);
        }
    }
}
