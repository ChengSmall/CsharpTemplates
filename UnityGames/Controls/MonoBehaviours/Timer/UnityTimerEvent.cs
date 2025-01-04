using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Cheng.Timers.Unitys
{

    /// <summary>
    /// 计时触发器
    /// </summary>
    /// <remarks>
    /// 使用计时器实现的触发器机制，每次经过特定时间执行一次事件
    /// </remarks>
    public class UnityTimerEvent : MonoBehaviour
    {

        public UnityTimerEvent()
        {
            timer = new UnityTimer(UnityTimerType.time);
            actionEvent = new UnityEvent();
            timeSpanEvent = 2f;
        }

        #region 参数

#if UNITY_EDITOR
        [Tooltip("计时器参数")]
#endif
        [SerializeField] private UnityTimer timer;

        /// <summary>
        /// 事件执行的时间间隔
        /// </summary>
#if UNITY_EDITOR
        [Tooltip("事件执行的时间间隔")]
#endif
        public float timeSpanEvent;

        [SerializeField]
        private UnityEvent actionEvent;

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

        public void Reset()
        {
            actionEvent.RemoveAllListeners();
            timer.Restart();
            timeSpanEvent = 2f;
        }

        /// <summary>
        /// 获取用于计时的Unity计时器
        /// </summary>
        public UnityTimer Timer
        {
            get => timer;
        }

        /// <summary>
        /// 暂停计时器运行
        /// </summary>
        public void StopTime()
        {
            timer.Stop();
        }

        /// <summary>
        /// 开始或继续计时器运行
        /// </summary>
        public void Startime()
        {
            timer.Start();
        }

        private void Start()
        {
            timer.Restart();
        }

        private void Update()
        {

            if (timer.IsRunning)
            {

                if (timer.Elapsed >= timeSpanEvent)
                {
                    timer.Restart();
                    actionEvent.Invoke();
                }

            }

        }

        private void OnDestroy()
        {
            timer.Reset();
        }

        #endregion

    }

}
