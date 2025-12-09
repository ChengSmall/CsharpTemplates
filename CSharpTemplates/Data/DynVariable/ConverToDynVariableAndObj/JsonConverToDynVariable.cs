using System;
using System.Collections;
using System.Collections.Generic;
using Cheng.Json;


namespace Cheng.DataStructure.DynamicVariables
{

    /// <summary>
    /// Json和<see cref="DynVariable"/>对象互转化器
    /// </summary>
    public sealed class JsonConverToDynVariable : ConverToDynVariableAndObj<JsonVariable>
    {

        public override DynVariable ToDynVariable(JsonVariable obj)
        {
            return f_jsonToDyn(obj);
        }

        public override JsonVariable ToObj(DynVariable dynVariable)
        {
            return f_dynToJson(dynVariable);
        }

        /// <summary>
        /// 将json数据对象转化为动态数据对象
        /// </summary>
        /// <param name="json">json对象</param>
        /// <returns>转化后的对象</returns>
        public static DynVariable JsonToDynVariable(JsonVariable json)
        {
            return f_jsonToDyn(json);
        }

        /// <summary>
        /// 将动态数据对象转化为json对象
        /// </summary>
        /// <param name="dynVariable">dyn对象</param>
        /// <returns>转化后的对象</returns>
        public static JsonVariable DynVariableToJson(DynVariable dynVariable)
        {
            return f_dynToJson(dynVariable);
        }

        #region

        private static DynVariable f_jsonToDyn(JsonVariable json)
        {
            if (json is null) return DynVariable.EmptyValue;
            var jtype = json.DataType;

            if(jtype > 0 && jtype <= JsonType.Null)
            {
                //简单类型
                switch (jtype)
                {
                    case JsonType.Integer:
                        if(int.MinValue <= json.Integer && json.Integer <= int.MaxValue)
                        {
                            return DynVariable.CreateInt32((int)json.Integer);
                        }
                        return DynVariable.CreateInt64(json.Integer);
                    case JsonType.RealNum:
                        return DynVariable.CreateDouble(json.RealNum);
                    case JsonType.Boolean:
                        return json.Boolean ? DynVariable.BooleanTrue : DynVariable.BooleanFalse;
                    case JsonType.String:
                        return DynVariable.CreateString(json.String);
                    case JsonType.Null:
                        return DynVariable.EmptyValue;
                }
            }
            if(jtype == JsonType.List)
            {
                var jarr = json.Array;
                DynList dlist = new DynList(jarr.Count);
                foreach (var jsonItem in jarr)
                {
                    dlist.Add(f_jsonToDyn(jsonItem));
                }
                return dlist;
            }
            else if(jtype == JsonType.Dictionary)
            {
                var jdict = json.JsonObject;
                DynDictionary ddict = new DynDictionary(jdict.Count, jdict.Comparer);
                foreach (var jpair in jdict)
                {
                    ddict[jpair.Key] = f_jsonToDyn(jpair.Value);
                }
                return ddict;
            }
            return DynVariable.EmptyValue;
        }

        private static JsonVariable f_dynToJson(DynVariable dyn)
        {
            if (dyn is null) return JsonNull.Nullable;

            var dtype = dyn.DynType;

            if(dtype < DynVariableType.List)
            {
                //简单类型
                switch (dtype)
                {
                    case DynVariableType.Empty:
                        return JsonNull.Nullable;
                    case DynVariableType.Int32:
                        return new JsonInteger(dyn.Int32Value);
                    case DynVariableType.Int64:
                        return new JsonInteger(dyn.Int64Value);
                    case DynVariableType.Float:
                        return new JsonRealNumber(dyn.FloatValue);
                    case DynVariableType.Double:
                        return new JsonRealNumber(dyn.DoubleValue);
                    case DynVariableType.Boolean:
                        return  new JsonBoolean(dyn.BooleanValue);
                    case DynVariableType.String:
                        return new JsonString(dyn.StringValue);
                }
            }
            if(dtype == DynVariableType.List)
            {
                var dlist = dyn.DynamicList;
                JsonList jlist = new JsonList(dlist.Count);
                foreach (var item in dlist)
                {
                    jlist.Add(f_dynToJson(item));
                }
                return jlist;
            }
            else if(dtype == DynVariableType.Dictionary)
            {
                var ddict = dyn.DynamicDictionary;
                JsonDictionary jdict = new JsonDictionary(ddict.Count, ddict.Comparer);
                foreach (var dpair in ddict)
                {
                    jdict[dpair.Key] = f_dynToJson(dpair.Value);
                }
                return jdict;
            }

            return JsonNull.Nullable;
        }

        #endregion

    }

}