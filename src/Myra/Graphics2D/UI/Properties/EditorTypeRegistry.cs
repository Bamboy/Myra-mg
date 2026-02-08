using System;
using System.Collections.Generic;
using System.Linq;
using Myra.Utility;

namespace Myra.Graphics2D.UI.Properties
{
    public sealed class EditorTypeRegistry
    {
        public readonly Type EditorType;
        public readonly string[] PropertyTypes;

        public EditorTypeRegistry(Type editorType, params Type[] propertyTypes)
        {
            EditorType = editorType;
            PropertyTypes = TypeToString(propertyTypes);
        }

        public bool CanEditType(string value)
        {
            foreach (string other in PropertyTypes)
            {
                if(StringComparer.InvariantCultureIgnoreCase.Equals(other, value))
                    return true;
            }
            return false;
        }

        private static string TypeToString(Type value) => value.GetFriendlyName();
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