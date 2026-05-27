using System;

namespace Cheng.Algorithm.HashCodes
{


    internal sealed class EnumValueHashCode<TE> : BaseHashCode64<TE> where TE : unmanaged, Enum
    {

        public EnumValueHashCode()
        {
            Type type = Enum.GetUnderlyingType(typeof(TE));
            
            if (type == typeof(uint))
            {
                p_tc = TypeCode.UInt32;
            }
            else if (type == typeof(int))
            {
                p_tc = TypeCode.Int32;
            }
            else if (type == typeof(ushort))
            {
                p_tc = TypeCode.UInt16;
            }
            else if (type == typeof(short))
            {
                p_tc = TypeCode.Int16;
            }
            else if (type == typeof(byte))
            {
                p_tc = TypeCode.Byte;
            }
            else if (type == typeof(sbyte))
            {
                p_tc = TypeCode.SByte;
            }
            else
            {
                p_tc = TypeCode.Int64;
            }
        }

        private TypeCode p_tc;

        public sealed unsafe override long GetHashCode64(TE value)
        {
            switch (p_tc)
            {
                case TypeCode.SByte:
                    return *(sbyte*)&value;
                case TypeCode.Byte:
                    return *(byte*)&value;
                case TypeCode.Int16:
                    return *(short*)&value;
                case TypeCode.UInt16:
                    return *(ushort*)&value;
                case TypeCode.Int32:
                    return *(int*)&value;
                case TypeCode.UInt32:
                    return *(uint*)&value;
                default:
                    return *(long*)&value;
            }
        }

    }

}
