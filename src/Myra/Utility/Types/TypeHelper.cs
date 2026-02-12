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
    }
}