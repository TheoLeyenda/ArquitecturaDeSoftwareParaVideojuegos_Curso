using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using UnityEngine;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.Entities.Events;
using ZooArchitect.View.Resources;

namespace ZooArchitect.View.Entities
{
    internal sealed class EntityFactoryView : IDisposable
    {

        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
        private PrefabsRegistryView PrefabsRegistryView => ServiceProvider.Instance.GetService<PrefabsRegistryView>();
        public EntityFactoryView()
        {
            EventBus.Subscribe<EntityCreatedEvent<Entity>>(OnEntityCreated);
        }

        private void OnEntityCreated(in EntityCreatedEvent<Entity> callback)
        {
            UnityEngine.Object.Instantiate(PrefabsRegistryView.Get(callback.blueprintId),
                new Vector3((float)callback.coordinate.Origin.X, 0.0f, (float)callback.coordinate.Origin.Y), // TODO Coordinate to Vector3 translator
                Quaternion.identity);
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<EntityCreatedEvent<Entity>>(OnEntityCreated);
        }
    }
}
