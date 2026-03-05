using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using System.Reflection;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.Data;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.Architecture.GameLogic;
using ZooArchitect.Architecture.GameLogic.Events;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Controllers
{
	public sealed class BuyControllerArchitecture : IDisposable
	{
		private BuyCatalog BuyCatalog => ServiceProvider.Instance.GetService<BuyCatalog>();
		private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
		private Wallet Wallet => ServiceProvider.Instance.GetService<Wallet>();
		private Scene Scene => ServiceProvider.Instance.GetService<Scene>();
		private GameplayLogic GameplayLogic => ServiceProvider.Instance.GetService<GameplayLogic>();
		private BlueprintRegistry BlueprintRegistry => ServiceProvider.Instance.GetService<BlueprintRegistry>();

		private const string TABLE_BLUPRINT_SEPARATOR = ":";

		private Dictionary<string, MethodInfo> purchaseMethods;

		public BuyControllerArchitecture()
		{
			EventBus.Subscribe<BuyItemRequestEvent>(OnBuyItemRequest);
			EventBus.Subscribe<BuyItemRequestAcceptedEvent>(OnBuyItemRequestAccepted);
			purchaseMethods = new Dictionary<string, MethodInfo>();
		}

		private void OnBuyItemRequest(in BuyItemRequestEvent buyItemRequestEvent)
		{
			BuyItem itemToBuy = BuyCatalog[buyItemRequestEvent.buyItemName];

			if (!Wallet.HasResourceAmount(itemToBuy.costResource, itemToBuy.cost))
			{
				EventBus.Raise<BuyItemRequestRejectedEvent>(itemToBuy.name, $"{itemToBuy.name} price: {itemToBuy.cost} - " +
							$"{itemToBuy.costResource} in Wallet: {Wallet.GetResourceAmount(itemToBuy.costResource)} " +
							$"So much expensive! ");
				return;
			}

			EventBus.Raise<BuyItemRequestAcceptedEvent>(buyItemRequestEvent.buyItemName);
		}

		private void OnBuyItemRequestAccepted(in BuyItemRequestAcceptedEvent buyItemRequestAcceptedEvent)
		{
			BuyItem itemToBuy = BuyCatalog[buyItemRequestAcceptedEvent.buyItemName];

			string[] purchaseEntries = itemToBuy.resourceToBuy.Split(TABLE_BLUPRINT_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);

			string purchaseTable = purchaseEntries[0];
			string purchaseId = purchaseEntries[1];

			foreach (string purchasableItemTable in TableNames.PURCHASABLE_ELEMENTS_TABLES)
			{
				if (string.Equals(purchaseTable, purchasableItemTable))
				{
					if (!purchaseMethods.ContainsKey(purchaseTable))
					{
						purchaseMethods.Add(purchaseTable, GetType().GetMethod(
							TableNames.PURCHASABLE_ELEMENTS_MAPPING[purchaseTable],
							BindingFlags.Public | BindingFlags.Instance));
					}

					purchaseMethods[purchaseTable].Invoke(this, new object[] { itemToBuy, purchaseTable, purchaseId });
					return;
				}
			}
		}

		public void PurchaseResource(BuyItem buyItem, string purchaseTable, string blueprintId)
		{
			EventBus.Raise<RemoveResourceToWalletEvent>(buyItem.costResource, buyItem.cost);
			EventBus.Raise<AddResourceToWalletEvent>(blueprintId, buyItem.resourceToBuyAmount);
		}

		public void PurchaseEntity(BuyItem buyItem, string purchaseTable, string blueprintId)
		{
			Type entityType = TableNames.ENTITY_TABLE_NAME_ENTITY_TYPE[purchaseTable];
			if (BlueprintRegistry.TryGetValue(purchaseTable, blueprintId, Entity.TIER_KEY, out string tier))
			{
				if (GameplayLogic.ZooTier < Convert.ToInt32(tier))
				{
					EventBus.Raise<SpawnRequestRejectedEvent>(blueprintId, new Coordinate(Scene.HumanEntryPoint),
						 entityType.Name, $"Unable to spawn {entityType.Name} has higher tier that the zoo");
				}
			}
			EventBus.Raise<RemoveResourceToWalletEvent>(buyItem.costResource, buyItem.cost);
			EventBus.Raise<SpawnRequestAcceptedEvent>(blueprintId, new Coordinate(Scene.HumanEntryPoint),
				purchaseTable, entityType.Name);
		}

		public void Dispose()
		{
			EventBus.Unsubscribe<BuyItemRequestEvent>(OnBuyItemRequest);
		}
	}
}
