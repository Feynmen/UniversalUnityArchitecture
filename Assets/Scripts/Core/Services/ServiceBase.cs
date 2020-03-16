using System.Collections.Generic;
using Core.Controllers;
using Core.Interfaces;
using UnityEngine;

namespace Core.Services
{
    public abstract class ServiceBase : IInitializable, IReleaseble, IService
    {
        public string Id
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                {
                    _id = GetType().Name;
                }

                return _id;
            }
        }

        private static ServiceExecuter _serviceExecuter;

        private static ServiceExecuter ServiceExecuterInstance
        {
            get
            {
                if (_serviceExecuter == null)
                {
                    _serviceExecuter = new GameObject(nameof(ServiceExecuter)).AddComponent<ServiceExecuter>();
                }

                return _serviceExecuter;
            }
        }

        private string _id;

        public abstract void Init();

        public virtual void Release()
        {
            ServiceExecuterInstance.Release();
        }

        protected abstract void OnRegister(SceneControllerBase sceneController);
        protected abstract void OnUnregister(SceneControllerBase sceneController);

        public void Register(SceneControllerBase sceneController)
        {
            ServiceExecuterInstance.SubscribeScene(this, sceneController);
            OnRegister(sceneController);
        }

        public void Unregister(SceneControllerBase sceneController)
        {
            ServiceExecuterInstance.UnsubscribeScene(this, sceneController);
            OnUnregister(sceneController);
        }


        private class ServiceExecuter : MonoBehaviour, IReleaseble
        {
            [SerializeField] private List<string> _services;
            
            private readonly Dictionary<string, Service> _servicesDictionary = new Dictionary<string, Service>();

            private bool _isReleased;
            
            private void Awake()
            {
                DontDestroyOnLoad(gameObject);
            }

            public void SubscribeScene(ServiceBase serviceBase, SceneControllerBase controllerBase)
            {
                if (!_servicesDictionary.ContainsKey(serviceBase.Id))
                {
                    _servicesDictionary.Add(serviceBase.Id,new Service(serviceBase));
                    if (_services == null)
                    {
                        _services = new List<string>();
                    }
                    _services.Add(serviceBase.Id);
                }
                _servicesDictionary[serviceBase.Id].SubscribeScene(controllerBase);
            }
            
            public void UnsubscribeScene(ServiceBase serviceBase, SceneControllerBase controllerBase)
            {
                if (_servicesDictionary.TryGetValue(serviceBase.Id, out var service))
                {
                    service.UnsubscribeScene(controllerBase);
                }
            }

            private void Update()
            {
                foreach (var services in _servicesDictionary.Values)
                {
                    services.UpdateService();
                }
            }

            private void OnDestroy()
            {
                Release();
            }

            public void Release()
            {
                if (_isReleased)
                {
                    return;
                }
                foreach (var service in _servicesDictionary.Values)
                {
                    service.Release();
                }
                _servicesDictionary.Clear();
                _services?.Clear();
                _isReleased = true;
            }
            
            private readonly struct Service : IReleaseble
            {
                public string Id { get; }
                public ServiceBase ServiceObject { get; }
                public IUpdaterService UpdaterObject { get; }

                private readonly HashSet<SceneControllerBase> _sceneControllers;
                
                public Service(ServiceBase service)
                {
                    Id = service.Id;
                    ServiceObject = service;
                    UpdaterObject = service as IUpdaterService;
                    _sceneControllers = new HashSet<SceneControllerBase>();
                }

                public void SubscribeScene(SceneControllerBase controllerBase)
                {
                    _sceneControllers.Add(controllerBase);
                }

                public void UnsubscribeScene(SceneControllerBase controllerBase)
                {
                    _sceneControllers.Remove(controllerBase);
                }

                public void UpdateService()
                {
                    if (UpdaterObject == null)
                    {
                        return;
                    }

                    foreach (var sceneControllerBase in _sceneControllers)
                    {
                        UpdaterObject.Update(sceneControllerBase);
                    }
                }
                
                public void Release()
                {
                    _sceneControllers.Clear();
                }

                #region Equals members

                public bool Equals(ServiceBase other)
                {
                    return Id.Equals(other.GetType().Name);
                }
                
                public bool Equals(Service other)
                {
                    return Id == other.Id;
                }

                public override bool Equals(object obj)
                {
                    return obj is Service other && Equals(other) || obj is ServiceBase serviceBase && Equals(serviceBase);
                }

                public override int GetHashCode()
                {
                    return (Id != null ? Id.GetHashCode() : 0);
                }

                #endregion
                
                public override string ToString()
                {
                    return $"{nameof(Id)}: {Id}";
                }

            }
        }
    }
}