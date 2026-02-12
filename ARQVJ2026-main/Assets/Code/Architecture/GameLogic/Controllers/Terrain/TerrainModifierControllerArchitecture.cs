using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Controllers
{
    public sealed class TerrainModifierControllerArchitecture : IDisposable
    {
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
        private EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();
        private Scene Scene => ServiceProvider.Instance.GetService<Scene>();

        public TerrainModifierControllerArchitecture()
        {
            EventBus.Subscribe<ModifyTerrainRequestEvent>(OnModifyTerrainRequest);
            EventBus.Subscribe<SpawnJailRequestAcceptedEvent>(OnSpawnJailAccepted);
        }

        private void OnSpawnJailAccepted(in SpawnJailRequestAcceptedEvent spawnJailRequestAcceptedEvent)
        {
            Coordinate newJailCoordinate = new Coordinate(spawnJailRequestAcceptedEvent.origin, spawnJailRequestAcceptedEvent.end);

            foreach (Point perimeterPoint in newJailCoordinate.Perimeter)
            {
                EventBus.Raise<ModifyTerrainRecuestAceptedEvent>(
                perimeterPoint,
                perimeterPoint,
                Scene.HabitatWallTileDefinition);
            }

            foreach (Point innerPoint in newJailCoordinate.Inner)
            {
                EventBus.Raise<ModifyTerrainRecuestAceptedEvent>(
                innerPoint,
                innerPoint,
                Scene.HabitatTileDefinition);
            }
        }

        private void OnModifyTerrainRequest(in ModifyTerrainRequestEvent modifyTerrainRequestEvent)
        {
            Coordinate tentativeModificationCoordinate = new Coordinate(modifyTerrainRequestEvent.origin, modifyTerrainRequestEvent.end);

            foreach (Jail jail in EntityRegistry.Jails)
            {
                if (jail.coordinate.Overlaps(tentativeModificationCoordinate))
                {
                    EventBus.Raise<ModifyTerrainRecuestRejectedEvent>(
                        modifyTerrainRequestEvent.origin,
                        modifyTerrainRequestEvent.end,
                        modifyTerrainRequestEvent.newTileId);
                    return;
                }
            }

            EventBus.Raise<ModifyTerrainRecuestAceptedEvent>(
                modifyTerrainRequestEvent.origin,
                modifyTerrainRequestEvent.end,
                modifyTerrainRequestEvent.newTileId);
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<ModifyTerrainRequestEvent>(OnModifyTerrainRequest);
            EventBus.Unsubscribe<SpawnJailRequestAcceptedEvent>(OnSpawnJailAccepted);
        }
    }
}
