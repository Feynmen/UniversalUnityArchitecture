using System;
using UnityEngine;

namespace Core.Atributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class Inject : Attribute
    {
        private readonly Type _injectionType;

        public Inject(Type type, bool isSignalizate = true)
        {
            _injectionType = type;
            if (isSignalizate)
            {
                Debug.LogWarning("You use manual inject of " + _injectionType);
            }
        }

        public Inject()
        {
            _injectionType = null;
        }

        public Type GetInjectionType
        {
            get { return _injectionType; }
        }

        public override string ToString()
        {
            return "Inject "  + _injectionType;
        }
    }
}
