using System;
using System.Runtime;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Cheng.Json.Convers
{

    internal sealed unsafe class c_ConverTo_Enum<T> : ConverToJsonAndObj<T> where T : unmanaged, System.Enum
    {
        public c_ConverTo_Enum()
        {
        }

        public override bool CanToObj => true;
        public override bool CanToJson => true;
        public override bool CanConverAll => true;

        public override JsonVariable ToJsonVariable(T obj)
        {
            ulong re;
            switch (sizeof(T))
            {
                case 2:
                    ushort i16 = *(ushort*)&obj;
                    re = i16;
                    break;
                case 4:
                    uint i32 = *(uint*)&obj;
                    re = i32;
                    break;
                case 8:
                    re = *(ulong*)&obj;
                    break;
                default:
                    byte i8 = *(byte*)&obj;
                    re = i8;
                    break;
            }

            return JsonVariable.CreateInteger((long)re);
        }

        public override T ToObj(JsonVariable json)
        {
            if (json is null) throw new ArgumentNullException();
            if (json.DataType != JsonType.Integer) throw new NotImplementedException();

            ulong re = (ulong)json.Integer;

            switch (sizeof(T))
            {
                case 8:
                    return *(T*)&re;
                case 4:
                    uint i32 = (uint)(re & uint.MaxValue);
                    return *(T*)&i32;
                case 2:
                    ushort i16 = (ushort)(re & ushort.MaxValue);
                    return *(T*)&i16;
                default:
                    byte b = (byte)(re & byte.MaxValue);
                    return *(T*)&b;
            }
        }
    }

}