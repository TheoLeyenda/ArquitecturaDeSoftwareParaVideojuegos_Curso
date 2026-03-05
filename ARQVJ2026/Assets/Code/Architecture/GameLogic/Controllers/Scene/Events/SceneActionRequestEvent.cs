using ImageCampus.ToolBox.Events;

namespace ZooArchitect.Architecture.Controllers.Events
{
    public struct SceneActionRequestEvent : IEvent
    {
        public uint targetEntityId;
        public string methodName;

        public void Assign(params object[] parameters)
        {
            targetEntityId = (uint)parameters[0];
            methodName = (string)parameters[1];
        }

        public void Reset()
        {
            targetEntityId = default(uint);
            methodName = default(string);
        }
    }
}
