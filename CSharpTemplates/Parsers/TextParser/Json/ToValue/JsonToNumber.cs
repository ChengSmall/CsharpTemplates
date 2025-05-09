using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using Cheng.DataStructure.NumGenerators;
using Cheng.Algorithm.Randoms;

namespace Cheng.Json.GeneratorNumbers
{

    /// <summary>
    /// json值类型结构转化器
    /// </summary>
    public class JsonValueGenerator
    {

        #region 构造

        public JsonValueGenerator()
        {
            defaultNumGenerator = new NumGeneratorValue(0);
        }

        #endregion

        #region 参数

        /// <summary>
        /// 默认为0的值生成器
        /// </summary>
        protected readonly NumGenerator defaultNumGenerator;

        #endregion

        #region 功能

        #region 封装

        /// <summary>
        /// 创建一个随机数生成器
        /// </summary>
        /// <remarks>可在派生类重写以创建定制的随机数生成器对象</remarks>
        /// <returns>一个新的随机数生成器</returns>
        protected virtual BaseRandom CreateRandom()
        {
            return new LICRandom((((long)Environment.TickCount) << 16) ^ DateTime.UtcNow.Ticks);
        }

        private NumGenerator f_fixedValue(JsonDictionary json)
        {
            if(json.TryGetValue("value", out var value))
            {
                var jtype = value.DataType;
                //纯数值判断
                if (jtype == JsonType.Integer)
                {
                    return new NumGeneratorValue(value.Integer);
                }
                if (jtype == JsonType.RealNum)
                {
                    return new NumGeneratorValue(value.RealNum);
                }
            }
            return ErrorJsonToNum(json);
        }

        private NumGenerator f_random(JsonDictionary json)
        {
            var min = json["min"];
            var max = json["max"];
            return new NumGeneratorRandomRange(CreateRandom(), JsonToNum(min),
                (new NumGeneratorMath(NumGeneratorMath.OperationType.Add, JsonToNum(max), NumGenerator.CreateValue(1))));
        }

        private NumGenerator f_randomBer(JsonDictionary json)
        {
            var n = json["n"];
            var p = json["p"];
            return new NumGeneratorBernoulliRandom(JsonToNum(n), JsonToNum(p), CreateRandom());
        }

        private NumGenerator f_math(JsonDictionary json)
        {
            var arith = json["arith"];
            var artype = arith.String;

            NumGeneratorMath.OperationType type;

            /*
            add => 表示 x + y
            sub => 表示 x - y
            mult => 表示乘法 x * y
            div => 表示除法 x / y；如果y是0则返回值不做定义
            mod => 计算 x / y 的余数
            sqrt => 计算 x 的平方，忽略y参数，此时也可以不写y参数
            pow => 计算 x 的 y 次方
            */

            var x = json["x"];

            switch (artype)
            {
                case "add":
                    type = NumGeneratorMath.OperationType.Add;
                    break;
                case "sub":
                    type = NumGeneratorMath.OperationType.Sub;
                    break;
                case "mult":
                    type = NumGeneratorMath.OperationType.Mult;
                    break;
                case "div":
                    type = NumGeneratorMath.OperationType.Div;
                    break;
                case "mod":
                    type = NumGeneratorMath.OperationType.Mod;
                    break;
                case "sqrt":
                    type = NumGeneratorMath.OperationType.Sqrt;
                    break;
                case "pow":
                    type = NumGeneratorMath.OperationType.Pow;
                    break;
                default:
                    return ErrorJsonToNum(json);
            }

            var num_x = JsonToNum(x);

            if (type == NumGeneratorMath.OperationType.Sqrt)
            {
                return new NumGeneratorMath(type, num_x, defaultNumGenerator);
            }

            var num_y = JsonToNum(json["y"]);

            return new NumGeneratorMath(type, num_x, num_y);
        }

        /// <summary>
        /// 将json转化为值的通用函数
        /// </summary>
        /// <remarks>从该函数开始判断一个值类型结构的json并返回值生成器</remarks>
        /// <param name="json">表示值类型结构的json对象，不可传入null</param>
        /// <returns>值生成器</returns>
        protected NumGenerator JsonToNum(JsonVariable json)
        {
            var jtype = json.DataType;

            //纯数值判断
            if (jtype == JsonType.Integer)
            {
                return new NumGeneratorValue(json.Integer);
            }
            if(jtype == JsonType.RealNum)
            {
                return new NumGeneratorValue(json.RealNum);
            }

            if(jtype == JsonType.Dictionary)
            {
                //可扩展结构
                var jd = json.JsonObject;

                try
                {
                    json = jd["type"];
                    var type = json.String;

                    if (type == "fixed")
                    {
                        return f_fixedValue(jd);
                    }
                    if (type == "random")
                    {
                        return f_random(jd);
                    }
                    if (type == "arith")
                    {
                        return f_math(jd);
                    }
                    if (type == "bernoulli")
                    {
                        return f_randomBer(jd);
                    }

                    return OtherJsonToNum(jd);
                }
                catch (Exception)
                {
                    return ErrorJsonToNum(jd);
                }

            }
            return ErrorJsonToNum(json);

        }

        /// <summary>
        /// 出现错误格式时调用的函数
        /// </summary>
        /// <param name="json">错误的json结构</param>
        /// <returns>定义出错后返回的值，默认是<see cref="defaultNumGenerator"/></returns>
        protected virtual NumGenerator ErrorJsonToNum(JsonVariable json)
        {
            return defaultNumGenerator;
        }

        /// <summary>
        /// 基础标准之外的可扩展结构
        /// </summary>
        /// <remarks>type参数在四个基础标准之外的json结构</remarks>
        /// <param name="json">可扩展结构</param>
        /// <returns>扩展返回值，默认是<see cref="defaultNumGenerator"/></returns>
        protected virtual NumGenerator OtherJsonToNum(JsonDictionary json)
        {
            return defaultNumGenerator;
        }

        #endregion

        #region 参数访问

        #endregion

        #region 功能

        /// <summary>
        /// 使用值类型结构返回一个值生成器
        /// </summary>
        /// <param name="json">值类型结构的json</param>
        /// <returns>一个值生成器</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public NumGenerator JsonToGenerator(JsonVariable json)
        {
            if (json is null) throw new ArgumentNullException();

            return JsonToNum(json);
        }

        /// <summary>
        /// 使用解析器将文本读取为json并将其当作值类型结构返回一个值生成器
        /// </summary>
        /// <param name="parser">json解析器</param>
        /// <param name="reader">值类型结构的json文本</param>
        /// <returns>一个值生成器</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotImplementedException">解析器错误</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="Exception">其它错误</exception>
        public NumGenerator JsonToGenerator(IJsonParser parser, TextReader reader)
        {
            if (parser is null || reader is null) throw new ArgumentNullException();
            return JsonToGenerator(parser.ToJsonData(reader));
        }

        /// <summary>
        /// 使用解析器将文本读取为json并将其当作值类型结构返回一个值生成器
        /// </summary>
        /// <param name="parser">json解析器</param>
        /// <param name="filePath">储存值类型结构json文本的文件路径</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotImplementedException">解析器错误</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="PathTooLongException">路径错误</exception>
        /// <exception cref="FileNotFoundException">文件不存在</exception>
        /// <exception cref="Exception">其它错误</exception>
        public NumGenerator JsonToGeneratorByFile(IJsonParser parser, string filePath)
        {
            if (parser is null || filePath is null) throw new ArgumentNullException();
            return JsonToGenerator(parser.FileToJson(filePath));
        }

        #endregion

        #endregion

    }

}
