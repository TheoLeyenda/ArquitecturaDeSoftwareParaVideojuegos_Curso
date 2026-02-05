using ImageCampus.ToolBox.Events;

namespace ZooArchitect.Architecture.GameLogic.Events
{
    public struct AddResourceToWlletEvent : IEvent
    {
        public string resourceName;
        public long amount;

        public void Assign(params object[] parameters)
        {
            resourceName = (string)parameters[0];
            amount = (long)parameters[1];
        }

        public void Reset()
        {
            resourceName = default(string);
            amount = default(long);
        }
    }
}
