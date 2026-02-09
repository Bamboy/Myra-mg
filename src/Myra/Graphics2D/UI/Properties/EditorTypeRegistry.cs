using System;
using System.Collections.Generic;
using System.Linq;
using Myra.Utility;

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

        public bool CanEditType(Type value)
        {
            for (int i = 0; i < Types.Length; i++)
            {
                if (Types[i].IsInterface && Types[i].IsAssignableFrom(value))
                    return true;
            }
            return CanEditType( TypeToString(value) );
        }
        public bool CanEditType(string value)
        {
            foreach (string other in TypeNames)
            {
                if(StringComparer.InvariantCultureIgnoreCase.Equals(other, value))
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