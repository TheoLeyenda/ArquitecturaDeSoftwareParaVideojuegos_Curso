using ImageCampus.ToolBox.Events;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using ZooArchitect.Architecture.GameLogic.Events;

namespace ZooArchitect.Architecture.GameLogic
{
    public sealed class Wallet : IService, IDisposable
    {
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

        public bool IsPersistance => false;

        private readonly Dictionary<string, Resource> resources;

        public Wallet()
        {
            EventBus.Subscribe<AddResourceToWlletEvent>(AddResource);
            EventBus.Subscribe<RemoveResourceToWlletEvent>(RemoveResource);

            resources = new Dictionary<string, Resource>();

            CreateResource(new Resource("Plata", 0, long.MaxValue, 1000));
            CreateResource(new Resource("Comida de Animales", 0, long.MaxValue, 50));
            CreateResource(new Resource("Comida de Visitantes", 0, long.MaxValue, 50));
            CreateResource(new Resource("Limpieza", 0, 100, 100));
            CreateResource(new Resource("Reputación", 0, long.MaxValue, 800));
            CreateResource(new Resource("Trabajadores", 0, 500, 3));
            CreateResource(new Resource("Animales", 0, 500, 0));

            void CreateResource(Resource resource)
            {
                resources.Add(resource.Name, resource);
            }
        }

        private void AddResource(in AddResourceToWlletEvent addResourceToWlletEvent)
        {
            resources[addResourceToWlletEvent.resourceName].AddResource(addResourceToWlletEvent.amount);
        }

        private void RemoveResource(in RemoveResourceToWlletEvent removeResourceToWlletEvent)
        {
            resources[removeResourceToWlletEvent.resourceName].RemoveResource(removeResourceToWlletEvent.amount);
        }

        internal bool HasResourceAmount(string resource, long amount) 
        {
            return resources[resource].CurrentValue >= amount;
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<AddResourceToWlletEvent>(AddResource);
            EventBus.Unsubscribe<RemoveResourceToWlletEvent>(RemoveResource);
        }
    }
}
