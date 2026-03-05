using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using UnityEngine;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.Logs;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.View.Controller
{
    internal class SceneControllerView : ControllerView
    {
        private Architecture.Scene Scene => ServiceProvider.Instance.GetService<Architecture.Scene>();
        private EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();


        public SceneControllerView()
        {
            EventBus.Subscribe<SceneActionRequestRejectedEvent>(OnSceneRequestRejected); 
        }

        public override void Tick(float deltaTime)
        {
            if (Input.GetMouseButtonDown(1))
            {
                CreateController();
            }
        }

        public override void CreateController()
        {
            Coordinate clickCoordiante = new Coordinate(GameScene.GetMouseGridCoordinate());
            List<uint> entityIdsInPoint = Scene.GetEntitiesIn(in clickCoordiante);

            Dictionary<string, Action> actions = new Dictionary<string, Action>();

            foreach (uint id in entityIdsInPoint)
            {
                if (EntityRegistry[id].PerformableActions != null && EntityRegistry[id].PerformableActions.Count > 0)
                {
                    foreach (string performableAction in EntityRegistry[id].PerformableActions)
                    {
                        actions.Add(performableAction, () =>
                        {
                            EventBus.Raise<SceneActionRequestEvent>(id, performableAction);
                        });
                    }

                    Display(actions);
                    break;
                }
            }

        }

        private void OnSceneRequestRejected(in SceneActionRequestRejectedEvent sceneActionRequestRejectedEvent)
        {
            GameConsole.Log("Scene action rejected due: " + sceneActionRequestRejectedEvent.message);
        }

        public override void Dispose()
        {
            EventBus.Unsubscribe<SceneActionRequestRejectedEvent>(OnSceneRequestRejected);
        }
    }
}
