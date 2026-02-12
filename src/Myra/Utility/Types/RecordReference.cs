using System;
using Myra.Graphics2D.UI.Properties;

namespace Myra.Utility.Types
{
    public abstract class RecordReference : IRecordReference
    {
        protected RecordReference(Record backer)
        {
            Record = backer;
        }
        
        public Record Record { get; }
        public virtual Type Type => Record?.Type;
        public virtual bool IsReadOnly
        {
            get
            {
                if (Record == null)
                    return true;
                return Record.HasSetter;
            }
        }
        
        public void SetValue(object field, object value) => Internal_Set(field, value);
        protected virtual void Internal_Set(object field, object value)
        {
            if (IsReadOnly)
                return;
            Record?.SetValue(field, value);
        }
        protected virtual object Internal_Get(object field) => Record?.GetValue(field);
        object IRecordReference.GetValue(object field) => Internal_Get(field);
    }
    public abstract class RecordReference<T> : RecordReference, IRecordReference<T>
    {
        protected RecordReference(Record backer) : base(backer)
        {
            
        }
        
        public T GetValue(object field)
        {
            object obj = Internal_Get(field);
            return (T)obj;
        }

        public void SetValue(object field, T value)
        {
            Internal_Set(field, value);
        }
    }

    public abstract class StructRecordReference<T> : RecordReference<T>, IStructTypeRef<T> where T : struct
    {
        static StructRecordReference()
        {
            isNullable = TypeHelper<T>.Info.IsNullable;
            realType = TypeHelper<T>.GetNullableTypeOrPassThrough();
        }
        
        // ReSharper disable once StaticMemberInGenericType
        private static readonly bool isNullable;
        public bool IsNullable => isNullable;
        // ReSharper disable once StaticMemberInGenericType
        private static readonly Type realType;
        public override Type Type => realType;

        protected StructRecordReference(Record backer) : base(backer)
        {
            
        }
        
    }

    public abstract class NumberReference<T> : StructRecordReference<T>, INumberTypeRef<T> where T : struct
    {
        protected NumberReference(Record backer) : base(backer)
        {
            
        }
    }

    public sealed class WholeNumberRef<T> : NumberReference<T>, IWholeNumberTypeRef<T> where T : struct
    {
        static WholeNumberRef()
        {
            if (TypeHelper<T>.Info.IsWholeNumber == false)
                throw new ArgumentException($"Invalid Generic Argument: {typeof(T)}");
        }
        public WholeNumberRef(Record backer) : base(backer)
        {
            
        }
    }
    public sealed class FracNumberRef<T> : NumberReference<T>, IFracNumberTypeRef<T> where T : struct
    {
        static FracNumberRef()
        {
            if (TypeHelper<T>.Info.IsFractionalNumber == false)
                throw new ArgumentException($"Invalid Generic Argument: {typeof(T)}");
        }
        public FracNumberRef(Record backer) : base(backer)
        {
            
        }
    }
}