using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Atributes;
using Core.Interfaces;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Core.Controllers
{
    public abstract class SceneControllerBase : MonoBehaviorWraper
    {
        [SerializeField] protected string sceneName;

        private const string DEBUG_APPSYS_PREFAB_PATH = "Prefabs/Debug_ApplicationSystem";
        
        private readonly Dictionary<Type, ControllerBase> _controllers = new Dictionary<Type, ControllerBase>();
        private CoreSystem _system;

        public bool IsReady { private set; get; }

        private Queue<IEnumerator> _preInitExecutionQueue;
        public Queue<IEnumerator> PreInitExecutionQueue
        {
            get
            {
                if (_preInitExecutionQueue == null)
                {
                    _preInitExecutionQueue = new Queue<IEnumerator>();
                }
                return _preInitExecutionQueue;
            }
        }

        public abstract IEnumerator Init();
     
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
            foreach (var controller in _controllers)
            {
                (controller.Value as IReleaseble).Release();
            }
            GC.Collect();
        }

        #region Injection Methods

        private void SelfInjection()
        {
            //TODO Change to this and test
            var sceneController = (from SceneControllerBase cb in FindObjectsOfType(typeof(SceneControllerBase)) where cb.gameObject.scene.name.Equals(sceneName) select cb).FirstOrDefault();
            if (sceneController != null)
            {
                InjectObject(sceneController);
            }
            else
            {
                Debug.LogError("Can't find SceneController on this scene, please add it and start again!");
            }
        }

        private void ControllersInjection()
        {
            var controllers = from ControllerBase cb in FindObjectsOfType(typeof(ControllerBase)) where cb.gameObject.scene.name.Equals(sceneName) select cb;

            foreach (var controller in controllers)
            {
                InjectObject(controller);
                _controllers.Add(controller.GetType(), controller);
            }
        }

        private void InjectObject(System.Object objectToInject)
        {
            var type = objectToInject.GetType();
            var injectionObjects = from propertie in type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                   where Attribute.IsDefined(propertie, typeof(Inject), true)
                                   select
                                   new
                                   {
                                       Property = propertie,
                                       Atribute = propertie.GetCustomAttributes(typeof(Inject), true)[0] as Inject
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
            _system = FindObjectOfType(typeof(CoreSystem)) as CoreSystem;

            if (_system == null)
            {
#if UNITY_EDITOR
                var appSystem = Resources.Load<ApplicationSystem>(DEBUG_APPSYS_PREFAB_PATH);
                Instantiate(appSystem.gameObject);
                _system = appSystem;
                Debug.Log("[CoreSystem] Created " + appSystem.name);
#else
                Debug.LogError("Can't find ApplicationSystem on this scene, please add it and start again!");
                throw new Exception("Can't find ApplicationSystem on this scene, please add it and start again!");
#endif
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
            ControllerBase controller;
            return _controllers.TryGetValue(controllerType, out controller) ? controller : null;
        }

        public bool TryGet<T>(out T controller) where T : ControllerBase
        {
            controller = Get<T>();
            return controller != null;
        }

        public void Enable<T>() where T : ControllerBase
        {
            T controller;
            if (TryGet(out controller))
            {
                controller.Enable();
            }
        }

        public void Disable<T>() where T : ControllerBase
        {
            T controller;
            if (TryGet(out controller))
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

