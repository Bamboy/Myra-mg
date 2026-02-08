namespace Myra.Graphics2D.UI.Properties
{
    [PropertyEditor(typeof(BooleanPropertyEditor), typeof(bool))]
    public sealed class BooleanPropertyEditor : PropertyEditor<bool>
    {
        public BooleanPropertyEditor(IInspector owner, Record methodInfo) : base(owner, methodInfo)
        {
            
        }
        
        protected override bool CreatorPicker(out WidgetCreatorDelegate creatorDelegate)
        {
            creatorDelegate = CreateCheckBox;
            return true;
        }

        private bool CreateCheckBox(out Widget widget)
        {
            if (_owner.SelectedField == null)
            {
                widget = null;
                return false;
            }

            var propertyType = _record.Type;
            bool value = GetValue(_owner.SelectedField);
            
            var cb = new CheckButton
            {
                IsChecked = value
            };
            
            if (_record.HasSetter)
            {
                cb.Click += (sender, args) =>
                {
                    SetValue(_owner.SelectedField, cb.IsChecked);
                    _owner.FireChanged(propertyType.Name);
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