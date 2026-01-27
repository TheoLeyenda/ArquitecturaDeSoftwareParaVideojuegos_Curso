using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using System.Reflection;
using UnityEngine;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.Entities.Events;
using ZooArchitect.View.Resources;

namespace ZooArchitect.View.Entities
{
    internal sealed class EntityFactoryView : IDisposable
    {

        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
        private EntityRegistryView EntityRegistryView => ServiceProvider.Instance.GetService<EntityRegistryView>();

        private PrefabsRegistryView PrefabsRegistryView => ServiceProvider.Instance.GetService<PrefabsRegistryView>();

        private MethodInfo registerEntityMethod;


        public EntityFactoryView()
        {
            EventBus.Subscribe<EntityCreatedEvent<Entity>>(OnEntityCreated);

            registerEntityMethod = EntityRegistryView.GetType().GetMethod(EntityRegistryView.RegisterMethodName,
                BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private void OnEntityCreated(in EntityCreatedEvent<Entity> callback)
        {
            GameObject instance = UnityEngine.Object.Instantiate(PrefabsRegistryView.Get(callback.blueprintId),
                new Vector3((float)callback.coordinate.Origin.X, 0.0f, (float)callback.coordinate.Origin.Y), // TODO Coordinate to Vector3 translator
                Quaternion.identity);
            
            registerEntityMethod.Invoke(EntityRegistryView, new object[] { instance.GetComponent<EntityView>() });
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<EntityCreatedEvent<Entity>>(OnEntityCreated);
        }
    }
}
