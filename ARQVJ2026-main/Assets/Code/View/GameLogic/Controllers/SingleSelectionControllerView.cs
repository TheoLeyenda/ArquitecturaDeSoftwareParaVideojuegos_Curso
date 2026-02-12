using System;
using System.Collections.Generic;
using UnityEngine;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.View.Controller
{
    internal abstract class SingleSelectionControllerView : ControllerView
    {
        public override void Tick(float deltaTime)
        {
            if (Input.GetMouseButtonDown(1))
            {
                CreateController();
            }
        }

        public override void CreateController()
        {
            Point clickPoint = GameScene.GetMouseGridCoordinate();
            List<string> blueprints = GetValidBlueprints(clickPoint);
            if (blueprints.Count == 0)
                return;

            Display(GetActionsToDisplay(clickPoint, blueprints));
        }

        protected abstract List<string> GetValidBlueprints(Point clickPoint);
        protected abstract Dictionary<string, Action> GetActionsToDisplay(Point clickPoint, List<string> blueprints);
    }
}
