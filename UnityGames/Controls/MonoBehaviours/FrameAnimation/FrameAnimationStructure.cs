using System;
using UnityEngine;


namespace Cheng.Unitys.Animators.FrameAnimations
{

    /// <summary>
    /// 帧动画参数
    /// </summary>
    [Serializable]
    public struct FrameAnimationParser
    {

        #region 参数和初始化

        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="time">动画每帧间隔时间</param>
        /// <param name="loop">是否循环播放</param>
        public FrameAnimationParser(float time, bool loop)
        {
            frameTime = time;
            this.loop = loop;
        }
        
        [SerializeField] private float frameTime;
        [SerializeField] private bool loop;
        #endregion

        #region 参数获取

        /// <summary>
        /// 动画每帧间隔时间
        /// </summary>
        public float FrameTime => frameTime;

        /// <summary>
        /// 是否允许循环播放
        /// </summary>
        public bool LoopPlayback => loop;

        /// <summary>
        /// 获取动画参数
        /// </summary>
        /// <param name="frameTime">获取动画每帧间隔时间的引用</param>
        /// <returns>是否循环播放，循环播放返回true，否则返回false</returns>
        public bool GetValue(out float frameTime)
        {
            frameTime = this.frameTime;
            return loop;
        }

        #endregion

        #region Editor
#if UNITY_EDITOR
        /// <summary>
        /// 帧间隔字段名
        /// </summary>
        public const string FrameTimeName = nameof(frameTime);
        /// <summary>
        /// 循环布尔字段名
        /// </summary>
        public const string LoopName = nameof(loop);
#endif
        #endregion

    }

}
