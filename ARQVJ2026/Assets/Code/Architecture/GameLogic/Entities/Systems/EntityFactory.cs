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
        private Dictionary<string, Type> entityClassNameToType;
        private Dictionary<Type, ConstructorInfo> entityConstructors;
        private MethodInfo registerEntityMethod;
        private MethodInfo raiseEntityCreatedMethod;


        private Dictionary<Type, object> creationSubsctiptions;
        private MethodInfo subscriveToCreationMethod;
        private MethodInfo unsubscriveMethod;
        private MethodInfo raiseEntityRequestAcceptedMethd;
        public EntityFactory()
        {
            this.lastAssignedEntityId = Entity.UNASSIGNED_ENTITY_ID;
            entityConstructors = new Dictionary<Type, ConstructorInfo>();
            creationSubsctiptions = new Dictionary<Type, object>();
            entityClassNameToType = new Dictionary<string, Type>();
            registerEntityMethod = EntityRegistry.GetType().GetMethod(EntityRegistry.RegisterMethodName,
                BindingFlags.NonPublic | BindingFlags.Instance);

            raiseEntityCreatedMethod = GetType().GetMethod(nameof(RaiseEntityCreated), BindingFlags.NonPublic | BindingFlags.Instance);

            subscriveToCreationMethod = GetType().GetMethod(nameof(SubscribeToCreation), BindingFlags.NonPublic | BindingFlags.Instance);
            raiseEntityRequestAcceptedMethd = GetType().GetMethod(nameof(RaseEntityRequestAccepted), BindingFlags.NonPublic | BindingFlags.Instance);
            unsubscriveMethod = typeof(EventBus).GetMethod(nameof(EventBus.Unsubscribe), BindingFlags.Public | BindingFlags.Instance);
            
            
            RegisterEntityMethods();

            EventBus.Subscribe<SpawnRequestAcceptedEvent>(RaiseEventAsGeneric);
        }

        private void RaiseEventAsGeneric(in SpawnRequestAcceptedEvent spawnRequestAcceptedEvent)
        {
            Type raisingType = entityClassNameToType[spawnRequestAcceptedEvent.entityTypeName];
            raiseEntityRequestAcceptedMethd.MakeGenericMethod(raisingType).Invoke(this, new object[] { spawnRequestAcceptedEvent });
        }

        private void RaseEntityRequestAccepted<EntityType>(SpawnRequestAcceptedEvent spawnRequestAcceptedEvent) where EntityType : Entity
        {
            EventBus.Raise<SpawnRequestAcceptedEvent<EntityType>>
                (
                spawnRequestAcceptedEvent.blueprintToSpawn,
                spawnRequestAcceptedEvent.coordinateToSpawn,
                spawnRequestAcceptedEvent.blueprintTable
                );
        }

        private void SubscribeToCreation<EntityType>() where EntityType : Entity
        {
            EventBus.EventCallback<SpawnRequestAcceptedEvent<EntityType>> callback =
                EventBus.SubscribeAndReturn<SpawnRequestAcceptedEvent<EntityType>>(SpawnEntity);
            creationSubsctiptions.Add(typeof(SpawnRequestAcceptedEvent<EntityType>), callback);

            void SpawnEntity(in SpawnRequestAcceptedEvent<EntityType> callback)
            {
                CreateInstance<EntityType>(callback.blueprintToSpawn,
                 callback.coordinateToSpawn,
                 callback.blueprintTable);
            }
        }

        private void UnsubscribeToCreation()  
        {
            foreach (KeyValuePair<Type, object> methodsToUnsubscribe in creationSubsctiptions)
            {
                unsubscriveMethod.MakeGenericMethod(methodsToUnsubscribe.Key).
                    Invoke(EventBus, new object[] { methodsToUnsubscribe.Value });
            }
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
                        subscriveToCreationMethod.MakeGenericMethod(type).Invoke(this, new object[0]);
                        entityClassNameToType.Add(type.Name, type);
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
            UnsubscribeToCreation();
            EventBus.Unsubscribe<SpawnRequestAcceptedEvent>(RaiseEventAsGeneric);
        }
    }
}
