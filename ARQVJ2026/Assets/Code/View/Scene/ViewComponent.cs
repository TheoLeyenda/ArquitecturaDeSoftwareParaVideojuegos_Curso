using ImageCampus.ToolBox.Dataflow;
using System;
using UnityEngine;

namespace ZooArchitect.View.Scene
{
    internal abstract class ViewComponent : MonoBehaviour, IInitable, ITickable, IDisposable
    {
        public virtual void Init() { }

        public virtual void Init(params object[] parameters) 
        {
            Init();
        }

        public virtual void LateInit() { }

        public virtual void Tick(float deltaTime) { }

        public virtual void Dispose() { }

    }

}
