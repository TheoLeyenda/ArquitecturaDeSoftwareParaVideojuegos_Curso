using ImageCampus.ToolBox.Blueprints;

namespace ZooArchitect.Architecture.GameLogic
{
	public struct Resource
	{
		[BlueprintParameter("Name")] private string name;
		[BlueprintParameter("Min posible value")] private long minValue;
		[BlueprintParameter("Max posible value")] private long maxValue;
		[BlueprintParameter("Start amount")] private long currentValue;

		public string Name => name;
		public long CurrentValue => currentValue;

		public Resource(string name, long minValue, long maxValue, long startValue)
		{
			this.name = name;
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.currentValue = System.Math.Clamp(startValue, minValue, maxValue);
		}

		public void AddResource(long amount)
		{
			currentValue = System.Math.Clamp(currentValue + amount, minValue, maxValue);
		}

		public void RemoveResource(long amount)
		{
			currentValue = System.Math.Clamp(currentValue - amount, minValue, maxValue);
		}

		public void SetResourceAmount(long amount)
		{
			currentValue = System.Math.Clamp(amount, minValue, maxValue);
		}
	}
}
