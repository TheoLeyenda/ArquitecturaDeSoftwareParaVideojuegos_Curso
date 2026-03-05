using ImageCampus.ToolBox.Events;

namespace ZooArchitect.Architecture.GameLogic.Events
{
    public struct TileModifiedEvent : IEvent
    {
        public int newTileId;
        public int xCoord;
        public int yCoord;

        public void Assign(params object[] parameters)
        {
            newTileId = (int)parameters[0];
            xCoord = (int)parameters[1];
            yCoord = (int)parameters[2];
        }

        public void Reset()
        {
            newTileId = 0;
            xCoord = -1;
            yCoord = -1;
        }
    }
}
