using Cheng.Algorithm.HashCodes;
using System;


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

}