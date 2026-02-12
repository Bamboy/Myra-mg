using System;
using System.Collections.Generic;
using System.Reflection;

namespace Myra.Utility.Types
{
    public static class TypeHelper
    {
        private static Dictionary<Type, Func<TypeInfo>> _lookup;
        internal static void RegisterHelperForType<T>()
        {
            if(_lookup == null)
                _lookup = new Dictionary<Type, Func<TypeInfo>>(16);

            Type helpingType = typeof(T);
            _lookup.Add(helpingType, () => TypeHelper<T>.Info);
        }
        public static bool TryGetInfoForType(Type type, out TypeInfo result)
        {
            if (_lookup != null)
            {
                if (_lookup.TryGetValue(type, out var getterFunc))
                {
                    result = getterFunc.Invoke();
                    return (int)result.Code > 0;
                }
            }
            result = default;
            return false;
        }
        
        /// <summary>
        /// If <typeparamref name="T"/> is <see cref="System.Nullable{}"/>, return the generic type the nullable holds, else return type <typeparamref name="T"/>.
        /// </summary>
        public static void GetNullableTypeOrPassThrough(ref Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = type.GenericTypeArguments[0];
        }
    }
    
    /// <summary>
    /// Provides static helpers and cached info about a generic type <typeparamref name="T"/>.
    /// Accessing a type via this class will load a <see cref="TypeInfo"/> associated with that type.
    /// </summary>
    internal static class TypeHelper<T>
    {
        static TypeHelper()
        {
            _type = typeof(T);
            _info = new TypeInfo(_type);
            TypeHelper.RegisterHelperForType<T>();

            FindMethodTryParse();
        }

        private static readonly Type _type;
        private static readonly TypeInfo _info;
        
        /// <summary>
        /// Cached information about generic type <typeparamref name="T"/>.
        /// </summary>
        public static TypeInfo Info => _info;

        /// <summary>
        /// If <typeparamref name="T"/> is <see cref="System.Nullable{}"/>, return the generic type the nullable holds, else return type <typeparamref name="T"/>.
        /// </summary>
        public static Type GetNullableTypeOrPassThrough()
        {
            if (!_info.IsNullable)
                return _type;
            return _type.GenericTypeArguments[0];
        }
        
        public static bool CanAssign(Type other) => _type.IsAssignableFrom(other);

        private static MethodInfo _tryParse;
        private static void FindMethodTryParse()
        {
            const string METHOD_NAME = "TryParse";
            Type type = typeof(T);
            try
            {
                // public static bool TryParse(string str, out TData value)
                _tryParse = type.GetMethod(METHOD_NAME,
                    BindingFlags.Static | BindingFlags.Public, null,
                    new[] { typeof(string), type.MakeByRefType() }, null);
                
                if (_tryParse == null)
                {
                    throw new Exception($"Reflection Error: '{type.Name}' does not contain a public static method '{METHOD_NAME}'");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Unhandled Reflection Error: {e}");
            }
        }
        
        /// <summary>
        /// Attempts find and invoke <typeparamref name="T"/>'s static TryParse() method using Reflection.
        /// </summary>
        public static bool TryParse(string str, out T data)
        {
            if (_tryParse == null)
                throw new NotSupportedException($"Not supported: {typeof(T)}.TryParse()");
            
            // public static bool TryParse(string str, out TData value)
            // The method's "out TData" gets written to object[] array.
            object[] param = new object[] { str, default(T) }; 
            object result = _tryParse.Invoke(null, param); 
                
            if (result is bool didConvert)
            {
                // Read what the method wrote as "out T"
                data = didConvert ? (T)param[1] : default;
                return didConvert;
            }
            
            // We really might want to throw an exception here instead
            data = default;
            return false;
        }
    }
}