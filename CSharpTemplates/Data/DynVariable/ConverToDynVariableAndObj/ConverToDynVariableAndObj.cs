using System;
using System.Reflection;

namespace Cheng.DataStructure.DynamicVariables
{

    /// <summary>
    /// 将对象转化为<see cref="DynVariable"/>的公共接口
    /// </summary>
    public interface IObjToDynVariable
    {

        /// <summary>
        /// 将当前对象转化为<see cref="DynVariable"/>
        /// </summary>
        /// <returns>转化后的对象</returns>
        DynVariable ToDynVariable();
    }

    /// <summary>
    /// <typeparamref name="T"/>对象与<see cref="DynVariable"/>相互转化的公共接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConverToDynVariableAndObj<T>
    {

        /// <summary>
        /// 将指定对象转化为<see cref="DynVariable"/>
        /// </summary>
        /// <param name="obj">要转化的对象</param>
        /// <returns>转化后的对象</returns>
        DynVariable ToDynVariable(T obj);

        /// <summary>
        /// 从<see cref="DynVariable"/>提取并转化到<typeparamref name="T"/>对象
        /// </summary>
        /// <param name="dynVariable">要转化的对象</param>
        /// <returns>转化后的对象</returns>
        /// <exception cref="NotImplementedException">无法转化</exception>
        T ToObj(DynVariable dynVariable);

    }

    /// <summary>
    /// <typeparamref name="T"/>对象与<see cref="DynVariable"/>相互转换的公共基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ConverToDynVariableAndObj<T> : IConverToDynVariableAndObj<T>
    {

        public abstract DynVariable ToDynVariable(T obj);

        public abstract T ToObj(DynVariable dynVariable);

        /// <summary>
        /// 默认实现的<see cref="DynVariable"/>转化器
        /// </summary>
        /// <remarks>
        /// <para>检索类型的<see cref="IObjToDynVariable"/>实现与<see cref="DynVariable"/>参数构造函数来创建默认的对象转化方法；以及基元类型对应的对象转换方法；否则无法实现</para>
        /// </remarks>
        public static ConverToDynVariableAndObj<T> Default
        {
            get => sp_defConver;
        }

        #region 封装

        static ConverToDynVariableAndObj<T> sp_defConver = f_createDef();

        static ConverToDynVariableAndObj<T> f_createDef()
        {
            ConverToDynVariableAndObj<T> re;
            var type = typeof(T);

            try
            {
                re = f_IsBaseObj(type);
                if (re != null) return re;

                if (type.IsEnum)
                {
                    //枚举类型
                    re = f_createEnumObj();
                    if (re != null) return re;
                }

                var isToDyn = typeof(IObjToDynVariable).IsAssignableFrom(type);

                type.GetConstructors(System.Reflection.BindingFlags.Public);

                //构造
                var cons = f_getHaveDynParCtor(type);
                //是否有Dyn单参数构造函数
                bool isCons = (object)cons != (object)null;

                bool isValue = type.IsValueType;

                if (isToDyn)
                {
                    if (isCons)
                    {
                        //继承接口且有构造
                        if (isValue)
                        {
                            return f_createStructAllHavCov(type, cons) ?? f_createNotFuncDef();
                        }
                        else
                        {
                            return f_createAllHavCov(type, cons) ?? f_createNotFuncDef();
                        }
                    }
                    else
                    {
                        //继承接口但无构造
                        if (isValue)
                        {
                            return f_createStructOnlyToDyn(type) ?? f_createNotFuncDef();
                        }
                        else
                        {
                            return f_createOnlyToDynv(type) ?? f_createNotFuncDef();
                        }
                    }
                }
                else
                {
                    if (isCons)
                    {
                        //无接口但有构造
                        return new c_NotInterfaceHavCtor<T>(cons);
                    }
                    else
                    {
                        //啥都木有...
                        return f_createNotFuncDef();
                    }
                }
            }
            catch (Exception)
            {
                return f_createNotFuncDef();
            }
        }

        static ConverToDynVariableAndObj<T> f_createEnumObj()
        {
            var type = typeof(T);
            //枚举基础值类型
            var bType = type.GetEnumUnderlyingType();

            if(bType == typeof(int) || bType == typeof(uint))
            {
                return Activator.CreateInstance(typeof(c_ConverTo_Enum_Int32<>).MakeGenericType(type)) as ConverToDynVariableAndObj<T>;
            }
            if (bType == typeof(long) || bType == typeof(ulong))
            {
                return Activator.CreateInstance(typeof(c_ConverTo_Enum_Int64<>).MakeGenericType(type)) as ConverToDynVariableAndObj<T>;
            }
            if (bType == typeof(byte) || bType == typeof(sbyte) || bType == typeof(short) || bType == typeof(ushort))
            {
                return Activator.CreateInstance(typeof(c_ConverTo_Enum_Int32Less<>).MakeGenericType(type)) as ConverToDynVariableAndObj<T>;
            }
            return null;
        }

#if DEBUG
        /// <summary>
        /// 创造有继承接口有构造的值类型泛型派生类
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ctor"></param>
        /// <returns></returns>
#endif
        static ConverToDynVariableAndObj<T> f_createStructAllHavCov(Type type, ConstructorInfo ctor)
        {
            //派生类型有泛型参数type
            var makeGenType = typeof(c_ObjDynAllConverIsStruct<>).MakeGenericType(type);
            //获取构造函数
            var mctor = makeGenType.GetConstructor(new Type[1] { typeof(ConstructorInfo) });
            return mctor.Invoke(new object[] { ctor }) as ConverToDynVariableAndObj<T>;
        }

        static ConverToDynVariableAndObj<T> f_createAllHavCov(Type type, ConstructorInfo ctor)
        {
            //派生类型有泛型参数type
            var makeGenType = typeof(c_ObjDynAllConver<>).MakeGenericType(type);
            //获取构造函数
            var mctor = makeGenType.GetConstructor(new Type[1] { typeof(ConstructorInfo) });
            return mctor.Invoke(new object[] { ctor }) as ConverToDynVariableAndObj<T>;
        }

        static ConverToDynVariableAndObj<T> f_createStructOnlyToDyn(Type type)
        {
            //派生类型有泛型参数type
            var makeGenType = typeof(c_ObjDynAllConverIsStruct<>).MakeGenericType(type);
            return Activator.CreateInstance(makeGenType) as ConverToDynVariableAndObj<T>;
        }

        static ConverToDynVariableAndObj<T> f_createOnlyToDynv(Type type)
        {
            //派生类型有泛型参数type
            var makeGenType = typeof(c_ObjDynAllConver<>).MakeGenericType(type);
            return Activator.CreateInstance(makeGenType) as ConverToDynVariableAndObj<T>;
        }

        static ConverToDynVariableAndObj<T> f_IsBaseObj(Type type)
        {

            if (typeof(int) == type)
            {
                return new c_ConverTo_Int32() as ConverToDynVariableAndObj<T>;
            }

            if (typeof(long) == type)
            {
                return new c_ConverTo_Int64() as ConverToDynVariableAndObj<T>;
            }

            if (typeof(float) == type)
            {
                return new c_ConverTo_Float() as ConverToDynVariableAndObj<T>;
            }

            if (typeof(double) == type)
            {
                return new c_ConverTo_Double() as ConverToDynVariableAndObj<T>;
            }

            if (typeof(string) == type)
            {
                return new c_ConverTo_String() as ConverToDynVariableAndObj<T>;
            }

            if (typeof(uint) == type)
            {
                return new c_ConverTo_UInt32() as ConverToDynVariableAndObj<T>;
            }

            if (typeof(ulong) == type)
            {
                return new c_ConverTo_UInt64() as ConverToDynVariableAndObj<T>;
            }

            if (typeof(bool) == type)
            {
                return new c_ConverTo_Boolean() as ConverToDynVariableAndObj<T>;
            }

            if (typeof(Guid) == type)
            {
                return new c_ConverTo_Guid() as ConverToDynVariableAndObj<T>;
            }

            if (typeof(TimeSpan) == type)
            {
                return new c_ConverTo_TimeSpan() as ConverToDynVariableAndObj<T>;
            }

            if (typeof(DateTime) == type)
            {
                return new c_ConverTo_DateTime() as ConverToDynVariableAndObj<T>;
            }

            if (typeof(Cheng.Json.JsonVariable) == type)
            {
                return new JsonConverToDynVariable() as ConverToDynVariableAndObj<T>;
            }

            return null;
        }

#if DEBUG
        /// <summary>
        /// 获取存在DynVar单参的构造函数
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
#endif
        static ConstructorInfo f_getHaveDynParCtor(Type type)
        {
            const BindingFlags flags =
                BindingFlags.Public |
                BindingFlags.Instance
                ;
            
            //构造函数组
            var cons = type.GetConstructors(flags);

            foreach (var con in cons)
            {
                //参数组
                var pars = con.GetParameters();
                if(pars.Length == 1)
                {
                    //单参数
                    if (typeof(DynVariable) == pars[0].ParameterType)
                    {
                        //单Dyn构造函数
                        return con;
                    }
                }
            }

            return null;
        }

        static ConverToDynVariableAndObj<T> f_createNotFuncDef()
        {
            return new c_ObjAllNotFunc();
        }

        private sealed class c_ObjAllNotFunc : ConverToDynVariableAndObj<T>
        {
            public override DynVariable ToDynVariable(T obj)
            {
                throw new NotImplementedException();
            }

            public override T ToObj(DynVariable dynVariable)
            {
                throw new NotImplementedException();
            }
        }
        #endregion

    }

    internal class c_ObjDynAllConverIsStruct<T> : ConverToDynVariableAndObj<T> where T : struct, IObjToDynVariable
    {
        public c_ObjDynAllConverIsStruct()
        {
            p_ctor = null;
            p_ctorInvokePar = new object[1];
        }
        public c_ObjDynAllConverIsStruct(ConstructorInfo ctor)
        {
            p_ctor = ctor;
            p_ctorInvokePar = new object[1];
        }

        private readonly ConstructorInfo p_ctor;
        private object[] p_ctorInvokePar;

        public override DynVariable ToDynVariable(T obj)
        {
            return obj.ToDynVariable();
        }

        public override T ToObj(DynVariable dynVariable)
        {
            if(p_ctor is null) throw new NotImplementedException();
            try
            {
                p_ctorInvokePar[0] = dynVariable;
                return (T)p_ctor.Invoke(p_ctorInvokePar);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
        }

    }

    internal class c_ObjDynAllConver<T> : ConverToDynVariableAndObj<T> where T : IObjToDynVariable
    {
        public c_ObjDynAllConver()
        {
            p_empty = DynVariable.CreateEmpty();
            p_ctor = null;
            p_ctorInvokePar = new object[1];
        }

        public c_ObjDynAllConver(ConstructorInfo ctor)
        {
            p_empty = DynVariable.CreateEmpty();
            p_ctor = ctor;
            p_ctorInvokePar = new object[1];
        }

        private readonly DynVariable p_empty;
        private readonly ConstructorInfo p_ctor;
        private object[] p_ctorInvokePar;

        public override DynVariable ToDynVariable(T obj)
        {
            if (obj == null) return p_empty;
            return obj.ToDynVariable();
        }

        public override T ToObj(DynVariable dynVariable)
        {
            if (p_ctor is null) throw new NotImplementedException();
            try
            {
                p_ctorInvokePar[0] = dynVariable;
                return (T)p_ctor.Invoke(p_ctorInvokePar);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
        }
    }

    internal class c_NotInterfaceHavCtor<T> : ConverToDynVariableAndObj<T>
    {

        public c_NotInterfaceHavCtor(ConstructorInfo ctor)
        {
            p_ctor = ctor;
            p_ctorInvokePar = new object[1];
        }
        private readonly ConstructorInfo p_ctor;
        private object[] p_ctorInvokePar;

        public override DynVariable ToDynVariable(T obj)
        {
            throw new NotImplementedException();
        }

        public override T ToObj(DynVariable dynVariable)
        {
            try
            {
                p_ctorInvokePar[0] = dynVariable;
                return (T)p_ctor.Invoke(p_ctorInvokePar);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
        }
    }

}
#if DEBUG

#endif