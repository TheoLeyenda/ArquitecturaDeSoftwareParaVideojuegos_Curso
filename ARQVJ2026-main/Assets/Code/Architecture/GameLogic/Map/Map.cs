using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using ZooArchitect.Architecture.Data;
using ZooArchitect.Architecture.Exceptions;
using ZooArchitect.Architecture.GameLogic.Events;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.GameLogic
{
    public sealed class Map
    {
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
        private BlueprintRegistry BlueprintRegistry => ServiceProvider.Instance.GetService<BlueprintRegistry>();
        private BlueprintBinder BlueprintBinder => ServiceProvider.Instance.GetService<BlueprintBinder>();


        private uint sizeX;
        private uint sizeY;

        private Tile[,] grid;
        private Dictionary<string, List<(int x, int y)>> instances;

        private Dictionary<int, TileData> tileDatas;
        private Dictionary<int, string> tileHashToName;

        private List<string> uniqueTileDefinitions;
        public IReadOnlyList<string> UniqueTileDefinitions => uniqueTileDefinitions;

        public bool HasInstancesOf(string tileID) => instances.ContainsKey(tileID) && instances[tileID].Count > 0;
        public int GetInstanceAmountOf(string tileID) => instances[tileID].Count;

        public IEnumerable<string> GetTileDefinitionIDs => tileHashToName.Values;

        public Map(uint sizeX, uint sizeY)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;

            tileDatas = new Dictionary<int, TileData>();
            tileHashToName = new Dictionary<int, string>();
            instances = new Dictionary<string, List<(int x, int y)>>();
            uniqueTileDefinitions = new List<string>();


            foreach (string tileTypeID in BlueprintRegistry.BlueprintsOf(TableNames.TILE_TYPES_TABLE_NAME))
            {
                object tileData = new TileData();

                try
                {
                    BlueprintBinder.Apply(ref tileData, TableNames.TILE_TYPES_TABLE_NAME, tileTypeID);
                }
                catch (DataMisalignedException exception)
                {
                    throw new DataEntryException($"Failed to read {TableNames.TILE_TYPES_TABLE_NAME} - {tileTypeID}\n({exception.Message})");
                }

                if (((TileData)tileData).isUnique)
                {
                    uniqueTileDefinitions.Add(tileTypeID);
                }

                tileDatas.Add(tileTypeID.GetHashCode(), (TileData)tileData);
                tileHashToName.Add(tileTypeID.GetHashCode(), tileTypeID);
            }

            grid = new Tile[sizeX, sizeY];
            int defaultDataHash = 0;
            string defaultDataID = string.Empty;
            foreach (KeyValuePair<int, TileData> tileData in tileDatas)
            {
                if (tileData.Value.isDefault)
                {
                    defaultDataHash = tileData.Key.GetHashCode();
                    defaultDataID = tileHashToName[defaultDataHash];
                    break;
                }
            }

            if (defaultDataHash == 0)
                throw new BrokenGameRuleException("Missing defualt tile definition in Blueprints.xlsx");

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    grid[x, y] = new Tile(defaultDataHash);

                    if (!instances.ContainsKey(defaultDataID))
                        instances.Add(defaultDataID, new List<(int x, int y)>());

                    instances[defaultDataID].Add((x, y));

                    EventBus.Raise<TileCreatedEvent>(defaultDataHash, x, y);
                }
            }

            EventBus.Raise<MapCreatedEvent>();
        }

        public void SwapTile((int x, int y) coordinate, string newTileId)
        {
            if (!tileHashToName.ContainsValue(newTileId))
                throw new System.MissingFieldException($"{newTileId} is not a valid Tile key definition.");

            if (!instances.ContainsKey(newTileId))
                instances.Add(newTileId, new List<(int x, int y)>());

            Tile originalTileInCoordinate = grid[coordinate.x, coordinate.y];
            string originalTileId = tileHashToName[originalTileInCoordinate.tileTypeId];

            instances[originalTileId].Remove(coordinate);
            instances[newTileId].Add(coordinate);

            grid[coordinate.x, coordinate.y].tileTypeId = newTileId.GetHashCode();

            EventBus.Raise<TileModifiedEvent>(grid[coordinate.x, coordinate.y].tileTypeId,
                coordinate.x, coordinate.y);
        }

        public (uint x, uint y) GetSize() => (sizeX, sizeY);

        public bool IsCoordinateInsideMap(Coordinate coordinate)
        {
            (uint x, uint y) mapSize = GetSize();

            foreach (Point point in coordinate.Points)
            {
                if (point.X >= mapSize.x || point.X < 0 || point.Y >= mapSize.y || point.Y < 0)
                    return false;
            }

            return true;
        }
    }
}
