using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using UnityEngine;
using ZooArchitect.Architecture.GameLogic.Events;
using ZooArchitect.Architecture.Math;
using ZooArchitect.View.Data;
using ZooArchitect.View.Resources;

namespace ZooArchitect.View.Scene
{
    internal sealed class MapView : ViewComponent
    {
        private BlueprintRegistry BlueprintRegiistry => ServiceProvider.Instance.GetService<BlueprintRegistry>();
        private BlueprintBinder BlueprintBinder => ServiceProvider.Instance.GetService<BlueprintBinder>();

        private PrefabsRegistryView PrefabsRegistryView => ServiceProvider.Instance.GetService<PrefabsRegistryView>();

        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

        private Grid grid;
        private Dictionary<int, (string ID, string path)> pathToTilePrefabByIDHash;

        public override void Init()
        {
            base.Init();
            grid = gameObject.AddComponent<Grid>();

            LoadTilePrefabPaths();

            EventBus.Subscribe<TileCreatedEvent>(OnTileCreated);
            EventBus.Subscribe<MapCreatedEvent>(OnMapCreated);
            EventBus.Subscribe<TileModifiedEvent>(OnTileModified);
        }


        private void LoadTilePrefabPaths()
        {
            pathToTilePrefabByIDHash = new Dictionary<int, (string, string)>();

            foreach (string blueprint in BlueprintRegiistry.BlueprintsOf(TableNamesView.TILES_VIEW_TABLE_NAME))
            {
                object tileViewData = new TileViewData();
                BlueprintBinder.Apply(ref tileViewData, TableNamesView.TILES_VIEW_TABLE_NAME, blueprint);
                pathToTilePrefabByIDHash.Add(((TileViewData)tileViewData).ArchitectureIHHash, (((TileViewData)tileViewData).architectureID, ((TileViewData)tileViewData).prefabPath));
            }
        }

        private void OnMapCreated(in MapCreatedEvent mapCreatedEvent)
        {
            EventBus.Unsubscribe<TileCreatedEvent>(OnTileCreated);
        }

        private void OnTileCreated(in TileCreatedEvent tileCreatedEvent)
        {
            CreateTile(tileCreatedEvent.tileId, tileCreatedEvent.xCoord, tileCreatedEvent.yCoord);
        }
        private void OnTileModified(in TileModifiedEvent tileModifiedEvent)
        {
            //TODO: Destroy old
            CreateTile(tileModifiedEvent.newTileId, tileModifiedEvent.xCoord, tileModifiedEvent.yCoord);
        }

        private void CreateTile(int tileId, int coordX, int CoordY) 
        {
            GameObject tileToSpawn = PrefabsRegistryView.Get(TableNamesView.TILES_VIEW_TABLE_NAME, pathToTilePrefabByIDHash[tileId].ID);
            SpriteRenderer sprite = Instantiate(tileToSpawn,
                grid.CellToLocal(new Vector3Int(coordX, CoordY, 0))
                + new Vector3(grid.cellSize.x * 0.5f, grid.cellSize.y * 0.5f, 0.0f),
                Quaternion.identity, grid.gameObject.transform).GetComponent<SpriteRenderer>();
            sprite.sortingOrder = GameScene.MAP_DRAWING_ORDER;
        }

        public override void LateInit()
        {
            base.LateInit();
        }

        public Vector3 CoordinateToGrid(Coordinate coordinate)
        {
            Vector3Int coord = new Vector3Int(coordinate.Origin.X, coordinate.Origin.Y, 0);
            Vector3 output = grid.GetCellCenterWorld(coord);
            output.z = 0.0f;
            return output;
        }

        public Point GetCoordinateAsPointInGrid(CameraView cameraView, Vector3 coordinate) 
        {
            Vector3 mouseScreePosition = coordinate;
            Vector3 mouseWorldPosition = cameraView.GameCamera.ScreenToWorldPoint(mouseScreePosition);
            mouseScreePosition.z = 0.0f;

            Vector3Int cellCoordinate = grid.WorldToCell(mouseWorldPosition);
            return new Point(cellCoordinate.x, cellCoordinate.y);
        }

        public Point GetMouseCoordinateAsPointInGrid(CameraView cameraView)
        {
            return GetCoordinateAsPointInGrid(cameraView, Input.mousePosition);
        }

        public override void Dispose()
        {
            base.Dispose();
            EventBus.Unsubscribe<TileCreatedEvent>(OnTileCreated);
            EventBus.Unsubscribe<MapCreatedEvent>(OnMapCreated);
            EventBus.Unsubscribe<TileModifiedEvent>(OnTileModified);
        }

        struct TileViewData
        {
            [BlueprintParameter("Architecture ID")] public string architectureID;
            [BlueprintParameter("Prefab resource path")] public string prefabPath;

            public int ArchitectureIHHash => architectureID.GetHashCode();
        }

    }
}
