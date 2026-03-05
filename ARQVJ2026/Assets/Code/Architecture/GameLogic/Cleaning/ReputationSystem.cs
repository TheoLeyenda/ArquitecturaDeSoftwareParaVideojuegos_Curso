using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Dataflow;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Rules;
using ImageCampus.ToolBox.Services;
using System;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.Entities.Events;
using ZooArchitect.Architecture.GameLogic.Events;

namespace ZooArchitect.Architecture.GameLogic
{
	public sealed class ReputationSystem : IInitable, IDisposable
	{
		private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
		private RuleFactory RuleFactory => ServiceProvider.Instance.GetService<RuleFactory>();
		private EntityRegistry EntityRegistry => ServiceProvider.Instance.GetService<EntityRegistry>();
		private GameplayLogic GameplayLogic => ServiceProvider.Instance.GetService<GameplayLogic>();
		private Wallet Wallet => ServiceProvider.Instance.GetService<Wallet>();

		[BlueprintParameter("Tiers thresholds")] private long[] thresholds;

		[BlueprintParameter("Reputation resource key")] private string reputationResource;

		[BlueprintParameter("Increase reputation on day change rule")] private string onDayChageRuleKey;
		private Rule onDayChageRule;

		[BlueprintParameter("On change day check sucsess increase amount")] private long onChangeDayChechSucsessIncreaseAmount;
		[BlueprintParameter("On change day whitout deads increase amount")] private long onChangeDayWhitoutDeadsIncreaseAmount;

		[BlueprintParameter("Reputation cost for animal dead")] private long reputationCostForAnimalDead;
		[BlueprintParameter("Reputation cost for visitor dead")] private long reputationCostForVisitorDead;
		[BlueprintParameter("Reputation cost for worker dead")] private long reputationCostForWorkerDead;
		[BlueprintParameter("Reputation cost for services debt")] private long reputationCostForServicesDebt;

		private bool entityDeadThisDay;

		public ReputationSystem()
		{
			entityDeadThisDay = false;
			EventBus.Subscribe<DayChangeEvent>(OnDayChange);
			EventBus.Subscribe<EntityDestroyedEvent>(OnEntityDies);
		}


		public void Init()
		{
		}

		public void LateInit()
		{
			onDayChageRule = RuleFactory.GetRule(onDayChageRuleKey);
		}

		public int ReputationTier 
		{
			get 
			{
				long currentReputation = Wallet.GetResourceAmount(reputationResource);
				int currentTier = 0;
				for (int i = 0; i < thresholds.Length; i++)
				{
					if (currentReputation >= thresholds[i])
					{
						currentTier = i;
					}
				}

				return currentTier + 1;
			}
		}

		private void OnDayChange(in DayChangeEvent _)
		{
			if (onDayChageRule.Evaluate())
				EventBus.Raise<AddResourceToWalletEvent>(reputationResource, onChangeDayChechSucsessIncreaseAmount);

			if (GameplayLogic.HasDebt)
				EventBus.Raise<RemoveResourceToWalletEvent>(reputationResource, reputationCostForServicesDebt);

			if (!entityDeadThisDay)
				EventBus.Raise<AddResourceToWalletEvent>(reputationResource, onChangeDayWhitoutDeadsIncreaseAmount);
			entityDeadThisDay = false;

		}

		private void OnEntityDies(in EntityDestroyedEvent entityDestroyedEvent)
		{
			if (EntityRegistry[entityDestroyedEvent.entityID] is Animal)
				EventBus.Raise<RemoveResourceToWalletEvent>(reputationResource, reputationCostForAnimalDead);
			if (EntityRegistry[entityDestroyedEvent.entityID] is Visitor)
				EventBus.Raise<RemoveResourceToWalletEvent>(reputationResource, reputationCostForVisitorDead);
			if (EntityRegistry[entityDestroyedEvent.entityID] is Worker)
				EventBus.Raise<RemoveResourceToWalletEvent>(reputationResource, reputationCostForWorkerDead);

			entityDeadThisDay = true;
		}

		public void Dispose()
		{
			EventBus.Unsubscribe<DayChangeEvent>(OnDayChange);
			EventBus.Unsubscribe<EntityDestroyedEvent>(OnEntityDies);
		}
	}
}
