using ZooArchitect.Architecture.Controllers;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.Logs;
using ZooArchitect.Architecture.Math;
using ZooArchitect.View.Mapping;

namespace ZooArchitect.View.Controller
{
    [ViewOf(typeof(SpawnJailControllerArchitecture))]
    internal sealed class SpawnJailControllerView : GroupSelectionControllerView
    {
        public SpawnJailControllerView()
        {
            EventBus.Subscribe<SpawnRequestRejectedEvent<Jail>>(OnSpawnJailRequestRejected);
            EventBus.Subscribe<SpawnRequestAcceptedEvent<Jail>>(OnSpawnJailRequestAccepted);
        }

        public override void CreateController()
        {
            EventBus.Raise<SpawnRequestEvent<Jail>>(
                EntitiesLogic.GetJailBlueprint(),
                new Coordinate(StartGroupClickPosition, FinishGroupClickPosition));
        }

        private void OnSpawnJailRequestRejected(in SpawnRequestRejectedEvent<Jail> spawnJainRequestRejectedEvent)
        {
            GameConsole.Log("Spawn jail rejected!");
            FeedbackFactory.SpawnNegativeFeedback(new UnityEngine.Vector3(spawnJainRequestRejectedEvent.coordiateToSpawn.End.x, spawnJainRequestRejectedEvent.coordiateToSpawn.End.y));
        }

        private void OnSpawnJailRequestAccepted(in SpawnRequestAcceptedEvent<Jail> spawnJailRequestAcceptedEvent)
        {
            FeedbackFactory.SpawnPositiveFeedback(new UnityEngine.Vector3(spawnJailRequestAcceptedEvent.coordinateToSpawn.End.x, spawnJailRequestAcceptedEvent.coordinateToSpawn.End.y));
        }

        public override void Dispose()
        {
            EventBus.Unsubscribe<SpawnRequestRejectedEvent<Jail>>(OnSpawnJailRequestRejected);
            EventBus.Unsubscribe<SpawnRequestAcceptedEvent<Jail>>(OnSpawnJailRequestAccepted);
        }
    }
}
