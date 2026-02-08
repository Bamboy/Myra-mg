using System;

namespace Myra.Graphics2D.UI.Properties
{
    /// <summary>
    /// Attribute that ties a concrete <see cref="PropertyEditor"/> to one or more property types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PropertyEditorAttribute : Attribute
    {
        private readonly Type attached;
        private readonly Type[] editTypes;
        
        public PropertyEditorAttribute(Type attached, params Type[] editTypes)
        {
            this.attached = attached;
            this.editTypes = editTypes;
        }
        public EditorTypeRegistry GetRegistry() => new EditorTypeRegistry(attached, editTypes);
    }
    
    /// <summary>
    /// Encapsulates a widget and .Net property or field, for the purposes of display or editing by the user.
    /// </summary>
    public abstract class PropertyEditor : IRecord
    {
        protected delegate bool WidgetCreatorDelegate(out Widget widget);
        
        protected readonly IInspector _owner;
        protected readonly Record _record;
        
        public Widget Widget { get; protected set; }
        
        /// <summary>
        /// Creates a new widget attached to the given Record
        /// </summary>
        protected PropertyEditor(IInspector owner, Record methodInfo)
        {
            _owner = owner;
            _record = methodInfo;
            if (TryCreateWidget(out Widget editor))
                Widget = editor;
        }

        private bool TryCreateWidget(out Widget widget) => TryCreateEditorWidget(out widget);
        protected abstract bool TryCreateEditorWidget(out Widget widget);
        
        public Type Type => _record.Type;
        object IRecord.GetValue(object field) => _record.GetValue(field);
        public void SetValue(object field, object value)
        {
            _record.SetValue(field, value);
            _owner.FireChanged(_record.Name);
        }
    }
    
    /// <inheritdoc cref="PropertyEditor"/>
    public abstract class PropertyEditor<T> : PropertyEditor, IRecord<T>
    {
        protected abstract bool CreatorPicker(out WidgetCreatorDelegate creatorDelegate);
        
        protected override bool TryCreateEditorWidget(out Widget widget)
        {
            if (CreatorPicker(out var func))
                return func.Invoke(out widget);
            widget = null;
            return false;
        }
        
        protected PropertyEditor(IInspector owner, Record methodInfo) : base(owner, methodInfo)
        {
            
        }
        
        /// <summary>
        /// Gets the value from the field
        /// </summary>
        public virtual T GetValue(object field)
        {
            object o = _record.GetValue(field);
            if (o is T ot)
                return ot;
            return default;
        }
        /// <summary>
        /// Tries to set the value to to the field
        /// </summary>
        public virtual void SetValue(object field, T value)
        {
            _record.SetValue(field, value);
            _owner.FireChanged(_record.Name);
        }
    }
}