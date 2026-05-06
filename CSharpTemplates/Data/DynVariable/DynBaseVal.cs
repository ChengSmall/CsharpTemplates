using Cheng.Algorithm.HashCodes;
using Cheng.Memorys;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cheng.DataStructure.DynamicVariables
{

    internal sealed unsafe class Dyn32bit : DynVariable
    {

        public Dyn32bit(int value)
        {
            this.value = value;
            p_type = DynVariableType.Int32;
        }

        public Dyn32bit(float value)
        {
            p_type = DynVariableType.Float;
            this.value = *(int*)&value;
        }

        public readonly int value;
        public readonly DynVariableType p_type;

        public override DynVariableType DynType
        {
            get => p_type;
        }

        public override int Int32Value
        {
            get
            {
                if (p_type == DynVariableType.Int32) return value;

                throw new NotSupportedException();
            }
        }

        public override float FloatValue
        {
            get
            {
                if (p_type == DynVariableType.Float)
                {
                    int re = value;
                    return *(float*)&re;
                }
                throw new NotSupportedException();
            }
        }

        public override int CompareTo(DynVariable other)
        {
            if (other is null) return 1;
            var t = DynType;
            var ot = other.DynType;
            if (t != ot)
            {
                return (byte)t - (byte)ot;
            }

            if(t == DynVariableType.Int32)
            {
                return Int32Value.CompareTo(other.Int32Value);
            }
            return FloatValue.CompareTo(other.FloatValue);
        }

        public override bool Equals(DynVariable other)
        {
            if (other is null) return false;
            var t = DynType;
            var ot = other.DynType;
            if (t != ot)
            {
                return false;
            }
            if (t == DynVariableType.Int32)
            {
                return Int32Value == other.Int32Value;
            }
            return FloatValue == other.FloatValue;
        }

        public override int GetHashCode()
        {
            return value;
        }

        public override long GetHashCode64()
        {
            return value;
        }

        public override string ToString()
        {
            var va = value;
            if (p_type == DynVariableType.Int32) return va.ToString();
            return (*(float*)&va).ToString();
        }

        public override DynVariable Clone()
        {
            return this;
        }

    }

    internal sealed unsafe class Dyn64bit : DynVariable
    {

        public Dyn64bit(long value)
        {
            this.value = value;
            p_type = (DynVariableType.Int64);
        }

        public Dyn64bit(double value)
        {
            p_type = DynVariableType.Double;
            this.value = *(long*)&value;
        }

        public readonly long value;
        public readonly DynVariableType p_type;

        public override DynVariableType DynType
        {
            get => p_type;
        }

        public override long Int64Value
        {
            get
            {
                if (p_type == DynVariableType.Int64) return value;
                throw new NotSupportedException();
            }
        }

        public override double DoubleValue
        {
            get
            {
                if (p_type == DynVariableType.Double)
                {
                    long re = value;
                    return *(double*)&re;
                }

                throw new NotSupportedException();
            }
        }

        public override int CompareTo(DynVariable other)
        {
            if (other is null) return 1;
            var t = DynType;
            var ot = other.DynType;
            if (t != ot)
            {
                return (byte)t - (byte)ot;
            }

            if (t == DynVariableType.Int32)
            {
                return Int32Value.CompareTo(other.Int32Value);
            }
            return FloatValue.CompareTo(other.FloatValue);
        }

        public override bool Equals(DynVariable other)
        {
            if (other is null) return false;
            var t = DynType;
            var ot = other.DynType;
            if (t != ot)
            {
                return false;
            }
            if (t == DynVariableType.Int32)
            {
                return Int32Value == other.Int32Value;
            }
            return FloatValue == other.FloatValue;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override long GetHashCode64()
        {
            return value;
        }

        public override string ToString()
        {
            var va = value;
            if(p_type == DynVariableType.Int64) return va.ToString();
            return (*(double*)&va).ToString();
        }

        public override DynVariable Clone()
        {
            return this;
        }
    }

    internal sealed class DynBoolean : DynVariable
    {
        public DynBoolean(bool value)
        {
            this.value = value;
        }

        public readonly bool value;

        public override DynVariableType DynType => DynVariableType.Boolean;

        public override bool BooleanValue => value;

        public override int CompareTo(DynVariable other)
        {
            if (other is null) return 1;
            var ot = other.DynType;
            if (ot != DynVariableType.Boolean)
            {
                return DynVariableType.Boolean - ot;
            }
            return value.CompareTo(other.BooleanValue);
        }

        public override bool Equals(DynVariable other)
        {
            if (other is null) return false;
            var ot = other.DynType;
            if (ot != DynVariableType.Boolean)
            {
                return false;
            }
            return value == other.BooleanValue;
        }

        public override int GetHashCode()
        {
            return value ? 1 : 0;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public override DynVariable Clone()
        {
            return this;
        }
    }

    internal sealed class DynString : DynVariable
    {
        public DynString(string value)
        {
            this.value = value ?? string.Empty;
        }

        public readonly string value;

        public override DynVariableType DynType => DynVariableType.String;

        public override string StringValue => value;

        public override int CompareTo(DynVariable other)
        {
            if (other is null) return 1;
            var ot = other.DynType;
            if (ot != DynVariableType.String)
            {
                return DynVariableType.String - ot;
            }
            return string.CompareOrdinal(value, other.StringValue);
        }

        public override bool Equals(DynVariable other)
        {
            if (other is null) return false;
            var ot = other.DynType;
            if (ot != DynVariableType.String)
            {
                return false;
            }
            return value == other.StringValue;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override long GetHashCode64()
        {
            return value.GetHashCode64();
        }

        public override string ToString()
        {
            return value;
        }

        public override DynVariable Clone()
        {
            return this;
        }
    }

    internal sealed class DynEmpty : DynVariable
    {
        public DynEmpty()
        {
        }

        public override DynVariableType DynType => DynVariableType.Empty;

        public override bool IsEmpty => true;

        public override bool Equals(DynVariable other)
        {
            if (other is null) return true;
            return other.DynType == DynVariableType.Empty;
        }

        public override int CompareTo(DynVariable other)
        {
            if (other is null) return 0;
            return other.DynType - DynVariableType.Empty;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            return string.Empty;
        }

        public override DynVariable Clone()
        {
            return this;
        }
    }

    /// <summary>
    /// 字节序列
    /// </summary>
    public sealed unsafe class DynBytes : DynVariable, IReadOnlyList<byte>
    {

        /// <summary>
        /// 实例化一个字节序列
        /// </summary>
        /// <param name="buffer">要初始化到对象的字节数组</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public DynBytes(byte[] buffer) : this(buffer, false)
        {}

        /// <summary>
        /// 实例化一个字节序列
        /// </summary>
        /// <param name="buffer">要初始化到对象的字节数组</param>
        /// <param name="origin">使用true允许传入原数据，false则将拷贝<paramref name="buffer"/>到新的实例</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public DynBytes(byte[] buffer, bool origin)
        {
            if (buffer is null) throw new ArgumentNullException();
            if(origin) p_buffer = buffer;
            else p_buffer = (byte[])buffer.Clone();
        }

        private byte[] p_buffer;

        /// <summary>
        /// 访问字节数据
        /// </summary>
        /// <param name="offset">起始偏移</param>
        /// <returns></returns>
        public byte this[int offset]
        {
            get
            {
                return p_buffer[offset];
            }
        }

        /// <summary>
        /// 获取内部封装的字节数组对象
        /// </summary>
        /// <returns></returns>
        public byte[] GetBaseByteArray()
        {
            return p_buffer;
        }

        /// <summary>
        /// 字节长度
        /// </summary>
        public int Length => p_buffer.Length;

        public override DynVariableType DynType => DynVariableType.Bytes;

        public override DynBytes DynamicBytes => this;

        public override bool Equals(DynVariable other)
        {
            if (other is null) return false;
            if (other.DynType == DynVariableType.Bytes)
            {
                return MemoryOperation.EqualsBytes(p_buffer, other.DynamicBytes.p_buffer);
            }
            return false;
        }

        public override long GetHashCode64()
        {
            const ulong FNV_OFFSET_BASIS = 14695981039346656037UL;
            const ulong FNV_PRIME = 1099511628211UL;

            var length = p_buffer.Length;
            int i;
            ulong hash;
            fixed (byte* bptr = p_buffer)
            {
                ulong* ip = (ulong*)bptr;
                int mod = length / 8;

                hash = FNV_OFFSET_BASIS;
                for (i = 0; i < mod; i++)
                {
                    hash = (hash ^ ip[i]) * FNV_PRIME;
                }

                ip += i;
                //剩余bytes
                mod = length % 8;
                if(mod > 0)
                {
                    byte* bp = (byte*)ip;
                    ulong lhash = 0;
                    //byte* hptr = (byte*)&lhash;
                    for (i = 0; i < mod; i++)
                    {
                        //hptr[i] = bp[i];
                        lhash |= (((ulong)bp[i]) << (i * 8));
                    }
                    hash ^= lhash;
                }
            }

            return (long)hash;
        }

        public override int GetHashCode()
        {
            return GetHashCode64().GetHashCode();
        }

        public override string ToString()
        {
            return string.Empty;
        }

        public override DynVariable Clone()
        {
            return this;
        }

        int IReadOnlyCollection<byte>.Count => p_buffer.Length;

        IEnumerator<byte> IEnumerable<byte>.GetEnumerator()
        {
            return ((IEnumerable<byte>)p_buffer).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return p_buffer.GetEnumerator();
        }
    }

}