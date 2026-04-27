#if UNITY_STANDALONE_WIN

using System;
using UnityEngine;
using UnityEngine.Windows;

#if UNITY_EDITOR
using UnityEditor;
#endif

using Cheng.Unitys.Windows.XInput;
using Cheng.Unitys.Windows.XInput.Win32API;
using Cheng.DataStructure.Cherrsdinates;

using GObj = UnityEngine.GameObject;
using UObj = UnityEngine.Object;
using XIn = Cheng.Unitys.Windows.XInput.Win32API.WindowsXInput;

namespace Cheng.Unitys.Windows.XInput
{

    /// <summary>
    /// XInput控制器参数
    /// </summary>
    [Serializable]
    public struct XInputReaderData
    {

        #region 参数

        internal PointInt2 leftVector;

        internal PointInt2 rightVector;

        internal Vector2 leftVectorF;

        internal Vector2 rightVectorF;

        internal XInputGamePad gamePad;

        internal uint update;

        internal float leftTriggerVectorF;

        internal float rightTriggerVectorF;

        internal GamePadButtons nowFrameDown;

        internal GamePadButtons nowFrameUp;

        internal short leftTriggerVector;

        internal short rightTriggerVector;

        internal int nowLink;

        #endregion

        #region 功能

        /// <summary>
        /// 控制器是否已连接
        /// </summary>
        /// <value>仅当该参数是true时，其它参数才有效</value>
        public bool IsLink
        {
            get => nowLink == 0;
        }

        /// <summary>
        /// 控制器当前状态的返回码，无错误返回0
        /// </summary>
        public int ErrorCode
        {
            get => nowLink;
        }

        /// <summary>
        /// 当前的控制器状态
        /// </summary>
        public XInputGamePad NowGamePad
        {
            get => gamePad;
        }

        /// <summary>
        /// 当前帧按下的按钮
        /// </summary>
        public GamePadButtons KeyDownButtons
        {
            get => nowFrameDown;
        }

        /// <summary>
        /// 当前帧抬起的按钮
        /// </summary>
        public GamePadButtons KeyUpButtons
        {
            get => nowFrameUp;
        }

        /// <summary>
        /// 当前帧左摇杆的位移
        /// </summary>
        /// <value>
        /// <para>表示当前帧左摇杆与上一帧左摇杆的差值，x和y分别代表横轴和纵轴，每一个轴的位移值范围在[-2,2]</para>
        /// </value>
        public Vector2 LeftJoystickVector
        {
            get => leftVectorF;
        }

        /// <summary>
        /// 当前帧右摇杆的位移
        /// </summary>
        /// <value>
        /// <para>表示当前帧右摇杆与上一帧右摇杆的差值，x和y分别代表横轴和纵轴，每一个轴的位移值范围在[-2,2]</para>
        /// </value>
        public Vector2 RightJoystickVector
        {
            get => rightVectorF;
        }

        /// <summary>
        /// 当前帧左摇杆的位移原始数据，每个轴范围在[-65535, 65535]
        /// </summary>
        public PointInt2 LeftJoystickVectorInt
        {
            get => leftVector;
        }

        /// <summary>
        /// 当前帧右摇杆的位移原始数据，每个轴范围在[-65535, 65535]
        /// </summary>
        public PointInt2 RightJoystickVectorInt
        {
            get => rightVector;
        }

        /// <summary>
        /// 当前帧左扳机的力度变化
        /// </summary>
        /// <value>当前帧扳机与上一帧扳机的差值，范围在[-1,1]</value>
        public float LeftTriggerVector
        {
            get => leftTriggerVectorF;
        }

        /// <summary>
        /// 当前帧右扳机的力度变化
        /// </summary>
        /// <value>当前帧扳机与上一帧扳机的差值，范围在[-1,1]</value>
        public float RightTriggerVector
        {
            get => rightTriggerVectorF;
        }

        /// <summary>
        /// 当前帧左扳机的力度变化原始数据，范围在[-255, 255]
        /// </summary>
        public short LeftTriggerVectorInt
        {
            get => leftTriggerVector;
        }

        /// <summary>
        /// 当前帧右扳机的力度变化原始数据，范围在[-255, 255]
        /// </summary>
        public short RightTriggerVectorInt
        {
            get => rightTriggerVector;
        }

        #endregion

    }

    /// <summary>
    /// XInput控制器输入读取脚本
    /// </summary>
    /// <remarks>
    /// <para>启用脚本后，使用<see cref="GetXInputData(int)"/>获取指定索引下的控制器数据</para>
    /// <para>建议在初始化阶段创建脚本并设置为全局模式，即可从<see cref="GlobalXInputReader"/>获取脚本对象，也能够为<see cref="Cheng.ButtonTemplates.UnityButtons.Windows.XInputPadUnityButton"/>对象提供帧变化参数</para>
    /// </remarks>
#if UNITY_EDITOR
    [AddComponentMenu("Cheng/Windows/Input/XInput输入获取")]
#endif
    [DisallowMultipleComponent]
    public sealed class WindowsXInputReader : MonoBehaviour
    {

        #region 构造

        public WindowsXInputReader()
        {
        }

        #endregion

        #region 参数

        private XInputReaderData[] p_states;

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 访问指定索引的控制器参数
        /// </summary>
        /// <param name="index">控制器索引，范围[0,3]</param>
        /// <returns>索引<paramref name="index"/>的控制器参数</returns>
        /// <exception cref="NotImplementedException">未启用脚本</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public ref readonly XInputReaderData GetXInputData(int index)
        {
            if (p_states is null) throw new NotImplementedException("未启动脚本");
            if (index < 0 || index > 3) throw new ArgumentOutOfRangeException(nameof(index));

            return ref p_states[index];
        }

        /// <summary>
        /// 访问指定索引的控制器是否已连接
        /// </summary>
        /// <param name="index">控制器索引，范围[0,3]</param>
        /// <returns>成功连接<paramref name="index"/>索引控制器返回true；没有连接或未初始化脚本返回false</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public bool IsXInputLink(int index)
        {
            if (index < 0 || index > 3) throw new ArgumentOutOfRangeException(nameof(index));
            if (p_states is null) return false;

            return p_states[index].IsLink;
        }

        /// <summary>
        /// 访问指定索引的控制器参数（未严格限制访问时期）
        /// </summary>
        /// <param name="index">控制器索引，范围[0,3]</param>
        /// <returns>索引<paramref name="index"/>的控制器参数</returns>
        public ref readonly XInputReaderData UnsafeGetXInputData(int index)
        {
            return ref p_states[index];
        }


        #endregion

        #region 单例

        private static WindowsXInputReader sp_reader = null;

        /// <summary>
        /// 一个XInput控制器读取的全局脚本
        /// </summary>
        /// <value>该参数在初始化第一个<see cref="WindowsXInputReader"/>脚本时自动创建</value>
        public static WindowsXInputReader GlobalXInputReader
        {
            get => sp_reader;
        }

        #endregion

        #endregion

        #region 运行
#pragma warning disable IDE0051

        private void Awake()
        {
            sp_reader = this;
        }

        private void Start()
        {
            p_states = new XInputReaderData[8];
            for (int i = 0; i < 8; i++)
            {
                p_states[i].update = 0;
                p_states[i].nowLink = -1;
            }
        }

#if DEBUG
        /// <summary>
        /// 更新帧状态
        /// </summary>
        /// <param name="data">待更新数据</param>
        /// <param name="oldPad">上一次的变化帧</param>
#endif
        static void f_updateFrameState(ref XInputReaderData data, XInputGamePad oldPad)
        {
            ushort nowDown = 0, nowUp = 0;
            ushort dnDown = (ushort)data.nowFrameDown, dnUp = (ushort)data.nowFrameUp;
            for (int i = 0; i < 16; i++)
            {
                //按钮是1或0
                var b = ((oldPad.buttons >> i) & 1) == 1;
                if (b)
                {
                    //上一帧是 1
                    //这一帧不是瞬间 1
                    nowDown &= (ushort)(~(1U << i));

                    //判断这一帧是瞬间 0
                    if(((dnUp >> i) & 1) == 1)
                    {
                        nowUp |= ((ushort)(1U << i));
                    }
                }
                else
                {
                    //上一帧是 0
                    //这一帧不是瞬间 0
                    nowUp &= (ushort)(~(1U << i));

                    //判断这一帧是瞬间 1
                    if (((dnDown >> i) & 1) == 1)
                    {
                        nowUp |= ((ushort)(1U << i));
                    }
                }
            }

            data.nowFrameDown = (GamePadButtons)nowDown;
            data.nowFrameUp = (GamePadButtons)nowUp;

            // 摇杆帧向量
            data.leftVector = new PointInt2(
                data.gamePad.thumbLX - oldPad.thumbLX,
                data.gamePad.thumbLY - oldPad.thumbLY
                );

            data.rightVector = new PointInt2(
                data.gamePad.thumbRX - oldPad.thumbRX,
                data.gamePad.thumbRY - oldPad.thumbRY
                );

            const float bth = (65535f / 2f);
            data.leftVectorF = new Vector2(data.leftVector.x / bth, data.leftVector.y / bth);
            data.rightVectorF = new Vector2(data.rightVector.x / bth, data.rightVector.y / bth);

            // 扳机帧向量
            data.leftTriggerVector = (short)((int)data.gamePad.leftTrigger - oldPad.leftTrigger);
            data.leftTriggerVector = (short)((int)data.gamePad.rightTrigger - oldPad.rightTrigger);

            data.leftTriggerVectorF = data.leftTriggerVectorF / 255f;
            data.rightTriggerVectorF = data.rightTriggerVectorF / 255f;
        }

        private void Update()
        {
            // 添加 上一帧保存功能
            XInputState xs;
            for (int i = 0; i < 4; i++)
            {
                ref var state = ref p_states[i];
                var last = state;
                p_states[i + 4] = last;
                state.nowLink = XIn.TryGetState(i, out xs);
                if (state.nowLink == 0)
                {
                    state.nowLink = 0;
                    if (state.update == xs.UpdateNumber)
                    {
                        //没有数据更新，重置帧状态
                        state.nowFrameDown = 0;
                        state.nowFrameUp = 0;
                        state.leftTriggerVector = 0;
                        state.rightTriggerVector = 0;
                        state.leftVector = new PointInt2(0, 0);
                        state.rightVector = new PointInt2(0, 0);
                        continue;
                    }

                    state.update = xs.UpdateNumber;
                    state.gamePad = xs.Gamepad;

                    f_updateFrameState(ref state, last.gamePad);
                }
            }
        }

#if DEBUG
        private void OnDestroy()
        {
            sp_reader = null;
        }
#endif

#pragma warning restore IDE0051
        #endregion

    }

}

#endif