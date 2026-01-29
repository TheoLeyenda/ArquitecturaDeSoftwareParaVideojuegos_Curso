using ImageCampus.ToolBox.Scheduling;
using ImageCampus.ToolBox.Services;
using System.IO;
using UnityEngine;
using ZooArchitect.Architecture;
using ZooArchitect.View.Controller;
using ZooArchitect.View.Entities;
using ZooArchitect.View.Logs;
using ZooArchitect.View.Mapping;
using ZooArchitect.View.Resources;

namespace ZooArchitect.View
{
    [ViewOf(typeof(Gameplay))]
    public sealed class GameplayView : MonoBehaviour
    {
        private TaskScheduler TaskScheduler => ServiceProvider.Instance.GetService<TaskScheduler>();

        private string BluprintsPath => Path.Combine(Application.streamingAssetsPath, "Blueprints", "Blueprints.xlsx");

        private Gameplay gameplay;
        private ConsoleView consoleView;
        private EntityFactoryView entityFactoryView;
        private SpawnEntityControllerView spawnEntityControllerView;
        void Awake()
        {
            ViewArchitectureMap.Init();

            gameplay = new Gameplay(BluprintsPath);

            ServiceProvider.Instance.AddService<EntityRegistryView>(new EntityRegistryView());
            ServiceProvider.Instance.AddService<PrefabsRegistryView>(new PrefabsRegistryView());
            entityFactoryView = new EntityFactoryView();

            consoleView = new ConsoleView();
        }

        private void Start()
        {
            gameplay.Init();
            
            
            gameplay.LateInit();
            spawnEntityControllerView = new SpawnEntityControllerView();
        }

        void Update()
        {
            gameplay.Tick(Time.deltaTime);
            spawnEntityControllerView.Tick(Time.deltaTime);
        }

        private void OnDisable()
        {
            gameplay.Dispose();
            consoleView.Dispose();
            spawnEntityControllerView.Dispose();
        }
    }
}
