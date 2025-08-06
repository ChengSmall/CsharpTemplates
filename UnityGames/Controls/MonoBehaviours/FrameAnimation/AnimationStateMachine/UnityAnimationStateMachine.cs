using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Cheng.Timers.Unitys;
using Cheng.DataStructure.FiniteStateMachine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cheng.Unitys.Animators.FrameAnimations.FiniteStateMachine
{

    /// <summary>
    /// 帧动画有限状态机
    /// </summary>
    /// <remarks>
    /// <para>管理<see cref="FrameAnimation"/>动画器的有限状态机</para>
    /// </remarks>
#if UNITY_EDITOR
    [AddComponentMenu("Cheng/2D/动画/帧动画有限状态机")]
#endif
    [DisallowMultipleComponent]
    public sealed class UnityAnimationStateMachine : MonoBehaviour
    {

        #region 结构

        private struct FindAnimation
        {
            public FindAnimation(FrameAnimation ani)
            {
                this.ani = ani;
            }
            public FrameAnimation ani;

            public bool Pre(StateNode<AnimationNodePar> pre)
            {
                return pre.State.Animation == ani;
            }

        }

        private struct FindAnimationName
        {
            public FindAnimationName(string n)
            {
                name = n;
            }
            public string name;

            public bool Pre(StateNode<AnimationNodePar> pre)
            {
                return pre.State.Animation.name == name;
            }
        }


        #endregion

        #region 构造

        public UnityAnimationStateMachine()
        {
            initAnimation = null;
            stateNodes = new List<AnimationStateMachineLink>();
        }

        #endregion

        #region 参数

        #region 外部检查器参数

#if UNITY_EDITOR
        [Tooltip("指定初始动画脚本\n在初始化该脚本后播放的动画，可为空")]
#endif
        [SerializeField] private FrameAnimation initAnimation;

#if UNITY_EDITOR
        [Tooltip("状态机动画节点")]
#endif
        [SerializeField] private List<AnimationStateMachineLink> stateNodes;

        
        #endregion

        #region 内部参数

        private StateMachine<AnimationNodePar> p_aniStateMachine;

        #endregion

        #endregion

        #region 参数访问

        /// <summary>
        /// 运行前参数-状态机节点
        /// </summary>
        public List<AnimationStateMachineLink> StateNodes
        {
            get => stateNodes;
        }

        /// <summary>
        /// 获取动画状态机对象
        /// </summary>
        /// <returns>仅在Unity播放器运行时可获取到</returns>
        public StateMachine<AnimationNodePar> StateMachine
        {
            get => p_aniStateMachine;
        }

        #endregion

        #region 运行

        #region 初始化

        #region 封装事件

        private void fe_anyOverAnim(FrameAnimation ani, int frame)
        {

            if (ani.LoopPlayback) return;

            FindAnimation fn = new FindAnimation(ani);

            var index = p_aniStateMachine.FindIndex(fn.Pre);

            if (index < 0) return;

            //停止播放事件

            p_aniStateMachine.OnlyStopState(index);

        }

        #endregion

        private void Awake()
        {
            p_aniStateMachine = new StateMachine<AnimationNodePar>();
            var sm = p_aniStateMachine;

            sm.RunStateNodeEvent += fe_runAniStateNode;
            sm.StopStateNodeEvent += re_stopAniStateNode;
            f_addNodes(sm);
        }

        private void f_addNodes(StateMachine<AnimationNodePar> sm)
        {
            //状态机实例

            //运行前状态节点集合
            var nodes = stateNodes;

            int i;
            int length;
            int j;
            int count;
            AnimationStateMachineLink tp;
            AnimationNodePar ant;
            
            StateNode<AnimationNodePar> snt;

            #region 添加所有状态机节点
            //添加所有状态机节点
            length = nodes.Count;     
            for (i = 0; i < length; i++)
            {
                //获取运行前状态机节点元素
                tp = nodes[i];

                //获取动画脚本
                ant = new AnimationNodePar(tp.Animation, -1);
                
                //实例化节点
                snt = new StateNode<AnimationNodePar>(ant);

                //ant.FinishedPlayingEvent += fe_overAnim;
                ant.Animation.FinishedPlayingEvent += fe_anyOverAnim;
                //添加
                sm.Add(snt);
            }
            #endregion

            #region 为所有节点添加连接节点

            FindAnimation fn;
            FrameAnimation[] links;
            StateNode<AnimationNodePar> linkNode;
            FrameAnimation next;

            //为所有节点添加连接节点
            for (i = 0; i < length; i++)
            {
                //获取运行前状态机节点元素
                tp = nodes[i];

                //获取添加后的状态机节点
                snt = sm[i];

                //获取运行前节点要连接的节点
                links = tp.Links;

                #region 将 links 数组连接到 snt
                //将links数组连接到snt
                count = links.Length;

                //获取次节点下一个运行
                next = tp.OverNext;

                for (j = 0; j < count; j++)
                {
                    //获取此次要连接的动画脚本
                    fn = new FindAnimation(links[j]);

                    //获取已有的匹配项
                    linkNode = sm.Find(fn.Pre);
                    if (linkNode is null) continue; //没有跳过

                    //连接到i索引
                    snt.Add(linkNode);

                    //判断当前连接是否等于下一个播放
                    if(links[j] == next)
                    {
                        snt.State = new AnimationNodePar(snt.State.Animation, j);
                    }

                }

                #endregion

            }

            #endregion

        }

        /// <summary>
        /// 运行动画
        /// </summary>
        /// <param name="node"></param>
        private void fe_runAniStateNode(StateNode<AnimationNodePar> node)
        {
            var st = node.State;


            var ani = st.Animation;
            ani.StartAnimation();
        }

        /// <summary>
        /// 停止动画
        /// </summary>
        /// <param name="node"></param>
        private void re_stopAniStateNode(StateNode<AnimationNodePar> node)
        {
            var st = node.State;

            var ani = st.Animation;
            ani.ResetAnimation();

            int index = st.NextIndex;
            if (index < 0) return;

            p_aniStateMachine.RunState(index);
        }

        private void Start()
        {
            var initani = initAnimation;
            bool flag = initani != null;
            if (flag)
            {
                FindAnimation fn = new FindAnimation(initani);

                //初始化动画
                p_aniStateMachine.InitState(fn.Pre);
            }

        }

        #endregion

        #region 回收

        private void OnDestroy()
        {
            var sm = p_aniStateMachine;

            int length = sm.Count;
            int i;

            for (i = 0; i < length; i++)
            {
                sm[i]?.Close();
            }

            sm.Close();
            p_aniStateMachine = null;
        }

        #endregion

        #region 功能

        /// <summary>
        /// 切换到指定状态节点的动画
        /// </summary>
        /// <remarks>需要在运行中时切换</remarks>
        /// <param name="frameAnimation">要切换到的动画播放器</param>
        /// <returns>是否成功切换</returns>
        public bool SwitchingAniState(FrameAnimation frameAnimation)
        {
            FindAnimation fn = new FindAnimation(frameAnimation);
            return p_aniStateMachine.SwitchingState(fn.Pre);
        }

        #endregion

        #endregion

    }

}
