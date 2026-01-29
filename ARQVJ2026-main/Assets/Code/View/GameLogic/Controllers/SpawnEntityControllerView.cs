using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Dataflow;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using UnityEngine;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.Data;
using ZooArchitect.Architecture.Logs;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.View.Controller
{
    public sealed class SpawnEntityControllerView : ITickable, IDisposable
    {
        private List<KeyCode> keys = new List<KeyCode>()
        {
            KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5
        };

        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
        private BlueprintRegistry BlueprintRegistry => ServiceProvider.Instance.GetService<BlueprintRegistry>();

        private List<string> animalsBlueprints;

        public SpawnEntityControllerView()
        {
            animalsBlueprints = BlueprintRegistry.BlueprintsOf(TableNames.ANIMALS_TABLE_NAME);
            EventBus.Subscribe<SpawnEntityRequestRejectedEvent>(OnSpawnRejected);
        }


        public void Tick(float deltaTime)
        {
            for (int i = 0; i < animalsBlueprints.Count; i++)
            {
                if (Input.GetKeyDown(keys[i]))
                {
                    EventBus.Raise<SpawnEntityRequestEvent>(animalsBlueprints[i], new Coordinate(new Point(i, i)));
                }
            }
        }
        private void OnSpawnRejected(in SpawnEntityRequestRejectedEvent spawnEntityRequestRejectedEvent)
        {
            GameConsole.Warning($"Spawn of {spawnEntityRequestRejectedEvent.blueprintToSpawn} in {spawnEntityRequestRejectedEvent.coordinateToSpawn} rejected");
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<SpawnEntityRequestRejectedEvent>(OnSpawnRejected);
        }
    }
}
