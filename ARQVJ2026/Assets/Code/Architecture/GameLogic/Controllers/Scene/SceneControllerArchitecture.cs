using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.Entities;

namespace ZooArchitect.Architecture.Controllers
{
    public sealed class SceneControllerArchitecture : IDisposable
    {
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
        private EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();


        public SceneControllerArchitecture()
        {
            EventBus.Subscribe<SceneActionRequestEvent>(OnSceneActionRequest);
            EventBus.Subscribe<SceneActionRequestAcceptedEvent>(OnSceneActionRecuestAccepted);
        }
        private void OnSceneActionRequest(in SceneActionRequestEvent sceneActionRequestEvent)
        {
            if(EntityRegistry[sceneActionRequestEvent.targetEntityId].ChechForPerfomableActions[sceneActionRequestEvent.methodName].Invoke())
                EventBus.Raise<SceneActionRequestAcceptedEvent>(sceneActionRequestEvent.targetEntityId, sceneActionRequestEvent.methodName);
            else
                EventBus.Raise<SceneActionRequestRejectedEvent>(sceneActionRequestEvent.targetEntityId, sceneActionRequestEvent.methodName,
                    $"Unable to perform action");
        }

        private void OnSceneActionRecuestAccepted(in SceneActionRequestAcceptedEvent sceneActionRequestAcceptedEvent)
        {
            EntityRegistry[sceneActionRequestAcceptedEvent.targetEntityId].PerfomableMethods[sceneActionRequestAcceptedEvent.methodName].Invoke();
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<SceneActionRequestEvent>(OnSceneActionRequest);
            EventBus.Unsubscribe<SceneActionRequestAcceptedEvent>(OnSceneActionRecuestAccepted);
        }
    }
}
