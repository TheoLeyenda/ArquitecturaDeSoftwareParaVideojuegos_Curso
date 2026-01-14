
using System;
using System.Collections.Concurrent;
using TheoLeyenda.ToolBox.Resetteable;

namespace TheoLeyenda.ToolBox.Pool 
{
    public class ConcurrentPool 
    {
        private readonly ConcurrentDictionary<Type, ConcurrentStack<IRessetteable>> concurrentPool = new ConcurrentDictionary<Type, ConcurrentStack<IRessetteable>>();

        public RessetteableType Get<RessetteableType>(params object[] parameters) where RessetteableType : IRessetteable 
        {
            Type resseteableType = typeof(RessetteableType);
            if (!concurrentPool.ContainsKey(resseteableType)) 
            {
                concurrentPool.TryAdd(resseteableType, new ConcurrentStack<IRessetteable>());
            }

            RessetteableType value;
            if (concurrentPool[resseteableType].Count > 0)
            {
                concurrentPool[resseteableType].TryPop(out IRessetteable resetteable);
                value = (RessetteableType)resetteable;
            }
            else 
            {
                value = (RessetteableType)Activator.CreateInstance(resseteableType);
            }

            value.Assign(parameters);
            return value;
        }

        public void Release<RessetteableType>(RessetteableType ressetteable) where RessetteableType : IRessetteable 
        {
            ressetteable.Reset();
            concurrentPool[typeof(RessetteableType)].Push(ressetteable);
        }
    }
}