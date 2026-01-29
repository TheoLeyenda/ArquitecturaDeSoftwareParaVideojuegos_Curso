using ImageCampus.ToolBox.Scheduling;
using ImageCampus.ToolBox.Services;
using System.IO;
using UnityEngine;
using ZooArchitect.Architecture;
using ZooArchitect.View.Logs;
using ZooArchitect.View.Mapping;
using ZooArchitect.View.Resources;

namespace ZooArchitect.View
{
    [ViewOf(typeof(Gameplay))]
    public sealed class GameplayView : MonoBehaviour
    {
        private TaskScheduler TaskScheduler => ServiceProvider.Instance.GetService<TaskScheduler>();
        private GameScene GameScene => ServiceProvider.Instance.GetService<GameScene>();

        private string BluprintsPath => Path.Combine(Application.streamingAssetsPath, "Blueprints", "Blueprints.xlsx");

        private Gameplay gameplay;
        private ConsoleView consoleView;

        void Awake()
        {
            ViewArchitectureMap.Init();

            gameplay = new Gameplay(BluprintsPath);
            ServiceProvider.Instance.AddService<PrefabsRegistryView>(new PrefabsRegistryView());

            ServiceProvider.Instance.AddService<GameScene>
                (GameScene.AddSceneComponent<GameScene>("Scene", this.transform));

            consoleView = new ConsoleView();
        }

        private void Start()
        {
            gameplay.Init();
            GameScene.Init();


            gameplay.LateInit();
            GameScene.LateInit();
        }

        void Update()
        {
            gameplay.Tick(Time.deltaTime);
            GameScene.Tick(Time.deltaTime);
        }

        private void OnDisable()
        {
            gameplay.Dispose();
            GameScene.Dispose();
            consoleView.Dispose();
        }
    }
}
