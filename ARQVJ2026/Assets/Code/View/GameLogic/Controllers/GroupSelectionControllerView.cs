using UnityEngine;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.View.Controller
{
    internal abstract class GroupSelectionControllerView : ControllerView
    {
        private Point startGroupClickPoint;
        private Point finishGroupClickPoint;

        protected Point StartGroupClickPosition => startGroupClickPoint;
        protected Point FinishGroupClickPosition => finishGroupClickPoint;

        public override void Tick(float deltaTime)
        {
            if (Input.GetMouseButtonDown(1))
            {
                startGroupClickPoint = GameScene.GetMouseGridCoordinate();
            }

            if (Input.GetMouseButtonUp(1))
            {
                finishGroupClickPoint = GameScene.GetMouseGridCoordinate();
                CreateController();
            }
        }
    }
}
