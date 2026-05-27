using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;

using Cheng.Memorys;

namespace Cheng.DataStructure.UnmanagedMemoryagements
{

    /// <summary>
    /// 非托管固定对象基类
    /// </summary>
    /// <remarks>
    /// <para>基于<see cref="UnmanagedMemoryagement"/>的非托管数据实现，在释放对象时不会主动释放管理器对象</para>
    /// </remarks>
    public abstract class UnObject : SafreleaseUnmanagedResources
    {

        #region 构造

        protected UnObject(UnmanagedMemoryagement umemg)
        {
            p_umemg = umemg ?? throw new ArgumentNullException();
        }

        #endregion

        #region 释放

        /// <summary>
        /// 重写时调用父实现
        /// </summary>
        /// <param name="disposeing"></param>
        /// <returns></returns>
        protected override bool Disposeing(bool disposeing)
        {
            p_umemg = null;
            return true;
        }

        #endregion

        #region 参数

        protected UnmanagedMemoryagement p_umemg;

        #endregion

        #region 功能

        /// <summary>
        /// 该对象是否已被释放或者基对象管理器已释放
        /// </summary>
        public bool ObjectIsRelease
        {
            get
            {
                return IsDispose || p_umemg.IsDispose;
            }
        }

        #endregion

    }

}