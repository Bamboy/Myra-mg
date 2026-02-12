using System;
using Generic.Math;

namespace Myra.Utility.Types
{
    /// <summary>
    /// Supplemental generic math methods for <see cref="GenericMath"/>.
    /// </summary>
    public static class GenericMathExtra<TNum> where TNum : struct
    {
        static GenericMathExtra()
        {
            Type arg = typeof(TNum);
            TypeInfo info = TypeHelper<TNum>.Info;
            
            if(info.IsNullable)
                throw new ArgumentException($"Invalid Generic-Type Argument: '{arg}', Nullable types are unsupported");
            if(!info.IsNumber)
                throw new ArgumentException($"Invalid Generic-Type Argument: '{arg}', Only numeric types are supported");
        }
        
        public static TNum Min(TNum lhs, TNum rhs)
            => GenericMath.LessThan(lhs, rhs) ? lhs : rhs;
        public static TNum Max(TNum lhs, TNum rhs)
            => GenericMath.GreaterThan(lhs, rhs) ? lhs : rhs;
        public static TNum Clamp(TNum value, TNum minValue, TNum maxValue)
        {
            if (GenericMath.Equal(minValue, maxValue))
                return minValue;
            
            TNum min = Min(minValue, maxValue);
            TNum max = Max(minValue, maxValue);
            
            if (GenericMath<TNum>.LessThanOrEqual(value, min))
                return min;
            if (GenericMath<TNum>.GreaterThanOrEqual(value, max))
                return max;
            return value;
        }
        public static TNum Clamp(TNum value, TNum? minValue, TNum? maxValue)
        {
            bool limitMin = minValue.HasValue, limitMax = maxValue.HasValue;
            if (limitMin & limitMax)
                return Clamp(value, minValue.Value, maxValue.Value);
            if (!limitMin & !limitMax)
                return value;
            
            // limitMin != limitMax...
            if (limitMin && GenericMath<TNum>.LessThanOrEqual(value, minValue.Value))
                return minValue.Value;
            if (limitMax && GenericMath<TNum>.GreaterThanOrEqual(value, maxValue.Value))
                return maxValue.Value;
            return value;
        }
    }
}