using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using ZooArchitect.Architecture.Controllers;
using ZooArchitect.Architecture.Controllers.Events;
using ZooArchitect.Architecture.GameLogic;
using ZooArchitect.View.Mapping;

namespace ZooArchitect.View.Controller
{
    [ViewOf(typeof(BuyControllerArchitecture))]
    internal sealed class BuyControllerView : ControllerView
    {
        private BuyCatalog BuyCatalog => ServiceProvider.Instance.GetService<BuyCatalog>();

        private bool isOpen;
        private bool shouldOpen;

        public BuyControllerView()
        {
            isOpen = false;
            shouldOpen = false;
        }

        public override void OnSelect()
        {
            if (!isOpen)
            {
                shouldOpen = true;
            }
        }

        public override void Tick(float deltaTime)
        {
            if (shouldOpen)
            {
                CreateController();
                shouldOpen = false;
                isOpen = true;
            }
        }

        public override void CreateController()
        {
            Dictionary<string, Action> controls = new Dictionary<string, Action>();

            foreach (BuyItem item in BuyCatalog.BuyItems)
            {
                controls.Add(item.name, () =>
                {
                    EventBus.Raise<BuyItemRequestEvent>(item.name);
                    isOpen = false;
                });
            }

            Display(controls, "BUY");
        }

        public override void Dispose()
        {
        }
    }
}
