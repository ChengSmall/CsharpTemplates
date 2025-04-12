using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

using System.Collections;
using System.Collections.Generic;

namespace Cheng.Timers.Unitys
{

    /// <summary>
    /// 计时触发器
    /// </summary>
    /// <remarks>
    /// 使用协程等待实现的触发器机制，每次经过特定时间执行一次事件
    /// </remarks>
#if UNITY_EDITOR
    [AddComponentMenu("Cheng/其它/计时触发器")]
#endif
    public sealed class UnityTimerEvent : MonoBehaviour
    {

        public UnityTimerEvent()
        {
            realTime = true;
            invokeAction = true;
            actionEvent = new UnityEvent();
            timeSpanEvent = 2f;
            p_runCoroutine = null;
        }

        #region 参数

        /// <summary>
        /// 事件执行的时间间隔
        /// </summary>
#if UNITY_EDITOR
        [Tooltip("事件执行的时间间隔")]
        [Min(0)]
#endif
        [SerializeField] private float timeSpanEvent;

#if UNITY_EDITOR
        [Tooltip("计时触发器是否为真实运行时间")]
#endif
        [SerializeField] private bool realTime;

#if UNITY_EDITOR
        [Tooltip("是否进行事件触发")]
#endif
        [SerializeField] private bool invokeAction;

        [SerializeField]
        private UnityEvent actionEvent;

        #endregion

        #region 可访问参数

        /// <summary>
        /// 触发器时间间隔
        /// </summary>
        public float TimeSpanEvent
        {
            get => timeSpanEvent;
            set
            {
                if (value <= 0) throw new System.ArgumentOutOfRangeException("value", "参数小于或等于0");
                timeSpanEvent = value;
                f_ifStartResetRun();
            }
        }

        /// <summary>
        /// 计时触发器是否为真实运行时间
        /// </summary>
        /// <value>
        /// <para>如果参数设为true，则使用无视时间缩放参数的等待时间；若参数为false，则触发等待时间与时间缩放参数相关联</para>
        /// </value>
        public bool RealTime
        {
            get => realTime;
            set
            {
                bool iv = realTime != value;
                realTime = value;
                if(iv) f_ifStartResetRun();
            }
        }

        /// <summary>
        /// 是否进行事件触发
        /// </summary>
        /// <value>
        /// <para>若该参数设为false，则在计时触发器进行事件触发时不进行触发，但是不影响运行；参数为true时正常触发事件</para>
        /// </value>
        public bool InvokeAction
        {
            get => invokeAction;
            set => invokeAction = value;
        }

        /// <summary>
        /// 当前触发器是否处于运作状态
        /// </summary>
        public bool Running
        {
            get => (object)p_runCoroutine != null;
        }

        #endregion

        #region 事件

        /// <summary>
        /// 每次时间到期要执行的事件
        /// </summary>
        public event UnityAction ActionEvent
        {
            add
            {
                actionEvent.AddListener(value);
            }
            remove
            {
                actionEvent.RemoveListener(value);
            }
        }

        /// <summary>
        /// 每次时间到期后执行的Unity事件
        /// </summary>
        public UnityEvent UnityActionEvent
        {
            get => actionEvent;
        }

        #endregion

        #region 功能

        /// <summary>
        /// 停止计时触发器运行
        /// </summary>
        public void StopTime()
        {
            f_stopRun();
        }

        /// <summary>
        /// 开始计时器运行
        /// </summary>
        public void StartTime()
        {
            if(enabled) f_startRun();
        }
        //public

        #endregion

        #region 运行

        private IEnumerator f_CoroutineRun(float waitTime, bool real)
        {
            object waitObj;
            if (real)
            {
                if(waitTime == 0)
                {
                    waitObj = null;
                }
                else
                {
                    waitObj = new WaitForSecondsRealtime(waitTime);
                }
                
            }
            else
            {
                if (waitTime == 0)
                {
                    waitObj = null;
                }
                else
                {
                    waitObj = new WaitForSeconds(waitTime);
                }
                
            }

            Loop:
            yield return waitObj;
            if(invokeAction) this.actionEvent?.Invoke();
            goto Loop;
        }

        private Coroutine p_runCoroutine;

        private void f_startRun()
        {
            if ((object)p_runCoroutine != null) StopCoroutine(p_runCoroutine);
            p_runCoroutine = StartCoroutine(f_CoroutineRun(this.timeSpanEvent, realTime));
        }

        private void f_stopRun()
        {
            if ((object)p_runCoroutine != null) StopCoroutine(p_runCoroutine);
            p_runCoroutine = null;
        }

        private void f_ifStartResetRun()
        {
            if ((object)p_runCoroutine != null)
            {
                StopCoroutine(p_runCoroutine);
                p_runCoroutine = StartCoroutine(f_CoroutineRun(this.timeSpanEvent, realTime));
            }
        }

        private void OnDestroy()
        {
            f_stopRun();
        }

        private void Reset()
        {
            f_stopRun();
            actionEvent.RemoveAllListeners();
            //timer.Restart();
            timeSpanEvent = 2f;
            realTime = true;
            invokeAction = true;
        }

        private void OnEnable()
        {
            f_startRun();
        }

        private void OnDisable()
        {
            f_stopRun();
        }

        #endregion

    }

}
