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

        private GameScene GameScene => ServiceProvider.Instance.GetService<GameScene>();

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
            ViewComponent viewComponent = GameScene.AddSceneComponent(
               ViewArchitectureMap.ViewOf(EntityRegistry[callback.entityCreatedId].GetType()),
               PrefabsRegistryView.Get(callback.blueprintId).name + $"  -  Architecture type: {EntityRegistry[callback.entityCreatedId].GetType().Name} - ID: {callback.entityCreatedId}",
               GameScene.EntitiesContainer.transform,
               PrefabsRegistryView.Get(callback.blueprintId));

            viewComponent.transform.position = new Vector3((float)callback.coordinate.Origin.X, 0.0f, (float)callback.coordinate.Origin.Y); // TODO Coordinate to Vector3 translator

            setEntityIdMethod.Invoke(viewComponent, new object[] { callback.entityCreatedId });

            registerEntityMethod.Invoke(EntityRegistryView, new object[] { viewComponent });
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<EntityCreatedEvent<Entity>>(OnEntityCreated);
        }
    }
}
