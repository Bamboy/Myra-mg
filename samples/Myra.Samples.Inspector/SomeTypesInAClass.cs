using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Myra.Samples.Inspector
{
    public enum MyEnum
    {
        One = 1,
        Five = 5,
        Twenty = 20
    }
    
    public class SomeTypesInAClass
    {
        public MyEnum enumValue = MyEnum.Five;

        public bool checkbox = true;

        public string stringValue = "Hotdog";

        public Color colorValue = Color.Honeydew;

        public List<string> stringCollection = new List<string>();

        public byte @byte = 250;
    }
    public class SomeNumerics
    {
        public byte @byte = 250;
        public sbyte @sbyte = -43;
        public short @short = short.MaxValue - 4;
        public ushort @ushort = 333;
        public int @int = -30;
        public uint @uint = 30;
        public long @long = 0;
        public ulong @ulong = ulong.MaxValue - 4;
        public float @float = 1.0f;
        public double @double = Math.PI;
        public decimal @decimal = decimal.MinusOne;
    }
}