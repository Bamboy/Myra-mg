using System;
using Myra.Utility;

namespace Myra.Graphics2D.UI.Properties
{
    [PropertyEditor(typeof(StringPropertyEditor), typeof(string))]
    public sealed class StringPropertyEditor : PropertyEditor<string>
    {
        public StringPropertyEditor(IInspector owner, Record methodInfo) : base(owner, methodInfo)
        {
            
        }

        protected override bool CreatorPicker(out WidgetCreatorDelegate creatorDelegate)
        {
            creatorDelegate = CreateTextBox;
            // CreateTextBoxAsFilePath
            return true;
        }
        
        private bool CreateTextBox(out Widget widget)
        {
            if (_owner.SelectedField == null)
            {
                widget = null;
                return false;
            }
            
            var propertyType = _record.Type;
            var value = GetValue(_owner.SelectedField);

            var tf = new TextBox
            {
                Text = value != null ? value : string.Empty
            };

            if (_record.HasSetter)
            {
                tf.TextChanged += (sender, args) =>
                {
                    try
                    {
                        object result;

                        if (propertyType.IsNullablePrimitive())
                        {
                            if (string.IsNullOrEmpty(tf.Text))
                            {
                                result = null;
                            }
                            else
                            {
                                result = Convert.ChangeType(tf.Text, _record.Type.GetNullableType());
                            }
                        }
                        else
                        {
                            result = Convert.ChangeType(tf.Text, _record.Type);
                        }

                        SetValue(_owner.SelectedField, result);
                        /*
                        if (record.Type.IsValueType)
                        {
                            var tg = this;
                            var pg = tg._parentGrid;
                            while (pg != null && tg._parentProperty != null)
                            {
                                tg._parentProperty.SetValue(pg._object, tg._object);

                                if (!tg._parentProperty.Type.IsValueType)
                                {
                                    break;
                                }

                                tg = pg;
                                pg = tg._parentGrid;
                            }
                        }*/
                    }
                    catch (Exception)
                    {
                        // TODO: Rework this ugly type conversion solution
                    }
                };
            }
            else
            {
                tf.Enabled = false;
            }

            widget = tf;
            return true;
        }
        
        public override void SetWidgetValue(object value)
        {
            var tb = Widget as TextBox;
            if (value == null)
                tb.Text = string.Empty;
            else if (value is string str)
                tb.Text = str;
        }

        /*
        private bool CreateTextBoxAsFilePath(Record record, out Widget widget)
        {
            var propertyType = record.Type;
            var value = record.GetValue(Owner.SelectedField);

            var result = new HorizontalStackPanel
            {
                Spacing = 8
            };

            TextBox path = null;
            if (attribute.ShowPath)
            {
                path = new TextBox
                {
                    Readonly = true,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                if (value != null)
                {
                    path.Text = value.ToString();
                }

                StackPanel.SetProportionType(path, ProportionType.Fill);
                result.Widgets.Add(path);
            }

            var button = new Button
            {
                Tag = value,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Content = new Label
                {
                    Text = "Change...",
                    HorizontalAlignment = HorizontalAlignment.Center,
                }
            };
            Grid.SetColumn(button, 1);

            if (Record.HasSetter)
            {
                button.Click += (sender, args) =>
                {
                    var dlg = new FileDialog(attribute.DialogMode)
                    {
                        Filter = attribute.Filter
                    };

                    if (value != null)
                    {
                        var filePath = value.ToString();
                        if (!Path.IsPathRooted(filePath) && !string.IsNullOrEmpty(Owner.BasePath))
                        {
                            filePath = Path.Combine(Owner.BasePath, filePath);
                        }
                        dlg.FilePath = filePath;
                    }
                    else if (!string.IsNullOrEmpty(Owner.BasePath))
                    {
                        dlg.Folder = Owner.BasePath;
                    }

                    dlg.Closed += (s, a) =>
                    {
                        if (!dlg.Result)
                        {
                            return;
                        }

                        try
                        {
                            var filePath = dlg.FilePath;
                            if (!string.IsNullOrEmpty(Owner.BasePath))
                            {
                                filePath = PathUtils.TryToMakePathRelativeTo(filePath, Owner.BasePath);
                            }

                            if (path != null)
                            {
                                path.Text = filePath;
                            }

                            SetValue(record, _object, filePath);

                            Owner.FireChanged(propertyType.Name);
                        }
                        catch (Exception)
                        {
                        }
                    };

                    dlg.ShowModal(Owner.Desktop);
                };
            }
            else
            {
                button.Enabled = false;
            }

            result.Widgets.Add(button);
            widget = result;
            return true;
        }*/
    }
}