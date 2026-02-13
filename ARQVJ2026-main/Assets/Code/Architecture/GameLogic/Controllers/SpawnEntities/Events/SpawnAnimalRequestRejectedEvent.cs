using ImageCampus.ToolBox.Events;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Controllers.Events
{
    public struct SpawnAnimalRequestRejectedEvent : IEvent
    {
        public string blueprintToSpawn;
        public Point pointToSpawn;
        public string message;
        public void Assign(params object[] parameters)
        {
            blueprintToSpawn = (string)parameters[0];
            pointToSpawn = (Point)parameters[1];

            if (parameters.Length > 2)
                message = (string)parameters[2];
        }

        public void Reset()
        {
            blueprintToSpawn = string.Empty;
            pointToSpawn = default(Point);
            message = default(string);
        }
    }
}
