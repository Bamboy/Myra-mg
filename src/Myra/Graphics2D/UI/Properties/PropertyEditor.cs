using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Myra.Graphics2D.UI.Properties
{
    /// <summary>
    /// Encapsulates a widget and .Net property or field, for the purposes of display or editing by the user.
    /// </summary>
    public abstract class PropertyEditor : IRecord
    {
        //public static readonly Dictionary<>
        static PropertyEditor()
        {
            
        }
        
        protected readonly IInspector Owner;
        public readonly Record Record;
        //TODO cache attributes ?
        
        public Widget Widget { get; protected set; }
        
        protected PropertyEditor(IInspector owner, Record record)
        {
            Owner = owner;
            Record = record;
            if (TryCreateWidget(out Widget editor))
            {
                Widget = editor;
            }
            else
            {
                throw new Exception();
            }
        }

        protected bool TryCreateWidget(out Widget widget)
        {
            var atts = Record.FindAttributes<Attribute>();
            return TryCreateEditorWidget(Record, out widget, atts);
        }
        protected abstract bool TryCreateEditorWidget(Record record, out Widget widget, params Attribute[] attributes);
        
        Type IRecord.Type => Record.Type;
        object IRecord.GetValue(object field) => Record.GetValue(field);
        void IRecord.SetValue(object field, object value) => Record.SetValue(field, value);
    }
    
    /// <inheritdoc cref="PropertyEditor"/>
    public abstract class PropertyEditor<T> : PropertyEditor, IRecord<T>
    {
        protected delegate bool WidgetCreatorDelegate(Record record, out Widget widget);
        protected abstract bool CreatorPicker(in Attribute[] attributes, out WidgetCreatorDelegate creatorDelegate);
        
        protected override bool TryCreateEditorWidget(Record record, out Widget widget, params Attribute[] attributes)
        {
            if (CreatorPicker(attributes, out var func))
                return func.Invoke(record, out widget);
            widget = null;
            return false;
        }
        
        protected PropertyEditor(IInspector owner, Record record) : base(owner, record)
        {
            
        }
        
        /// <summary>
        /// Gets the value from the field
        /// </summary>
        public virtual T GetValue(object field)
        {
            object o = Record.GetValue(field);
            if (o is T ot)
                return ot;
            return default;
        }
        /// <summary>
        /// Tries to set the value to to the field
        /// </summary>
        public virtual void SetValue(object field, T value)
        {
            Record.SetValue(field, value);
        }
    }

    public sealed class BooleanEditor : PropertyEditor<bool>
    {
        public BooleanEditor(IInspector owner, Record record) : base(owner, record)
        {
            
        }
        
        protected override bool CreatorPicker(in Attribute[] attributes, out WidgetCreatorDelegate creatorDelegate)
        {
            creatorDelegate = CreateCheckBox;
            return true;
        }

        private bool CreateCheckBox(Record record, out Widget widget)
        {
            var propertyType = record.Type;
            bool value = GetValue(Owner.SelectedField);
            
            var cb = new CheckButton
            {
                IsChecked = value
            };
            
            if (Record.HasSetter)
            {
                cb.Click += (sender, args) =>
                {
                    SetValue(Owner.SelectedField, cb.IsChecked);
                    Owner.FireChanged(propertyType.Name);
                };
            }
            else
            {
                cb.Enabled = false;
            }

            widget = cb;
            return true;
        }
    }
}