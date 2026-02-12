using System;

namespace Myra.Utility.Types
{
    public struct Range<TNumber> where TNumber : struct
    {
        static Range()
        {
            Type arg = typeof(TNumber);
            TypeInfo info = TypeHelper<TNumber>.Info;
            
            if(info.IsNullable)
                throw new ArgumentException($"Invalid Generic-Type Argument: '{arg}', Nullable types are unsupported");
            if(!info.IsNumber)
                throw new ArgumentException($"Invalid Generic-Type Argument: '{arg}', Only numeric types are supported");
            
        }
        // TODO import https://github.com/HelloKitty/Generic.Math
        
        public Range(TNumber min, TNumber max)
        {
            _applyMin = true;
            _applyMax = true;
            _min = min;
            _max = max;
        }
        public Range(TNumber? min = null, TNumber? max = null)
        {
            _applyMin = min.HasValue;
            _applyMax = max.HasValue;
            _min = _applyMin ? min.Value : default;
            _max = _applyMax ? max.Value : default;
        }

        private bool _applyMin, _applyMax;
        private TNumber _min, _max;

        public TNumber? Min
        {
            get => _min;
            set
            {
                _applyMin = value.HasValue;
                _min = value ?? default;
            }
        }
        public TNumber? Max
        {
            get => _max;
            set
            {
                _applyMax = value.HasValue;
                _max = value ?? default;
            }
        }
        
        //Math.Max(_min, value);
    }
}