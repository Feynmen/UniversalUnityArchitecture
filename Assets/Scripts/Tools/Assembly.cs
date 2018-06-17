using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Tools.Patterns;
using UnityEngine;

namespace Tools {
    public class Assembly : Singleton<Assembly> {

        private const BindingFlags BINDING_FLAGS_PRIVATE = BindingFlags.NonPublic | BindingFlags.Instance;
        private const BindingFlags BINDING_FLAGS_PUBLIC = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        private readonly Type[] _allTypes = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
        
        public T FindAndNullSafely<T, TAttribute>(T[] array, Predicate<TAttribute> match) where T : MemberInfo where TAttribute : Attribute {
            //Tools.Assert.Instance.IsArgumentNullException(match);
            for (var i = 0; i < array.Length; i++) {
                if (array[i] != null) {
                    var attrs = (TAttribute[]) array[i].GetCustomAttributes(typeof (TAttribute), false);
                    if (attrs.Length > 0 && attrs.Any(t => match(t))) {
                        var result = array[i];
                        array[i] = default(T);
                        return result;
                    }
                }
            }
            return default(T);
        }

        #region Subclasses

        public bool IsSubclass(Type baseType, Type subclassType) {
            if (baseType == null) {
                throw new ArgumentNullException("baseType");
            }
            if (subclassType == null) {
                throw new ArgumentNullException("subclassType");
            }

            // way #1
            //return subclassType.IsSubclassOf(baseType);
            // way #2
            if (baseType.IsInterface)
            {
                return subclassType.GetInterface(baseType.Name) != null;
            }
            else
            {
                var typeToCheck = subclassType.BaseType;
                while (typeToCheck != null)
                {
                    if (typeToCheck == baseType)
                    {
                        return true;
                    }
                    typeToCheck = typeToCheck.BaseType;
                }
                return false;
            }
        }

        //public static bool IsSameOrSubclassOf(Type baseType, Type subclassType)
        //{
        //    if (baseType.IsInterface)
        //    {
        //        return subclassType.GetInterface(baseType.Name) != null;
        //    }
        //    return subclassType.IsSubclassOf(baseType);
        //}

        public List<Type> GetSubclassList<T>(bool inclusiveAbstract = true) {
            return GetSubclassList(typeof (T), inclusiveAbstract);
        }

        public List<Type> GetSubclassList(Type type, bool inclusiveAbstract = true) {
            if (type == null) {
                throw new ArgumentNullException("type");
            }

            var result = new List<Type>();
            for (var i = 0; i < _allTypes.Length; i++) {
                if (_allTypes[i].BaseType == type && (inclusiveAbstract || !_allTypes[i].IsAbstract)) {
                    result.Add(_allTypes[i]);
                }
            }
            return result;
        }

        public List<Type> GetSubclassListThroughHierarchy<T>(bool inclusiveAbstract = true) {
            return GetSubclassListThroughHierarchy(typeof (T), inclusiveAbstract);
        }

        public List<Type> GetSubclassListThroughHierarchy(Type type, bool inclusiveAbstract = true) {
            if (type == null) {
                throw new Exception("Type is null");
            }
            var result = new List<Type>();
            for (var i = 0; i < _allTypes.Length; i++)
            {
                if ((inclusiveAbstract || !_allTypes[i].IsAbstract) && IsSubclass(type, _allTypes[i])) {
                    result.Add(_allTypes[i]);
                }
            }
            return result;
        }

        #endregion

        #region FieldInfo

        public bool IsCustomeAttribute<T, TAttribute>(T info) where T : MemberInfo where TAttribute : Attribute {
            return info.GetCustomAttributes(typeof (TAttribute), false).Length > 0;
        }

        public List<FieldInfo> GetAllPrivateFieldinfoList<TAttribute>(Type type) where TAttribute : Attribute {
            var lst = GetHeirarchyFieldinfoList(type, BINDING_FLAGS_PRIVATE);
            var result = new List<FieldInfo>(lst.Count);
            result.AddRange(lst.Where(IsCustomeAttribute<FieldInfo, TAttribute>));
            return result;
        }

        public List<FieldInfo> GetAllPublicReadonlyFieldinfoList(Type type) {
            var lst = GetHeirarchyFieldinfoList(type, BINDING_FLAGS_PUBLIC);
            var result = new List<FieldInfo>(lst.Count);
            result.AddRange(lst.Where(t => t.IsInitOnly));
            return result;
        }

        public List<FieldInfo> GetHeirarchyFieldinfoList(Type type, BindingFlags bindingFlags) {
            //Tool.Assert.IsArgumentNullException(type);
            // if this root type then parse it else get parent's field list and fill up it
            var list = new List<FieldInfo>();
            var typeToParse = type;
            do {
                var fieldInfos = typeToParse.GetFields(bindingFlags);
                list.AddRange(fieldInfos);
                typeToParse = typeToParse.BaseType;
            } while (typeToParse != null);
            return list;
        }

        #endregion
    }
}