using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using ZooArchitect.Architecture.Controllers.Events;

namespace ZooArchitect.Architecture.Controllers
{
    public sealed class TerrainModifierControllerArchitecture : IDisposable
    {
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

        public TerrainModifierControllerArchitecture()
        {

            EventBus.Subscribe<ModifyTerrainRequestEvent>(OnModifyTerrainRequest);
        }

        private void OnModifyTerrainRequest(in ModifyTerrainRequestEvent modifyTerrainRequestEvent)
        {
            EventBus.Raise<ModifyTerrainRecuestAceptedEvent>(
                modifyTerrainRequestEvent.origin, 
                modifyTerrainRequestEvent.end, 
                modifyTerrainRequestEvent.newTileId);
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<ModifyTerrainRequestEvent>(OnModifyTerrainRequest);
        }
    }
}
