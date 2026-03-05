
using ImageCampus.ToolBox.Blueprints;

namespace ZooArchitect.Architecture.GameLogic
{
    public struct BuyItem 
    {
       [BlueprintParameter("Name")] public string name;
       [BlueprintParameter("Cost")] public long cost;
       [BlueprintParameter("Cost resource")] public string costResource;
       [BlueprintParameter("Resource to buy amount")] public long resourceToBuyAmount;
       [BlueprintParameter("Resource to buy")] public string resourceToBuy;
    }
}
