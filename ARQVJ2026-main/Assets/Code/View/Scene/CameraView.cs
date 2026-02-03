using UnityEngine;

namespace ZooArchitect.View.Scene
{
    internal sealed class CameraView : ViewComponent
    {
        private Camera gameCamera;

        public Camera GameCamera  => gameCamera;

        public override void Init(params object[] parameters)
        {
            base.Init(parameters);
            gameCamera = parameters[0] as Camera;
        }
    }
}
