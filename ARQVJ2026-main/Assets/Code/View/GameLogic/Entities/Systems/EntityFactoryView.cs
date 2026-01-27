using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using System.Reflection;
using UnityEngine;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.Entities.Events;
using ZooArchitect.View.Mapping;
using ZooArchitect.View.Resources;

namespace ZooArchitect.View.Entities
{
    [ViewOf(typeof(EntityFactory))]
    internal sealed class EntityFactoryView : IDisposable
    {
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
        private EntityRegistryView EntityRegistryView => ServiceProvider.Instance.GetService<EntityRegistryView>();
        private PrefabsRegistryView PrefabsRegistryView => ServiceProvider.Instance.GetService<PrefabsRegistryView>();
        private EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();

        private MethodInfo registerEntityMethod;
        private MethodInfo setEntityIdMethod;

        public EntityFactoryView()
        {
            EventBus.Subscribe<EntityCreatedEvent<Entity>>(OnEntityCreated);

            registerEntityMethod = EntityRegistryView.GetType().GetMethod(EntityRegistryView.RegisterMethodName,
                BindingFlags.NonPublic | BindingFlags.Instance);

            setEntityIdMethod = typeof(EntityView).GetMethod(EntityView.SetIdMethodName, 
                BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private void OnEntityCreated(in EntityCreatedEvent<Entity> callback)
        {
            GameObject instance = UnityEngine.Object.Instantiate(PrefabsRegistryView.Get(callback.blueprintId),
                new Vector3((float)callback.coordinate.Origin.X, 0.0f, (float)callback.coordinate.Origin.Y), // TODO Coordinate to Vector3 translator
                Quaternion.identity);
            
            Component viewComponent = instance.AddComponent(
                ViewArchitectureMap.ViewOf(EntityRegistry[callback.entityCreatedId].GetType()));

            viewComponent.gameObject.name += $"  -  Architecture type: {EntityRegistry[callback.entityCreatedId].GetType().Name} - ID: {callback.entityCreatedId}";

            setEntityIdMethod.Invoke(viewComponent, new object[] { callback.entityCreatedId });

            registerEntityMethod.Invoke(EntityRegistryView, new object[] { viewComponent });
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<EntityCreatedEvent<Entity>>(OnEntityCreated);
        }
    }
}
