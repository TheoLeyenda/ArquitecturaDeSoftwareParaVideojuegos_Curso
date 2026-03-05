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
            List<string> options = GetValidOptions(clickPoint);
            if (options.Count == 0)
                return;

            Display(GetActionsToDisplay(clickPoint, options));
        }

        protected abstract List<string> GetValidOptions(Point clickPoint);
        protected abstract Dictionary<string, Action> GetActionsToDisplay(Point clickPoint, List<string> blueprints);
    }
}
