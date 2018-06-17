using System.Collections;
using UnityEngine;

namespace Core
{
    public class ApplicationSystem : CoreSystem
    {
        private static ApplicationSystem _instance;
        private bool _isInited;
        
        #region Unity methods

        private void Awake()
        {
            if (!_isInited)
            {
                SystemInit();
            }
        }

        private void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            DontDestroyOnLoad(gameObject);
        }
        
        #endregion

        public void SystemInit()
        {
            if (_instance != null)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _isInited = true;
                _instance = this;
                Init();
            }
        }

        public static Coroutine StartMainCoroutine(IEnumerator enumerator)
        {
            return _instance.StartCoroutine(enumerator);
        }
    }
}
