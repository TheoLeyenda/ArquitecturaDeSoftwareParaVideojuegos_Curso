using ImageCampus.ToolBox.Dataflow;
using ImageCampus.ToolBox.Scheduling;
using ImageCampus.ToolBox.Services;
using System.IO;
using UnityEngine;
using ZooArchitect.Architecture;
using ZooArchitect.View.Logs;

namespace ZooArchitect.View
{
    public sealed class GameplayView : MonoBehaviour
    {
        private TaskScheduler TaskScheduler => ServiceProvider.Instance.GetService<TaskScheduler>();

        private string BluprintsPath => Path.Combine(Application.streamingAssetsPath, "Blueprints", "Blueprints.xlsx");

        private Gameplay gameplay;
        private ConsoleView consoleView;

        void Awake()
        {
            gameplay = new Gameplay(BluprintsPath);
            consoleView = new ConsoleView();
        }

        private void Start()
        {
            gameplay.Init();
            gameplay.LateInit();
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

    internal class ViewComponent : MonoBehaviour, IInitable, IUpdateable
    {
        public virtual void Init() { }

        public virtual void LateInit() { }

        public virtual void Update(float deltaTime) { }
    }

}
