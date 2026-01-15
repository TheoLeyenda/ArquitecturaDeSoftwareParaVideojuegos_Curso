using ImageCampus.ToolBox.Scheduling;
using ImageCampus.ToolBox.Services;
using UnityEngine;
using ZooArchitect.Architecture;
using ZooArchitect.View.Logs;

namespace ZooArchitect.View
{
    public sealed class GameplayView : MonoBehaviour
    {
        private TaskScheduler TaskScheduler => ServiceProvider.Instance.GetService<TaskScheduler>();

        private Gameplay gameplay;
        private ConsoleView consoleView;
        void Start()
        {
            gameplay = new Gameplay();
            consoleView = new ConsoleView();
        }

        void Update()
        {
            gameplay.Update(Time.deltaTime);
            
        }

        private void OnDisable()
        {
            consoleView.Dispose();
        }
    }
}
