using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Scheduling;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.GameLogic.Events;

namespace ZooArchitect.Architecture.GameLogic
{
	public sealed class CleaningLogic : IDisposable
	{
		[BlueprintParameter("Cleaning resource name")] private string cleaningResourceName;
		[BlueprintParameter("Cleaning resource amount")] private long cleaningResourceAmount;
		[BlueprintParameter("Needed infrastructures")] private string[] neededInfrastructures;

		private EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();
		private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

		private TaskScheduler TaskScheduler => ServiceProvider.Instance.GetService<TaskScheduler>();

		public CleaningLogic()
		{
			EventBus.Subscribe<DayChangeEvent>(OnDayChange);
		}

		private void OnDayChange(in DayChangeEvent _)
		{
			List<string> neededInftastructuresToClean = new List<string>(neededInfrastructures);
			foreach (Infrastructure infrastructure in EntityRegistry.Infrastructures)
			{
				if (neededInftastructuresToClean.Contains(infrastructure.Name))
					neededInftastructuresToClean.Remove(infrastructure.Name);
			}

			if (neededInftastructuresToClean.Count > 0)
				return;

			uint selectedWorker = Entity.UNASSIGNED_ENTITY_ID;
			foreach (Worker worker in EntityRegistry.Workers)
			{
				if (worker.IsAbiable() && worker.CanDoCleaning)
				{
					selectedWorker = worker.ID;
					break;
				}
			}

			if (selectedWorker == Entity.UNASSIGNED_ENTITY_ID)
				return;
			EntityRegistry.GetAs<Worker>(selectedWorker).SetAsWorking();
			TaskScheduler.Schedule(() =>
			{
				EventBus.Raise<AddResourceToWalletEvent>(cleaningResourceName, cleaningResourceAmount);
				EntityRegistry.GetAs<Worker>(selectedWorker).EndWork();
			},
			EntityRegistry.GetAs<Worker>(selectedWorker).WorkTime);
		}

		public void Dispose()
		{
			EventBus.Unsubscribe<DayChangeEvent>(OnDayChange);
		}
	}
}
