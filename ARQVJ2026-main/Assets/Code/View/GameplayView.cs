using Architecture;
using UnityEngine;

namespace View
{
    public sealed class GameplayView : MonoBehaviour
    {
        private Gameplay gameplay;
        void Start()
        {
            gameplay = new Gameplay();
        }

        void Update()
        {
            gameplay.Update(Time.deltaTime);
            Debug.Log(gameplay.Data);
        }
    }
}
