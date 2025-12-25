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
            long re;
            switch (sizeof(T))
            {
                case 2:
                    re = *(ushort*)&obj;
                    break;
                case 4:
                    re = *(uint*)&obj;
                    break;
                case 8:
                    re = *(long*)&obj;
                    break;
                default:
                    re = *(byte*)&obj;
                    break;
            }

            return JsonVariable.CreateInteger(re);
        }

        public override T ToObj(JsonVariable json)
        {
            if (json is null) throw new ArgumentNullException();
            if (json.DataType != JsonType.Integer) throw new NotImplementedException();

            long re = json.Integer;

            switch (sizeof(T))
            {
                case 8:
                    return *(T*)&re;
                case 4:
                    uint i32 = (uint)re;
                    return *(T*)&i32;
                case 2:
                    ushort i16 = (ushort)re;
                    return *(T*)&i16;
                default:
                    byte b = (byte)re;
                    return *(T*)&b;
            }
        }
    }

}