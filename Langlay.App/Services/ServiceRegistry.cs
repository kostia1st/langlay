using System;
using System.Collections.Generic;
using System.Linq;

namespace Product {
    public class ServiceRegistry {
        public static ServiceRegistry Instance { get; set; }

        private IDictionary<Type, object> _registryCache = new Dictionary<Type, object>();
        private IList<object> _registry = new List<object>();

        public T Register<T>(T service) where T : class {
            if (_registry.Contains(service)) {
                throw new InvalidOperationException($"The service {service.ToString()} is already registered");
            }
            _registry.Add(service);
            _registryCache.Clear();
            return service;
        }

        public void RegisterMany<T>(IList<T> services) where T : class {
            foreach (var service in services) {
                Register(service);
            }
        }

        public T Unregister<T>(T service) where T : class {
            if (!_registry.Contains(service)) {
                throw new InvalidOperationException($"The service {service.ToString()} is not registered");
            }
            _registry.Remove(service);
            _registryCache.Clear();
            return service;
        }

        public void Unregister<T>() where T : class {
            var services = GetMany<T>();
            foreach (var service in services) {
                Unregister(service);
            }
        }

        public void UnregisterMany<T>(IList<T> services) where T : class {
            foreach (var service in services) {
                Unregister(service);
            }
        }

        public IList<T> GetMany<T>() where T : class {
            return _registry.Where(x => x is T).Cast<T>().ToList();
        }

        public T Get<T>() where T : class {
            var type = typeof(T);
            if (!_registryCache.ContainsKey(type)) {
                var service = GetMany<T>().FirstOrDefault();
                if (service != null) {
                    _registryCache[type] = service;
                }
            }
            if (_registryCache.ContainsKey(type))
                return (T) _registryCache[type];
            return null;
        }
    }
}
