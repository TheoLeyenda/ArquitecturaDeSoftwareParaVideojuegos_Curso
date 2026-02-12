using ImageCampus.ToolBox.Events;
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
    [ViewOf(typeof(SpawnAnimalControllerArchitecture))]
    internal sealed class SpawnAnimalControllerView : SingleSelectionControllerView
    {
        public SpawnAnimalControllerView()
        {
            EventBus.Subscribe<SpawnAnimalRequestRejectedEvent>(OnSpawnRejected);
        }

        protected override List<string> GetValidBlueprints(Point clickPoint)
        {
            return EntitiesLogic.ValidAnimalsToSpawnIn(clickPoint);
        }

        protected override Dictionary<string, Action> GetActionsToDisplay(Point clickPoint, List<string> blueprints)
        {
            Dictionary<string, Action> spawnEntities = new Dictionary<string, Action>();
            for (int i = 0; i < blueprints.Count; i++)
            {
                int index = i;
                spawnEntities.Add($"Spawn {blueprints[index]}", () =>
                {
                    EventBus.Raise<SpawnAnimalRequestEvent>(blueprints[index], clickPoint);
                });
            }
            return spawnEntities;
        }

        private void OnSpawnRejected(in SpawnAnimalRequestRejectedEvent spawnEntityRequestRejectedEvent)
        {
            GameConsole.Warning($"Spawn of {spawnEntityRequestRejectedEvent.blueprintToSpawn} in {spawnEntityRequestRejectedEvent.pointToSpawn} rejected");
        }

        public override void Dispose()
        {
            EventBus.Unsubscribe<SpawnAnimalRequestRejectedEvent>(OnSpawnRejected);
        }
    }
}
