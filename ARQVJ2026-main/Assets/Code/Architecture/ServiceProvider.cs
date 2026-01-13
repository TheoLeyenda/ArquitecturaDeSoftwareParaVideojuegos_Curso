using Architecture.Service;
using System;
using System.Collections.Generic;

namespace Architecture
{
    public sealed class ServiceProvider
    {
        private static ServiceProvider instance;

        public static ServiceProvider Instance
        {
            get 
            {
                if (instance == null) 
                {
                    instance = new ServiceProvider();
                }
                return instance;
            }

            private set => instance = value;
        }

        private readonly Dictionary<Type, IService> services = new Dictionary<Type, IService>();

        private ServiceProvider() { }

        public void AddService<ServiceType>(IService service) where ServiceType : class, IService 
        {
            if (!services.ContainsKey(typeof(ServiceType))) 
            {
                services.Add(typeof(ServiceType), service);
            }
        }

        public bool RemoveService<ServiceType>() where ServiceType : class, IService 
        {
            if (!services.ContainsKey(typeof(ServiceType)))
                throw new KeyNotFoundException();

            return services.Remove(typeof(ServiceType));
        }

        public bool ContainsService<ServiceType>() where ServiceType : class, IService 
        {
            return services.ContainsKey(typeof(ServiceType));
        }

        public ServiceType GetService<ServiceType>() where ServiceType : class, IService 
        {
            return services[typeof(ServiceType)] as ServiceType;
        }

        public void ClearAllService() 
        {
            services.Clear();
        }

        public void ClearAllNonPersistanceServices() 
        {
            List<Type> nonPersistanceServiceTypes = new List<Type>();
            foreach (KeyValuePair<Type, IService> service in services) 
            {
                if (service.Value.IsPersistance)
                    nonPersistanceServiceTypes.Add(service.Key);
            }

            foreach (Type keyToRemove in nonPersistanceServiceTypes) 
            {
                services.Remove(keyToRemove);
            }
        }
    }
}
