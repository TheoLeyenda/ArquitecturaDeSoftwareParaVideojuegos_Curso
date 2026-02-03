using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
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
            GameObject tileToSpawn = PrefabsRegistryView.Get(TableNamesView.TILES_VIEW_TABLE_NAME, pathToTilePrefabByIDHash[tileCreatedEvent.tileId].ID);
            SpriteRenderer sprite = Instantiate(tileToSpawn,
                grid.CellToLocal(new Vector3Int(tileCreatedEvent.xCoord, tileCreatedEvent.yCoord, 0))
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


        public override void Dispose()
        {
            base.Dispose();
            EventBus.Unsubscribe<TileCreatedEvent>(OnTileCreated);
            EventBus.Unsubscribe<MapCreatedEvent>(OnMapCreated);
        }

        struct TileViewData
        {
            [BlueprintParameter("Architecture ID")] public string architectureID;
            [BlueprintParameter("Prefab resource path")] public string prefabPath;

            public int ArchitectureIHHash => architectureID.GetHashCode();
        }

    }
}
