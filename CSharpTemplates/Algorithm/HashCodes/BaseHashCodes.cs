using System;

namespace Cheng.Algorithm.HashCodes
{
    #region
    internal class Int32HashCode64 : BaseHashCode64<int>
    {
        public override long GetHashCode64(int value)
        {
            return value.GetHashCode64();
        }
    }
    internal class Int64HashCode64 : BaseHashCode64<long>
    {
        public override long GetHashCode64(long value)
        {
            return value.GetHashCode64();
        }
    }
    internal class UInt32HashCode64 : BaseHashCode64<uint>
    {
        public override long GetHashCode64(uint value)
        {
            return value.GetHashCode64();
        }
    }
    internal class UInt64HashCode64 : BaseHashCode64<ulong>
    {
        public override long GetHashCode64(ulong value)
        {
            return value.GetHashCode64();
        }
    }

    internal class Int16HashCode64 : BaseHashCode64<short>
    {
        public override long GetHashCode64(short value)
        {
            return value;
        }
    }
    internal class UInt16HashCode64 : BaseHashCode64<ushort>
    {
        public override long GetHashCode64(ushort value)
        {
            return value;
        }
    }
    internal class ByteHashCode64 : BaseHashCode64<byte>
    {
        public override long GetHashCode64(byte value)
        {
            return value;
        }
    }
    internal class SByteHashCode64 : BaseHashCode64<sbyte>
    {
        public override long GetHashCode64(sbyte value)
        {
            return value;
        }
    }

    internal class CharHashCode64 : BaseHashCode64<char>
    {
        public override long GetHashCode64(char value)
        {
            return value;
        }
    }
    internal class StringHashCode64 : BaseHashCode64<string>
    {
        public override long GetHashCode64(string value)
        {
            return value.GetHashCode64();
        }
    }

    internal class DecHashCode64 : BaseHashCode64<decimal>
    {
        public override long GetHashCode64(decimal value)
        {
            return value.GetHashCode64();
        }
    }

    internal class FloatHashCode64 : BaseHashCode64<float>
    {
        public override long GetHashCode64(float value)
        {
            return value.GetHashCode64();
        }
    }
    internal class DoubleHashCode64 : BaseHashCode64<double>
    {
        public override long GetHashCode64(double value)
        {
            return value.GetHashCode64();
        }
    }

    internal class BooleanHashCode64 : BaseHashCode64<bool>
    {
        public override long GetHashCode64(bool value)
        {
            return value.GetHashCode64();
        }
    }

    internal class DateTimeHashCode64 : BaseHashCode64<DateTime>
    {
        public override long GetHashCode64(DateTime value)
        {
            return value.GetHashCode64();
        }
    }
    internal class TimeSpanHashCode64 : BaseHashCode64<TimeSpan>
    {
        public override long GetHashCode64(TimeSpan value)
        {
            return value.GetHashCode64();
        }
    }

    internal class GuidHashCode64 : BaseHashCode64<Guid>
    {
        public override long GetHashCode64(Guid value)
        {
            return value.GetHashCode64();
        }
    }
    #endregion

    internal class TypeHashCode64<T> : BaseHashCode64<T> where T : IHashCode64
    {
        public TypeHashCode64()
        {
            //p_isValue = typeof(T).IsValueType;
        }
        //private readonly bool p_isValue;
        public override long GetHashCode64(T value)
        {
            //if (p_isValue) return value.GetHashCode64();
            return (value == null) ? 0 : value.GetHashCode64();
            //return (value?.GetHashCode64()).GetValueOrDefault();
        }
    }


    internal class TypeHashCode64Value<T> : BaseHashCode64<T> where T : struct, IHashCode64
    {

        public TypeHashCode64Value()
        {
            //p_isValue = typeof(T).IsValueType;
        }
        //private readonly bool p_isValue;
        public override long GetHashCode64(T value)
        {
            //if (p_isValue) return value.GetHashCode64();
            return value.GetHashCode64();
            //return (value?.GetHashCode64()).GetValueOrDefault();
        }
    }

}
