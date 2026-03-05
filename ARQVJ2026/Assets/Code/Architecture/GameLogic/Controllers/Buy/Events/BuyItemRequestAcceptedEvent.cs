using ImageCampus.ToolBox.Events;

namespace ZooArchitect.Architecture.Controllers.Events
{
    public struct BuyItemRequestAcceptedEvent : IEvent
    {
        public string buyItemName;

        public void Assign(params object[] parameters)
        {
            buyItemName = (string)parameters[0];
        }

        public void Reset()
        {
            buyItemName = default(string);
        }
    }
}
