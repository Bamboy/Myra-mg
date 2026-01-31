using System;
using System.Reflection;

namespace Myra.Graphics2D.UI.Properties
{
    public interface IRecord
    {
        Type Type { get; }
        object GetValue(object field);
        void SetValue(object field, object value);
    }

    public interface IRecord<T> : IRecord
    {
        new T GetValue(object field);
        void SetValue(object field, T value);
    }
}