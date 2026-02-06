using ImageCampus.ToolBox.Dataflow;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using ZooArchitect.Architecture.Controllers;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.GameLogic;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture
{
    public sealed class Scene : IService, IInitable, ITickable, IDisposable
    {
        public bool IsPersistance => false;

        private EntitiesLogic EntitiesLogic => ServiceProvider.Instance.GetService<EntitiesLogic>();
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

        private Wallet Wallet => ServiceProvider.Instance.GetService<Wallet>();

        private SpawnEntityControllerArchitecture spawnEntityControllerArchitecture;
        private TerrainModifierControllerArchitecture TerrainModifierControllerArchitecture;

        private Map map;

        public void Init()
        {
            ServiceProvider.Instance.AddService<Time>(new Time());
            ServiceProvider.Instance.AddService<DayNightCycle>(new DayNightCycle());
            ServiceProvider.Instance.AddService<Wallet>(new Wallet());
            ServiceProvider.Instance.AddService<EntityRegistry>(new EntityRegistry());
            ServiceProvider.Instance.AddService<EntityFactory>(new EntityFactory());
            ServiceProvider.Instance.AddService<EntitiesLogic>(new EntitiesLogic());

        }

        public void LateInit()
        {
            map = new Map(100, 100);
            EventBus.Subscribe<ModifyTerrainRecuestAceptedEvent>(OnModifyTerrainRequestAcepted);
            spawnEntityControllerArchitecture = new SpawnEntityControllerArchitecture();
            TerrainModifierControllerArchitecture = new TerrainModifierControllerArchitecture();
        }

        public void Tick(float deltaTime)
        {
            EntitiesLogic.Tick(deltaTime);
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<ModifyTerrainRecuestAceptedEvent>(OnModifyTerrainRequestAcepted);

            spawnEntityControllerArchitecture.Dispose();
            EntitiesLogic.Dispose();
            Wallet.Dispose();
        }

        public bool IsCoordinateInsideMap(Coordinate coordinate)
        {
            return map.IsCoordinateInsideMap(coordinate);
        }

        public List<string> GetValidTilesForSelection(Coordinate coordinate)
        {
            List<string> output = new List<string>(map.GetTileDefinitionIDs);

            if (!coordinate.IsSingleCoordinate)
            {
                foreach (string uniqueTileDefinition in map.UniqueTileDefinitions)
                {
                    output.Remove(uniqueTileDefinition);
                }
            }
            else
            {
                foreach (string uniqueTileDefinition in map.UniqueTileDefinitions)
                {
                    if (map.HasInstancesOf(uniqueTileDefinition) && map.GetInstanceAmountOf(uniqueTileDefinition) >= 1)
                    {
                        output.Remove(uniqueTileDefinition);
                    }
                }
            }

            return output;
        }


        private void OnModifyTerrainRequestAcepted(in ModifyTerrainRecuestAceptedEvent modifyTerrainRecuestAceptedEvent)
        {
            for (int x = modifyTerrainRecuestAceptedEvent.origin.X; x <= modifyTerrainRecuestAceptedEvent.end.X; x++)
            {
                for (int y = modifyTerrainRecuestAceptedEvent.origin.Y; y <= modifyTerrainRecuestAceptedEvent.end.Y; y++)
                {
                    map.SwapTile((x,y), modifyTerrainRecuestAceptedEvent.newTileId);
                }
            }
        }
    }
}