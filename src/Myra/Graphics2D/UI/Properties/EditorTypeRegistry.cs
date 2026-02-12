using System;
using System.Collections.Generic;
using System.Linq;
using Myra.Utility;
using Myra.Utility.Types;

namespace Myra.Graphics2D.UI.Properties
{
    public sealed class EditorTypeRegistry
    {
        public readonly Type EditorType;
        private readonly Type[] Types;
        private readonly string[] TypeNames;

        public EditorTypeRegistry(Type editorType, params Type[] propertyTypes)
        {
            EditorType = editorType;
            Types = propertyTypes;
            TypeNames = TypeToString(propertyTypes);
        }

        public bool CanEditType(Type type, bool allowCasts = true)
        {
            if (!allowCasts)
            {
                for (int i = 0; i < Types.Length; i++)
                {
                    if (object.ReferenceEquals(Types[i], type))
                        return true;
                }
                return CanEditType( TypeToString(type) );
            }
            
            for (int i = 0; i < Types.Length; i++)
            {
                Type supported = Types[i];
                if (object.ReferenceEquals(supported, type))
                    return true;
                if (supported.IsInterface && supported.IsAssignableFrom(type))
                    return true;
            }
            return CanEditType( TypeToString(type) );
        }
        public bool CanEditType(string value)
        {
            foreach (string supported in TypeNames)
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