using System;
using System.Collections.Generic;
using System.Linq;
using Core.Managers;
using Core.Services;
using Tools;
using UnityEngine;

namespace Core
{
    public class CoreSystem : MonoBehaviorWrapper
    {
        [SerializeField] private List<ManagerInfo> _managersInfo = new List<ManagerInfo>();
        
        public static ServiceBase[] Services { get; }
        private static readonly Dictionary<Type, IManager> _managers = new Dictionary<Type, IManager>();

        static CoreSystem()
        {
            var managerTypes = ToolsGetter.Assembly.GetSubclassListThroughHierarchy<IManager>(false);
            foreach (var managerType in managerTypes)
            {
                var manager = (IManager)Activator.CreateInstance(managerType);
                _managers.Add(managerType, manager);
            }
            
            var services = ToolsGetter.Assembly.GetSubclassListThroughHierarchy<ServiceBase>(false);
            Services = new ServiceBase[services.Count];
            for (var i = 0; i < services.Count; i++)
            {
                Services[i] = (ServiceBase)Activator.CreateInstance(services[i]);
                Debug.Log($"[CoreSystem] Service ({services[i].Name}) Created!");
            }
        }

        protected void Init()
        {
            foreach (var manager in _managers)
            {
                manager.Value.Init();
                _managersInfo.Add(new ManagerInfo() { Name = manager.Key.Name });
            }
            
            foreach (var service in Services)
            {
                service.Init();
            }
        }

        public T GetManager<T>() where T : IManager
        {
            var type = typeof(T);
            return (T) GetManager(type);
        }

        public object GetManager(Type type)
        {
            var manager = _managers.FirstOrDefault(x => x.Key == type || x.Key.GetInterfaces().Contains(type)).Value;
            return manager;
        }

        protected void DisposeAllManagers()
        {
            _managers.ToList().ForEach(x =>
            {
                if (x.Value != null)
                {
                    x.Value.Release();
                }
            });
        }

        private void OnApplicationQuit()
        {
            DisposeAllManagers();
        }

        [Serializable]
        private class ManagerInfo
        {
            [HideInInspector] public string Name;
        }
    }
}
