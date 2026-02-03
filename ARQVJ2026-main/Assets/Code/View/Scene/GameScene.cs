using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using UnityEngine;
using ZooArchitect.Architecture.Math;
using ZooArchitect.View.Controller;
using ZooArchitect.View.Data;
using ZooArchitect.View.Entities;
using ZooArchitect.View.Resources;

namespace ZooArchitect.View.Scene
{
    internal sealed class GameScene : ViewComponent , IService
    {
        public bool IsPersistance => false;

        public const int MAP_DRAWING_ORDER = 0;
        public const int ENTITIES_DRAWING_ORDER = 1;

        private PrefabsRegistryView PrefabsRegistryView => ServiceProvider.Instance.GetService<PrefabsRegistryView>();
        private ContextMenuView ContextMenuView => ServiceProvider.Instance.GetService<ContextMenuView>();

        private EntityFactoryView entityFactoryView;
        private SpawnEntityControllerView spawnEntityControllerView;

        private Container mapContainer;
        private Container entitiesContainer;

        internal Container MapContainer => mapContainer;
        internal Container EntitiesContainer => entitiesContainer;

        private MapView mapView;
        private CameraView cameraView;

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

        public override void Init(params object[] parameters)
        {
            base.Init(parameters);
            Canvas canvasView = parameters[0] as Canvas;

            GameObject contextMenuPrefab = PrefabsRegistryView.Get(TableNamesView.UI_VIEW_TABLE_NAME, "ContextMenuContainer");
            GameObject buttonMenuPrefab = PrefabsRegistryView.Get(TableNamesView.UI_VIEW_TABLE_NAME, "ButtonPrefab");

            ContextMenuView contextMenuView = GameScene.AddSceneComponent<ContextMenuView>("ContextMenu", canvasView.transform, contextMenuPrefab);
            ServiceProvider.Instance.AddService<ContextMenuView>(contextMenuView);
            contextMenuView.Init(contextMenuPrefab, buttonMenuPrefab, canvasView);

            GameObject cameraPrefab = PrefabsRegistryView.Get(TableNamesView.CAMERA_VIEW_TABLE_NAME, "GameCamera");

            cameraView = GameScene.AddSceneComponent<CameraView>("Main Game Camera", null, cameraPrefab);
            cameraView.Init(cameraView.GetComponent<Camera>());
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
            ContextMenuView.Tick(deltaTime);
        }

        public Vector3 CoordinateToWorld(Coordinate coordinate) 
        {
            return mapView.CoordinateToGrid(coordinate);
        }

        public Point GetMouseGridCoordinate() 
        {
            return mapView.GetMouseCoordinateAsPointInGrid(cameraView);
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
