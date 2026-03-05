using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using ZooArchitect.Architecture.Data;
using ZooArchitect.Architecture.GameLogic.Events;

namespace ZooArchitect.Architecture.GameLogic
{
    public sealed class Wallet : IDataService, IDisposable
    {
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

        private BlueprintRegistry BlueprintRegistry => ServiceProvider.Instance.GetService<BlueprintRegistry>();

        private BlueprintBinder BlueprintBinder => ServiceProvider.Instance.GetService<BlueprintBinder>();

        private BlueprintFieldsCache BlueprintFieldsCache => ServiceProvider.Instance.GetService<BlueprintFieldsCache>();

        public bool IsPersistance => false;

		private readonly Dictionary<string, string> resourceKeyToResourceName;
        private readonly Dictionary<string, Resource> resources;

        public Dictionary<string, Resource> Resources => resources;

        public Wallet()
        {
            EventBus.Subscribe<AddResourceToWalletEvent>(AddResource);
            EventBus.Subscribe<RemoveResourceToWalletEvent>(RemoveResource);

            resourceKeyToResourceName = new Dictionary<string, string>();
            resources = new Dictionary<string, Resource>();

			foreach (string resourceBlueprint in BlueprintRegistry.BlueprintsOf(TableNames.RESOURCES_TABLE_NAME))
			{
                object newResource = new Resource();
                BlueprintBinder.Apply(ref newResource, TableNames.RESOURCES_TABLE_NAME, resourceBlueprint);
                resourceKeyToResourceName.Add(resourceBlueprint, ((Resource)newResource).Name);
                resources.Add(((Resource)newResource).Name, (Resource)newResource);
            }
        }

        private void AddResource(in AddResourceToWalletEvent addResourceToWlletEvent)
        {
            resources[resourceKeyToResourceName[addResourceToWlletEvent.resourceName]]
                .AddResource(addResourceToWlletEvent.amount);
        }

        private void RemoveResource(in RemoveResourceToWalletEvent removeResourceToWlletEvent)
        {
            resources[resourceKeyToResourceName[removeResourceToWlletEvent.resourceName]]
                .RemoveResource(removeResourceToWlletEvent.amount);
        }

        internal bool HasResourceAmount(string resource, long amount) 
        {
            return resources[resourceKeyToResourceName[resource]].CurrentValue >= amount;
        }

        public long GetResourceAmount(string resource) 
        {
            return resources[resourceKeyToResourceName[resource]].CurrentValue;
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<AddResourceToWalletEvent>(AddResource);
            EventBus.Unsubscribe<RemoveResourceToWalletEvent>(RemoveResource);
        }

        public string ServiceReference => TableNames.RESOURCES_TABLE_NAME;

        public object GetDataValue(string[] dataPath)
		{
            Resource targetResource = resources[resourceKeyToResourceName[dataPath[1]]];
            return Convert.ToInt32(BlueprintFieldsCache[typeof(Resource), dataPath[2]].GetValue(targetResource));
		}
	}
}
