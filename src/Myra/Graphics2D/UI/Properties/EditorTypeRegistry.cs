using System;
using System.Collections.Generic;
using System.Linq;
using Myra.Utility;
using Myra.Utility.Types;

namespace Myra.Graphics2D.UI.Properties
{
    internal sealed class EditorTypeRegistry
    {
        private readonly Type _editorType;
        private readonly Type[] _types;
        private readonly string[] _typeNames;
        public Type EditorType => _editorType;
        
        /// <summary>
        /// Return true if the editor type is Generic and not yet complete.
        /// </summary>
        public bool IsOpenGenericType => _editorType.IsGenericTypeDefinition;
        
        public EditorTypeRegistry(Type editorType, params Type[] propertyTypes)
        {
            _editorType = editorType;
            _types = propertyTypes;
            _typeNames = TypeToString(propertyTypes);
        }

        /// <summary>
        /// Returns true if this editor type can support <paramref name="type"/>.
        /// </summary>
        /// <param name="allowCasts">Allow casting <paramref name="type"/> to an intermediate type? (Like interfaces)</param>
        public bool CanEditType(Type type, bool allowCasts = true)
        {
            if (!allowCasts)
            {
                for (int i = 0; i < _types.Length; i++)
                {
                    if (object.ReferenceEquals(_types[i], type))
                        return true;
                }
                return CanEditType( TypeToString(type) );
            }
            
            for (int i = 0; i < _types.Length; i++)
            {
                Type supported = _types[i];
                if (object.ReferenceEquals(supported, type))
                    return true;
                if (supported.IsInterface && supported.IsAssignableFrom(type))
                    return true;
            }
            return CanEditType( TypeToString(type) );
        }
        public bool CanEditType(string value)
        {
            foreach (string supported in _typeNames)
            {
                if(StringComparer.InvariantCultureIgnoreCase.Equals(supported, value))
                    return true;
            }
            return false;
        }
        
        //TODO use a less expensive conversion here.
        //We only use string for the purpose of testing type/type equality across assembly
        internal static string TypeToString(Type value) => value.GetFriendlyName(); 
        private static string[] TypeToString(params Type[] args)
        {
            if (args == null || args.Length == 0)
                return null;
            HashSet<string> result = new HashSet<string>();
            for (int i = 0; i < args.Length; i++)
            {
                result.Add(TypeToString(args[i]));
            }
            return result.ToArray();
        }
    }
}