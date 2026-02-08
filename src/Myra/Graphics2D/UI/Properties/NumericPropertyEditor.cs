using System;
using Myra.Utility;

namespace Myra.Graphics2D.UI.Properties
{
	[PropertyEditor(typeof(NumericPropertyEditor), 
		typeof(byte), typeof(sbyte), typeof(byte?), typeof(sbyte?),
		typeof(short), typeof(ushort), typeof(short?), typeof(ushort?), 
		typeof(int), typeof(uint), typeof(int?), typeof(uint?), 
		typeof(long), typeof(ulong), typeof(long?), typeof(ulong?),
		typeof(float), typeof(float?), typeof(double), typeof(double?))]
	public sealed class NumericPropertyEditor : PropertyEditor 
    {
	    public NumericPropertyEditor(IInspector owner, Record methodInfo) : base(owner, methodInfo)
	    {
            
	    }
	    
	    protected override bool TryCreateEditorWidget(out Widget widget)
	    {
		    return CreateNumericEditor(_record, out widget);
		    //if (CreatorPicker(attributes, out var func))
			//    return func.Invoke(methodInfo, out widget);
		    widget = null;
		    return false;
	    }
	    
	    private bool CreateNumericEditor(Record record, out Widget widget)
        {
	        if (_owner.SelectedField == null)
	        {
		        widget = null;
		        return false;
	        }
	        
	        var propertyType = record.Type;
        	object value = record.GetValue(_owner.SelectedField);

        	var numericType = propertyType;
        	if (propertyType.IsNullablePrimitive())
        	{
        		numericType = propertyType.GetNullableType();
        	}

        	var spinButton = new SpinButton
        	{
        		Integer = numericType.IsNumericInteger(),
        		Nullable = propertyType.IsNullablePrimitive(),
        		Value = value != null ? (float)Convert.ChangeType(value, typeof(float)) : default(float?)
        	};

        	if (_record.HasSetter)
        	{
        		spinButton.ValueChanged += (sender, args) =>
        		{
        			try
        			{
        				object result;

        				if (spinButton.Value != null)
        				{
        					result = Convert.ChangeType(spinButton.Value.Value, numericType);
        				}
        				else
        				{
        					result = null;
        				}

        				SetValue(_owner.SelectedField, result);

        				if (record.Type.IsValueType)
        				{
        					// Handle structs
        					/*
					        var tg = this;
        					var pg = tg._parentGrid;
        					while (pg != null && tg._parentProperty != null && tg._parentProperty.Type.IsValueType)
        					{
        						tg._parentProperty.SetValue(pg._object, tg._object);

        						if (!tg._parentProperty.Type.IsValueType)
        						{
        							break;
        						}

        						tg = pg;
        						pg = tg._parentGrid;
        					}*/
        				}

        				_owner.FireChanged(record.Name);
        			}
        			catch (InvalidCastException)
        			{
        				// TODO: Rework this ugly type conversion solution
        			}
        			catch (Exception ex)
        			{
        				spinButton.Value = args.OldValue;
        				var dialog = Dialog.CreateMessageBox("Error", ex.ToString());
        				dialog.ShowModal(_owner.Desktop);
        			}
        		};
        	}
        	else
        	{
        		spinButton.Enabled = false;
        	}

	        widget = spinButton;
	        return true;
        }
        
    }
}