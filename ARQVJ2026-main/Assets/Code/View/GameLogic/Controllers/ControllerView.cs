using ImageCampus.ToolBox.Dataflow;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using ZooArchitect.Architecture.Entities;
using ZooArchitect.View.Scene;

namespace ZooArchitect.View.Controller
{
    internal abstract class ControllerView : ITickable, IDisposable
    {
        protected EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
        protected GameScene GameScene => ServiceProvider.Instance.GetService<GameScene>();
        protected EntitiesLogic EntitiesLogic => ServiceProvider.Instance.GetService<EntitiesLogic>();
        private ContextMenuView ContextMenuView => ServiceProvider.Instance.GetService<ContextMenuView>();

        public abstract void Tick(float deltaTime);

        public abstract void Dispose();

        protected void Display(Dictionary<string, Action> controls)
        {
            ContextMenuView.Show(controls);
        }
        public abstract void CreateController();

    }
}
