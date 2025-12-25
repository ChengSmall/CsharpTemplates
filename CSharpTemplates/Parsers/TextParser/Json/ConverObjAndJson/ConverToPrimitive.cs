using System;
using System.Runtime;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Cheng.Json.Convers
{

    internal sealed class c_ConverTo_Int32 : ConverToJsonAndObj<int>
    {
        public override bool CanToObj => true;

        public override bool CanToJson => true;

        public override JsonVariable ToJsonVariable(int obj)
        {
            return JsonVariable.CreateInteger(obj);
        }

        public override int ToObj(JsonVariable json)
        {
            if (json is null) throw new ArgumentNullException();
            var jt = json.DataType;
            if (jt == JsonType.Integer) return (int)json.Integer;
            if (jt == JsonType.RealNum) return (int)json.Number;
            throw new NotImplementedException();
        }
    }

    internal sealed class c_ConverTo_UInt32 : ConverToJsonAndObj<uint>
    {
        public override bool CanToObj => true;

        public override bool CanToJson => true;

        public override JsonVariable ToJsonVariable(uint obj)
        {
            return JsonVariable.CreateInteger(obj);
        }

        public override uint ToObj(JsonVariable json)
        {
            if (json is null) throw new ArgumentNullException();
            var jt = json.DataType;
            if (jt == JsonType.Integer) return (uint)json.Integer;
            if (jt == JsonType.RealNum) return (uint)json.Number;
            throw new NotImplementedException();
        }
    }

    internal sealed class c_ConverTo_Int64 : ConverToJsonAndObj<long>
    {
        public override bool CanToObj => true;

        public override bool CanToJson => true;

        public override JsonVariable ToJsonVariable(long obj)
        {
            return JsonVariable.CreateInteger(obj);
        }

        public override long ToObj(JsonVariable json)
        {
            if (json is null) throw new ArgumentNullException();
            var jt = json.DataType;
            if (jt == JsonType.Integer) return json.Integer;
            if (jt == JsonType.RealNum) return (long)json.Number;
            throw new NotImplementedException();
        }
    }

    internal sealed class c_ConverTo_UInt64 : ConverToJsonAndObj<ulong>
    {
        public override bool CanToObj => true;

        public override bool CanToJson => true;

        public override JsonVariable ToJsonVariable(ulong obj)
        {
            return JsonVariable.CreateInteger((long)obj);
        }

        public override ulong ToObj(JsonVariable json)
        {
            if (json is null) throw new ArgumentNullException();
            var jt = json.DataType;
            if (jt == JsonType.Integer) return (ulong)json.Integer;
            if (jt == JsonType.RealNum) return (ulong)json.Number;
            throw new NotImplementedException();
        }
    }

    internal sealed class c_ConverTo_Float : ConverToJsonAndObj<float>
    {
        public override bool CanToObj => true;

        public override bool CanToJson => true;

        public override JsonVariable ToJsonVariable(float obj)
        {
            return JsonVariable.CreateNumber(obj);
        }

        public override float ToObj(JsonVariable json)
        {
            if (json is null) throw new ArgumentNullException();
            var jt = json.DataType;
            if (jt == JsonType.Integer) return (float)json.Integer;
            if (jt == JsonType.RealNum) return (float)json.Number;
            throw new NotImplementedException();
        }
    }

    internal sealed class c_ConverTo_Double : ConverToJsonAndObj<double>
    {
        public override bool CanToObj => true;

        public override bool CanToJson => true;

        public override JsonVariable ToJsonVariable(double obj)
        {
            return JsonVariable.CreateNumber(obj);
        }

        public override double ToObj(JsonVariable json)
        {
            if (json is null) throw new ArgumentNullException();
            var jt = json.DataType;
            if (jt == JsonType.Integer) return (double)json.Integer;
            if (jt == JsonType.RealNum) return (double)json.Number;
            throw new NotImplementedException();
        }
    }

    internal sealed class c_ConverTo_String : ConverToJsonAndObj<string>
    {
        public override bool CanToObj => true;

        public override bool CanToJson => true;

        public override JsonVariable ToJsonVariable(string obj)
        {
            return JsonVariable.CreateString(obj);
        }

        public override string ToObj(JsonVariable json)
        {
            if (json is null || json.IsNull) return null;
            var jt = json.DataType;
            if (jt == JsonType.String) return json.String;
            throw new NotImplementedException();
        }
    }

    internal sealed class c_ConverTo_Boolean : ConverToJsonAndObj<bool>
    {
        public override bool CanToObj => true;

        public override bool CanToJson => true;

        public override JsonVariable ToJsonVariable(bool obj)
        {
            return JsonVariable.CreateBoolean(obj);
        }

        public override bool ToObj(JsonVariable json)
        {
            if (json is null) throw new ArgumentNullException();
            var jt = json.DataType;
            if (jt == JsonType.Boolean) return json.Boolean;
            throw new NotImplementedException();
        }
    }

    internal sealed class c_ConverTo_Int16 : ConverToJsonAndObj<short>
    {
        public override bool CanToObj => true;

        public override bool CanToJson => true;

        public override JsonVariable ToJsonVariable(short obj)
        {
            return JsonVariable.CreateInteger(obj);
        }

        public override short ToObj(JsonVariable json)
        {
            if (json is null) throw new ArgumentNullException();
            var jt = json.DataType;
            if (jt == JsonType.Integer) return (short)json.Integer;
            if (jt == JsonType.RealNum) return (short)json.Number;
            throw new NotImplementedException();
        }
    }

    internal sealed class c_ConverTo_UInt16 : ConverToJsonAndObj<ushort>
    {
        public override bool CanToObj => true;

        public override bool CanToJson => true;

        public override JsonVariable ToJsonVariable(ushort obj)
        {
            return JsonVariable.CreateInteger(obj);
        }

        public override ushort ToObj(JsonVariable json)
        {
            if (json is null) throw new ArgumentNullException();
            var jt = json.DataType;
            if (jt == JsonType.Integer) return (ushort)json.Integer;
            if (jt == JsonType.RealNum) return (ushort)json.Number;
            throw new NotImplementedException();
        }
    }

    internal sealed class c_ConverTo_Byte : ConverToJsonAndObj<byte>
    {
        public override bool CanToObj => true;

        public override bool CanToJson => true;

        public override JsonVariable ToJsonVariable(byte obj)
        {
            return JsonVariable.CreateInteger(obj);
        }

        public override byte ToObj(JsonVariable json)
        {
            if (json is null) throw new ArgumentNullException();
            var jt = json.DataType;
            if (jt == JsonType.Integer) return (byte)json.Integer;
            if (jt == JsonType.RealNum) return (byte)json.Number;
            throw new NotImplementedException();
        }
    }

    internal sealed class c_ConverTo_SByte : ConverToJsonAndObj<sbyte>
    {
        public override bool CanToObj => true;

        public override bool CanToJson => true;

        public override JsonVariable ToJsonVariable(sbyte obj)
        {
            return JsonVariable.CreateInteger(obj);
        }

        public override sbyte ToObj(JsonVariable json)
        {
            if (json is null) throw new ArgumentNullException();
            var jt = json.DataType;
            if (jt == JsonType.Integer) return (sbyte)json.Integer;
            if (jt == JsonType.RealNum) return (sbyte)json.Number;
            throw new NotImplementedException();
        }
    }

    internal sealed class c_ConverTo_Char : ConverToJsonAndObj<char>
    {
        public override bool CanToObj => true;
        public override bool CanToJson => true;

        public override JsonVariable ToJsonVariable(char obj)
        {
            return JsonVariable.CreateString(new string(obj, 1));
        }

        public override char ToObj(JsonVariable json)
        {
            if (json.IsNullable()) throw new NotImplementedException();
            if (json.DataType == JsonType.String)
            {
                if(json.String.Length > 0) return json.String[0];
            }
            if(json.DataType == JsonType.Integer)
            {
                return (char)json.Integer;
            }
            throw new NotImplementedException();
        }
    }

}
