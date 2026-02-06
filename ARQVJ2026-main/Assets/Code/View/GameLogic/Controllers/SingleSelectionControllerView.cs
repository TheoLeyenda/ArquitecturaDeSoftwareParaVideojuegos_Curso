using UnityEngine;

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
    }
}
