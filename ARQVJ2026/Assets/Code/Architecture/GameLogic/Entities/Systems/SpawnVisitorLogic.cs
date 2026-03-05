using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Cast;
using ImageCampus.ToolBox.Dataflow;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Scheduling;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.Data;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.Entities.Events;
using ZooArchitect.Architecture.GameLogic.Events;

namespace ZooArchitect.Architecture.GameLogic
{
	internal sealed class SpawnVisitorLogic : IInitable, ITickable, IDisposable
	{
		private BlueprintRegistry BlueprintRegistry => ServiceProvider.Instance.GetService<BlueprintRegistry>();
		private DayNightCycle DayNightCycle => ServiceProvider.Instance.GetService<DayNightCycle>();
		private TaskScheduler TaskScheduler => ServiceProvider.Instance.GetService<TaskScheduler>();
		private Wallet Wallet => ServiceProvider.Instance.GetService<Wallet>();
		private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
		private EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();
		private Scene Scene => ServiceProvider.Instance.GetService<Scene>();

		private Random random;
		private bool isInDelay;

		public SpawnVisitorLogic()
		{
			random = new Random();
			EventBus.Subscribe<EntityCreatedEvent<Visitor>>(StartSpawnDelay);
		}


		public void Init()
		{
		}

		public void LateInit()
		{
		}

		public void Tick(float deltaTime)
		{
			string blueprintToSpawn = PickRandomSpawnCandidate();
			if (CanSpawnNow(blueprintToSpawn))
			{
				Spawn(blueprintToSpawn);
			}
		}

		private string PickRandomSpawnCandidate()
		{
			List<string> visitorBlueprints = BlueprintRegistry.BlueprintsOf(TableNames.VISITORS_TABLE_NAME);
			int randomIndex = random.Next(0, visitorBlueprints.Count);
			return visitorBlueprints[randomIndex];
		}

		private bool CanSpawnNow(string visitorBlueprint)
		{
			if (isInDelay)
				return false;

			string costResourceKey = BlueprintRegistry[TableNames.VISITORS_TABLE_NAME,
													visitorBlueprint, Visitor.ENTRANCE_COST_RESOURCE_KEY];

			long costPrice = StringCast.Convert<long>(BlueprintRegistry[TableNames.VISITORS_TABLE_NAME,
													visitorBlueprint, Visitor.ENTRANCE_COST]);

			if (!Wallet.HasResourceAmount(costResourceKey, costPrice))
				return false;

			List<string> neededInftastructuresToSpawn = StringCast.Convert<List<string>>(BlueprintRegistry[TableNames.VISITORS_TABLE_NAME,
												visitorBlueprint, Visitor.NEEDED_INFTASTRUCTURES_TO_SPAWN]);

			foreach (Infrastructure infrastructure in EntityRegistry.Infrastructures)
			{
				if (neededInftastructuresToSpawn.Contains(infrastructure.Name))
					neededInftastructuresToSpawn.Remove(infrastructure.Name);
			}

			if (neededInftastructuresToSpawn.Count > 0)
				return false;

			if (!Scene.HasHumanEntryPoint)
				return false;

			string[] validSpawnTimeSteps = StringCast.Convert<string[]>(BlueprintRegistry[TableNames.VISITORS_TABLE_NAME,
												visitorBlueprint, Visitor.VALID_SPAWN_TIME_STEPS_KEY]);


			foreach (string step in validSpawnTimeSteps)
			{
				if (DayNightCycle.IsThisStep(step))
				{
					return true;
				}
			}
			return false;
		}

		private void Spawn(string visitorBlueprint)
		{
			string entranceResourceKey = BlueprintRegistry[TableNames.VISITORS_TABLE_NAME,
													visitorBlueprint, Visitor.ENTRANCE_PRICE_RESOURCE_KEY];

			float entrancePrice = StringCast.Convert<float>(BlueprintRegistry[TableNames.VISITORS_TABLE_NAME,
													visitorBlueprint, Visitor.ENTRANCE_PRICE]);

			EventBus.Raise<AddResourceToWalletEvent>(entranceResourceKey, entrancePrice);

			string costResourceKey = BlueprintRegistry[TableNames.VISITORS_TABLE_NAME,
													visitorBlueprint, Visitor.ENTRANCE_COST_RESOURCE_KEY];

			long costPrice = StringCast.Convert<long>(BlueprintRegistry[TableNames.VISITORS_TABLE_NAME,
													visitorBlueprint, Visitor.ENTRANCE_COST]);

			EventBus.Raise<RemoveResourceToWalletEvent>(costResourceKey, costPrice);

			EventBus.Raise<SpawnRequestAcceptedEvent<Visitor>>(visitorBlueprint, Scene.HumanEntryPoint, 
				TableNames.VISITORS_TABLE_NAME);
		}

		private void StartSpawnDelay(in EntityCreatedEvent<Visitor> entityCreatedEvent)
		{
			isInDelay = true;
			TaskScheduler.Schedule(() => { isInDelay = false; }, EntityRegistry.GetAs<Visitor>(entityCreatedEvent.entityCreatedId).SpawnDelay);
		}

		public void Dispose()
		{
			EventBus.Unsubscribe<EntityCreatedEvent<Visitor>>(StartSpawnDelay);
		}
	}
}
