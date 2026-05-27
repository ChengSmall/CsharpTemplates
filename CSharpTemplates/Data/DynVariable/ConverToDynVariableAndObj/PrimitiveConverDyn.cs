using System;
using Cheng.IO;
using Cheng.Memorys;

namespace Cheng.DataStructure.DynamicVariables
{

    #region 基本类型

    internal sealed class c_ConverTo_Int32 : ConverToDynVariableAndObj<int>
    {
        public override DynVariable ToDynVariable(int obj)
        {
            return DynVariable.CreateInt32(obj);
        }

        public override int ToObj(DynVariable dynVariable)
        {
            if(dynVariable is null) throw new NotImplementedException();
            if (dynVariable.DynType == DynVariableType.Int32) return dynVariable.Int32Value;
            if (dynVariable.DynType == DynVariableType.Int64) return (int)dynVariable.Int64Value;
            throw new NotImplementedException();
        }
    }

    internal sealed class c_ConverTo_UInt32 : ConverToDynVariableAndObj<uint>
    {
        public override DynVariable ToDynVariable(uint obj)
        {
            return DynVariable.CreateInt32((int)obj);
        }

        public override uint ToObj(DynVariable dynVariable)
        {
            if (dynVariable is null) throw new NotImplementedException();
            if (dynVariable.DynType == DynVariableType.Int32) return (uint)dynVariable.Int32Value;
            if (dynVariable.DynType == DynVariableType.Int64) return (uint)dynVariable.Int64Value;
            throw new NotImplementedException();
        }
    }

    internal sealed class c_ConverTo_Int64 : ConverToDynVariableAndObj<long>
    {
        public override DynVariable ToDynVariable(long obj)
        {
            return DynVariable.CreateInt64(obj);
        }

        public override long ToObj(DynVariable dynVariable)
        {
            if (dynVariable is null) throw new NotImplementedException();
            if (dynVariable.DynType == DynVariableType.Int32) return dynVariable.Int32Value;
            if (dynVariable.DynType == DynVariableType.Int64) return dynVariable.Int64Value;
            throw new NotImplementedException();
        }
    }

    internal sealed class c_ConverTo_UInt64 : ConverToDynVariableAndObj<ulong>
    {
        public override DynVariable ToDynVariable(ulong obj)
        {
            return DynVariable.CreateInt64((long)obj);
        }

        public override ulong ToObj(DynVariable dynVariable)
        {
            if (dynVariable is null) throw new NotImplementedException();
            if (dynVariable.DynType == DynVariableType.Int32) return (ulong)dynVariable.Int32Value;
            if (dynVariable.DynType == DynVariableType.Int64) return (ulong)dynVariable.Int64Value;
            throw new NotImplementedException();
        }
    }

    internal sealed class c_ConverTo_Float : ConverToDynVariableAndObj<float>
    {
        public override DynVariable ToDynVariable(float obj)
        {
            return DynVariable.CreateFloat(obj);
        }

        public override float ToObj(DynVariable dynVariable)
        {
            if (dynVariable is null) throw new NotImplementedException();
            if (dynVariable.DynType == DynVariableType.Float) return dynVariable.FloatValue;
            throw new NotImplementedException();
        }
    }

    internal sealed class c_ConverTo_Double : ConverToDynVariableAndObj<double>
    {
        public override DynVariable ToDynVariable(double obj)
        {
            return DynVariable.CreateDouble(obj);
        }

        public override double ToObj(DynVariable dynVariable)
        {
            if (dynVariable is null) throw new NotImplementedException();
            if (dynVariable.DynType == DynVariableType.Float) return dynVariable.FloatValue;
            if (dynVariable.DynType == DynVariableType.Double) return dynVariable.DoubleValue;
            throw new NotImplementedException();
        }
    }

    internal sealed class c_ConverTo_Boolean : ConverToDynVariableAndObj<bool>
    {
        public c_ConverTo_Boolean()
        {
        }

        public override DynVariable ToDynVariable(bool obj)
        {
            return obj ? DynVariable.BooleanTrue : DynVariable.BooleanFalse;
        }

        public override bool ToObj(DynVariable dynVariable)
        {
            if (dynVariable is null) throw new NotImplementedException();
            if(dynVariable.DynType == DynVariableType.Boolean) return dynVariable.BooleanValue;
            throw new NotImplementedException();
        }
    }

    internal sealed class c_ConverTo_String : ConverToDynVariableAndObj<string>
    {

        public override DynVariable ToDynVariable(string obj)
        {
            if (obj is null) return DynVariable.EmptyValue;
            return DynVariable.CreateString(obj);
        }

        public override string ToObj(DynVariable dynVariable)
        {
            if (dynVariable is null) throw new NotImplementedException();
            if (dynVariable.DynType == DynVariableType.String) return dynVariable.StringValue;
            throw new NotImplementedException();
        }
    }

    internal sealed class c_ConverTo_Guid : ConverToDynVariableAndObj<Guid>
    {
        public override DynVariable ToDynVariable(Guid obj)
        {
            return DynVariable.CreateString(obj.ToString("D"));
        }

        public override Guid ToObj(DynVariable dynVariable)
        {
            if(dynVariable is null) throw new NotImplementedException();
            try
            {
                return new Guid(dynVariable.StringValue);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
        }
    }

    internal sealed class c_ConverTo_TimeSpan : ConverToDynVariableAndObj<TimeSpan>
    {
        public override DynVariable ToDynVariable(TimeSpan obj)
        {
            return DynVariable.CreateInt64(obj.Ticks);
        }

        public override TimeSpan ToObj(DynVariable dynVariable)
        {
            if (dynVariable is null) throw new NotImplementedException();
            var dt = dynVariable.DynType;
            if (dt == DynVariableType.Int64) return new TimeSpan(dynVariable.Int64Value);
            if (dt == DynVariableType.Int32) return new TimeSpan(dynVariable.Int32Value);
            throw new NotImplementedException();
        }
    }

    internal sealed class c_ConverTo_DateTime : ConverToDynVariableAndObj<DateTime>
    {
        public override DynVariable ToDynVariable(DateTime obj)
        {
            var tick = (ulong)obj.Ticks;
            var kind = (ulong)obj.Kind;
            ulong t = tick | (kind << 62);
            return DynVariable.CreateInt64((long)t);
        }

        public override DateTime ToObj(DynVariable dynVariable)
        {
            if (dynVariable is null) throw new NotImplementedException();
            var dt = dynVariable.DynType;
            if (dt == DynVariableType.Int64)
            {
                var t = (ulong)dynVariable.Int64Value;
                var tick = t & (0b11UL << 62);
                DateTimeKind kind = (DateTimeKind)((t >> 62) & 0b11);
                return new DateTime((long)tick, kind);
            }

            throw new NotImplementedException();
        }
    }

    internal sealed class c_ConverTo_Decimal : ConverToDynVariableAndObj<decimal>
    {
        public c_ConverTo_Decimal()
        {
            bits = new int[4];
        }
        private int[] bits;

        public override DynVariable ToDynVariable(decimal obj)
        {
            var bits = decimal.GetBits(obj);
            DynList dlist = new DynList(4);
            for (int i = 0; i < 4; i++)
            {
                dlist.Add(bits[i]);
            }
            return dlist;
        }

        public override decimal ToObj(DynVariable dynVariable)
        {
            if (dynVariable is null) throw new NotImplementedException();
            try
            {
                var dlist = dynVariable.DynamicList;
                bits[0] = (int)(dlist[0]);
                bits[1] = (int)(dlist[1]);
                bits[2] = (int)(dlist[2]);
                bits[3] = (int)(dlist[3]);
                return new decimal(bits);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
        }
    }

    #endregion

    #region enum

    internal sealed unsafe class c_ConverTo_Enum_Int32<T> : ConverToDynVariableAndObj<T> where T : unmanaged, System.Enum
    {
        public c_ConverTo_Enum_Int32()
        {
        }

        public override DynVariable ToDynVariable(T obj)
        {
            return DynVariable.CreateInt32(*(int*)&obj);
        }

        public override T ToObj(DynVariable dynVariable)
        {
            if (dynVariable is null) throw new NotImplementedException();
            var type = dynVariable.DynType;
            int t;
            switch (type)
            {
                case DynVariableType.Int32:
                    t = dynVariable.Int32Value;
                    break;
                case DynVariableType.Int64:
                    t = (int)dynVariable.Int64Value;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return *(T*)&t;
        }
    }

    internal sealed unsafe class c_ConverTo_Enum_Int64<T> : ConverToDynVariableAndObj<T> where T : unmanaged, System.Enum
    {
        public c_ConverTo_Enum_Int64()
        {
        }

        public override DynVariable ToDynVariable(T obj)
        {
            return DynVariable.CreateInt64(*(long*)&obj);
        }

        public override T ToObj(DynVariable dynVariable)
        {
            if (dynVariable is null) throw new NotImplementedException();
            var type = dynVariable.DynType;
            long t;
            switch (type)
            {
                case DynVariableType.Int32:
                    t = dynVariable.Int32Value;
                    break;
                case DynVariableType.Int64:
                    t = dynVariable.Int64Value;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return *(T*)&t;
        }
    }

#if DEBUG
    /// <summary>
    /// 比4字节小的枚举类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
#endif
    internal sealed unsafe class c_ConverTo_Enum_Int32Less<T> : ConverToDynVariableAndObj<T> where T : unmanaged, System.Enum
    {
        private enum BTType
        {
            Byte,
            SByte,
            Short,
            UShort,
        }

        public c_ConverTo_Enum_Int32Less()
        {
            var bt = typeof(T).GetEnumUnderlyingType();
            if(bt == typeof(short))
            {
                btType = BTType.Short;
            }
            else if(bt == typeof(ushort))
            {
                btType = BTType.UShort;
            }
            else if (bt == typeof(byte))
            {
                btType = BTType.Byte;
            }
            else
            {
                btType = BTType.SByte;
            }
        }

        private readonly BTType btType;

        public override DynVariable ToDynVariable(T obj)
        {
            int t;

            switch (btType)
            {
                case BTType.Byte:
                    t = (int)(*(byte*)&obj);
                    break;
                case BTType.SByte:
                    t = (int)(*(sbyte*)&obj);
                    break;
                case BTType.Short:
                    t = (int)(*(short*)&obj);
                    break;
                default:
                    t = (int)(*(ushort*)&obj);
                    break;
            }

            return DynVariable.CreateInt32(t);
        }

        public override T ToObj(DynVariable dynVariable)
        {
            if (dynVariable is null) throw new NotImplementedException();
            var dt = dynVariable.DynType;
            int t;
            switch (dt)
            {
                case DynVariableType.Int32:
                    t = dynVariable.Int32Value;
                    break;
                case DynVariableType.Int64:
                    t = (int)dynVariable.Int64Value;
                    break;
                default:
                    throw new NotImplementedException();
            }

            switch (btType)
            {
                case BTType.Byte:
                    byte b = (byte)t;
                    return *(T*)&b;
                case BTType.SByte:
                    sbyte sb = (sbyte)t;
                    return *(T*)&sb;
                case BTType.Short:
                    short s = (short)t;
                    return *(T*)&s;
                default:
                    ushort us = (ushort)t;
                    return *(T*)&us;
            }

        }
    }

    #endregion

}
#if DEBUG
#endif