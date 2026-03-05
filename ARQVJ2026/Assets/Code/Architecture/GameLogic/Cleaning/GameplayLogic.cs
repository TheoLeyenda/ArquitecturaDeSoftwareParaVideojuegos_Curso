using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Dataflow;
using ImageCampus.ToolBox.Services;
using System;
using ZooArchitect.Architecture.Data;

namespace ZooArchitect.Architecture.GameLogic
{
	public sealed class GameplayLogic : IService, IInitable, IDisposable
	{
		public bool IsPersistance => false;

		private BlueprintBinder BlueprintBinder => ServiceProvider.Instance.GetService<BlueprintBinder>();

		private CleaningLogic cleaningLogic;
		private ReputationSystem reputationSystem;
		private ServicesLogic servicesLogic;

		public GameplayLogic()
		{
			object cleaningLogicObj = new CleaningLogic();
			BlueprintBinder.Apply(ref cleaningLogicObj, TableNames.CLEANING_SERVICE_TABLE_NAME, nameof(CleaningLogic));
			cleaningLogic = (CleaningLogic)cleaningLogicObj;

			object reputationSystemObj = new ReputationSystem();
			BlueprintBinder.Apply(ref reputationSystemObj, TableNames.REPUTATION_SYSTEM_TABLE_NAME, nameof(ReputationSystem));
			reputationSystem = (ReputationSystem)reputationSystemObj;

			object serviceLogicObj = new ServicesLogic();
			BlueprintBinder.Apply(ref serviceLogicObj, TableNames.SERVICES_LOGIC_TABLE_NAME, nameof(ServicesLogic));
			servicesLogic = (ServicesLogic)serviceLogicObj;
		}

		public void Init()
		{
			reputationSystem.Init();
		}

		public void LateInit()
		{
			reputationSystem.LateInit();
		}

		public bool HasDebt => servicesLogic.AcumulativeDebt > 0;

		public int ZooTier => reputationSystem.ReputationTier;

		public void Dispose()
		{
			cleaningLogic.Dispose();
			reputationSystem.Dispose();
			servicesLogic.Dispose();
		}
	}
}
