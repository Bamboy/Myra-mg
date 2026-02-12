using System;
using Generic.Math;
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
	    public NumericPropertyEditor(IInspector owner, Record methodInfo) : base(owner, methodInfo)
	    {
            if(methodInfo == null)
	            throw new NullReferenceException(nameof(methodInfo));
            //if (typeof(TNum) != methodInfo.Type)
	        //    throw new ArgumentException($"Type mismatch: Record '{methodInfo.Type}', Generic '{typeof(TNum)}'");
	    }
	    
	    protected override bool TryCreateEditorWidget(out Widget widget)
	    {
		    return CreateNumericEditor(_record, out widget);
	    }
	    
	    private bool CreateNumericEditor(Record record, out Widget widget)
        {
	        if (_owner.SelectedField == null)
	        {
		        widget = null;
		        return false;
	        }
	        
	        Type type = record.Type;
	        bool isNullable = type.IsNullablePrimitive();
	        
	        object obj = record.GetValue(_owner.SelectedField);
	        //TypeHelper.GetNullableTypeOrPassThrough(ref type);
	        TNum? convert;
	        if (isNullable)
	        {
		        convert = (TNum?)obj;
	        }
	        else
	        {
		        convert = (TNum)obj;
	        }
	        //obj = Convert.ChangeType(obj, type);
	        
	        var spinButton = new SpinButton<TNum>()
        	{
        		Nullable = isNullable,
		        Value = convert,
        	};

        	if (_record.HasSetter)
        	{
        		spinButton.ValueChanged += (sender, args) =>
        		{
        			try
        			{
				        if(IsNullable)
							SetValue(_owner.SelectedField, args.NewValue);
				        else
					        SetValue(_owner.SelectedField, args.NewValue.GetValueOrDefault());

        				_owner.FireChanged(record.Name);
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
	    
	    public TNum GetValue(object field) => (TNum)_record.GetValue(field);
	    public void SetValue(object field, TNum value) => base.SetValue(field, value);
	    public bool IsNullable => TypeHelper<TNum>.Info.IsNullable;
    }
}