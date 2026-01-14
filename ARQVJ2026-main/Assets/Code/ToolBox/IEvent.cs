using TheoLeyenda.ToolBox.Resetteable;

namespace TheoLeyenda.ToolBox.Events
{
    public interface IEvent : IRessetteable
    {
    }

    public struct GameInitializedEvent : IEvent
    {
        public void Assign(params object[] parameters)
        {
        }

        public void Reset()
        {
        }
    }
}
