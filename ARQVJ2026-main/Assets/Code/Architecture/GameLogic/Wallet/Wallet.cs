using ImageCampus.ToolBox.Services;
using System.Collections.Generic;

namespace ZooArchitect.Architecture.GameLogic
{
    public sealed class Wallet : IService
    {
        public bool IsPersistance => false;

        private readonly Dictionary<string, Resource> resources;

        public Wallet()
        {
            resources = new Dictionary<string, Resource>();

            AddResource(new Resource("Plata", 0, long.MaxValue, 1000));
            AddResource(new Resource("Comida de Animales", 0, long.MaxValue, 50));
            AddResource(new Resource("Comida de Visitantes", 0, long.MaxValue, 50));
            AddResource(new Resource("Limpieza", 0, 100, 100));
            AddResource(new Resource("Reputación", 0, long.MaxValue, 800));
            AddResource(new Resource("Trabajadores", 0, 500, 3));
            AddResource(new Resource("Animales", 0, 500, 0));

            void AddResource(Resource resource)
            {
                resources.Add(resource.Name, resource);
            }
        }

    }
}
