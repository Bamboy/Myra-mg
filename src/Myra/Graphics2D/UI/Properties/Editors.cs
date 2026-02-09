using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Myra.Utility;

namespace Myra.Graphics2D.UI.Properties
{
    internal static class Editors
    {
        private static ReadOnlyCollection<EditorTypeRegistry> _registry;
        private static bool _init;
        
        internal static bool TryGetEditorTypeForType(Type propertyKind, out Type editorType)
        {
            if (!_init)
                InitializeRegistry();

            string str = EditorTypeRegistry.TypeToString(propertyKind);
            for (int i = 0; i < _registry.Count; i++)
            {
                if (_registry[i].CanEditType(str))
                {
                    editorType = _registry[i].EditorType;
                    return true;
                }
            }
            
            for (int i = 0; i < _registry.Count; i++)
            {
                if (_registry[i].CanEditType(propertyKind))
                {
                    editorType = _registry[i].EditorType;
                    return true;
                }
            }
            //typeof(IList).IsAssignableFrom(propertyKind)
            editorType = null;
            return false;
        }
        
        /// <inheritdoc cref="PropertyEditor.InitializeRegistry"/>
        internal static void InitializeRegistry(int predictedCount = 16, params Assembly[] fromAssemblies)
        {
            Assembly[] scanAsm;
            if (fromAssemblies == null || fromAssemblies.Length <= 0)
                scanAsm = AppDomain.CurrentDomain.GetAssemblies(); //this scans way more assemblies than needed, but works
            else
            {
                scanAsm = fromAssemblies;
                if(!fromAssemblies.Contains(typeof(Editors).Assembly))
                    Console.WriteLine("PropertyEditor.InitializeRegistry() warning: Myra's Assembly was not included.");
                //maybe ensure fromAssemblies contains Myra (this) assembly?
            }
            
            Reflective_LoadTypeRegistry(predictedCount, scanAsm, out List<EditorTypeRegistry> registry);

            _registry = registry.AsReadOnly();
            _init = true;
        }
        
        /// <summary>
        /// Generate <see cref="EditorTypeRegistry"/> objects for each concrete <see cref="PropertyEditor"/> implementation found.
        /// That implementation must also have a <see cref="PropertyEditorAttribute"/>.
        /// </summary>
        private static void Reflective_LoadTypeRegistry(int predictedCount, IEnumerable<Assembly> assemblies, out List<EditorTypeRegistry> registry)
        {
            registry = new List<EditorTypeRegistry>(predictedCount);
            foreach (Assembly asm in assemblies)
            {

                foreach (Type type in asm.GetTypes()
                    .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(PropertyEditor)) ))
                {
                    PropertyEditorAttribute att = type.FindAttribute<PropertyEditorAttribute>();
                    if (att == null)
                        continue;
                    EditorTypeRegistry reg = att.GetRegistry();
                    if(reg == null)
                        continue;
                    registry.Add(reg);
                }
            }
        }
        
    }
}