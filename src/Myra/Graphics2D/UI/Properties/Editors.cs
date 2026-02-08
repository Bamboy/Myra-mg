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
        private static readonly Type[] ActivatorTypeArgs = { typeof(IInspector), typeof(Record) };
        private static ReadOnlyCollection<EditorTypeRegistry> EditorRegistry;
        private static bool _init;
        public static PropertyEditor Create(IInspector inspector, Record methodInfo)
        {
            if (TryGetEditorTypeForType(methodInfo.Type, out Type editorType))
            {
                var ctor = editorType.GetConstructor(ActivatorTypeArgs);
                if (ctor != null)
                {
                    try
                    {
                        //This also creates the widget
                        object obj = Activator.CreateInstance(editorType, inspector, methodInfo);
                        return obj as PropertyEditor;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
            else
            {
                Console.WriteLine("Could not find property editor for type: "+methodInfo.Type);
            }
            return null;
        }
        
        internal static bool TryGetEditorTypeForType(Type propertyKind, out Type editorType)
        {
            if (!_init)
                InitializeRegistry();

            string nice = propertyKind.GetFriendlyName();
            for (int i = 0; i < EditorRegistry.Count; i++)
            {
                if (EditorRegistry[i].CanEditType(nice))
                {
                    editorType = EditorRegistry[i].EditorType;
                    return true;
                }
            }
            editorType = null;
            return false;
        }
        
        public static void InitializeRegistry(params Assembly[] fromAssemblies)
        {
            List<Assembly> scanAsm;
            if (fromAssemblies == null || fromAssemblies.Length <= 0)
                scanAsm = new List<Assembly> { typeof(Editors).Assembly, Assembly.GetEntryAssembly() };
            else
            {
                scanAsm = new List<Assembly>(fromAssemblies);
                scanAsm.Add(typeof(Editors).Assembly);
                scanAsm.Add(Assembly.GetEntryAssembly());
            }
            
            Reflective_LoadTypeRegistry<PropertyEditor>(
                16,
                scanAsm,
                out List<EditorTypeRegistry> registry
            );

            EditorRegistry = registry.AsReadOnly();
            _init = true;
        }
        
        /// <summary>
        /// Creates all concrete classes that inherit the type, from the specified assemblies. Only creates types with no-argument constructors.
        /// </summary>
        /// <typeparam name="T">The base abstract type</typeparam>
        /// <param name="assemblies">The assemblies to scan for inheritors</param>
        private static void Reflective_LoadTypeRegistry<T>(int predictedCount, IEnumerable<Assembly> assemblies, out List<EditorTypeRegistry> registry) where T : class
        {
            registry = new List<EditorTypeRegistry>(predictedCount);
            foreach (Assembly asm in assemblies)
            {
                // LoadAttributesFromConcreteSubTypes<T>(asm, results);
                foreach (Type type in asm.GetTypes()
                    .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
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