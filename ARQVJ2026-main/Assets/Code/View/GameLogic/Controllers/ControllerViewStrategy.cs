using ImageCampus.ToolBox.Dataflow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZooArchitect.View.Controller
{
    public sealed class ControllerViewStrategy : ITickable, IDisposable
    {
        private Dictionary<Type,ControllerView> controllers;

        private Type currentStrategyType;

        private IReadOnlyDictionary<KeyCode, Type> StrategyBinder { get; } =
        new Dictionary<KeyCode, Type>()
        {
            {KeyCode.Alpha1, typeof(SpawnEntityControllerView) },
            {KeyCode.Alpha2, typeof(TerrainModifierControllerView) },
        };

        public ControllerViewStrategy()
        {
            controllers = new Dictionary<Type, ControllerView>();
            currentStrategyType = typeof(SpawnEntityControllerView);
            controllers.Add(typeof(SpawnEntityControllerView), new SpawnEntityControllerView());
            controllers.Add(typeof(TerrainModifierControllerView), new TerrainModifierControllerView());
        }

        public void Tick(float deltaTime)
        {
            foreach (KeyValuePair<KeyCode, Type> strategyBinding in StrategyBinder)
            {
                if (Input.GetKeyDown(strategyBinding.Key))
                {
                    currentStrategyType = strategyBinding.Value;
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
