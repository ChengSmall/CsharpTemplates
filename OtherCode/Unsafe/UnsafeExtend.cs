using Cheng.Streams;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Cheng.Memorys
{

    /// <summary>
    /// 不安全代码的低级别内存操作扩展
    /// </summary>
    /// <remarks>
    /// <para><see cref="System.Runtime.CompilerServices.Unsafe"/> 类的扩展操作，在使用该功能之前请确保程序已正确引用<see cref="System.Runtime.CompilerServices.Unsafe"/></para>
    /// </remarks>
    public static unsafe class UnsafeExtend
    {

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
            return ref Unsafe.AsRef<T>(ptr.ToPointer());
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
        /// <param name="source">要复制的值</param>
        /// <param name="copyTo">要复制到的目标位置</param>
        public static void Copy<T>(void* source, ref T copyTo)
        {
            Unsafe.Copy(ref copyTo, source);
        }

        /// <summary>
        /// 将指定地址指向的之以<typeparamref name="T"/>对象写入到指定目标
        /// </summary>
        /// <typeparam name="T">指针指向的内容要解释的类型</typeparam>
        /// <param name="source">要复制的值</param>
        /// <param name="copyTo">要复制到的目标位置</param>
        public static void Copy<T>(ref T source, void* copyTo)
        {
            Unsafe.Copy(copyTo, ref source);
        }

        /// <summary>
        /// 将一个内存块拷贝到另一个内存块中
        /// </summary>
        /// <param name="source">要拷贝的原内存地址</param>
        /// <param name="copyTo">要拷贝到的位置</param>
        /// <param name="size">要拷贝的内存块大小</param>
        public static void CopyBlock(void* source, void* copyTo, uint size)
        {
            Unsafe.CopyBlock(copyTo, source, size);
        }

        /// <summary>
        /// 将一个内存块拷贝到另一个内存块中
        /// </summary>
        /// <param name="source">要拷贝的原内存地址</param>
        /// <param name="copyTo">要拷贝到的位置</param>
        /// <param name="size">要拷贝的内存块大小</param>
        public static void CopyBlock(IntPtr source, IntPtr copyTo, uint size)
        {
            CopyBlock(source.ToPointer(), copyTo.ToPointer(), size);
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

        /// <summary>
        /// 从指定引用添加指定元素类型的偏移量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">要添加偏移的引用</param>
        /// <param name="elementOffset">类型偏移量，偏移块大小由类型大小决定</param>
        /// <returns>添加偏移后的引用</returns>
        public static ref T Add<T>(ref T source, int elementOffset)
        {
            return ref Unsafe.Add<T>(ref source, elementOffset);
        }

        /// <summary>
        /// 从指定引用添加指定元素类型的偏移量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">要添加偏移的引用</param>
        /// <param name="elementOffset">类型偏移量，偏移块大小由类型大小决定</param>
        /// <returns>添加偏移后的引用</returns>
        public static ref T Add<T>(ref T source, IntPtr elementOffset)
        {
            return ref Unsafe.Add<T>(ref source, elementOffset);
        }

        /// <summary>
        /// 从指定引用添加指定元素类型的偏移量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">要添加偏移的引用</param>
        /// <param name="elementOffset">类型偏移量，偏移块大小由类型大小决定</param>
        /// <returns>添加偏移后的引用</returns>
        public static ref T Add<T>(ref T source, void* elementOffset)
        {
            return ref Unsafe.Add<T>(ref source, new IntPtr(elementOffset));
        }

        /// <summary>
        /// 从指定引用添加指定元素类型的偏移量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">要添加偏移的引用</param>
        /// <param name="elementOffset">类型偏移量，偏移块大小由类型大小决定</param>
        /// <returns>添加偏移后的引用</returns>
        public static ref T Add<T>(ref T source, CPtr elementOffset)
        {
            return ref Unsafe.Add<T>(ref source, new IntPtr(elementOffset.ToPointer()));
        }

        #endregion

    }

}
#if DEBUG
#endif