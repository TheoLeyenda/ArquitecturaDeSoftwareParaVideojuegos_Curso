using ImageCampus.ToolBox.Dataflow;
using UnityEngine;

namespace ZooArchitect.View
{
    internal class ViewComponent : MonoBehaviour, IInitable, ITickable
    {
        public virtual void Init() { }

        public virtual void LateInit() { }

        public virtual void Tick(float deltaTime) { }
    }

}
