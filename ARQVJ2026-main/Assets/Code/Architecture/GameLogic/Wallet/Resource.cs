using System;

namespace ZooArchitect.Architecture.GameLogic
{
    public struct Resource
    {
        private string name;
        private long minValue;
        private long maxValue;
        private long currentValue;

        public string Name => name;
        public long CurrentValue => currentValue;

        public Resource(string name, long minValue, long maxValue, long startValue)
        {
            this.name = name;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.currentValue = Math.Clamp(startValue, minValue, maxValue);
        }

        public void AddResource(long amount)
        {
            currentValue = Math.Clamp(currentValue + amount, minValue, maxValue);
        }

        public void RemoveResource(long amount)
        {
            currentValue = Math.Clamp(currentValue - amount, minValue, maxValue);
        }

        public void SetResourceAmount(long amount)
        {
            currentValue = Math.Clamp(amount, minValue, maxValue);
        }
    }
}
