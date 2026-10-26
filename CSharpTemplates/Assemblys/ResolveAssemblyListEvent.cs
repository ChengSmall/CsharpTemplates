using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Runtime;

namespace Cheng.Systems.AppDomains
{

    /// <summary>
    /// <see cref="ResolveEventHandler"/>委托顺序获取返回值的委托封装
    /// </summary>
    public sealed class ResolveAssemblyListEvent
    {

        #region 构造

        /// <summary>
        /// 实例化<see cref="ResolveEventHandler"/>委托封装
        /// </summary>
        public ResolveAssemblyListEvent()
        {
            p_list = new LinkedList<ResolveEventHandler>();
        }

        #endregion

        #region 参数

        private LinkedList<ResolveEventHandler> p_list;

        #endregion

        #region 功能

        /// <summary>
        /// 注册一个事件
        /// </summary>
        public event ResolveEventHandler AssemblyResolveEvent
        {
            add
            {
                if (value is null) return;
                lock (p_list)
                {
                    p_list.AddLast(value);
                }
            }
            remove
            {
                if (value is null) return;
                lock (p_list)
                {
                    var node = p_list.First;
                    while (node != null)
                    {
                        var va = node.Value;
                        if(va == value)
                        {
                            p_list.Remove(node);
                            break;
                        }
                        node = node.Next;
                    }
                }
            }
        }

        /// <summary>
        /// 清空所有注册的事件
        /// </summary>
        public void ClearEvent()
        {
            lock (p_list) p_list.Clear();
        }

        /// <summary>
        /// 按顺序调用从<see cref="AssemblyResolveEvent"/>注册的委托对象
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns>第一个获取到非null的对象；如果全部是null返回null</returns>
        public Assembly ListResolveEvent(object sender, ResolveEventArgs args)
        {
            var node = p_list.First;
            while (node != null)
            {
                var va = node.Value;
                if (va is object)
                {
                    var ass = va.Invoke(sender, args);
                    if (ass is object) return ass;
                }
                node = node.Next;
            }
            return null;
        }

        #endregion

    }

}
