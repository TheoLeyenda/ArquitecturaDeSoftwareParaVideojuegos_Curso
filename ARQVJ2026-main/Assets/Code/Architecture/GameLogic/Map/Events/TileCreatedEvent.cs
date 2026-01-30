using ImageCampus.ToolBox.Events;

namespace ZooArchitect.Architecture.GameLogic.Events
{
    public struct TileCreatedEvent : IEvent
    {
        public int tileId;
        public int xCoord;
        public int yCoord;

        public void Assign(params object[] parameters)
        {
            tileId = (int)parameters[0];
            xCoord = (int)parameters[1];
            yCoord = (int)parameters[2];
        }

        public void Reset()
        {
            tileId = 0;
            xCoord = -1;
            yCoord = -1;
        }
    }
}
