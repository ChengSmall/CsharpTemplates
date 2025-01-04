using System;
using UnityEngine;


namespace Cheng.Unitys.Debugs
{

    /// <summary>
    /// 计算帧率脚本
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UnityFrameTime : MonoBehaviour
    {

        #region 参数

        public UnityFrameTime()
        {
            p_dateTime = 0;
        }

        //当前帧间隔
        private float p_dateTime;
      
        #endregion

        #region 运行

        private void Start()
        {
            p_dateTime = Time.unscaledDeltaTime;
        }

        private void Update()
        {
            p_dateTime = Time.unscaledDeltaTime;
        }

        #endregion

        #region 访问

        /// <summary>
        /// 获取当前帧率（每秒帧数）
        /// </summary>
        public int FPS
        {
            get
            {
                return (int)Math.Round(1 / (double)p_dateTime);                
            }
        }

        /// <summary>
        /// 获取当前帧率（每秒帧数）
        /// </summary>
        public float FPSFloat
        {
            get => 1 / p_dateTime;
        }

        #endregion

    }

}
