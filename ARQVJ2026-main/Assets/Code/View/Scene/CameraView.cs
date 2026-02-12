using UnityEngine;

namespace ZooArchitect.View.Scene
{
    internal sealed class CameraView : ViewComponent
    {
#if MOUSE_CAMERA_CONTROLS
        private const bool MOUSE_CONTROLS = true;
#else
        private const bool MOUSE_CONTROLS = false;
#endif

        private const float MOVE_SPEED = 10.0f;
        private const float SMOOTH_TIME = 0.15f;
        private const float EDGE_SIZE = 20.0f;

        private Vector3 velocity;
        private Vector3 targetPosition;

        private Camera gameCamera;

        public Camera GameCamera => gameCamera;

        public override void Init(params object[] parameters)
        {
            base.Init(parameters);
            gameCamera = parameters[0] as Camera;
            GameCamera.transform.position = Vector3.back;
        }

        public override void Tick(float deltaTime)
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 diretion = Vector3.zero;

            if (Input.GetKey(KeyCode.LeftArrow) || (MOUSE_CONTROLS && mousePosition.x <= EDGE_SIZE))
                diretion.x = -1.0f;
            else if (Input.GetKey(KeyCode.RightArrow) || (MOUSE_CONTROLS && mousePosition.x >= (Screen.width - EDGE_SIZE)))
                diretion.x = 1.0f;

            if (Input.GetKey(KeyCode.DownArrow) || (MOUSE_CONTROLS && mousePosition.y <= EDGE_SIZE))
                diretion.y = -1.0f;
            else if (Input.GetKey(KeyCode.UpArrow) || (MOUSE_CONTROLS && mousePosition.y >= (Screen.height - EDGE_SIZE)))
                diretion.y = 1.0f;

            if (diretion.sqrMagnitude > 0.0f)
            {
                diretion.Normalize();
                targetPosition += diretion * MOVE_SPEED * deltaTime;
            }

            targetPosition.z = -1.0f;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, SMOOTH_TIME);
        }
    }
}
