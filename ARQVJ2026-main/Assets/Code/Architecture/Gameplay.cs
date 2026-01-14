using TheoLeyenda.ToolBox.Updateable;
using TheoLeyenda.ToolBox.EventBus;
using TheoLeyenda.ToolBox.ServiceProvider;

namespace ZooArchitect.Architecture
{
    public sealed class Gameplay : IUpdateable
    {
        public EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

        public Gameplay()
        {
            ServiceProvider.Instance.AddService<EventBus>(new EventBus());
        }

        public void Init() {}

        public void Update(float deltaTime){}
    }
}