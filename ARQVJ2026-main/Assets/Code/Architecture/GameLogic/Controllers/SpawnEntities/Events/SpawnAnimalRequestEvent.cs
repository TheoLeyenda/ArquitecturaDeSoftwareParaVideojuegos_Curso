using ImageCampus.ToolBox.Events;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Controllers.Events
{
    public struct SpawnAnimalRequestEvent : IEvent
    {
        public string blueprintToSpawn;
        public Point pointToSpawn;

        public void Assign(params object[] parameters)
        {
            blueprintToSpawn = (string)parameters[0];
            pointToSpawn = (Point)parameters[1];
        }

        public void Reset()
        {
            blueprintToSpawn = string.Empty;
            pointToSpawn = default(Point);
        }
    }
}
