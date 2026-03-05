using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Events;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Entities
{
    public sealed class Jail : Structure
    {
        [BlueprintParameter("Days whitout maintenance to broke")] private int daysWhitoutMaintenanceToBroke;
        [BlueprintParameter("Chance to broke jail on low maintenance")] private float chanceToBrokeJailOnLowMaintenance;
        [BlueprintParameter("On broke jail tile to swap wall")] private string onBrokeJailTileToSwapWall;

        private Jail(uint ID, Coordinate coordinate) : base(ID, coordinate)
        {
        }

        internal override void DecreaseDailyMaintenance()
        {
            base.DecreaseDailyMaintenance();

            if ((maxMaintenance - currentMaintenance) > dailyMaintenanceDecrease * daysWhitoutMaintenanceToBroke)
            {
                foreach (Point jailWallPoint in coordinate.Perimeter)
                {
                    if (random.Next(0, 100) < chanceToBrokeJailOnLowMaintenance)
                    {
                        EventBus.Raise<ModifyTerrainRecuestAceptedEvent>(jailWallPoint, jailWallPoint, onBrokeJailTileToSwapWall);
                    }
                }
            }
        }
    }
}
