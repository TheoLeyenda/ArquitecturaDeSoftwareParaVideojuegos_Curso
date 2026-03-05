using ImageCampus.ToolBox.Events;

namespace ZooArchitect.Architecture.Controllers.Events
{
    public struct ChangeTimeScaleRequestRejectedEvent : IEvent
    {
        public void Assign(params object[] parameters)
        {
        }

        public void Reset()
        {
        }
    }
}
