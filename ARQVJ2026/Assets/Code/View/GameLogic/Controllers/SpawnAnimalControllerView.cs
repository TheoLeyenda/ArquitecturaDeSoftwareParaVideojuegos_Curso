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
            EventBus.Subscribe<SpawnRequestRejectedEvent<Animal>>(OnSpawnRejected);
            EventBus.Subscribe<SpawnRequestAcceptedEvent<Animal>>(OnSpawnAccepted);
        }

		protected override List<string> GetValidOptions(Point clickPoint)
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
                    EventBus.Raise<SpawnRequestEvent<Animal>>(blueprints[index], new Coordinate(clickPoint));
                });
            }
            return spawnEntities;
        }

        private void OnSpawnRejected(in SpawnRequestRejectedEvent<Animal> spawnEntityRequestRejectedEvent)
        {
            GameConsole.Warning($"Spawn of {spawnEntityRequestRejectedEvent.blueprintToSpawn} in {spawnEntityRequestRejectedEvent.coordiateToSpawn.Origin} rejected" +
               (string.IsNullOrEmpty(spawnEntityRequestRejectedEvent.message) ? string.Empty : "\n" + spawnEntityRequestRejectedEvent.message));
            FeedbackFactory.SpawnNegativeFeedback(new UnityEngine.Vector3(spawnEntityRequestRejectedEvent.coordiateToSpawn.Origin.x,
                spawnEntityRequestRejectedEvent.coordiateToSpawn.Origin.y));
        }

        private void OnSpawnAccepted(in SpawnRequestAcceptedEvent<Animal> spawnAnimalRequestAcceptedEvent)
        {
            FeedbackFactory.SpawnPositiveFeedback(new UnityEngine.Vector3(spawnAnimalRequestAcceptedEvent.coordinateToSpawn.Origin.x,
                spawnAnimalRequestAcceptedEvent.coordinateToSpawn.Origin.y));
        }

        public override void Dispose()
        {
            EventBus.Unsubscribe<SpawnRequestRejectedEvent<Animal>>(OnSpawnRejected);
            EventBus.Unsubscribe<SpawnRequestAcceptedEvent<Animal>>(OnSpawnAccepted);
        }
    }
}
