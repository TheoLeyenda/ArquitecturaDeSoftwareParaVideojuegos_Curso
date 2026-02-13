using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using ZooArchitect.Architecture.Data;
using ZooArchitect.Architecture.GameLogic.Events;

namespace ZooArchitect.Architecture.GameLogic
{
    public sealed class Wallet : IService, IDisposable
    {
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

        private BlueprintRegistry BlueprintRegistry => ServiceProvider.Instance.GetService<BlueprintRegistry>();

        private BlueprintBinder BlueprintBinder => ServiceProvider.Instance.GetService<BlueprintBinder>();

        public bool IsPersistance => false;


        private readonly Dictionary<string, Resource> resources;

        public Wallet()
        {
            EventBus.Subscribe<AddResourceToWlletEvent>(AddResource);
            EventBus.Subscribe<RemoveResourceToWalletEvent>(RemoveResource);

            resources = new Dictionary<string, Resource>();

			foreach (string resourceBlueprint in BlueprintRegistry.BlueprintsOf(TableNames.RESOURCES_TABLE_NAME))
			{
                object newResource = new Resource();
                BlueprintBinder.Apply(ref newResource, TableNames.RESOURCES_TABLE_NAME, resourceBlueprint);
                resources.Add(((Resource)newResource).Name, (Resource)newResource);
            }
        }

        private void AddResource(in AddResourceToWlletEvent addResourceToWlletEvent)
        {
            resources[addResourceToWlletEvent.resourceName].AddResource(addResourceToWlletEvent.amount);
        }

        private void RemoveResource(in RemoveResourceToWalletEvent removeResourceToWlletEvent)
        {
            resources[removeResourceToWlletEvent.resourceName].RemoveResource(removeResourceToWlletEvent.amount);
        }

        internal bool HasResourceAmount(string resource, long amount) 
        {
            return resources[resource].CurrentValue >= amount;
        }

        public long GetResourceAmount(string resource) 
        {
            return resources[resource].CurrentValue;
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<AddResourceToWlletEvent>(AddResource);
            EventBus.Unsubscribe<RemoveResourceToWalletEvent>(RemoveResource);
        }

    }

}
