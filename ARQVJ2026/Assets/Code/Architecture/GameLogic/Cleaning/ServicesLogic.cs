using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.GameLogic.Events;

namespace ZooArchitect.Architecture.GameLogic
{
	public sealed class ServicesLogic : IDisposable
	{
		private EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();
		private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
		private Wallet Wallet => ServiceProvider.Instance.GetService<Wallet>();

		[BlueprintParameter("Cost resource key")] private string costResourceKey;

		[BlueprintParameter("Power structure key")] private string powerStructureKey;
		[BlueprintParameter("Water structure key")] private string waterStructureKey;

		[BlueprintParameter("Power cost per day")] private long powerCostPerDay;
		[BlueprintParameter("Water cost per day")] private long waterCostPerDay;

		private long acumulativeDebt;
		public long AcumulativeDebt  => acumulativeDebt; 

		public ServicesLogic()
		{
			acumulativeDebt = 0;
			EventBus.Subscribe<DayChangeEvent>(OnDayChange);
		}

		private void OnDayChange(in DayChangeEvent _)
		{
			if (acumulativeDebt <= Wallet.GetResourceAmount(costResourceKey))
			{
				EventBus.Raise<RemoveResourceToWalletEvent>(costResourceKey, acumulativeDebt);
				acumulativeDebt = 0;
			}

			bool hasPowerStructure = false;
			bool hasWaterStructure = false;
			foreach (Infrastructure infrastructure in EntityRegistry.Infrastructures)
			{
				if (!hasPowerStructure && string.Equals(infrastructure.Name, powerStructureKey))
					hasPowerStructure = true;

				if (!hasWaterStructure && string.Equals(infrastructure.Name, waterStructureKey))
					hasWaterStructure = true;
			}

			if (hasPowerStructure)
			{
				if (Wallet.HasResourceAmount(costResourceKey, powerCostPerDay))
					EventBus.Raise<RemoveResourceToWalletEvent>(costResourceKey, powerCostPerDay);
				else
					acumulativeDebt += powerCostPerDay;
			}

			if (hasWaterStructure)
			{
				if (Wallet.HasResourceAmount(costResourceKey, waterCostPerDay))
					EventBus.Raise<RemoveResourceToWalletEvent>(costResourceKey, waterCostPerDay);
				else
					acumulativeDebt += powerCostPerDay;
			}
		}

		public void Dispose()
		{
			EventBus.Unsubscribe<DayChangeEvent>(OnDayChange);
		}
	}
}
