using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.Entities.Events;
using ZooArchitect.View.Data;
using ZooArchitect.View.Mapping;
using ZooArchitect.View.Resources;
using ZooArchitect.View.Scene;

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

        private void OnEntityCreated(in EntityCreatedEvent<Entity> entityCreatedEvent)
        {
            bool IsSingleCoordinate = EntityRegistry[entityCreatedEvent.entityCreatedId].coordinate.IsSingleCoordinate;
            List<object> parameters = new List<object>();

            GameObject prefab = PrefabsRegistryView.Get(TableNamesView.ArchitectureToView[entityCreatedEvent.blueprintTable], entityCreatedEvent.blueprintId);

            GameObject objectToInstance = IsSingleCoordinate ? prefab : null;

            ViewComponent viewComponent = GameScene.AddSceneComponent(
               ViewArchitectureMap.ViewOf(EntityRegistry[entityCreatedEvent.entityCreatedId].GetType()),
               PrefabsRegistryView.Get(TableNamesView.ArchitectureToView[entityCreatedEvent.blueprintTable], entityCreatedEvent.blueprintId).name + $"  -  Architecture type: {EntityRegistry[entityCreatedEvent.entityCreatedId].GetType().Name} - ID: {entityCreatedEvent.entityCreatedId}",
               GameScene.EntitiesContainer.transform,
               objectToInstance);

            viewComponent.transform.position = GameScene.CoordinateToWorld(EntityRegistry[entityCreatedEvent.entityCreatedId].coordinate);

            if (IsSingleCoordinate)
            {
                SpriteRenderer sprite = viewComponent.GetComponent<SpriteRenderer>();
                sprite.sortingOrder = GameScene.ENTITIES_DRAWING_ORDER;
            }
            else
            {
                parameters.Add(prefab);
                parameters.Add(GameScene.ENTITIES_DRAWING_ORDER);
            }

            setEntityIdMethod.Invoke(viewComponent, new object[] { entityCreatedEvent.entityCreatedId });

            viewComponent.Init(parameters.ToArray());

            registerEntityMethod.Invoke(EntityRegistryView, new object[] { viewComponent });

            viewComponent.LateInit();
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<EntityCreatedEvent<Entity>>(OnEntityCreated);
        }
    }
}
