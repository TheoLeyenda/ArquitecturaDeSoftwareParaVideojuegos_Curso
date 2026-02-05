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

        private Dictionary<int, TileData> tileDatas;
        private Dictionary<int, string> tileHashToName;

        public Map(uint sizeX, uint sizeY)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;

            tileDatas = new Dictionary<int, TileData>();
            tileHashToName = new Dictionary<int, string>();

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

                tileDatas.Add(tileTypeID.GetHashCode(), (TileData)tileData);
                tileHashToName.Add(tileTypeID.GetHashCode(), tileTypeID);
            }

            grid = new Tile[sizeX, sizeY];
            int defaultDataHash = 0;
            foreach (KeyValuePair<int, TileData> tileData in tileDatas)
            {
                if (tileData.Value.isDefault)
                {
                    defaultDataHash = tileData.Key.GetHashCode();
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
                    EventBus.Raise<TileCreatedEvent>(defaultDataHash, x, y);
                }
            }

            EventBus.Raise<MapCreatedEvent>();
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
