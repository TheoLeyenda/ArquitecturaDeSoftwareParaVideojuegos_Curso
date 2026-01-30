using ImageCampus.ToolBox.Dataflow;
using ImageCampus.ToolBox.Services;
using System;
using ZooArchitect.Architecture.Controllers;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.GameLogic;

namespace ZooArchitect.Architecture
{
    public sealed class Scene : IInitable, ITickable, IDisposable
    {
        private SpawnEntityControllerArchitecture spawnEntityControllerArchitecture;

        private Map map;

        public void Init()
        {
            ServiceProvider.Instance.AddService<Time>(new Time());
            ServiceProvider.Instance.AddService<DayNightCycle>(new DayNightCycle());
            ServiceProvider.Instance.AddService<Wallet>(new Wallet());
            ServiceProvider.Instance.AddService<EntityRegistry>(new EntityRegistry());
            ServiceProvider.Instance.AddService<EntityFactory>(new EntityFactory());
        }

        public void LateInit()
        {
            map = new Map(10, 10);
            spawnEntityControllerArchitecture = new SpawnEntityControllerArchitecture();
        }

        public void Tick(float deltaTime)
        {
        }

        public void Dispose()
        {
            spawnEntityControllerArchitecture.Dispose();
        }
    }
}