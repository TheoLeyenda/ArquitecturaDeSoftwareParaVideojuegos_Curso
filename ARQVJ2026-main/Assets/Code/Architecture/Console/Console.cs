using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using ZooArchitect.Architecture.Logs.Events;

namespace ZooArchitect.Architecture.Logs
{
    public static class Console 
    {
        private static EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

        public static void Log(string message) 
        {
            EventBus.Raise<ConsoleLogEvent>(message);
        }

        public static void Warning(string message)
        {
            EventBus.Raise<ConsoleWarningEvent>(message);
        }

        public static void Error(string message)
        {
            EventBus.Raise<ConsoleErrorEvent>(message);

        }
    }
}
