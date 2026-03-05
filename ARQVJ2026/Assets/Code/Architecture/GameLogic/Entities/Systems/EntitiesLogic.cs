using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Dataflow;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using ZooArchitect.Architecture.Data;
using ZooArchitect.Architecture.GameLogic;
using ZooArchitect.Architecture.GameLogic.Events;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Entities
{
    public sealed class EntitiesLogic : IService, ITickable, IDisposable
    {
        public bool IsPersistance => false;

        private EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();

        private BlueprintRegistry BlueprintRegistry => ServiceProvider.Instance.GetService<BlueprintRegistry>();

        private Scene Scene => ServiceProvider.Instance.GetService<Scene>();

        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

        private AnimalsLogic animalsLogic;
        private WorkersLogic workersLogic;
        private SpawnVisitorLogic spawnVisitorLogic;
        private StructureLogic structureLogic;
        public EntitiesLogic()
        {
            animalsLogic = new AnimalsLogic();
            workersLogic = new WorkersLogic();
            spawnVisitorLogic = new SpawnVisitorLogic();
            structureLogic = new StructureLogic();
            EventBus.Subscribe<DayChangeEvent>(OnDayChange);
        }

        private void OnDayChange(in DayChangeEvent _)
        {
            animalsLogic.FeedAnimals();
            animalsLogic.CheckSleepHours();
            workersLogic.PaidWorkers();
            structureLogic.DecreaseDailyMaintenance();
        }

        public void Tick(float deltaTime)
        {
            foreach (Entity entity in EntityRegistry.Entities)
            {
                entity.Tick(deltaTime);
            }

            animalsLogic.Tick(deltaTime);
            workersLogic.Tick(deltaTime);
            spawnVisitorLogic.Tick(deltaTime);
            structureLogic.Tick(deltaTime);
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<DayChangeEvent>(OnDayChange);
            animalsLogic.Dispose();
            workersLogic.Dispose();
            spawnVisitorLogic.Dispose();
            structureLogic.Dispose();
        }

        public List<string> ValidAnimalsToSpawnIn(Point point)
        {
            if (Scene.IsCoordinateInsideMap(new Coordinate(point)))
                return BlueprintRegistry.BlueprintsOf(TableNames.ANIMALS_TABLE_NAME);
            return new List<string>();
        }

        public List<string> ValidInfrastructuresToSpawnIn(Point point)
        {
            if (Scene.IsCoordinateInsideMap(new Coordinate(point)))
                return BlueprintRegistry.BlueprintsOf(TableNames.INFTRASTRUCTURE_TABLE_NAME);
            return new List<string>();
        }

        public string GetJailBlueprint()
        {
            return BlueprintRegistry.BlueprintsOf(TableNames.JAILS_TABLE_NAME)[0];
        }
    }

}