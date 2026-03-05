using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;
using ZooArchitect.Architecture.GameLogic;
using ZooArchitect.Architecture.GameLogic.Events;
using ZooArchitect.View.Data;
using ZooArchitect.View.Scene;

namespace ZooArchitect.View.UI
{
    internal sealed class ResourcesUI : ViewComponent
    {
        private Wallet Wallet => ServiceProvider.Instance.GetService<Wallet>();
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

        private BlueprintRegistry BlueprintRegistry => ServiceProvider.Instance.GetService<BlueprintRegistry>();
        private BlueprintBinder BlueprintBinder => ServiceProvider.Instance.GetService<BlueprintBinder>();

        private StringBuilder stringBuilder;
        private Text text;

        private List<ResourceViewData> resourceViewDatas;

        public override void Init()
        {
            stringBuilder = new StringBuilder();
            text = GetComponent<Text>();
            resourceViewDatas = new List<ResourceViewData>();

            EventBus.Subscribe<AddResourceToWalletEvent>(OnAddResource);
            EventBus.Subscribe<RemoveResourceToWalletEvent>(OnRemoveResource);


            foreach (string resources in BlueprintRegistry.BlueprintsOf(TableNamesView.RESOURCE_VIEW_TABLE_NAME))
            {
                object resourceViewData = new ResourceViewData();
                BlueprintBinder.Apply(ref resourceViewData, TableNamesView.RESOURCE_VIEW_TABLE_NAME, resources);
                resourceViewDatas.Add((ResourceViewData)resourceViewData);
            }

            DisplayResources();

            base.Init();
        }

        private void OnRemoveResource(in RemoveResourceToWalletEvent _)
        {
            DisplayResources();
        }

        private void OnAddResource(in AddResourceToWalletEvent _)
        {
            DisplayResources();
        }

        private void DisplayResources()
        {
            stringBuilder.Clear();

            foreach (ResourceViewData resources in resourceViewDatas)
            {
                stringBuilder.AppendFormat("<color={0}>", resources.color);
                stringBuilder.AppendFormat(resources.format, resources.architectureID, Wallet.GetResourceAmount(resources.architectureID));
                stringBuilder.Append("</color>");
                stringBuilder.Append("\n");
            }

            text.text = stringBuilder.ToString();
        }

        public override void Dispose()
        {
            EventBus.Unsubscribe<AddResourceToWalletEvent>(OnAddResource);
            EventBus.Unsubscribe<RemoveResourceToWalletEvent>(OnRemoveResource);
            base.Dispose();
        }

        private struct ResourceViewData
        {
            [BlueprintParameter("Architecture ID")] public string architectureID;
            [BlueprintParameter("Format")] public string format;
            [BlueprintParameter("Color")] public string color;
        }
    }
}
