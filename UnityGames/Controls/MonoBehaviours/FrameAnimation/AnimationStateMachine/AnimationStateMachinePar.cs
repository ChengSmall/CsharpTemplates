using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Cheng.DataStructure.FiniteStateMachine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cheng.Unitys.Animators.FrameAnimations.FiniteStateMachine
{
    
    /// <summary>
    /// 动画状态节点和其连接的节点外部参数
    /// </summary>
    [Serializable]
    public struct AnimationStateMachineLink
    {

        #region 构造
        /// <summary>
        /// 初始化动画脚本节点参数
        /// </summary>
        /// <param name="animation">此状态节点动画</param>
        /// <param name="links">此动画连接的其它节点</param>
        public AnimationStateMachineLink(FrameAnimation animation, FrameAnimation[] links, FrameAnimation overNext)
        {
            this.animation = animation;
            this.links = links;
            this.overNext = overNext;
        }
        #endregion

        #region 参数

#if UNITY_EDITOR
        [Tooltip("节点动画")]
#endif
        [SerializeField] private FrameAnimation animation;

#if UNITY_EDITOR
        [Tooltip("动画结束后续播的下一个节点")]
#endif
        [SerializeField] private FrameAnimation overNext;

#if UNITY_EDITOR
        [Tooltip("所连接的其它节点")]
#endif
        [SerializeField] private FrameAnimation[] links;


        #endregion

        #region 访问

        /// <summary>
        /// 节点动画
        /// </summary>
        public FrameAnimation Animation
        {
            get => animation;
        }

        /// <summary>
        /// 动画结束后续播的下一个节点
        /// </summary>
        public FrameAnimation OverNext
        {
            get => overNext;
        }

        /// <summary>
        /// 连接的其它动画
        /// </summary>
        public FrameAnimation[] Links
        {
            get => links;
        }

        #endregion

    }

    /// <summary>
    /// 动画节点参数
    /// </summary>
    public struct AnimationNodePar
    {

        #region 构造
        /// <summary>
        /// 初始化一个动画节点参数
        /// </summary>
        /// <param name="animation">动画脚本</param>
        /// <param name="nextIndex">节点参数</param>
        public AnimationNodePar(FrameAnimation animation, int nextIndex)
        {
            this.p_animation = animation;
            this.p_nextIndex = nextIndex;
        }
        #endregion

        #region 参数

        private readonly FrameAnimation p_animation;
        private readonly int p_nextIndex;

        #endregion

        #region 功能

        /// <summary>
        /// 动画脚本
        /// </summary>
        public FrameAnimation Animation => p_animation;
        /// <summary>
        /// 下一个播放的节点连接索引，没有则为-1
        /// </summary>
        public int NextIndex
        {
            get => p_nextIndex;
        }
        /// <summary>
        /// 是否存在下一个播放节点
        /// </summary>
        public bool HaveNext
        {
            get => p_nextIndex >= 0;
        }

        #endregion

    }

}
