using ImageCampus.ToolBox.Events;

namespace ZooArchitect.Architecture.Controllers.Events
{
    public struct BuyItemRequestRejectedEvent : IEvent
    {
        private string buyItemName;
        private string message;

        public void Assign(params object[] parameters)
        {
            buyItemName = (string)parameters[0];
            if (parameters.Length >= 1)
                message = (string)parameters[1];
        }

        public void Reset()
        {
            buyItemName = default(string);
            message = default(string);
        }
    }
}
