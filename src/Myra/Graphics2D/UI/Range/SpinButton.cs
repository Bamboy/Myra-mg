using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;
using Generic.Math;
using Myra.Events;
using Myra.Utility.Types;

#if MONOGAME || FNA
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Input;
#else
using Myra.Platform;
#endif

namespace Myra.Graphics2D.UI
{
	public class SpinButton<TNum> : Widget where TNum : struct
	{
		private readonly GridLayout _layout = new GridLayout();
		private readonly TextBox _textField;
		private readonly Button _upButton;
		private readonly Button _downButton;
		private int _decimalPlaces;
		private TNum _increment;
		private Range<TNum> _range;

		[Category("Behavior")]
		[DefaultValue(false)]
		public bool Nullable { get; set; }

		[DefaultValue(HorizontalAlignment.Left)]
		public override HorizontalAlignment HorizontalAlignment
		{
			get
			{
				return base.HorizontalAlignment;
			}
			set
			{
				base.HorizontalAlignment = value;
			}
		}

		[DefaultValue(VerticalAlignment.Top)]
		public override VerticalAlignment VerticalAlignment
		{
			get
			{
				return base.VerticalAlignment;
			}
			set
			{
				base.VerticalAlignment = value;
			}
		}
		
		[Category("Behavior")]
		[DefaultValue(null)]
		public TNum? Minimum { get => _range.Min; set => _range.Min = value; }
		
		[Category("Behavior")]
		[DefaultValue(null)]
		public TNum? Maximum { get => _range.Max; set => _range.Max = value; }
		
		[Category("Behavior")]
		[DefaultValue(null)]
		public TNum? Value
		{
			get
			{
				if (string.IsNullOrEmpty(_textField.Text))
				{
					return Nullable ? default(TNum?) : GenericMathExtra<TNum>.Zero;
				}

				if(TypeHelper<TNum>.TryParse(_textField.Text, out TNum result))
				{
					return result;
				}

				return null;
			}

			set
			{
				if (value == null && !Nullable)
				{
					throw new Exception("value can't be null when Nullable is false");
				}

				if (value.HasValue)
				{
					value = _range.Clamp(value.Value);
				}

				if (FixedNumberSize)
				{
					throw new NotImplementedException();
					string MajorString = "";
					int k = 0;
					int k2 = 0;
					if (Maximum.HasValue)
					{
						k = GenericMathExtra<TNum>.Abs(Maximum.Value).ToString().Length;
					}
					if (Minimum.HasValue)
					{
						k2 = GenericMathExtra<TNum>.Abs(Minimum.Value).ToString().Length;
					}
					k = k > k2 ? k : k2;
					for (int i = 0; i < k; i++)
					{
						MajorString += "0";
					}
					if (value.HasValue && GenericMath.GreaterThanOrEqual(value.Value, GenericMathExtra<TNum>.Zero))
					{
						MajorString = " " + MajorString;
					}
					string MinorString = ".";
					for (int i = 0; i < _decimalPlaces; i++)
					{
						MinorString += "0";
					}
					//_textField.Text = value.HasValue ? value.Value.ToString(MajorString + MinorString) : string.Empty;
				}
				else
				{
					_textField.Text = value.HasValue ? value.Value.ToString() : string.Empty;
				}

				if (_textField.Text != null)
				{
					_textField.CursorPosition = 0;
				}
			}
		}

		[Category("Behavior")]
		[DefaultValue(1)]
		public TNum Increment
		{
			get
			{
				return _increment;
			}
			set
			{
				_increment = value;
			}
		}
		
		[Category("Behavior")]
		[DefaultValue(1)]
		public TNum Mul_Increment { get; set; }

		[Category("Behavior")]
		[DefaultValue(0)]
		public int DecimalPlaces
		{
			get
			{
				return _decimalPlaces;
			}

			set
			{
				if (TypeHelper<TNum>.Info.IsWholeNumber)
				{
					_decimalPlaces = 0;
				}
				else
				{
					_decimalPlaces = value;
				}
			}
		}

		[Category("Behavior")]
		[DefaultValue(false)]
		public bool FixedNumberSize { get; set; }

		[XmlIgnore]
		[Browsable(false)]
		public TextBox TextBox => _textField;

		protected internal override bool AcceptsMouseWheel => true;

		/// <summary>
		/// Fires when the value is about to be changed
		/// Set Cancel to true if you want to cancel the change
		/// </summary>
		public event EventHandler<ValueChangingEventArgs<TNum?>> ValueChanging;

		/// <summary>
		/// Fires when the value had been changed
		/// </summary>
		public event EventHandler<ValueChangedEventArgs<TNum?>> ValueChanged;

		/// <summary>
		/// Fires only when the value had been changed by user(doesnt fire if it had been assigned through code)
		/// </summary>
		public event EventHandler<ValueChangedEventArgs<TNum?>> ValueChangedByUser;

		public SpinButton(string styleName = Stylesheet.DefaultStyleName)
		{
			ChildrenLayout = _layout;
			AcceptsKeyboardFocus = true;

			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;

			_layout.ColumnsProportions.Add(new Proportion(ProportionType.Fill));
			_layout.ColumnsProportions.Add(new Proportion());

			_layout.RowsProportions.Add(new Proportion());
			_layout.RowsProportions.Add(new Proportion());

			_textField = new TextBox
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				TextVerticalAlignment = VerticalAlignment.Center,
				AcceptsKeyboardFocus = false
			};
			Grid.SetRowSpan(_textField, 2);

			_textField.ValueChanging += _textField_ValueChanging;

			_textField.TextChanged += TextBoxOnTextChanged;
			_textField.TextChangedByUser += TextBoxOnTextChangedByUser;

			Children.Add(_textField);

			_upButton = new Button
			{
				Content = new Image
				{
					VerticalAlignment = VerticalAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center
				}
			};
			Grid.SetColumn(_upButton, 1);
			_upButton.Click += UpButtonOnUp;

			Children.Add(_upButton);

			_downButton = new Button
			{
				Content = new Image
				{
					VerticalAlignment = VerticalAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center
				}
			};
			Grid.SetColumn(_downButton, 1);
			Grid.SetRow(_downButton, 1);

			_downButton.Click += DownButtonOnUp;
			Children.Add(_downButton);

			SetStyle(styleName);
			
			Value = GenericMathExtra<TNum>.Zero;
			Increment = GenericMathExtra<TNum>.One;
			Mul_Increment = GenericMathExtra<TNum>.One;
		}

		private static TNum? StringToNumber(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return null;
			}

			if (TypeHelper<TNum>.TryParse(str, out TNum value))
			{
				return value;
			}

			return null;
		}

		private string NumberToString(TNum? value)
		{
			if (value.HasValue)
			{
				return value.Value.ToString();
			}

			if (Nullable)
			{
				return string.Empty;
			}
			// Default value
			return "0";
		}

		private void _textField_ValueChanging(object sender, ValueChangingEventArgs<string> e)
		{
			var str = e.NewValue;
			if (string.IsNullOrEmpty(str))
			{
			}
			else if (str == "-")
			{
				// Allow prefix 'minus' only if Minimum lower than zero
				if (Minimum.HasValue && GenericMath.GreaterThanOrEqual(Minimum.Value, GenericMathExtra<TNum>.Zero))
				{
					e.Cancel = true;
				}
			}
			else
			{
				TNum? newValue = null;
				if (TypeHelper<TNum>.TryParse(str, out TNum num) && _range.IsInRange(num))
				{
					newValue = num;
				}
				else
				{
					e.Cancel = true;
				}
				/*
				if (Integer)
				{
					int i;
					if (!int.TryParse(s, out i))
					{
						e.Cancel = true;
					}
					else
					{
						if ((Minimum != null && i < (int)Minimum) ||
							(Maximum != null && i > (int)Maximum))
						{
							e.Cancel = true;
						}
						else
						{
							newValue = i;
						}
					}
				}
				else
				{
					float f;
					if (!float.TryParse(s, out f))
					{
						e.Cancel = true;
					}
					else
					{
						if ((Minimum != null && f < Minimum) ||
							(Maximum != null && f > Maximum))
						{
							e.Cancel = true;
						}
						else
						{
							newValue = f;
						}
					}
				}*/

				// Now SpinButton's 
				if (ValueChanging != null)
				{
					var args = new ValueChangingEventArgs<TNum?>(Value, newValue);
					ValueChanging.Invoke(this, args);
					if (args.Cancel)
					{
						e.Cancel = true;
					}
					else
					{
						e.NewValue = args.NewValue.HasValue ? NumberToString(args.NewValue) : null;
					}
				}
			}
		}

		private void TextBoxOnTextChanged(object sender, ValueChangedEventArgs<string> eventArgs)
		{
			ValueChanged?.Invoke(this, new ValueChangedEventArgs<TNum?>(StringToNumber(eventArgs.OldValue), StringToNumber(eventArgs.NewValue)));
		}

		private void TextBoxOnTextChangedByUser(object sender, ValueChangedEventArgs<string> eventArgs)
		{
			ValueChangedByUser?.Invoke(this, new ValueChangedEventArgs<TNum?>(StringToNumber(eventArgs.OldValue), StringToNumber(eventArgs.NewValue)));
		}

		public void ApplySpinButtonStyle(SpinButtonStyle style)
		{
			ApplyWidgetStyle(style);

			if (style.TextBoxStyle != null)
			{
				_textField.ApplyTextBoxStyle(style.TextBoxStyle);
			}

			if (style.UpButtonStyle != null)
			{
				_upButton.ApplyImageButtonStyle(style.UpButtonStyle);
			}

			if (style.DownButtonStyle != null)
			{
				_downButton.ApplyImageButtonStyle(style.DownButtonStyle);
			}
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplySpinButtonStyle(stylesheet.SpinButtonStyles.SafelyGetStyle(name));
		}

		private void SpinValue(bool spinUpward, bool isMouseWheel)
		{
			TNum newValue, delta;
			if (!TypeHelper<TNum>.TryParse(_textField.Text, out newValue))
			{
				newValue = GenericMathExtra<TNum>.Zero;
			}

			if (isMouseWheel)
				delta = GenericMath<TNum>.Multiply(_increment, Mul_Increment);
			else
				delta = _increment;

			if (spinUpward)
				newValue = GenericMath<TNum>.Add(newValue, delta);
			else
				newValue = GenericMath<TNum>.Subtract(newValue, delta);

			if (_range.IsInRange(newValue))
			{
				bool changed = GenericMath<TNum>.NotEqual(Value.GetValueOrDefault(), newValue);
				TNum? oldValue = Value;
				Value = newValue;

				if (changed)
				{
					ValueChangedByUser?.Invoke(this, new ValueChangedEventArgs<TNum?>(oldValue, newValue));
				}
			}
		}
		private void UpButtonOnUp(object sender, EventArgs eventArgs) 
			=> SpinValue(true, false);
		private void DownButtonOnUp(object sender, EventArgs eventArgs) 
			=> SpinValue(false, false);

		public override void OnMouseWheel(float delta)
		{
			base.OnMouseWheel(delta);

			if (delta < 0 && _downButton.Visible && _downButton.Enabled)
			{
				SpinValue(false, true);
			}
			else if(delta > 0 && _upButton.Visible && _upButton.Enabled)
			{
				SpinValue(true, true);
			}
		}

		public override void OnGotKeyboardFocus()
		{
			base.OnGotKeyboardFocus();

			_textField.OnGotKeyboardFocus();
		}

		public override void OnLostKeyboardFocus()
		{
			base.OnLostKeyboardFocus();

			if (string.IsNullOrEmpty(_textField.Text) && !Nullable)
			{
				var defaultValue = "0";
				if (Minimum.HasValue && GenericMath<TNum>.GreaterThan(Minimum.Value, GenericMathExtra<TNum>.Zero))
				{
					defaultValue = NumberToString(Minimum.Value);
				}
				else if (Maximum.HasValue && GenericMath<TNum>.LessThan(Maximum.Value, GenericMathExtra<TNum>.Zero))
				{
					defaultValue = NumberToString(Maximum.Value);
				}

				_textField.Text = defaultValue;
			}

			_textField.OnLostKeyboardFocus();
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			_textField.OnKeyDown(k);
		}

		public override void OnChar(char c)
		{
			base.OnChar(c);

			_textField.OnChar(c);
		}

		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var spinButton = (SpinButton<TNum>)w;

			Nullable = spinButton.Nullable;
			Minimum = spinButton.Minimum;
			Maximum = spinButton.Maximum;
			Value = spinButton.Value;
			Increment = spinButton.Increment;
			DecimalPlaces = spinButton.DecimalPlaces;
			FixedNumberSize = spinButton.FixedNumberSize;
			Mul_Increment = spinButton.Mul_Increment;
		}
	}
	
	// Helper static class for types
	internal static class SpinButton
	{
		private static ReadOnlyDictionary<Type, Func<Widget>> _typeCtors = new ReadOnlyDictionary<Type, Func<Widget>>(new Dictionary<Type, Func<Widget>>
		{
			{ typeof(byte),   () => new SpinButton<byte>()   },
			{ typeof(sbyte),  () => new SpinButton<sbyte>()  },
			{ typeof(short),  () => new SpinButton<short>()  },
			{ typeof(ushort), () => new SpinButton<ushort>() },
			{ typeof(int),    () => new SpinButton<int>()    },
			{ typeof(uint),   () => new SpinButton<uint>()   },
			{ typeof(long),   () => new SpinButton<long>()   },
			{ typeof(ulong),  () => new SpinButton<ulong>()  },
			
			{ typeof(float),   () => new SpinButton<float>()   },
			{ typeof(double),  () => new SpinButton<double>()  },
			{ typeof(decimal), () => new SpinButton<decimal>() },
		});

		public static bool TryCreate(Type numberType, out Widget spinButton)
		{
			spinButton = default;
			return default;
		}
	}
}