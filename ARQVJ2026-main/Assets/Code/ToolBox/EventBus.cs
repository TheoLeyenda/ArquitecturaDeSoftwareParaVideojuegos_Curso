using System;
using System.Collections.Generic;
using TheoLeyenda.ToolBox.Events;
using TheoLeyenda.ToolBox.Service;
using TheoLeyenda.ToolBox.Pool;

namespace TheoLeyenda.ToolBox.EventBus
{
    public sealed class EventBus : IService 
    {
        private readonly Dictionary<Type, List<Delegate>> subscribers = new Dictionary<Type, List<Delegate>>();

        public bool IsPersistance => false;

        private readonly ConcurrentPool eventPool = new ConcurrentPool();
        public void Subscribe<EventType>(Action<EventType> callback) where EventType : struct, IEvent 
        {
            Type eventType = typeof(EventType);
            if (!subscribers.ContainsKey(eventType)) 
            {
                subscribers.Add(eventType, new List<Delegate>());
            }
            subscribers[eventType].Add(callback);
        }

        public void Unsubscribe<EventType>(Action<EventType> callback) where EventType : struct, IEvent
        {
            Type eventType = typeof(EventType);
            if (subscribers.TryGetValue(eventType, out List<Delegate> subscriptions)) 
            {
                subscriptions.Remove(callback);
            }
        }

        public void Raise<EventType>(params object[] parameters) where EventType : struct, IEvent 
        {
            Type eventType = typeof(EventType);

            EventType raisingEvent = eventPool.Get<EventType>(parameters);
            if (subscribers.TryGetValue(eventType, out List<Delegate> subscriptions)) 
            {
                foreach (Delegate callback in subscriptions) 
                {
                    ((Action<EventType>)callback)?.Invoke(raisingEvent);
                }
            }
            eventPool.Release(raisingEvent);
        }

        public void Clear() 
        {
            subscribers.Clear();
        }
    }
}