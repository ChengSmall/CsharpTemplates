using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cheng.Threads
{

    public delegate void TaskAction();    

    public delegate void TaskAction<T>(T args);

    public delegate bool TaskFunction();

    public delegate bool TaskFunction<T>(T args);

    /// <summary>
    /// 一个单线程任务
    /// </summary>
    public sealed class SingleTask
    {

        #region 构造

        /// <summary>
        /// 实例化一个单线程任务
        /// </summary>
        /// <param name="task">一个任务调用委托</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public SingleTask(TaskAction task)
        {
            if (task is null) throw new ArgumentNullException();

            p_task = task;
            p_start = false;
            p_over = false;
            p_onList = false;
            p_abnormalOver = false;
        }

        #endregion

        #region 参数

        internal TaskAction p_task;

        internal bool p_start;

        internal bool p_over;

        internal bool p_onList;

        internal bool p_abnormalOver;

        #endregion

        #region 功能

        #region 事件

        /// <summary>
        /// 在开始执行该任务之前引发的事件
        /// </summary>
        public event TaskAction StartTaskEvent;

        /// <summary>
        /// 任务执行完毕后引发的事件
        /// </summary>
        public event TaskAction OverTaskEvent;

        #endregion

        #region 参数访问

        /// <summary>
        /// 该任务是否正处于执行中
        /// </summary>
        public bool Running
        {
            get => p_start && (!p_over);
        }

        /// <summary>
        /// 该任务是否已开始执行
        /// </summary>
        public bool IsStartTask
        {
            get => p_start;
        }

        /// <summary>
        /// 该任务是否已执行完毕
        /// </summary>
        public bool TaskOver
        {
            get => p_over;
        }

        /// <summary>
        /// 该任务是否已执行并且执行完毕
        /// </summary>
        public bool TaskEnd
        {
            get => p_start && p_over;
        }

        /// <summary>
        /// 该任务是否处于任务队列
        /// </summary>
        public bool OnTaskQueue
        {
            get => p_onList;
        }

        /// <summary>
        /// 该任务是否非正常退出
        /// </summary>
        public bool AbnormalOver
        {
            get => p_abnormalOver;
        }

        #endregion

        #region 封装

        internal void f_invokeStartEvent()
        {
            StartTaskEvent?.Invoke();
        }

        internal void f_invokeTask()
        {
            p_task.Invoke();
        }

        internal void f_invokeOverTaskEvent()
        {
            OverTaskEvent?.Invoke();
        }

        internal void f_runTask()
        {
            p_start = true;
            f_invokeStartEvent();
            lock (this)
            {
                f_invokeTask();
            }
            f_invokeOverTaskEvent();
            p_over = true;
        }

        #endregion

        #endregion

    }

}
