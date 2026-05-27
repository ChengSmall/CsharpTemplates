using System;
using System.Runtime;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Cheng.DataStructure.DynamicVariables;

namespace Cheng.Json.Convers
{

    internal sealed class c_ConverTo_Array<ART> : ConverToJsonAndObj<ART[]>
    {
        public c_ConverTo_Array()
        {
        }

        public override bool CanToObj => ConverToJsonAndObj<ART>.Default.CanToObj;

        public override bool CanToJson => ConverToJsonAndObj<ART>.Default.CanToJson;

        public override bool CanConverAll => ConverToJsonAndObj<ART>.Default.CanConverAll;

        public override JsonVariable ToJsonVariable(ART[] obj)
        {
            if (obj is null) return JsonNull.Nullable;
            var cov = ConverToJsonAndObj<ART>.Default;
            var length = obj.Length;
            JsonList jlist = new JsonList(length);
            foreach (var item in obj)
            {
                jlist.Add(cov.ToJsonVariable(item));
            }
            return jlist;
        }

        public override ART[] ToObj(JsonVariable json)
        {
            if (json is null || json.IsNull) return null;
            if (json.DataType != JsonType.List) throw new NotImplementedException();
            var cov = ConverToJsonAndObj<ART>.Default;
            var jlist = json.Array;
            int length = jlist.Count;
            if (length == 0) return Array.Empty<ART>();
            ART[] rearr = new ART[length];
            for (int i = 0; i < length; i++)
            {
                rearr[i] = cov.ToObj(jlist[i]);
            }
            return rearr;
        }

    }

    internal sealed class c_ConverTo_List<LT> : ConverToJsonAndObj<List<LT>>
    {
        public c_ConverTo_List()
        {
        }

        public override bool CanToObj => ConverToJsonAndObj<LT>.Default.CanToObj;

        public override bool CanToJson => ConverToJsonAndObj<LT>.Default.CanToJson;

        public override bool CanConverAll => ConverToJsonAndObj<LT>.Default.CanConverAll;

        public override JsonVariable ToJsonVariable(List<LT> obj)
        {
            if (obj is null) return JsonNull.Nullable;
            var cov = ConverToJsonAndObj<LT>.Default;
            var length = obj.Count;
            JsonList jlist = new JsonList(length);
            foreach (var item in obj)
            {
                jlist.Add(cov.ToJsonVariable(item));
            }
            return jlist;
        }

        public override List<LT> ToObj(JsonVariable json)
        {
            if (json is null || json.IsNull) return null;
            if (json.DataType != JsonType.List) throw new NotImplementedException();
            var cov = ConverToJsonAndObj<LT>.Default;

            var jlist = json.Array;
            int length = jlist.Count;
            if (length == 0) return new List<LT>();

            List<LT> rearr = new List<LT>(length);
            foreach (var item in jlist)
            {
                rearr.Add(cov.ToObj(item));
            }
            //for (int i = 0; i < length; i++)
            //{
            //    rearr[i] = cov.ToObj(jlist[i]);
            //}
            return rearr;
        }
    }

    internal sealed class c_ConverTo_Dict<VT> : ConverToJsonAndObj<Dictionary<string, VT>>
    {
        public c_ConverTo_Dict()
        {
        }

        public override bool CanToObj => ConverToJsonAndObj<VT>.Default.CanToObj;

        public override bool CanToJson => ConverToJsonAndObj<VT>.Default.CanToJson;

        public override bool CanConverAll => ConverToJsonAndObj<VT>.Default.CanConverAll;

        public override JsonVariable ToJsonVariable(Dictionary<string, VT> obj)
        {
            var cov = ConverToJsonAndObj<VT>.Default;
            var length = obj.Count;
            JsonDictionary jd = new JsonDictionary(length, obj.Comparer);
            foreach (var pair in obj)
            {
                var vj = cov.ToJsonVariable(pair.Value);
                jd[pair.Key] = vj;
            }
            return jd;
        }

        public override Dictionary<string, VT> ToObj(JsonVariable json)
        {
            if (json is null || json.IsNull) return null;
            if (json.DataType != JsonType.Dictionary) throw new NotImplementedException();
            var cov = ConverToJsonAndObj<VT>.Default;
            var jd = json.JsonObject;
            int length = jd.Count;
            if (length == 0) return new Dictionary<string, VT>();
            var dict = new Dictionary<string, VT>(length, jd.Comparer);
            foreach (var pair in jd)
            {
                var v = cov.ToObj(pair.Value);
                dict[pair.Key] = v;
            }
            return dict;
        }
    }

    internal sealed class c_ConverTo_TimeSpan : ConverToJsonAndObj<TimeSpan>
    {
        public override bool CanToObj => true;

        public override bool CanToJson => true;

        public override JsonVariable ToJsonVariable(TimeSpan obj)
        {
            return JsonVariable.CreateInteger(obj.Ticks);
        }

        public override TimeSpan ToObj(JsonVariable json)
        {
            if (json is null) throw new NotImplementedException();
            var t = json.DataType;
            if (t == JsonType.Integer) return new TimeSpan(json.Integer);
            if (t == JsonType.RealNum) return new TimeSpan((long)json.Number);
            throw new NotImplementedException();
        }
    }

    internal sealed class c_ConverTo_DateTime : ConverToJsonAndObj<DateTime>
    {
        public override bool CanToObj => true;

        public override bool CanToJson => true;

        public override JsonVariable ToJsonVariable(DateTime obj)
        {
            ulong tick = (ulong)obj.Ticks;
            var kind = (ulong)obj.Kind;
            return JsonVariable.CreateInteger((long)(tick | (kind << 62)));
        }

        public override DateTime ToObj(JsonVariable json)
        {
            if (json is null) throw new NotImplementedException();
            var t = json.DataType;
            ulong tk;
            if (t == JsonType.Integer) tk = (ulong)(json.Integer);
            else if (t == JsonType.RealNum) tk = (ulong)json.Number;
            else throw new NotImplementedException();
            long tick = (long)(tk & (~(0b11UL << 62)));
            return new DateTime(tick, (DateTimeKind)(tk >> 62));
        }
    }

    internal sealed class c_ConverTo_Guid : ConverToJsonAndObj<Guid>
    {
        public override bool CanToObj => true;
        public override bool CanToJson => true;

        public override JsonVariable ToJsonVariable(Guid obj)
        {
            return JsonVariable.CreateString(obj.ToString("D"));
        }

        public override Guid ToObj(JsonVariable json)
        {
            if (json is null) throw new NotImplementedException();
            try
            {
                return new Guid(json.String);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
        }
    }

    internal sealed class c_ConverTo_This<T> : ConverToJsonAndObj<T> where T : JsonVariable
    {
        public override bool CanToObj => true;
        public override bool CanToJson => true;

        public override JsonVariable ToJsonVariable(T obj)
        {
            if (obj is null) return JsonVariable.JsonNullValue;
            return obj.Clone();
        }

        public override T ToObj(JsonVariable json)
        {
            json = json as T;
            return (T)(json?.Clone());
        }
    }

    internal sealed class c_ConverTo_DynObj : ConverToJsonAndObj<DynVariable>
    {
        public override bool CanToObj => true;
        public override bool CanToJson => true;

        public override JsonVariable ToJsonVariable(DynVariable obj)
        {
            return JsonConverToDynVariable.DynVariableToJson(obj);
        }

        public override DynVariable ToObj(JsonVariable json)
        {
            return JsonConverToDynVariable.JsonToDynVariable(json);
        }
    }

    internal sealed class c_ConverTo_Nullable<T> : ConverToJsonAndObj<T?> where T : struct
    {
        public c_ConverTo_Nullable()
        {
        }

        public override bool CanToObj => ConverToJsonAndObj<T>.Default.CanToObj;
        public override bool CanToJson => ConverToJsonAndObj<T>.Default.CanToJson;
        public override bool CanConverAll => ConverToJsonAndObj<T>.Default.CanConverAll;

        public override JsonVariable ToJsonVariable(T? obj)
        {
            if (obj.HasValue)
            {
                return ConverToJsonAndObj<T>.Default.ToJsonVariable(obj.Value);
            }
            return JsonNull.Nullable;
        }

        public override T? ToObj(JsonVariable json)
        {
            if (json is null) return null;
            if (json.IsNull) return null;
            return ConverToJsonAndObj<T>.Default.ToObj(json);
        }
    }

    internal sealed class c_ConverTo_Decimal : ConverToJsonAndObj<decimal>
    {
        public c_ConverTo_Decimal()
        {
            bits = new int[4];
        }

        private int[] bits;

        public override bool CanToObj => true;
        public override bool CanToJson => true;

        public override JsonVariable ToJsonVariable(decimal obj)
        {
            var bits = decimal.GetBits(obj);

            JsonList jlist = new JsonList(2);
            ulong i64 = (((uint)bits[0]) | (((ulong)((uint)bits[1])) << 32));
            jlist.Add(JsonVariable.CreateInteger((long)i64));

            i64 = (((uint)bits[2]) | (((ulong)((uint)bits[3])) << 32));
            jlist.Add(JsonVariable.CreateInteger((long)i64));
            return jlist;
        }

        public override decimal ToObj(JsonVariable json)
        {
            if (json.IsNullable()) throw new NotImplementedException();
            try
            {
                const ulong bit32 = 0xFFFFFFFF;

                var jlist = json.Array;
                var last = (ulong)jlist[0].Integer;
                var next = (ulong)jlist[1].Integer;

                bits[0] = (int)(last & bit32);
                bits[1] = (int)((last >> 32) & bit32);

                bits[2] = (int)(next & bit32);
                bits[3] = (int)((next >> 32) & bit32);

                return new decimal(bits);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

        }
    }

}