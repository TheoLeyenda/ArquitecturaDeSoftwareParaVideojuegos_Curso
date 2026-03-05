using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Dataflow;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using ZooArchitect.Architecture.Controllers;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.Data;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.Entities.Events;
using ZooArchitect.Architecture.GameLogic;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture
{
    public sealed class Scene : IService, IInitable, ITickable, IDisposable
    {
        public bool IsPersistance => false;

        private EntitiesLogic EntitiesLogic => ServiceProvider.Instance.GetService<EntitiesLogic>();
        private EntityFactory EntityFactory => ServiceProvider.Instance.GetService<EntityFactory>();
        private EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
        private Wallet Wallet => ServiceProvider.Instance.GetService<Wallet>();
        private BlueprintBinder BlueprintBinder => ServiceProvider.Instance.GetService<BlueprintBinder>();
        private GameplayLogic GameplayLogic => ServiceProvider.Instance.GetService<GameplayLogic>();

        private SpawnAnimalControllerArchitecture spawnAnmalControllerArchitecture;
        private TerrainModifierControllerArchitecture terrainModifierControllerArchitecture;
        private SpawnJailControllerArchitecture spawnJailControllerArchitecture;
        private SpawnInfrastructureControllerArchitecture spawnInfrastructureControllerArchitecture;
        private BuyControllerArchitecture buyControllerArchitecture;
        private SceneControllerArchitecture sceneControllerArchitecture;
        private TimeScaleController timeScaleController;

        private Map map;

        private Dictionary<Coordinate, List<uint>> entityIdsInCoordiante;

        public void Init()
        {
            ServiceProvider.Instance.AddService<Time>(new Time());
            ServiceProvider.Instance.AddService<DayNightCycle>(new DayNightCycle());
            ServiceProvider.Instance.AddService<Wallet>(new Wallet());
            ServiceProvider.Instance.AddService<EntityRegistry>(new EntityRegistry());
            ServiceProvider.Instance.AddService<EntityFactory>(new EntityFactory());
            ServiceProvider.Instance.AddService<EntitiesLogic>(new EntitiesLogic());
            ServiceProvider.Instance.AddService<BuyCatalog>(new BuyCatalog());
            ServiceProvider.Instance.AddService<GameplayLogic>(new GameplayLogic());
            GameplayLogic.Init();
        }

        public void LateInit()
        {
            uint mapSizeX = 100;
            uint mapSizeY = 100;
            map = new Map(mapSizeX, mapSizeY);
            entityIdsInCoordiante = new Dictionary<Coordinate, List<uint>>();

            for (int x = 0; x < mapSizeX; x++)
            {
                for (int y = 0; y < mapSizeY; y++)
                {
                    entityIdsInCoordiante.Add(new Coordinate(x, y), new List<uint>());
                }
            }

            GameplayLogic.LateInit();

            EventBus.Subscribe<ModifyTerrainRecuestAceptedEvent>(OnModifyTerrainRequestAcepted);
            EventBus.Subscribe<EntityCreatedEvent<Entity>>(OnEntityCreated);
            EventBus.Subscribe<EntityDestroyedEvent>(OnEntityDestroyed);
            EventBus.Subscribe<EntityMovedEvent>(OnEntityMoved);
            spawnAnmalControllerArchitecture = new SpawnAnimalControllerArchitecture();
            terrainModifierControllerArchitecture = new TerrainModifierControllerArchitecture();
            spawnJailControllerArchitecture = new SpawnJailControllerArchitecture();
            spawnInfrastructureControllerArchitecture = new SpawnInfrastructureControllerArchitecture();
            buyControllerArchitecture = new BuyControllerArchitecture();
            sceneControllerArchitecture = new SceneControllerArchitecture();
            timeScaleController = new TimeScaleController();
        }

        public void Tick(float deltaTime)
        {
            EntitiesLogic.Tick(deltaTime);
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<ModifyTerrainRecuestAceptedEvent>(OnModifyTerrainRequestAcepted);
            EventBus.Unsubscribe<EntityCreatedEvent<Entity>>(OnEntityCreated);
            EventBus.Unsubscribe<EntityDestroyedEvent>(OnEntityDestroyed);
            EventBus.Unsubscribe<EntityMovedEvent>(OnEntityMoved);

            spawnAnmalControllerArchitecture.Dispose();
            terrainModifierControllerArchitecture.Dispose();
            spawnJailControllerArchitecture.Dispose();
            spawnInfrastructureControllerArchitecture.Dispose();
            sceneControllerArchitecture.Dispose();
            EntitiesLogic.Dispose();
            EntityFactory.Dispose();
            Wallet.Dispose();
            GameplayLogic.Dispose();
            timeScaleController.Dispose();
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

            output.Remove(map.HabitatTileDefinition);
            output.Remove(map.HabitatWallTileDefinition);

            return output;
        }

        private void OnModifyTerrainRequestAcepted(in ModifyTerrainRecuestAceptedEvent modifyTerrainRecuestAceptedEvent)
        {
            for (int x = modifyTerrainRecuestAceptedEvent.origin.x; x <= modifyTerrainRecuestAceptedEvent.end.x; x++)
            {
                for (int y = modifyTerrainRecuestAceptedEvent.origin.y; y <= modifyTerrainRecuestAceptedEvent.end.y; y++)
                {
                    map.SwapTile((x, y), modifyTerrainRecuestAceptedEvent.newTileId);
                }
            }
        }

        public Coordinate MapCoordinate => map.GetCoordinate();
        public string HabitatTileDefinition => map.HabitatTileDefinition;
        public string HabitatWallTileDefinition => map.HabitatWallTileDefinition;
        public bool HasHumanEntryPoint => map.HasInstancesOf(map.HumanEntryTileDefinition);
        public Point HumanEntryPoint => map.GetHumanEntryPoint();
        public bool HasHumanExitPoint => map.HasInstancesOf(map.HumanExitTileDefinition);
        public Point HumanExitPoint => map.GetHumanExitPoint();
        public TileData GetTileDataOf(in Point point) => map.GetTileDataOf(point);


        private void OnEntityMoved(in EntityMovedEvent entityMovedEvent)
        {
            foreach (Point point in entityMovedEvent.oldCoodinate.Points)
            {
                entityIdsInCoordiante[new Coordinate(point)].Remove(entityMovedEvent.movedEntityId);

            }

            foreach (Point point in EntityRegistry[entityMovedEvent.movedEntityId].coordinate.Points)
            {
                entityIdsInCoordiante[new Coordinate(point)].Add(entityMovedEvent.movedEntityId);
            }
        }

        private void OnEntityDestroyed(in EntityDestroyedEvent entityDestroyedEvent)
        {
            foreach (Point point in EntityRegistry[entityDestroyedEvent.entityID].coordinate.Points)
            {
                entityIdsInCoordiante[new Coordinate(point)].Remove(entityDestroyedEvent.entityID);
            }
        }

        private void OnEntityCreated(in EntityCreatedEvent<Entity> entityCreatedEvent)
        {
            foreach (Point point in EntityRegistry[entityCreatedEvent.entityCreatedId].coordinate.Points)
            {
                entityIdsInCoordiante[new Coordinate(point)].Add(entityCreatedEvent.entityCreatedId);
            }
        }

        public List<uint> GetEntitiesIn(in Coordinate coordinate)
        {
            return entityIdsInCoordiante[coordinate];
        }
    }
}