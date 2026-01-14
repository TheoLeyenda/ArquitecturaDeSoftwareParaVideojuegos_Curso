using UnityEngine;
using ZooArchitect.Architecture;

namespace View
{
    public sealed class GameplayView : MonoBehaviour
    {
        private Gameplay gameplay;

        void Start()
        {
            gameplay = new Gameplay();
            gameplay.Init();

        }

        void Update()
        {
            gameplay.Update(Time.deltaTime);
        }
    }
}
