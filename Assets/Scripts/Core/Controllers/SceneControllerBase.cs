using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Atributes;
using Core.Interfaces;
using UnityEngine;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

namespace Core.Controllers
{
    public abstract class SceneControllerBase : MonoBehaviorWrapper, IReleaseble
    {
        [SerializeField] [ShowOnly] private string _sceneName;
        public bool IsReady { private set; get; }
        public Queue<IEnumerator> PreInitExecutionQueue => _preInitExecutionQueue ?? (_preInitExecutionQueue = new Queue<IEnumerator>());
        
        private readonly Dictionary<Type, ControllerBase> _controllers = new Dictionary<Type, ControllerBase>();
        private Queue<IEnumerator> _preInitExecutionQueue;
        private CoreSystem _system;

        #region Unity

        private void Awake()
        {
            InitSystem();
            SelfInjection();
        }

        protected virtual IEnumerator Start()
        {
            ControllersInjection();
            InitControllers();
            while (PreInitExecutionQueue.Count > 0)
            {
                yield return PreInitExecutionQueue.Dequeue();
            }
            yield return Init();
            IsReady = true;
        }

        private void OnDestroy()
        {
            Release();
            foreach (var controller in _controllers)
            {
                (controller.Value as IReleaseble).Release();
            }
            GC.Collect();
        }

        #endregion
        
        public abstract IEnumerator Init();
        
        public void RegisterController(ControllerBase controller)
        {
            var newControllerType = controller.GetType();
            if (!_controllers.ContainsKey(newControllerType))
            {
                InjectObject(controller);
                _controllers.Add(newControllerType, controller);
                (controller as IInitializable).Init();
            }
            else
            {
                Debug.LogError($"[SceneControllerBase] Registration Failed! {newControllerType.Name} already exist in {_sceneName}! ");
            }
        }

        public void UnregisterController(ControllerBase controller)
        {
            _controllers.Remove(controller.GetType());
            (controller as IReleaseble).Release();
        }
        
        public List<T> FindObjectsOnScene<T>() where T : Behaviour
        {
            return Resources.FindObjectsOfTypeAll<T>().Where(x => !string.IsNullOrEmpty(x.gameObject.scene.name) && x.gameObject.scene.name.Equals(_sceneName)).ToList();
        }
        
        public virtual void Release() { }

        #region Injection Methods

        private void SelfInjection()
        {
            var sceneController = FindObjectsOnScene<SceneControllerBase>().FirstOrDefault();
            if (sceneController != null)
            {
                InjectObject(sceneController);
            }
            else
            {
                Debug.LogError($"[SceneControllerBase] Can't find SceneController on {_sceneName} scene, please add it and start again!");
            }
        }

        private void ControllersInjection()
        {
            var controllers = FindObjectsOnScene<ControllerBase>();
            
            foreach (var controller in controllers)
            {
                var controllerType = controller.GetType();
                if (!_controllers.ContainsKey(controllerType))
                {
                    InjectObject(controller);
                    _controllers.Add(controllerType, controller);
                }
                else
                {
                    throw new SystemException($"Controller \"{controllerType.Name}\" in the game object \"{controller.gameObject.name}\" already exist on the scene \"{_sceneName}\"!" +
                                              $" You can't add more than one the same controller in to the same scene!");
                }
                
            }
        }

        private void InjectObject(System.Object objectToInject)
        {
            var type = objectToInject.GetType();
            var injectionObjects = from propertyInfo in type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                   where Attribute.IsDefined(propertyInfo, typeof(Inject), true)
                                   select
                                   new
                                   {
                                       Property = propertyInfo,
                                       Atribute = propertyInfo.GetCustomAttributes(typeof(Inject), true)[0] as Inject
                                   };

            foreach (var obj in injectionObjects.ToList())
            {
                if (obj.Atribute.GetInjectionType == typeof(SceneControllerBase))
                {
                    obj.Property.SetValue(objectToInject, this, null);
                }
                else
                {
                    var injectType = obj.Atribute.GetInjectionType != null
                        ? obj.Atribute.GetInjectionType
                        : obj.Property.PropertyType;
                    
                    var managerObj = _system.GetManager(injectType);
                    obj.Property.SetValue(objectToInject, managerObj, null);
                }
            }
        }

        #endregion

        #region Init System & Controllers

        private void InitSystem()
        {
            _system = FindObjectOfType<CoreSystem>();

            if (_system == null)
            {
                _system  = new GameObject(nameof(ApplicationSystem)).AddComponent<ApplicationSystem>();
                Debug.Log("[CoreSystem] Created " + _system.name);
            }
        }

        private void InitControllers()
        {
            foreach (var controller in _controllers)
            {
                try
                {
                    (controller.Value as IInitializable).Init();
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                    Debug.LogError(controller);
                }
            }
        }

        #endregion

        #region Methods for working with Controllers

        public T Get<T>() where T : ControllerBase
        {
            var type = typeof(T);
            if (_controllers.ContainsKey(type))
            {
                return _controllers[type] as T;
            }
            return default(T);
        }

        public ControllerBase Get(Type controllerType)
        {
            return _controllers.TryGetValue(controllerType, out var controller) ? controller : null;
        }

        public bool TryGet<T>(out T controller) where T : ControllerBase
        {
            controller = Get<T>();
            return controller != null;
        }

        public void Enable<T>() where T : ControllerBase
        {
            if (TryGet(out T controller))
            {
                controller.Enable();
            }
        }

        public void Disable<T>() where T : ControllerBase
        {
            if (TryGet(out T controller))
            {
                controller.Disable();
            }
        }

        public void Disable<T>(bool isDisableAll = false) where T : ControllerBase
        {
            if (isDisableAll)
            {
                _controllers.Where(x => x.Key == typeof(T))
                    .Select(x => x.Value)
                    .ToList()
                    .ForEach(x => x.Disable());
            }
            else
            {
                Disable<T>();
            }
        }

        #endregion
    }
}

