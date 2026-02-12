using System;
using ZooArchitect.Architecture.Controllers;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.Logs;
using ZooArchitect.View.Mapping;

namespace ZooArchitect.View.Controller
{
    [ViewOf(typeof(SpawnJailControllerArchitecture))]
    internal sealed class SpawnJailControllerView : GroupSelectionControllerView 
    {

        public SpawnJailControllerView()
        {
            EventBus.Subscribe<SpawnJainRequestRejectedEvent>(OnSpawnJailRequestRejected);
        }

        public override void CreateController()
        {
            EventBus.Raise<SpawnJailRequestEvent>(StartGroupClickPosition, 
                FinishGroupClickPosition,
                EntitiesLogic.GetJailBlueprint());
        }

        private void OnSpawnJailRequestRejected(in SpawnJainRequestRejectedEvent _)
        {
            GameConsole.Log("Spawn jail rejected!");
        }

        public override void Dispose()
        {
            EventBus.Unsubscribe<SpawnJainRequestRejectedEvent>(OnSpawnJailRequestRejected);
        }
    }
}
