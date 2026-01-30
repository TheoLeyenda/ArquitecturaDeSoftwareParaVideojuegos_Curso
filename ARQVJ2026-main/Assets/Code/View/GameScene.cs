using ImageCampus.ToolBox.Services;
using System;
using UnityEngine;
using ZooArchitect.View.Controller;
using ZooArchitect.View.Entities;

namespace ZooArchitect.View
{
    internal sealed class GameScene : ViewComponent , IService
    {
        public bool IsPersistance => false;

        private EntityFactoryView entityFactoryView;
        private SpawnEntityControllerView spawnEntityControllerView;

        private Container mapContainer;
        private Container entitiesContainer;

        internal Container MapContainer => mapContainer;
        internal Container EntitiesContainer => entitiesContainer;

        private MapView mapView;

        public override void Init()
        {
            base.Init();
            ServiceProvider.Instance.AddService<EntityRegistryView>(new EntityRegistryView());
            entityFactoryView = new EntityFactoryView();

            mapContainer = GameScene.AddSceneComponent<Container>("Map container", this.transform);
            mapContainer.Init();
            entitiesContainer = GameScene.AddSceneComponent<Container>("Entities container", this.transform);
            entitiesContainer.Init();

            mapView = GameScene.AddSceneComponent<MapView>("Map", MapContainer.transform);
            mapView.Init();
        }

        public override void LateInit()
        {
            base.LateInit();
            spawnEntityControllerView = new SpawnEntityControllerView();
            mapContainer.LateInit();
            entitiesContainer.LateInit();
            mapView.LateInit();
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            spawnEntityControllerView?.Tick(deltaTime);
            mapContainer.Tick(deltaTime);
            entitiesContainer.Tick(deltaTime);
            mapView.Tick(deltaTime);
        }

        public override void Dispose()
        {
            base.Dispose();
            spawnEntityControllerView?.Dispose();
            entityFactoryView.Dispose();
            mapContainer.Dispose();
            entitiesContainer.Dispose();
            mapView.Dispose();
        }

        public static ComponentType AddSceneComponent<ComponentType>(string name, Transform parent = null, GameObject prefab = null) where ComponentType : ViewComponent 
        {
            return AddSceneComponent(typeof(ComponentType), name, parent, prefab) as ComponentType;
        }

        public static ViewComponent AddSceneComponent(Type viewComponentType, string name, Transform parent = null, GameObject prefab = null)
        {
            if (!typeof(ViewComponent).IsAssignableFrom(viewComponentType))
                throw new InvalidOperationException();

            GameObject newSceneObject = prefab == null ? new GameObject() : UnityEngine.Object.Instantiate(prefab);
            newSceneObject.name = name;
            if (parent != null)
                newSceneObject.transform.parent = parent;
            return newSceneObject.AddComponent(viewComponentType) as ViewComponent;
        }
    }
}
