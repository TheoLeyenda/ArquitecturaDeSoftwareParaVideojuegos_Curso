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

        private Container entitiesContainer;

        internal Container EntitiesContainer => entitiesContainer; 

        public override void Init()
        {
            base.Init();
            ServiceProvider.Instance.AddService<EntityRegistryView>(new EntityRegistryView());
            entityFactoryView = new EntityFactoryView();
            entitiesContainer = GameScene.AddSceneComponent<Container>("Entities container", this.transform);
        }

        public override void LateInit()
        {
            base.LateInit();
            spawnEntityControllerView = new SpawnEntityControllerView();
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            spawnEntityControllerView.Tick(Time.deltaTime);
        }

        public override void Dispose()
        {
            base.Dispose();
            spawnEntityControllerView.Dispose();
            entityFactoryView.Dispose();
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
