using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.Data;
using ZooArchitect.Architecture.Entities.Events;
using ZooArchitect.Architecture.GameLogic;
using ZooArchitect.Architecture.GameLogic.Events;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Entities
{
    public sealed class Worker : Human
    {
        private Wallet Wallet => ServiceProvider.Instance.GetService<Wallet>();
        private GameplayLogic GameplayLogic => ServiceProvider.Instance.GetService<GameplayLogic>();


        [BlueprintParameter("Name")] private string name;
        [BlueprintParameter(TIER_KEY)] private int tier;
        [BlueprintParameter("Cost per day")] private long costPerDay;
        [BlueprintParameter("Cost resource")] private string costResource;

        [BlueprintParameter("Work time")] private int workTime;
        public int WorkTime => workTime;

        [BlueprintParameter("Fired cost")] private long firedCost;
        [BlueprintParameter("Fired resource")] private string firedResource;
        [BlueprintParameter("Upgradeable to")] private string upgradeableTo;

        [BlueprintParameter("Can do maintenance")] private bool canDoMaintenance;
        public bool CanDoMaintenance => canDoMaintenance;

        [BlueprintParameter("Can do cleaning")] private bool canDoCleaning;
        public bool CanDoCleaning => canDoCleaning;


        private bool isWorking;
        private Worker(uint ID, Coordinate coordinate) : base(ID, coordinate)
        {
            isWorking = false;
        }

        internal override Dictionary<string, Action> PerfomableMethods
        {
            get
            {
                Dictionary<string, Action> perfomableActions = new Dictionary<string, Action>();
                perfomableActions.Add(nameof(Upgrade), Upgrade);
                perfomableActions.Add(nameof(GetFired), GetFired);
                return perfomableActions;
            }
        }

		internal override Dictionary<string, Func<bool>> ChechForPerfomableActions 
        {
            get 
            {
                Dictionary<string, Func<bool>> perfomableActionsChecks = new Dictionary<string, Func<bool>>();
                perfomableActionsChecks.Add(nameof(Upgrade), CanBeUpgraded);
                perfomableActionsChecks.Add(nameof(GetFired), CanBeFried);
                return perfomableActionsChecks;
            }
        }

		public override void LateInit()
		{
            Travel(new Point(10, 10));
			base.LateInit();
		}


		public bool CanBeUpgraded()
        {
            return !string.Equals(upgradeableTo, string.Empty) && GameplayLogic.ZooTier <= tier;
        }

        private void Upgrade()
        {
            Destroy();
            EventBus.Raise<SpawnRequestAcceptedEvent<Worker>>(upgradeableTo, coordinate, TableNames.WORKERS_TABLE_NAME);
        }

        private bool CanBeFried() 
        {
            return Wallet.HasResourceAmount(firedResource, firedCost);
        }

        private void GetFired()
        {
            EventBus.Raise<RemoveResourceToWalletEvent>(firedResource, firedCost);
            Destroy();
        }

        public void GetPaid()
        {
            EventBus.Raise<RemoveResourceToWalletEvent>(costPerDay, costResource);
        }

        public bool IsAbiable()
        {
            return !isWorking;
        }

        public void SetAsWorking()
        {
            isWorking = true;
            EventBus.Raise<WorkerStartWorkEvent>(ID);
        }

        public void EndWork()
        {
            isWorking = false;
            EventBus.Raise<WorkerEndWorkEvent>(ID);

        }

    }
}
