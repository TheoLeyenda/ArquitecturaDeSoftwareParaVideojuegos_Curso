using System;
using System.Collections.Generic;
using TheoLeyenda.ToolBox.Updateable;
using TheoLeyenda.ToolBox.Service;

namespace TheoLeyenda.ToolBox.Scheduling 
{
    public sealed class TaskScheduler : IService, IUpdateable
    {
        private sealed class ScheduledCall
        {
            public readonly Action callback;
            public float remainingTime;

            public ScheduledCall(Action callback, float remainingTime)
            {
                this.callback = callback;
                this.remainingTime = remainingTime;
            }
        }

        private readonly List<ScheduledCall> scheduledCalls;

        public bool IsPersistance => false;

        public TaskScheduler() 
        {
            this.scheduledCalls = new List<ScheduledCall>();
        }

        public void Schedule(Action callback, float remainingTime)
        {
            scheduledCalls.Add(new ScheduledCall(callback, remainingTime));
        }

        public void Update(float deltaTime) 
        {
            for (int i = scheduledCalls.Count - 1; i >= 0; i--) 
            {
                ScheduledCall call = scheduledCalls[i];
                call.remainingTime -= deltaTime;

                if (call.remainingTime <= 0) 
                {
                    scheduledCalls.RemoveAt(i);
                    call.callback.Invoke();
                }
            }
        }
    }   
}