using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using ZooArchitect.Architecture.Controllers;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.Logs;
using ZooArchitect.Architecture.Math;
using ZooArchitect.View.Mapping;

namespace ZooArchitect.View.Controller
{
    [ViewOf(typeof(SpawnEntityControllerArchitecture))]
    internal sealed class SpawnEntityControllerView : SingleSelectionControllerView
    {
        public SpawnEntityControllerView()
        {
            EventBus.Subscribe<SpawnEntityRequestRejectedEvent>(OnSpawnRejected);
        }

        public override void CreateController()
        {
            Coordinate clickPoint = new Coordinate(GameScene.GetMouseGridCoordinate());
            List<string> animalsBlueprints = EntitiesLogic.ValidEntitiesToSpawnIn(clickPoint);
            if (animalsBlueprints.Count == 0)
                return;

            Dictionary<string, Action> spawnEntities = new Dictionary<string, Action>();
            for (int i = 0; i < animalsBlueprints.Count; i++)
            {
                int index = i;
                spawnEntities.Add($"Spawn {animalsBlueprints[index]}", () =>
                {
                    EventBus.Raise<SpawnEntityRequestEvent>(animalsBlueprints[index], clickPoint);
                });
            }

            Display(spawnEntities);
        }

        private void OnSpawnRejected(in SpawnEntityRequestRejectedEvent spawnEntityRequestRejectedEvent)
        {
            GameConsole.Warning($"Spawn of {spawnEntityRequestRejectedEvent.blueprintToSpawn} in {spawnEntityRequestRejectedEvent.coordinateToSpawn} rejected");
        }

        public override void Dispose()
        {
            EventBus.Unsubscribe<SpawnEntityRequestRejectedEvent>(OnSpawnRejected);
        }
    }
}
