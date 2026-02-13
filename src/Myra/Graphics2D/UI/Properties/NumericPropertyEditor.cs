using System;
using Generic.Math;
using Myra.Events;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using Myra.Utility.Types;

namespace Myra.Graphics2D.UI.Properties
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <typeparam name="TNum"></typeparam>
	[PropertyEditor(typeof(NumericPropertyEditor<>), 
		typeof(byte), typeof(sbyte), typeof(byte?), typeof(sbyte?),
		typeof(short), typeof(ushort), typeof(short?), typeof(ushort?), 
		typeof(int), typeof(uint), typeof(int?), typeof(uint?), 
		typeof(long), typeof(ulong), typeof(long?), typeof(ulong?),
		typeof(float), typeof(float?), typeof(double), typeof(double?), 
		typeof(decimal), typeof(decimal?))]
	public sealed class NumericPropertyEditor<TNum> : PropertyEditor, INumberTypeRef<TNum> where TNum : struct
	{
		private Type _type;
		private bool _nullable;
		private bool _valueConvert;
		
	    public NumericPropertyEditor(IInspector owner, Record methodInfo) : base(owner, methodInfo)
	    {
	    }
	    protected override void Initialize()
	    {
		    _type = _record.Type;
		    _nullable = _type.IsNullablePrimitive();

		    Type selfType = typeof(TNum);
		    _valueConvert = _type == typeof(byte) | _type == typeof(sbyte) | _type == typeof(byte?) | _type == typeof(sbyte?);
		    _valueConvert |= selfType == typeof(byte) | selfType == typeof(sbyte) | selfType == typeof(byte?) | selfType == typeof(sbyte?);
	    }

	    protected override bool TryCreateEditorWidget(out Widget widget)
	    {
		    return CreateNumericEditor(out widget);
	    }
	    
	    private bool CreateNumericEditor(out Widget widget)
        {
	        if (_owner.SelectedField == null)
	        {
		        widget = null;
		        return false;
	        }
	        
	        object obj = _record.GetValue(_owner.SelectedField);
	        TNum? convert;
	        if (_nullable)
	        {
		        convert = (TNum?)obj;
	        }
	        else
	        {
		        if (obj == null)
			        convert = MathHelper<TNum>.Zero;
		        else
			        convert = (TNum)obj;
	        }

	        if (_valueConvert)
	        {
		        widget = CreateByteDodge(convert);
	        }
	        else
	        {
		        widget = CreateNativeType(convert);
	        }
	        return true;
        }

        private Widget CreateNativeType(TNum? val)
        {
	        var spinButton = new SpinButton<TNum>()
	        {
		        Nullable = _nullable,
		        Value = val,
	        };

	        if (_record.HasSetter)
	        {
		        spinButton.ValueChanged += (sender, args) =>
		        {
			        try
			        {
				        if (IsNullable)
					        SetValue(_owner.SelectedField, args.NewValue);
				        else
					        SetValue(_owner.SelectedField, args.NewValue.GetValueOrDefault());

				        _owner.FireChanged(_record.Name);
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
	        return spinButton;
        }

        private Widget CreateByteDodge(TNum? val)
        {
	        // val is byte or sbyte which doesn't have math ops
	        var spinButton = new SpinButton<short>()
	        {
		        Nullable = _nullable,
		        Value = GenericMath<TNum?, short>.Convert(val),
		        Minimum = 0,
		        Maximum = 255,
	        };

	        if (_record.HasSetter)
	        {
		        spinButton.ValueChanged += (sender, args) =>
		        {
			        try
			        {
				        short? newShort = args.NewValue;
				        if (!newShort.HasValue)
				        {
					        if(IsNullable)
					        {
						        SetValue(_owner.SelectedField, null);
						        _owner.FireChanged(_record.Name);
						        return;
					        }
					        else
					        {
						        SetValue(_owner.SelectedField, GenericMath<TNum>.Zero);
						        _owner.FireChanged(_record.Name);
						        return;
					        }
				        }
				        
				        newShort = MathHelper<short>.Clamp(newShort.Value, 0, 255);
				        SetValue(_owner.SelectedField, GenericMath<short, TNum>.Convert(newShort.Value));
				        _owner.FireChanged(_record.Name);
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
	        return spinButton;
        }
        
	    public TNum GetValue(object field) => (TNum)_record.GetValue(field);
	    public void SetValue(object field, TNum value) => base.SetValue(field, value);
	    public bool IsNullable => _nullable;
    }
}