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
    [ViewOf(typeof(SpawnInfrastructureControllerArchitecture))]
    internal sealed class SpawnInfrastructureControllerView : SingleSelectionControllerView
    {
        public SpawnInfrastructureControllerView()
        {
            EventBus.Subscribe<SpawnRequestRejectedEvent<Infrastructure>>(OnSpawnRejected);
            EventBus.Subscribe<SpawnRequestAcceptedEvent<Infrastructure>>(OnSpawnAccepted);

        }
        protected override List<string> GetValidOptions(Point clickPoint)
        {
            return EntitiesLogic.ValidInfrastructuresToSpawnIn(clickPoint);
        }

        protected override Dictionary<string, Action> GetActionsToDisplay(Point clickPoint, List<string> blueprints)
        {
            Dictionary<string, Action> buildInfrastructures = new Dictionary<string, Action>();
            for (int i = 0; i < blueprints.Count; i++)
            {
                int index = i;
                buildInfrastructures.Add($"Build {blueprints[index]}", () =>
                {
                    EventBus.Raise<SpawnRequestEvent<Infrastructure>>(blueprints[index], new Coordinate(clickPoint));
                });
            }
            return buildInfrastructures;
        }

        private void OnSpawnRejected(in SpawnRequestRejectedEvent<Infrastructure> spawnInfrastructureRequestRejectedEvent)
        {
            GameConsole.Warning($"Build of {spawnInfrastructureRequestRejectedEvent.blueprintToSpawn} in {spawnInfrastructureRequestRejectedEvent.coordiateToSpawn.Origin} rejected");
            FeedbackFactory.SpawnNegativeFeedback(new UnityEngine.Vector3(spawnInfrastructureRequestRejectedEvent.coordiateToSpawn.Origin.x,
            spawnInfrastructureRequestRejectedEvent.coordiateToSpawn.Origin.y));
        }

        private void OnSpawnAccepted(in SpawnRequestAcceptedEvent<Infrastructure> spawnInfrastructureRequestAcceptedEvent)
        {
            FeedbackFactory.SpawnPositiveFeedback(new UnityEngine.Vector3(spawnInfrastructureRequestAcceptedEvent.coordinateToSpawn.Origin.x,
                spawnInfrastructureRequestAcceptedEvent.coordinateToSpawn.Origin.y));
        }

        public override void Dispose()
        {
            EventBus.Unsubscribe<SpawnRequestRejectedEvent<Infrastructure>>(OnSpawnRejected);
            EventBus.Unsubscribe<SpawnRequestAcceptedEvent<Infrastructure>>(OnSpawnAccepted);
        }
    }

}
