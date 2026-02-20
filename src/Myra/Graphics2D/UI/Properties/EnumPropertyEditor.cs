using System;
using Myra.Utility;

namespace Myra.Graphics2D.UI.Properties
{
    [PropertyEditor(typeof(EnumPropertyEditor), typeof(Enum))]
    public sealed class EnumPropertyEditor : PropertyEditor<Enum>
    {
        public EnumPropertyEditor(IInspector owner, Record methodInfo) : base(owner, methodInfo)
        {
            if (!methodInfo.Type.IsEnum)
                throw new TypeLoadException($"Record is not an enum: {methodInfo.Type}");
        }
        
        protected override bool CreatorPicker(out WidgetCreatorDelegate creatorDelegate)
        {
            //TODO - support flags enums
            creatorDelegate = CreateComboView;
            return true;
        }

        private bool CreateComboView(out Widget widget)
        {
            if (_owner.SelectedField == null)
            {
                widget = null;
                return false;
            }
            
            var propertyType = _record.Type;
            var value = _record.GetValue(_owner.SelectedField);

            var isNullable = propertyType.IsNullableEnum();
            var enumType = isNullable ? propertyType.GetNullableType() : propertyType;
            var values = Enum.GetValues(enumType);

            var cv = new ComboView();

            if (isNullable)
            {
                cv.Widgets.Add(new Label
                {
                    Text = string.Empty
                });
            }

            foreach (var v in values)
            {
                cv.Widgets.Add(new Label
                {
                    Text = v.ToString(),
                    Tag = v
                });
            }

            var selectedIndex = Array.IndexOf(values, value);
            if (isNullable)
            {
                ++selectedIndex;
            }
            cv.SelectedIndex = selectedIndex;
            
            if (_record.HasSetter)
            {
                cv.SelectedIndexChanged += (sender, args) =>
                {
                    if (cv.SelectedIndex != -1)
                    { 
                        SetValue(_owner.SelectedField, cv.SelectedItem.Tag);
                    }
                };
            }
            else
            {
                cv.Enabled = false;
            }

            widget = cv;
            return true;
        }
    }
}