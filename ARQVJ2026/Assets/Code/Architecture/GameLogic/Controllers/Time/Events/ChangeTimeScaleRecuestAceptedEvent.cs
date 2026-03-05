using ImageCampus.ToolBox.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZooArchitect.Architecture.Controllers.Events
{
    public struct ChangeTimeScaleRequestAcceptedEvent : IEvent
    {
        public void Assign(params object[] parameters)
        {
        }

        public void Reset()
        {
        }
    }
}
