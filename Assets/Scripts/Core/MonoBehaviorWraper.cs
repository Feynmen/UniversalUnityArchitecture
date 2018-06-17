using System;
using UnityEngine;

namespace Core
{
    public abstract class MonoBehaviorWraper : MonoBehaviour
    {
        private Transform _transform;
        
        public new Transform transform
        {
            get
            {
                if (_transform == null)
                {
                    _transform = GetComponent<Transform>();
                }
                return _transform;
            }
        }

        public virtual bool IsEnabled
        {
            get { return gameObject.activeSelf; }
        }

        public virtual void Enable()
        {
            if (!IsEnabled)
            {
                gameObject.SetActive(true);
            }
        }

        public virtual void Disable()
        {
            if (IsEnabled)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
