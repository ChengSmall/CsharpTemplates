using System;
using System.Collections.Generic;
using System.Collections;
using Cheng.Memorys;

namespace Cheng.DataStructure.FiniteStateMachine
{

    /// <summary>
    /// 有限状态机
    /// </summary>
    /// <typeparam name="T">表示一个状态的参数类型</typeparam>
    public class StateMachine<T> : SafreleaseUnmanagedResources
    {

        #region 释放

        protected override void UnmanagedReleasources()
        {
            p_runNodeEvent = null;
            p_stopNodeEvent = null;
        }

        /// <summary>
        /// 释放和注销状态机事件
        /// </summary>
        public override void Close()
        {
            Dispose(true);
        }

        #endregion

        #region 构造

        /// <summary>
        /// 实例化一个有限状态机
        /// </summary>
        public StateMachine()
        {
            p_nodes = new List<StateNode<T>>();
            p_nowNode = null;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 状态节点集合
        /// </summary>
        protected List<StateNode<T>> p_nodes;

        /// <summary>
        /// 当前处于运行中的节点
        /// </summary>
        protected StateNode<T> p_nowNode;

        /// <summary>
        /// 运行状态时引发
        /// </summary>
        protected StateNodeChangeEvent<T> p_runNodeEvent;
        /// <summary>
        /// 停止状态时引发
        /// </summary>
        protected StateNodeChangeEvent<T> p_stopNodeEvent;

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 状态机的状态节点数量
        /// </summary>
        public virtual int Count => p_nodes.Count;

        /// <summary>
        /// 按索引访问状态节点
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public virtual StateNode<T> this[int index]
        {
            get => p_nodes[index];
            set
            {
                p_nodes[index] = value ?? throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 获取当前正在运行中的状态
        /// </summary>
        /// <returns>若状态机内没有节点运行，则返回null</returns>
        public virtual StateNode<T> NowRunNode
        {
            get => p_nowNode;
        }

        #endregion

        #region 状态节点
        /// <summary>
        /// 添加一个状态节点
        /// </summary>
        /// <param name="node"></param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public virtual void Add(StateNode<T> node)
        {
            p_nodes.Add(node ?? throw new ArgumentNullException());
        }

        /// <summary>
        /// 删除一个状态节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public virtual bool Remove(StateNode<T> node)
        {
            bool flag = p_nodes.Remove(node ?? throw new ArgumentNullException());
            if (flag) node.Close();
            return flag;
        }

        /// <summary>
        /// 删除指定索引的状态节点
        /// </summary>
        /// <param name="index"></param>
        public virtual void RemoveAt(int index)
        {
            p_nodes[index]?.Close();
            p_nodes.RemoveAt(index);
        }

        private void f_clearAction(StateNode<T> node)
        {
            node?.Close();
        }

        /// <summary>
        /// 清空状态机节点
        /// </summary>
        public virtual void Clear()
        {
            p_nodes.ForEach(f_clearAction);
            p_nodes.Clear();
        }

        /// <summary>
        /// 查找匹配的节点
        /// </summary>
        /// <param name="match">条件谓词</param>
        /// <returns>第一个匹配的节点，没有匹配则为null</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public virtual StateNode<T> Find(Predicate<StateNode<T>> match)
        {
            return p_nodes.Find(match);
        }

        /// <summary>
        /// 查找匹配的节点
        /// </summary>
        /// <param name="match">条件谓词</param>
        /// <param name="node">第一个匹配的节点，没有匹配则为null</param>
        /// <returns>是否成功找到匹配的节点，找到返回true，没有返回false</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public virtual bool Find(Predicate<StateNode<T>> match, out StateNode<T> node)
        {
            int index = p_nodes.FindIndex(match);

            if(index < 0)
            {
                node = null;
                return false;
            }

            node = p_nodes[index];
            return true;
        }

        /// <summary>
        /// 确定是否存在指定节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns>存在返回true，不存在返回false</returns>
        public virtual bool Contains(StateNode<T> node)
        {
            return p_nodes.Contains(node);
        }

        /// <summary>
        /// 查找是否存在匹配的节点
        /// </summary>
        /// <param name="match">谓词</param>
        /// <returns>若存在匹配项返回true，不存在返回false</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public virtual bool Contains(Predicate<StateNode<T>> match)
        {
            return p_nodes.FindIndex(match) >= 0;
        }

        /// <summary>
        /// 搜索与指定谓词匹配的节点并返回索引
        /// </summary>
        /// <param name="match">谓词</param>
        /// <returns>匹配的节点索引，若没有匹配项返回-1</returns>
        public virtual int FindIndex(Predicate<StateNode<T>> match)
        {
            return p_nodes.FindIndex(match);
        }

        /// <summary>
        /// 查找该状态机下是否包含指定节点，并返回索引
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns>找到节点返回该节点索引，没有找到返回-1</returns>
        public virtual int IndexOf(StateNode<T> node)
        {
            return p_nodes.IndexOf(node);
        }

        #endregion

        #region 事件

        /// <summary>
        /// 开始运行一个状态时引发的事件
        /// </summary>
        /// <exception cref="ObjectDisposedException">时间系统已注销</exception>
        public virtual event StateNodeChangeEvent<T> RunStateNodeEvent
        {
            add
            {
                ThrowObjectDisposeException();
                if (p_runNodeEvent is null) p_runNodeEvent += value;
                else lock (p_runNodeEvent) p_runNodeEvent += value;
            }
            remove
            {
                ThrowObjectDisposeException();
                if (p_runNodeEvent is null) return;
                else lock (p_runNodeEvent) p_runNodeEvent -= value;
            }
        }

        /// <summary>
        /// 开始停止运行一个状态时引发的事件
        /// </summary>
        /// <exception cref="ObjectDisposedException">时间系统已注销</exception>
        public virtual event StateNodeChangeEvent<T> StopStateNodeEvent
        {
            add
            {
                ThrowObjectDisposeException();
                if (p_stopNodeEvent is null) p_stopNodeEvent += value;
                else lock (p_stopNodeEvent) p_stopNodeEvent += value;
            }
            remove
            {
                ThrowObjectDisposeException();
                if (p_stopNodeEvent is null) return;
                else lock (p_stopNodeEvent) p_stopNodeEvent -= value;
            }
        }

        #endregion

        #region 状态切换

        /// <summary>
        /// 仅运行指定索引下节点状态
        /// </summary>
        /// <remarks>该方法仅运行某个节点下的状态，不会对其它状态进行任何操作，不会修改运行状态参数</remarks>
        /// <param name="index">索引</param>
        /// <returns>开始运行的状态节点</returns>
        /// <exception cref="ArgumentException">索引超出范围</exception>
        public virtual StateNode<T> OnlyRunState(int index)
        {
            var node = p_nodes[index];
            p_runNodeEvent?.Invoke(node);
            return node;
        }

        /// <summary>
        /// 仅停止指定索引下的节点状态
        /// </summary>
        /// <remarks>该方法仅停止某个节点下的状态，不会对其它状态进行任何操作，不会修改运行状态参数</remarks>
        /// <param name="index">索引</param>
        /// <returns>停止运行的状态节点</returns>
        /// <exception cref="ArgumentException">索引超出范围</exception>
        public virtual StateNode<T> OnlyStopState(int index)
        {
            var node = p_nodes[index];
            p_stopNodeEvent?.Invoke(node);
            return node;
        }

        /// <summary>
        /// 仅设置当前正在运行的节点
        /// </summary>
        /// <param name="index">状态机下的节点索引</param>
        /// <exception cref="ArgumentException">索引超出范围</exception>
        public virtual void OnlySetNowState(int index)
        {
            p_nowNode = p_nodes[index];
        }

        /// <summary>
        /// 仅将当前正在运行的节点实例设置为null
        /// </summary>
        public virtual void ClearNowState()
        {
            p_nowNode = null;
        }

        /// <summary>
        /// 停止运行该状态机下的所有节点状态
        /// </summary>
        public virtual void StopAll()
        {

            lock (p_nodes)
            {
                int length = p_nodes.Count;
                for (int i = 0; i < length; i++)
                {
                    OnlyStopState(i);
                }
            }

            p_nowNode = null;
        }

        /// <summary>
        /// 初始化运行节点
        /// </summary>
        /// <remarks>用于第一次初始化状态机，该方法仅运行一个状态，不会停止其它状态</remarks>
        /// <param name="index">要初始化的节点索引</param>
        /// <exception cref="ArgumentException">索引超出范围</exception>
        public virtual void InitState(int index)
        {
            p_nowNode = OnlyRunState(index);
        }

        /// <summary>
        /// 切换到指定连接的索引状态
        /// </summary>
        /// <param name="linkIndex">要切换到的当前运行状态的连接索引</param>
        /// <exception cref="ArgumentException">索引超出范围或当前没有运行的状态</exception>
        public virtual void SwitchingState(int linkIndex)
        {
            if (linkIndex < 0 || linkIndex >= p_nodes.Count) throw new ArgumentOutOfRangeException();

            if (p_nowNode is null) throw new ArgumentNullException();

            var node = p_nowNode[linkIndex];

            p_stopNodeEvent?.Invoke(p_nowNode);
            p_runNodeEvent?.Invoke(node);

            p_nowNode = node;
        }

        /// <summary>
        /// 直接运行指定连接的索引状态
        /// </summary>
        /// <remarks>仅运行指定连接索引下的状态，不会停止上一个状态</remarks>
        /// <param name="linkIndex">要运行当前运行状态所在的连接索引状态</param>
        /// <exception cref="ArgumentException">索引超出范围或当前没有运行的状态</exception>
        public virtual void RunState(int linkIndex)
        {
            if (linkIndex < 0 || linkIndex >= p_nodes.Count) throw new ArgumentOutOfRangeException();

            if (p_nowNode is null) throw new ArgumentNullException();
            var node = p_nowNode[linkIndex];

            p_runNodeEvent?.Invoke(node);
            p_nowNode = node;
        }
        

        /// <summary>
        /// 停止当前运行的状态
        /// </summary>
        /// <returns>是否成功停止，成功停止返回true；若没有运行状态返回false</returns>
        public virtual bool StopNowState()
        {
            if (p_nowNode is null) return false;

            p_stopNodeEvent?.Invoke(p_nowNode);
            p_nowNode = null;
            return true;
        }

        /// <summary>
        /// 切换到指定匹配的状态
        /// </summary>
        /// <param name="match">条件谓词</param>
        /// <returns>若在当前运行状态的连接状态找到匹配项，返回true；没有匹配项返回false</returns>
        /// <exception cref="ArgumentException">没有运行状态</exception>
        public virtual bool SwitchingState(Predicate<StateNode<T>> match)
        {

            if (p_nowNode is null) throw new ArgumentNullException();

            lock (p_nowNode)
            {
                int index = p_nowNode.FindIndex(match);

                if (index < 0) return false;

                SwitchingState(index);

                return true;
            }
         
        }

        /// <summary>
        /// 初始化第一个匹配的运行节点
        /// </summary>
        /// <param name="match">条件谓词</param>
        /// <returns>若在当前状态机内找到匹配项，返回true；没有匹配项返回false</returns>
        public virtual bool InitState(Predicate<StateNode<T>> match)
        {
            lock (p_nodes)
            {
                int index = p_nodes.FindIndex(match);

                if (index < 0) return false;

                InitState(index);

                return true;
            }
        }

        /// <summary>
        /// 切换到指定的状态
        /// </summary>
        /// <param name="node">状态节点</param>
        /// <returns>若指定的状态节点在当前运行状态的连接状态找到，返回true，否则返回false</returns>
        /// <exception cref="ArgumentException">没有运行状态</exception>
        public virtual bool SwitchingState(StateNode<T> node)
        {

            if (p_nowNode is null) throw new ArgumentNullException();

            lock (p_nowNode)
            {
                int index = p_nowNode.IndexOf(node);

                if (index < 0) return false;

                SwitchingState(index);

                return true;
            }

        }

        /// <summary>
        /// 重置状态机并运行指定状态机索引下的状态节点
        /// </summary>
        /// <param name="index">索引</param>
        /// <exception cref="ArgumentException">索引超出范围</exception>
        public virtual void ResetRunState(int index)
        {

            if (index < 0 || index >= p_nodes.Count) throw new ArgumentOutOfRangeException();
            if(p_nowNode != null) p_stopNodeEvent?.Invoke(p_nowNode);
            p_nowNode = OnlyRunState(index);

        }

        #endregion

        #endregion

    }

}
