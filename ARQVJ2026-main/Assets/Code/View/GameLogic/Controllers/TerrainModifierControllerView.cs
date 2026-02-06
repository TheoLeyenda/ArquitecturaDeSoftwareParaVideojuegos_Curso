using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.Math;

using LogicScene = ZooArchitect.Architecture.Scene;

namespace ZooArchitect.View.Controller
{
    internal sealed class TerrainModifierControllerView : GroupSelectionControllerView
    {
        private LogicScene Scene => ServiceProvider.Instance.GetService<LogicScene>();

        public TerrainModifierControllerView()
        {
        }

        public override void CreateController()
        {
            Dictionary<string, Action> validTilesToSwap = new Dictionary<string, Action>();

            Coordinate selection = new Coordinate(StartGroupClickPosition, FinishGroupClickPosition);

            foreach (string tileID in Scene.GetValidTilesForSelection(selection))
            {
                string currentTileID = tileID;
                validTilesToSwap.Add($"Swap to {currentTileID}", () =>
                {
                    EventBus.Raise<ModifyTerrainRequestEvent>(selection.Origin, selection.End, currentTileID);
                });
            }

            Display(validTilesToSwap);
        }


        public override void Dispose()
        {
        }
    }
}
