using System;
using System.Runtime;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

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
        public static ConverToJsonAndObj<T> Default
        {
            get => cp_def;
        }

        #region 封装

        readonly static ConverToJsonAndObj<T> cp_def = f_createDef();

        static ConverToJsonAndObj<T> f_createDef()
        {
            var type = typeof(T);

            try
            {
                ConverToJsonAndObj<T> re;

                re = f_createPrimitiveObj();
                if (re != null) return re;

                //继承接口
                var isConIF = typeof(IConverToJson).IsAssignableFrom(type);
                //单参json构造
                var ctor = f_getCtor(type, typeof(JsonVariable));

                if((!isConIF))
                {
                    re = f_createOther();
                    if (re != null) return re;
                }

                return f_createInterfaceAndCtorByEnd(isConIF, ctor);
            }
            catch (Exception)
            {
                return new c_ConverToNotFuncDef();
            }

            return new c_ConverToNotFuncDef();
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
            if (type.IsAbstract) return null;
            if(type.IsInterface) return null;
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
            else if (type == typeof(bool))
            {
                return new c_ConverTo_Boolean() as ConverToJsonAndObj<T>;
            }
            else if (type == typeof(string))
            {
                return new c_ConverTo_String() as ConverToJsonAndObj<T>;
            }
            else if (type == typeof(float))
            {
                return new c_ConverTo_Float() as ConverToJsonAndObj<T>;
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
            else if (type == typeof(byte))
            {
                return new c_ConverTo_Byte() as ConverToJsonAndObj<T>;
            }
            else if (type == typeof(sbyte))
            {
                return new c_ConverTo_SByte() as ConverToJsonAndObj<T>;
            }
            return null;
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

            return null;
        }

        #endregion

    }

}