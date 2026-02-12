using System;
using Myra.Graphics2D.UI.Properties;

namespace Myra.Utility.Types
{
    
    public interface IRecordReference
    {
        Record Record { get; }
        Type Type { get; }
        bool IsReadOnly { get; }
        object GetValue(object field);
        void SetValue(object field, object value);
    }

    public interface IRecordReference<T> : IRecordReference
    {
        new T GetValue(object field);
        void SetValue(object field, T value);
    }
    
    public interface IStructTypeRef<T> : IRecordReference<T> where T : struct
    {
        bool IsNullable { get; }
    }

    public interface INumberTypeRef<T> : IStructTypeRef<T> where T : struct
    {
        // todo include min/max T limiters
    }

    public interface IWholeNumberTypeRef<T> : INumberTypeRef<T> where T : struct
    {
        
    }

    public interface IFracNumberTypeRef<T> : INumberTypeRef<T> where T : struct
    {
        
    }
    
    internal static class TypeInterfaceExtensions
    {
        /// <summary>
        /// Checks if the <see cref="Record"/> is null
        /// </summary>
        public static bool IsNull(this IRecordReference obj)
            => obj == null || obj.Record == null;
        
        public static TypeInfo TypeInfo<T>(this IRecordReference<T> obj) 
            => TypeHelper<T>.Info;
        public static bool IsNullable<T>(this IStructTypeRef<T> obj)  where T : struct
            => TypeHelper<T>.Info.IsNullable;
        public static bool IsSignedNumber<T>(this INumberTypeRef<T> obj)  where T : struct
            => TypeHelper<T>.Info.IsSignedNumber;

        public static Type GetNullableTypePassThrough<T>(this IStructTypeRef<T> obj) where T : struct
            => TypeHelper<T>.GetNullableTypeOrPassThrough();
    }
}