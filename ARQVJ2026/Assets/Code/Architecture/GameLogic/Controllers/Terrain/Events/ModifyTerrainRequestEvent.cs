using ImageCampus.ToolBox.Events;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Controllers.Events
{
    public struct ModifyTerrainRequestEvent : IEvent
    {
        public Point origin;
        public Point end;
        public string newTileId;

        public void Assign(params object[] parameters)
        {
            origin = (Point)parameters[0];
            end = (Point)parameters[1];
            newTileId = (string)parameters[2];
        }

        public void Reset()
        {
            origin = default(Point);
            end = default(Point);
            newTileId = default(string);
        }
    }
}
