using ImageCampus.ToolBox.Events;
using System;
using System.Collections.Generic;
using ZooArchitect.Architecture.Controllers;
using ZooArchitect.Architecture.Controllers.Events;
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
            EventBus.Subscribe<SpawnInfrastructureRequestRejectedEvent>(OnSpawnRejected);

        }
        protected override List<string> GetValidBlueprints(Point clickPoint)
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
                    EventBus.Raise<SpawnInfrastructureRequestEvent>(blueprints[index], clickPoint);
                });
            }
            return buildInfrastructures;
        }

        private void OnSpawnRejected(in SpawnInfrastructureRequestRejectedEvent spawnInfrastructureRequestRejectedEvent)
        {
            GameConsole.Warning($"Build of {spawnInfrastructureRequestRejectedEvent.blueprintToSpawn} in {spawnInfrastructureRequestRejectedEvent.pointToSpawn} rejected");
        }

        public override void Dispose()
        {
            EventBus.Unsubscribe<SpawnInfrastructureRequestRejectedEvent>(OnSpawnRejected);
        }
    }

}
