using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using ZooArchitect.Architecture.Logs.Events;

namespace ZooArchitect.View.Logs
{
    public sealed class ConsoleView : IDisposable
    {
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
        public ConsoleView()
        {
            EventBus.Subscribe<ConsoleLogEvent>(LogMessage);
            EventBus.Subscribe<ConsoleWarningEvent>(LogWarning);
            EventBus.Subscribe<ConsoleErrorEvent>(LogError);
        }

        private void LogMessage(ConsoleLogEvent consoleLogEvent)
        {
            UnityEngine.Debug.Log(consoleLogEvent.message);
        }

        private void LogWarning(ConsoleWarningEvent consoleWarningEvent)
        {
            UnityEngine.Debug.LogWarning(consoleWarningEvent.message);
        }

        private void LogError(ConsoleErrorEvent consoleErrorEvent)
        {
            UnityEngine.Debug.LogError(consoleErrorEvent.message);
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<ConsoleLogEvent>(LogMessage);
            EventBus.Unsubscribe<ConsoleWarningEvent>(LogWarning);
            EventBus.Unsubscribe<ConsoleErrorEvent>(LogError);
        }
    }
}
