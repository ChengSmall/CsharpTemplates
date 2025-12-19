using Cheng.Streams;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Cheng.Memorys
{

    /// <summary>
    /// 不安全代码的低级别内存操作口
    /// </summary>
    /// <remarks>
    /// <para>使用 System.Runtime.CompilerServices.Unsafe 类进行更底层的操作，在调用相关函数之前请确保程序已正确解析<see cref="UnsafeExtend.UnsafeAssembly"/></para>
    /// </remarks>
    public static unsafe class UnsafeExtend
    {

        #region 程序集

        #region 封装

        private class Lock_Obj
        {
            public Lock_Obj(Assembly @unsafe)
            {
                p_unsafe = @unsafe;
                p_event = fe_AppDomain_AssResolve;
            }

            public Assembly p_unsafe;
            public ResolveEventHandler p_event;

            private Assembly fe_AppDomain_AssResolve(object sender, ResolveEventArgs args)
            {
                var assName = args?.Name;
                if (string.IsNullOrEmpty(assName)) return null;
                assName = getSimpleAssemblyName(assName);
                if (assName == p_unsafe.GetName().Name)
                {
                    return p_unsafe;
                }
                return null;
            }
        }

        static readonly Lock_Obj p_lock;

        static UnsafeExtend()
        {
            var s = typeof(UnsafeExtend).Assembly.GetManifestResourceStream("Cheng.Resources.System.Runtime.CompilerServices.Unsafe.dll");
            p_lock = new Lock_Obj(Assembly.Load(s.ReadAll()));

        }

        static string getSimpleAssemblyName(string fullName)
        {
            int i;
            for (i = 0; i < fullName.Length; i++)
            {
                if (fullName[i] == ',') return fullName.Substring(0, i);
            }
            return fullName;
        }

        #endregion

        /// <summary>
        /// Unsafe程序集对象
        /// </summary>
        public static Assembly UnsafeAssembly => p_lock.p_unsafe;

        /// <summary>
        /// 对程序域解析<see cref="UnsafeAssembly"/>的函数
        /// </summary>
        public static ResolveEventHandler UnsafeResolveEventHandler
        {
            get => p_lock.p_event;
        }

        /// <summary>
        /// 向当前线程所在的程序域注册<see cref="UnsafeResolveEventHandler"/>事件
        /// </summary>
        public static void CurrentDomainRegisterUnsafeResolveEvent()
        {
            var dom = AppDomain.CurrentDomain;
            if(dom != null) dom.AssemblyResolve += p_lock.p_event;
        }

        /// <summary>
        /// 向指定程序域的<see cref="AppDomain.AssemblyResolve"/>注册<see cref="UnsafeResolveEventHandler"/>事件
        /// </summary>
        /// <remarks>
        /// <para>在使用Unsafe相关方法之前请确保已对程序域注册该事件，或者已手动配置解析<see cref="UnsafeAssembly"/>对象的代码</para>
        /// </remarks>
        /// <param name="dom">要注册事件的程序集</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public static void RegisterUnsafeResolveEvent(AppDomain dom)
        {
            if (dom is null) throw new ArgumentNullException();
            dom.AssemblyResolve += p_lock.p_event;
        }

        /// <summary>
        /// 向指定程序域的<see cref="AppDomain.AssemblyResolve"/>注销<see cref="UnsafeResolveEventHandler"/>事件
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="dom">要注销事件的程序集</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public static void UnRegisterUnsafeResolveEvent(AppDomain dom)
        {
            if (dom is null) throw new ArgumentNullException();
            dom.AssemblyResolve -= p_lock.p_event;
        }

        #endregion

        #region Unsafe

        /// <summary>
        /// 返回<typeparamref name="T"/>类型对象变量所在的地址
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">对象引用</param>
        /// <returns>引用的变量地址</returns>
        public static void* AsPointer<T>(ref T obj)
        {
            return Unsafe.AsPointer(ref obj);
        }

        /// <summary>
        /// 返回<typeparamref name="T"/>类型对象变量所在的地址
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">对象引用</param>
        /// <returns>引用的变量地址</returns>
        public static CPtr AsCPtr<T>(ref T obj)
        {
            return new CPtr(Unsafe.AsPointer(ref obj));
        }

        /// <summary>
        /// 返回<typeparamref name="T"/>类型对象变量所在的地址
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">对象引用</param>
        /// <returns>引用的变量地址</returns>
        public static IntPtr AsIntPtr<T>(ref T obj)
        {
            return new IntPtr(Unsafe.AsPointer(ref obj));
        }

        /// <summary>
        /// 返回类型<typeparamref name="T"/>占用的大小
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int Sizeof<T>()
        {
            return Unsafe.SizeOf<T>();
        }

        /// <summary>
        /// 将指定地址转化为描述<typeparamref name="T"/>类型变量的引用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr">地址</param>
        /// <returns><paramref name="ptr"/>指向的变量引用</returns>
        public static ref T AsRef<T>(void* ptr)
        {
            return ref Unsafe.AsRef<T>(ptr);
        }

        /// <summary>
        /// 将指定地址转化为描述<typeparamref name="T"/>类型变量的引用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr">地址</param>
        /// <returns><paramref name="ptr"/>指向的变量引用</returns>
        public static ref T AsRef<T>(CPtr ptr)
        {
            return ref Unsafe.AsRef<T>(ptr.p_ptr);
        }

        /// <summary>
        /// 将指定地址转化为描述<typeparamref name="T"/>类型变量的引用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr">地址</param>
        /// <returns><paramref name="ptr"/>指向的变量引用</returns>
        public static ref T AsRef<T>(IntPtr ptr)
        {
            return ref Unsafe.AsRef<T>(ptr.ToPointer());
        }

        /// <summary>
        /// 将给定的只读引用重新解释为对象引用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">只读对象</param>
        /// <returns>只读对象的引用</returns>
        public static ref T AsRef<T>(in T obj)
        {
            return ref Unsafe.AsRef(in obj);
        }

        /// <summary>
        /// 计算指定引用中从原对象到目标对象的字节偏移量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="origin">原点对象</param>
        /// <param name="target">目标对象</param>
        /// <returns>字节偏移量</returns>
        public static IntPtr ByteOffset<T>(ref T origin, ref T target)
        {
            return Unsafe.ByteOffset<T>(ref origin, ref target);
        }

        /// <summary>
        /// 确定两个引用是否指向同一位置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>两个引用是否指向同一位置，是同一个位置返回true，否则返回false</returns>
        public static bool AreSame<T>(ref T left, ref T right)
        {
            return Unsafe.AreSame(ref left, ref right);
        }

        /// <summary>
        /// 将对象引用<paramref name="obj"/>重新解释为对<typeparamref name="OT"/>类型变量的引用
        /// </summary>
        /// <typeparam name="T">原对象类型</typeparam>
        /// <typeparam name="OT">同一位置的新类型</typeparam>
        /// <param name="obj">引用对象</param>
        /// <returns>指向同一位置的<typeparamref name="OT"/>对象引用</returns>
        public static ref OT AsOther<T, OT>(ref T obj)
        {
            return ref Unsafe.As<T, OT>(ref obj);
        }

        /// <summary>
        /// 返回对指定装箱对象中包含的值类型的引用
        /// </summary>
        /// <typeparam name="T">装箱对象中包含的值的具体类型</typeparam>
        /// <param name="box">装箱到<see cref="object"/>的值类型对象</param>
        /// <returns>指向装箱对象中类型为<typeparamref name="T"/>的值的引用</returns>
        public static ref T Unbox<T>(object box) where T : struct
        {
            return ref Unsafe.Unbox<T>(box);
        }

        /// <summary>
        /// 将类型为<typeparamref name="T"/>的值复制到指定位置
        /// </summary>
        /// <typeparam name="T">要复制的值的类型</typeparam>
        /// <param name="destination">要复制的对象</param>
        /// <param name="source">要复制到的目标位置</param>
        public static void Copy<T>(ref T destination, void* source)
        {
            Unsafe.Copy(ref destination, source);
        }

        /// <summary>
        /// 将指定地址指向的之以<typeparamref name="T"/>对象写入到指定目标
        /// </summary>
        /// <typeparam name="T">指针指向的内容要解释的类型</typeparam>
        /// <param name="destination">指向要复制的内容</param>
        /// <param name="source">要复制到的目标位置引用</param>
        public static void Copy<T>(void* destination, ref T source)
        {
            Unsafe.Copy(destination, ref source);
        }

        /// <summary>
        /// 将给定对象强制转换为<typeparamref name="T"/>类型，且不执行动态类型检查
        /// </summary>
        /// <remarks>
        /// <para>如果你已经确保<paramref name="obj"/>对象属于可以安全转化为<typeparamref name="T"/>类型的对象，该方法可免去额外动态类型检查的开销</para>
        /// </remarks>
        /// <typeparam name="T">一个CLR托管类</typeparam>
        /// <param name="obj">对象</param>
        /// <returns>与<paramref name="obj"/>相同实例但类型不同的对象</returns>
        public static T AsTo<T>(object obj) where T : class
        {
            return Unsafe.As<T>(obj);
        }

        #endregion

    }

}
#if DEBUG
#endif