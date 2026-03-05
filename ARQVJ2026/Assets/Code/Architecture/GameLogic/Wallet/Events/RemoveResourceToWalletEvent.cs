using ImageCampus.ToolBox.Events;
using System;

namespace ZooArchitect.Architecture.GameLogic.Events
{
    public struct RemoveResourceToWalletEvent : IEvent
    {
        public string resourceName;
        public long amount;

        public void Assign(params object[] parameters)
        {
            resourceName = (string)parameters[0];
            amount = Convert.ToInt64(parameters[1]);
        }

        public void Reset()
        {
            resourceName = default(string);
            amount = default(long);
        }
    }
}
