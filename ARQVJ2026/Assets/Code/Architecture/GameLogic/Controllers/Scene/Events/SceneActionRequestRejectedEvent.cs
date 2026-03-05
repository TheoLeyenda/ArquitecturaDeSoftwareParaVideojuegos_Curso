using ImageCampus.ToolBox.Events;

namespace ZooArchitect.Architecture.Controllers.Events
{
    public struct SceneActionRequestRejectedEvent : IEvent
    {
        public uint targetEntityId;
        public string methodName;
        public string message;
        public void Assign(params object[] parameters)
        {
            targetEntityId = (uint)parameters[0];
            methodName = (string)parameters[1];
            if (parameters.Length > 2)
                message = (string)parameters[2];
        }

        public void Reset()
        {
            targetEntityId = default(uint);
            methodName = default(string);
            message = default(string);
        }
    }
}
