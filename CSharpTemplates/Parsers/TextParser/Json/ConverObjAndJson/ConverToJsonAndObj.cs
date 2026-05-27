using System;
using System.Runtime;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cheng.Json.Convers
{

    /// <summary>
    /// 提供转化到json对象功能的公共接口
    /// </summary>
    public interface IConverToJson
    {
        /// <summary>
        /// 将当前对象转化为json对象
        /// </summary>
        /// <returns>转化后的对象</returns>
        JsonVariable ToJsonVariable();
    }

    /// <summary>
    /// 提供json和<typeparamref name="T"/>对象相互转化的公共接口
    /// </summary>
    /// <typeparam name="T">与之相互转化的类型</typeparam>
    public interface IConverToJsonAndObj<T>
    {

        /// <summary>
        /// 是否允许将json对象转化为<typeparamref name="T"/>对象
        /// </summary>
        bool CanToObj { get; }

        /// <summary>
        /// 是否允许将<typeparamref name="T"/>对象转化为json对象
        /// </summary>
        bool CanToJson { get; }

        /// <summary>
        /// 将json对象转换为<typeparamref name="T"/>对象
        /// </summary>
        /// <param name="json">要转换的json对象</param>
        /// <returns>转换后的对象</returns>
        /// <exception cref="NotImplementedException">无法转换，格式不正确；或者不允许此功能</exception>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        T ToObj(JsonVariable json);

        /// <summary>
        /// 将<typeparamref name="T"/>对象转换为json对象
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <returns>转换后的json对象</returns>
        /// <exception cref="NotImplementedException">没有此功能</exception>
        JsonVariable ToJsonVariable(T obj);
    }

    /// <summary>
    /// 提供json和<typeparamref name="T"/>对象相互转化的公共基类
    /// </summary>
    /// <typeparam name="T">与之相互转化的类型</typeparam>
    public abstract class ConverToJsonAndObj<T> : IConverToJsonAndObj<T>
    {

        /// <summary>
        /// 是否允许将json对象转化为<typeparamref name="T"/>对象
        /// </summary>
        public abstract bool CanToObj { get; }

        /// <summary>
        /// 是否允许将<typeparamref name="T"/>对象转化为json对象
        /// </summary>
        public abstract bool CanToJson { get; }

        /// <summary>
        /// 该转换器是否允许json与对象相互转化
        /// </summary>
        public virtual bool CanConverAll
        {
            get => CanToJson && CanToObj;
        }

        /// <summary>
        /// 将json对象转换为<typeparamref name="T"/>对象
        /// </summary>
        /// <param name="json">要转换的json对象</param>
        /// <returns>转换后的对象</returns>
        /// <exception cref="NotImplementedException">无法转换，格式不正确</exception>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public abstract T ToObj(JsonVariable json);

        /// <summary>
        /// 将<typeparamref name="T"/>对象转换为json对象
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <returns>转换后的json对象</returns>
        public abstract JsonVariable ToJsonVariable(T obj);

        /// <summary>
        /// 默认实现的转换方法
        /// </summary>
        /// <remarks>
        /// <para>默认实现的json对象与.NET对象转换器，根据<typeparamref name="T"/>类型实现不同转换方法</para>
        /// <para>当<typeparamref name="T"/>是基本类型时，例如<see cref="int"/>、<see cref="float"/>、<see cref="string"/>，会直接与<see cref="JsonVariable"/>对象相匹配或相近的类型互相转换，例如<see cref="int"/>和<see cref="Cheng.Json.JsonType.Integer"/>，<see cref="float"/>和<see cref="JsonType.RealNum"/>；以及其他.NET内置类型<see cref="Guid"/>，<see cref="TimeSpan"/>也实现了相应的转换策略</para>
        /// <para>当<typeparamref name="T"/>是枚举类型时，会使用枚举类型的基类型整数实现与<see cref="JsonType.Integer"/>类型的转换器</para>
        /// <para>当<typeparamref name="T"/>是派生于<see cref="System.Array"/>的数组类型时，会实现与<see cref="JsonList"/>的相互转换，数组元素使用与之相对应的默认实现方法</para>
        /// <para>当<typeparamref name="T"/>类型属于<see cref="Dictionary{TKey, TValue}"/>，且Key的类型是字符串<see cref="string"/>时，将使用<see cref="JsonDictionary"/>与之转换，Value对象采用默认实现的转换方案</para>
        /// <para>
        /// 如果类型<typeparamref name="T"/>实现了<see cref="IConverToJson"/>接口，将会使用实现的接口来实现<see cref="ToJsonVariable(T)"/>函数；<br/>
        /// 如果<typeparamref name="T"/>存在一个构造函数，该构造参数仅有一个<see cref="JsonVariable"/>类型的参数时，则会基于此构造函数实现<see cref="ToObj(JsonVariable)"/>函数
        /// </para>
        /// </remarks>
        public static ConverToJsonAndObj<T> Default
        {
            get => LazyObj.def;
        }

        #region 封装

        private static class LazyObj
        {
            public readonly static ConverToJsonAndObj<T> def = f_createDef();
        }

        static ConverToJsonAndObj<T> f_createDef()
        {
            var type = typeof(T);

            try
            {
                ConverToJsonAndObj<T> re;

                re = f_createPrimitiveObj();
                if (re != null) return re;

                re = f_createEnum();
                if (re != null) return re;

                re = f_createOther();
                if (re != null) return re;

                //继承接口
                var isConIF = typeof(IConverToJson).IsAssignableFrom(type);
                //单参json构造
                var ctor = f_getCtor(type, typeof(JsonVariable));

                if ((!isConIF) && ctor is null)
                {
                    return new c_ConverToNotFuncDef();
                }
                return f_createInterfaceAndCtorByEnd(isConIF, ctor);
            }
            catch (Exception)
            {
                return new c_ConverToNotFuncDef();
            }
        }

        private sealed class c_ConverToNotFuncDef : ConverToJsonAndObj<T>
        {
            public override bool CanToObj => false;
            public override bool CanToJson => false;
            public override JsonVariable ToJsonVariable(T obj) => throw new NotImplementedException();
            public override T ToObj(JsonVariable json) => throw new NotImplementedException();
        }

        static ConstructorInfo f_getCtor(Type type, Type ifOnceParType)
        {
            //var type = typeof(T);
            //不能构造
            if (type.IsAbstract || type.IsInterface) return null;
            var ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var ctor in ctors)
            {
                var pars = ctor.GetParameters();
                if(pars.Length == 1)
                {
                    var par = pars[0];
                    if (par.ParameterType == ifOnceParType)
                    {
                        return ctor;
                    }
                }
            }

            return null;
        }

        static ConverToJsonAndObj<T> f_createInterfaceAndCtorByEnd(bool isIF, ConstructorInfo ctor)
        {
            var type = typeof(T);

            if (type.IsValueType)
            {
                //值类型
                if (isIF)
                {
                    var mt = typeof(c_ConverTo_IFAndCtorV<>).MakeGenericType(type);
                    //单参ctor构造
                    var mtctor = f_getCtor(mt, typeof(ConstructorInfo));
                    return mtctor.Invoke(new object[] { ctor }) as ConverToJsonAndObj<T>;
                }
                else
                {
                    var mt = typeof(c_ConverTo_Ctor<>).MakeGenericType(type);
                    //单参ctor构造
                    var mtctor = f_getCtor(mt, typeof(ConstructorInfo));
                    return mtctor.Invoke(new object[] { ctor }) as ConverToJsonAndObj<T>;
                }
            }

            if (isIF)
            {
                var mt = typeof(c_ConverTo_IFAndCtor<>).MakeGenericType(type);
                //单参ctor构造
                var mtctor = f_getCtor(mt, typeof(ConstructorInfo));
                return mtctor.Invoke(new object[] { ctor }) as ConverToJsonAndObj<T>;
            }
            else
            {
                var mt = typeof(c_ConverTo_Ctor<>).MakeGenericType(type);
                //单参ctor构造
                var mtctor = f_getCtor(mt, typeof(ConstructorInfo));
                return mtctor.Invoke(new object[] { ctor }) as ConverToJsonAndObj<T>;
            }

        }

        static ConverToJsonAndObj<T> f_createPrimitiveObj()
        {
            var type = typeof(T);

            if(type == typeof(long))
            {
                return new c_ConverTo_Int64() as ConverToJsonAndObj<T>;
            }
            else if (type == typeof(double))
            {
                return new c_ConverTo_Double() as ConverToJsonAndObj<T>;
            }
            else if (type == typeof(string))
            {
                return new c_ConverTo_String() as ConverToJsonAndObj<T>;
            }
            else if (type == typeof(float))
            {
                return new c_ConverTo_Float() as ConverToJsonAndObj<T>;
            }
            else if (type == typeof(bool))
            {
                return new c_ConverTo_Boolean() as ConverToJsonAndObj<T>;
            }
            else if (type == typeof(int))
            {
                return new c_ConverTo_Int32() as ConverToJsonAndObj<T>;
            }
            else if (type == typeof(uint))
            {
                return new c_ConverTo_UInt32() as ConverToJsonAndObj<T>;
            }
            else if (type == typeof(ulong))
            {
                return new c_ConverTo_UInt64() as ConverToJsonAndObj<T>;
            }
            else if (type == typeof(short))
            {
                return new c_ConverTo_Int16() as ConverToJsonAndObj<T>;
            }
            else if (type == typeof(ushort))
            {
                return new c_ConverTo_UInt16() as ConverToJsonAndObj<T>;
            }

            if(type == typeof(decimal))
            {
                return new c_ConverTo_Decimal() as ConverToJsonAndObj<T>;
            }

            if (type == typeof(byte))
            {
                return new c_ConverTo_Byte() as ConverToJsonAndObj<T>;
            }
            else if (type == typeof(sbyte))
            {
                return new c_ConverTo_SByte() as ConverToJsonAndObj<T>;
            }
            if(type == typeof(char))
            {
                return new c_ConverTo_Char() as ConverToJsonAndObj<T>;
            }
            return null;
        }

        static bool IsNullableType(Type type, out Type baseType)
        {
            baseType = null;
            if (type.IsValueType)
            {
                if (type.IsGenericType)
                {
                    if (type.ContainsGenericParameters) return false;
                    if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        var t = type.GetGenericArguments();
                        baseType = t[0];
                        return true;
                    }
                }
            }

            return false;
        }

        static ConverToJsonAndObj<T> f_createOther()
        {
            var type = typeof(T);

            if (type == typeof(TimeSpan))
            {
                return new c_ConverTo_TimeSpan() as ConverToJsonAndObj<T>;
            }
            if (type == typeof(DateTime))
            {
                return new c_ConverTo_DateTime() as ConverToJsonAndObj<T>;
            }
            if(type == typeof(Guid))
            {
                return new c_ConverTo_Guid() as ConverToJsonAndObj<T>;
            }

            if (type.IsArray)
            {
                //数组
                return Activator.CreateInstance(typeof(c_ConverTo_Array<>).MakeGenericType(type.GetElementType())) as ConverToJsonAndObj<T>;
                //return new c_ConverTo_Array<T>();
            }

            if (type.IsGenericType)
            {
                //属于泛型
                //获取基础泛型模板类
                var baseGenT = type.GetGenericTypeDefinition();

                if (!type.IsGenericTypeDefinition)
                {
                    //不是模板类

                    if (typeof(List<>) == baseGenT)
                    {
                        //集合
                        return Activator.CreateInstance(typeof(c_ConverTo_List<>).MakeGenericType(type.GetGenericArguments())) as ConverToJsonAndObj<T>;
                    }

                    if (typeof(Dictionary<,>) == baseGenT)
                    {
                        //是字典
                        //获取字典泛型模板类型集合
                        var genArgTypes = type.GetGenericArguments();
                        if (genArgTypes[0] == typeof(string))
                        {
                            //key是字符串
                            return Activator.CreateInstance(typeof(c_ConverTo_Dict<>).MakeGenericType(genArgTypes[1])) as ConverToJsonAndObj<T>;
                        }
                    }

                }
            }

            if (typeof(JsonVariable).IsAssignableFrom(type)) 
            {
                
                return Activator.CreateInstance(typeof(c_ConverTo_This<>).MakeGenericType(type)) as ConverToJsonAndObj<T>;
            }

            if (typeof(Cheng.DataStructure.DynamicVariables.DynVariable) == type)
            {
                return new c_ConverTo_DynObj() as ConverToJsonAndObj<T>;
            }

            if (IsNullableType(type, out Type nullBaseType))
            {
                return Activator.CreateInstance(typeof(c_ConverTo_Nullable<>).MakeGenericType(nullBaseType)) as ConverToJsonAndObj<T>;
            }

            return null;
        }

        static ConverToJsonAndObj<T> f_createEnum()
        {
            var type = typeof(T);

            if (type.IsEnum) return Activator.CreateInstance(typeof(c_ConverTo_Enum<>).MakeGenericType(type)) as ConverToJsonAndObj<T>;
            return null;
        }

        #endregion

    }

}