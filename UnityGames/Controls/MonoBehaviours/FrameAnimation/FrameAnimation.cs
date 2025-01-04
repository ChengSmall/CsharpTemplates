using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using Cheng.Timers.Unitys;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
using Cheng.Unitys.Editors;
#endif

namespace Cheng.Unitys.Animators.FrameAnimations
{

    /// <summary>
    /// Unity2D帧动画播放器
    /// </summary>
    /// <remarks>
    /// 控制<see cref="UnityEngine.SpriteRenderer"/>渲染器显示图像的帧动画控制器
    /// </remarks>
    [DisallowMultipleComponent]
    public sealed class FrameAnimation : MonoBehaviour
    {

        #region 结构
        /// <summary>
        /// Unity2D帧动画器事件委托
        /// </summary>
        /// <param name="animation">触发事件的动画器</param>
        /// <param name="frame">触发事件所在的动画帧数</param>
        public delegate void FrameAnimationEvent(FrameAnimation animation, int frame);
        #endregion

        #region init

        public FrameAnimation()
        {
            frameTime = new FrameAnimationParser(0.2f, true);
            sequenceFrame = new List<Sprite>();
            p_timer = new UnityRealTimer();
            p_nowFrame = 0;
            p_isPlay = false;
        }

        #endregion

        #region 参数

        #region 外部参数

#if UNITY_EDITOR
        [Tooltip("要附加到的渲染器")]
#endif
        [SerializeField] private SpriteRenderer spriteRenderer;

#if UNITY_EDITOR
        [Tooltip("每帧图片的滞留时间，单位秒")]
#endif
        [SerializeField] private FrameAnimationParser frameTime;

#if UNITY_EDITOR
        [Tooltip("动画序列帧图像集合")]
#endif
        [SerializeField]
        private List<Sprite> sequenceFrame;      

        #endregion

        #region 内部参数
        /// <summary>
        /// 内部计时器
        /// </summary>
        private UnityRealTimer p_timer;
        /// <summary>
        /// 帧动画事件
        /// </summary>
        private FrameAnimationEvent p_frameRenderEvent;
        /// <summary>
        /// 完播事件
        /// </summary>
        private FrameAnimationEvent p_aLoopEndEvent;

        private Sprite p_nowPlaySprite;

        /// <summary>
        /// 当前播放帧
        /// </summary>
        private int p_nowFrame;
        /// <summary>
        /// 是否正在播放
        /// </summary>
        private bool p_isPlay;
        #endregion

        #endregion

        #region 功能

        #region 参数访问
        /// <summary>
        /// 要附加到的渲染器
        /// </summary>
        public SpriteRenderer SpriteRenderer
        {
            get => spriteRenderer;
            set
            {
                spriteRenderer = value;
            }
        }

        /// <summary>
        /// 每帧图片的滞留时间，单位秒
        /// </summary>
        /// <value>若设置为0表示游戏内下一帧的间隔</value>
        /// <exception cref="ArgumentOutOfRangeException">设置的参数小于0</exception>
        public float FrameTime
        {
            get => frameTime.FrameTime;
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException();
                frameTime = new FrameAnimationParser(value, frameTime.LoopPlayback);
            }
        }
        /// <summary>
        /// 是否允许循环播放
        /// </summary>
        public bool LoopPlayback
        {
            get => frameTime.LoopPlayback;
            set
            {
                frameTime = new FrameAnimationParser(frameTime.FrameTime, value);
            }
        }

        /// <summary>
        /// 动画序列帧图像集合
        /// </summary>
        public List<Sprite> SequenceFrame
        {
            get => sequenceFrame;
        }

        /// <summary>
        /// 获取当前动画的总帧数（序列帧图像个数）
        /// </summary>
        public int Count
        {
            get => sequenceFrame.Count;
        }

        /// <summary>
        /// 动画当前正在播放的帧数，若没有播放则返回-1
        /// </summary>
        public int NowFrame
        {
            get => p_isPlay ? p_nowFrame : -1;
        }

        /// <summary>
        /// 当前动画是否正处于播放
        /// </summary>
        public bool IsPlay
        {
            get => p_isPlay;
        }

        /// <summary>
        /// 访问或设置帧动画器参数
        /// </summary>
        /// <exception cref="ArgumentException">设置的参数每帧间隔小于0</exception>
        public FrameAnimationParser Parameter
        {
            get => frameTime;
            set
            {
                if (value.FrameTime < 0) throw new ArgumentOutOfRangeException();

                frameTime = value;
            }
        }

        #endregion

        #region 动画播放
        /// <summary>
        /// 开始或继续播放动画
        /// </summary>
        public void StartAnimation()
        {
            if (!p_isPlay) p_isPlay = true;
            p_timer.Start();
        }

        /// <summary>
        /// 暂停动画播放
        /// </summary>
        public void StopAnimation()
        {
            if (p_isPlay) p_isPlay = false;
            p_timer.Stop();
            p_nowPlaySprite = null;
        }

        /// <summary>
        /// 暂停动画并重置动画播放进度
        /// </summary>
        public void ResetAnimation()
        {
            p_nowFrame = 0;
            p_nowPlaySprite = FirstFrame;
            if (p_isPlay) p_isPlay = false;
            p_timer.Reset();
        }

        /// <summary>
        /// 重新从第一帧开始播放动画
        /// </summary>
        public void RestartAnimation()
        {
            p_nowFrame = 0;
            p_nowPlaySprite = FirstFrame;
            if (!p_isPlay) p_isPlay = true;
            p_timer.Restart();
        }

        /// <summary>
        /// 跳转到指定帧
        /// </summary>
        /// <param name="frame">要跳转到的序列帧，范围在[0,<see cref="Count"/>)</param>
        /// <exception cref="ArgumentException">参数超出序列帧范围</exception>
        public void JumpToFrame(int frame)
        {
            var list = this.sequenceFrame;
            int count = list.Count;

            if (frame < 0 || frame >= count) throw new ArgumentOutOfRangeException();

            p_nowFrame = frame;
            p_nowPlaySprite = this.sequenceFrame[frame];
        }

        /// <summary>
        /// 序列帧第一帧图像，若没有帧序列则为null
        /// </summary>
        public Sprite FirstFrame
        {
            get
            {
                if (sequenceFrame.Count == 0) return null;
                return sequenceFrame[0];
            }
        }

        /// <summary>
        /// 序列帧最后一帧图像，若没有帧序列则为null
        /// </summary>
        public Sprite EndFrame
        {
            get
            {
                if (sequenceFrame.Count == 0) return null;
                return sequenceFrame[sequenceFrame.Count - 1];
            }
        }

        /// <summary>
        /// 当前正在播放的序列帧图像，若没有播放则为null
        /// </summary>
        public Sprite NowSprite
        {
            get => p_nowPlaySprite;
        }

        #endregion

        #region 事件
        /// <summary>
        /// 每次切换一帧后引发的事件
        /// </summary>
        public event FrameAnimationEvent FrameRenderEvent
        {
            add
            {
                if (p_frameRenderEvent is null) p_frameRenderEvent = value;
                else lock (p_frameRenderEvent) p_frameRenderEvent += value;
            }
            remove
            {
                if (p_frameRenderEvent is null) return;
                lock (p_frameRenderEvent) p_frameRenderEvent -= value;
            }
        }

        /// <summary>
        /// 每次动画播放完成后引发的事件
        /// </summary>
        public event FrameAnimationEvent FinishedPlayingEvent
        {
            add
            {
                if (p_aLoopEndEvent is null) p_aLoopEndEvent = value;
                else lock (p_aLoopEndEvent) p_aLoopEndEvent += value;
            }
            remove
            {
                if (p_aLoopEndEvent is null) return;
                lock (p_aLoopEndEvent) p_aLoopEndEvent -= value;
            }
        }

        #endregion

        #region 渲染器获取
        /// <summary>
        /// 从该脚本对象触发，向父类寻找2D渲染器
        /// </summary>
        /// <param name="spriteRenderer">要寻找的渲染器</param>
        /// <returns>是否成功找到，找到返回true，没有找到返回false</returns>
        public bool GetSpriteRenderByParent(out SpriteRenderer spriteRenderer)
        {
            Transform trans = transform;

            while ((object)trans != null)
            {
                if (trans.TryGetComponent<SpriteRenderer>(out spriteRenderer))
                {
                    return true;
                }
                trans = trans.parent;
            }

            spriteRenderer = null;
            return false;
        }

        /// <summary>
        /// 从该脚本对象触发，向父类寻找2D渲染器并赋值到<see cref="SpriteRenderer"/>
        /// </summary>
        /// <returns>是否成功找到，找到返回true，没有找到返回false</returns>
        public bool GetSpriteRender()
        {
            return GetSpriteRenderByParent(out spriteRenderer);
        }
        #endregion

        #endregion

        #region 运行

        private void Awake()
        {            
            p_timer.Start();
        }

        private void Start()
        {
            p_nowPlaySprite = null;
        }

        private void OnDestroy()
        {
            p_timer.Reset();
            p_aLoopEndEvent -= p_aLoopEndEvent;
            p_frameRenderEvent -= p_frameRenderEvent;
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
            ResetAnimation();
        }

        private void Update()
        {

            if (p_isPlay)
            {
                //是否循环
                bool loop;
                //帧间隔
                float frameTime;

                loop = this.frameTime.GetParser(out frameTime);
           
                //时间经过
                double elapsed = p_timer.Elapsed;

                if (frameTime == 0 || elapsed >= frameTime)
                {

                    //附加到的渲染器
                    var spr = this.spriteRenderer;
                    //序列帧
                    var list = this.sequenceFrame;
                    //帧图数量
                    int count = list.Count;

                    //到达停留时间
                    p_timer.Clear();

                    if (count == 0) return;

                    int end = count - 1;

                    //切换
                    p_nowPlaySprite = list[p_nowFrame];
                    spr.sprite = p_nowPlaySprite;
                    p_frameRenderEvent?.Invoke(this, p_nowFrame);

                    if (p_nowFrame < end)
                    {
                        p_nowFrame++;
                    }
                    else
                    {
                        //到达最后一帧
                        p_aLoopEndEvent?.Invoke(this, p_nowFrame);
                        if (loop)
                        {
                            //循环
                            p_nowFrame = 0;
                        }

                    }

                }

            }


        }

        #endregion

    }


}
