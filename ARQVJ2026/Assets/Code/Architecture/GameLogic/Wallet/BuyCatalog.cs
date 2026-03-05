using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Services;
using System.Collections.Generic;
using ZooArchitect.Architecture.Data;

namespace ZooArchitect.Architecture.GameLogic
{
    public sealed class BuyCatalog : IService
    {
        private BlueprintBinder BlueprintBinder => ServiceProvider.Instance.GetService<BlueprintBinder>();
        private BlueprintRegistry BlueprintRegistry => ServiceProvider.Instance.GetService<BlueprintRegistry>();

        public bool IsPersistance => false;

        private Dictionary<string, BuyItem> buyItems;

        public IReadOnlyCollection<BuyItem> BuyItems => buyItems.Values;

        public BuyCatalog()
        {
            buyItems = new Dictionary<string, BuyItem>();

            foreach (string itemBlueprint in BlueprintRegistry.BlueprintsOf(TableNames.BUY_ITEMS_TABLE_NAME))
            {
                object newItem = new BuyItem();
                BlueprintBinder.Apply(ref newItem, TableNames.BUY_ITEMS_TABLE_NAME, itemBlueprint);
                buyItems.Add(((BuyItem)newItem).name, (BuyItem)newItem);
            }
        }

        public BuyItem this[string itemName] => buyItems[itemName];
    }
}
