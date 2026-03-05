using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZooArchitect.Architecture.Controllers.Events;

namespace ZooArchitect.Architecture.Controllers
{
    public sealed class TimeScaleController : IDisposable
    {
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

        public TimeScaleController()
        {
            EventBus.Subscribe<ChangeTimeScaleRequestEvent>(OnChangeTimeScaleRequest);
        }

        private void OnChangeTimeScaleRequest(in ChangeTimeScaleRequestEvent changeTimeScaleRequestEvent)
        {
            EventBus.Raise<ChangeTimeScaleRequestAcceptedEvent>();
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<ChangeTimeScaleRequestEvent>(OnChangeTimeScaleRequest);
        }
    }
}
