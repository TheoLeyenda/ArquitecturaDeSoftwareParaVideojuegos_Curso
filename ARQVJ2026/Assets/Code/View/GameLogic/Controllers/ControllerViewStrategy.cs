using ImageCampus.ToolBox.Dataflow;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZooArchitect.View.Controller
{
    public sealed class ControllerViewStrategy : ITickable, IDisposable
    {
        private Dictionary<Type, ControllerView> controllers;

        private Type currentStrategyType;

        private IReadOnlyDictionary<KeyCode, Type> StrategyBinder { get; } =
        new Dictionary<KeyCode, Type>()
        {
            {KeyCode.Alpha1, typeof(SpawnAnimalControllerView) },
            {KeyCode.Alpha2, typeof(SpawnInfrastructureControllerView) },
            {KeyCode.Alpha3, typeof(TerrainModifierControllerView) },
            {KeyCode.Alpha4, typeof(SpawnJailControllerView) },
            {KeyCode.I, typeof(BuyControllerView)},
            {KeyCode.S, typeof(SceneControllerView) }
        };

        public ControllerViewStrategy()
        {
            controllers = new Dictionary<Type, ControllerView>();
            currentStrategyType = typeof(SpawnAnimalControllerView);
            controllers.Add(typeof(SpawnAnimalControllerView), new SpawnAnimalControllerView());
            controllers.Add(typeof(SpawnInfrastructureControllerView), new SpawnInfrastructureControllerView());
            controllers.Add(typeof(SpawnJailControllerView), new SpawnJailControllerView());
            controllers.Add(typeof(TerrainModifierControllerView), new TerrainModifierControllerView());
            controllers.Add(typeof(BuyControllerView), new BuyControllerView());
            controllers.Add(typeof(SceneControllerView), new SceneControllerView());
        }

        public void Tick(float deltaTime)
        {
            foreach (KeyValuePair<KeyCode, Type> strategyBinding in StrategyBinder)
            {
                if (Input.GetKeyDown(strategyBinding.Key))
                {
                    currentStrategyType = strategyBinding.Value;
                    controllers[currentStrategyType].OnSelect();
                }
            }

            controllers[currentStrategyType].Tick(deltaTime);
        }

        public void Dispose()
        {
            foreach (KeyValuePair<Type, ControllerView> controller in controllers)
            {
                controller.Value.Dispose();
            }
        }
    }
}
