using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using UnityEngine;

using Cheng.ButtonTemplates;
using Cheng.ButtonTemplates.Joysticks.Unitys;
using Cheng.ButtonTemplates.Joysticks;
using Cheng.ButtonTemplates.UnityButtons;

namespace Cheng.Controllers.Unitys
{

    /// <summary>
    /// Unity手柄参数——XBox手柄
    /// </summary>
    [Serializable]
    public class UnityXBoxGameController : GameController
    {

        #region 构造

        /// <summary>
        /// 实例化一个XBox手柄
        /// </summary>
        public UnityXBoxGameController()
        {
            p_mainJoystick = new UnityAxis();
            p_deputyJoystick = new UnityAxis();
            p_crossJoystick = new UnityAxis();
            p_button_LZ = new UnityAxisButton();
            p_button_RZ = new UnityAxisButton();
            f_init();
        }

        /// <summary>
        /// 实例化一个XBox手柄
        /// </summary>
        /// <param name="mainAxis">主摇杆</param>
        /// <param name="deputyAxis">副摇杆</param>
        /// <param name="crossAxis">十字键摇杆</param>
        /// <param name="buttonLZ">左按压式肩键</param>
        /// <param name="buttonRZ">右按压式肩键</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public UnityXBoxGameController(UnityAxis mainAxis, UnityAxis deputyAxis, UnityAxis crossAxis, UnityAxisButton buttonLZ, UnityAxisButton buttonRZ)
        {
            if(mainAxis is null || deputyAxis is null || crossAxis is null || buttonLZ is null || buttonRZ is null)
            {
                throw new ArgumentNullException();
            }

            p_mainJoystick = mainAxis;
            p_deputyJoystick = deputyAxis;
            p_crossJoystick = crossAxis;
            p_button_LZ = buttonLZ;
            p_button_RZ = buttonRZ;

            f_init();
        }

        private void f_init()
        {
            p_buttonA = new KeyCodeButton(KeyCode.JoystickButton0);
            p_buttonB = new KeyCodeButton(KeyCode.JoystickButton1);
            p_buttonX = new KeyCodeButton(KeyCode.JoystickButton2);
            p_buttonY = new KeyCodeButton(KeyCode.JoystickButton3);

            p_button_L = new KeyCodeButton(KeyCode.JoystickButton4);
            p_button_R = new KeyCodeButton(KeyCode.JoystickButton5);

            p_button_mainAxisDown = new KeyCodeButton(KeyCode.JoystickButton8);
            p_button_quAxisDown = new KeyCodeButton(KeyCode.JoystickButton9);

            p_button_rightMenu = new KeyCodeButton(KeyCode.JoystickButton7);
            p_button_leftMenu = new KeyCodeButton(KeyCode.JoystickButton6);
        }

        #endregion

        #region 参数

        /// <summary>
        /// 主摇杆
        /// </summary>
        [SerializeField] private UnityAxis p_mainJoystick;

        /// <summary>
        /// 副摇杆
        /// </summary>
        [SerializeField] private UnityAxis p_deputyJoystick;
        
        /// <summary>
        /// 十字按键摇杆
        /// </summary>
        [SerializeField] private UnityAxis p_crossJoystick;

        /// <summary>
        /// 左侧按压肩键
        /// </summary>
        [SerializeField] private UnityAxisButton p_button_LZ;

        /// <summary>
        /// 右侧按压肩键
        /// </summary>
        [SerializeField] private UnityAxisButton p_button_RZ;

        #region Keys

        [SerializeField] private KeyCodeButton p_buttonA;

        [SerializeField] private KeyCodeButton p_buttonB;

        [SerializeField] private KeyCodeButton p_buttonX;

        [SerializeField] private KeyCodeButton p_buttonY;

        /// <summary>
        /// 左侧按钮肩键
        /// </summary>
        [SerializeField] private KeyCodeButton p_button_L;

        /// <summary>
        /// 右侧按钮肩键
        /// </summary>
        [SerializeField] private KeyCodeButton p_button_R;

        /// <summary>
        /// 主摇杆下压按键
        /// </summary>
        [SerializeField] private KeyCodeButton p_button_mainAxisDown;

        /// <summary>
        /// 副摇杆下压按键
        /// </summary>
        [SerializeField] private KeyCodeButton p_button_quAxisDown;

        /// <summary>
        /// 右侧菜单键
        /// </summary>
        [SerializeField] private KeyCodeButton p_button_rightMenu;

        /// <summary>
        /// 左侧菜单键
        /// </summary>
        [SerializeField] private KeyCodeButton p_button_leftMenu;

        #endregion

        #region Editor
#if UNITY_EDITOR

        /// <summary>
        /// 主要摇杆变量名称
        /// </summary>
        public const string cp_mainJoystick_Name = nameof(p_mainJoystick);

        /// <summary>
        /// 右下摇杆变量名称
        /// </summary>
        public const string cp_deputyJoystick_Name = nameof(p_deputyJoystick);
        
        /// <summary>
        /// 十字摇杆变量名称
        /// </summary>
        public const string cp_crossJoystick_Name = nameof(p_crossJoystick);

        public const string cp_B_A_Name = nameof(p_buttonA);

        public const string cp_B_B_Name = nameof(p_buttonB);

        public const string cp_B_X_Name = nameof(p_buttonX);

        public const string cp_B_Y_Name = nameof(p_buttonY);

        public const string cp_B_L_Name = nameof(p_button_L);

        public const string cp_B_LZ_Name = nameof(p_button_LZ);

        public const string cp_B_R_Name = nameof(p_button_R);

        public const string cp_B_RZ_Name = nameof(p_button_RZ);

        public const string cp_B_LeftMenu_Name = nameof(p_button_leftMenu);

        public const string cp_B_RightMenu_Name = nameof(p_button_rightMenu);

        /// <summary>
        /// 主摇杆按压按钮变量名称
        /// </summary>
        public const string cp_B_mainAxisDown_Name = nameof(p_button_mainAxisDown);

        /// <summary>
        /// 副摇杆按压按钮变量名称
        /// </summary>
        public const string cp_B_qeAxisDown_Name = nameof(p_button_quAxisDown);

#endif
        #endregion

        #endregion

        #region 功能

        #region 参数访问

        #region 摇杆

        /// <summary>
        /// XBox手柄摇杆——左侧主摇杆
        /// </summary>
        public UnityAxis MainJoystick
        {
            get => p_mainJoystick; set => p_mainJoystick = value ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// XBox手柄摇杆——右下副摇杆
        /// </summary>
        public UnityAxis DeputyJoystick
        {
            get => p_deputyJoystick; set => p_deputyJoystick = value ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// XBox手柄摇杆——十字键摇杆
        /// </summary>
        public UnityAxis CrossJoystick
        {
            get => p_crossJoystick; set => p_crossJoystick = value ?? throw new ArgumentNullException();
        }

        #endregion

        #region 按钮

        /// <summary>
        /// XBox手柄按键——A
        /// </summary>
        /// <exception cref="ArgumentNullException">按钮设为null</exception>
        public KeyCodeButton A
        {
            get => p_buttonA;
            set => p_buttonA = value ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// XBox手柄按键——B
        /// </summary>
        /// <exception cref="ArgumentNullException">按钮设为null</exception>
        public KeyCodeButton B
        {
            get => p_buttonB;
            set => p_buttonB = value ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// XBox手柄按键——X
        /// </summary>
        /// <exception cref="ArgumentNullException">按钮设为null</exception>
        public KeyCodeButton X
        {
            get => p_buttonX;
            set => p_buttonX = value ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// XBox手柄按键——Y
        /// </summary>
        /// <exception cref="ArgumentNullException">按钮设为null</exception>
        public KeyCodeButton Y
        {
            get => p_buttonY; 
            set => p_buttonY = value ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// XBox手柄按键——L
        /// </summary>
        /// <exception cref="ArgumentNullException">按钮设为null</exception>
        public KeyCodeButton L
        {
            get => p_button_L;
            set => p_button_L = value ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// XBox手柄按键——R
        /// </summary>
        /// <exception cref="ArgumentNullException">按钮设为null</exception>
        public KeyCodeButton R
        {
            get => p_button_R;
            set => p_button_R = value ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// XBox手柄按键——LZ
        /// </summary>
        /// <exception cref="ArgumentNullException">按钮设为null</exception>
        public UnityAxisButton LZ
        {
            get => p_button_LZ;
            set => p_button_LZ = value ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// XBox手柄按键——RZ
        /// </summary>
        /// <exception cref="ArgumentNullException">按钮设为null</exception>
        public UnityAxisButton RZ
        {
            get => p_button_RZ; 
            set => p_button_RZ = value ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// XBox手柄按键——左侧菜单键
        /// </summary>
        /// <exception cref="ArgumentNullException">按钮设为null</exception>
        public KeyCodeButton LeftMenu
        {
            get => p_button_leftMenu; 
            set => p_button_leftMenu = value ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// XBox手柄按键——右侧菜单键
        /// </summary>
        /// <exception cref="ArgumentNullException">按钮设为null</exception>
        public KeyCodeButton RightMenu
        {
            get => p_button_rightMenu; 
            set => p_button_rightMenu = value ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// XBox手柄按键——主摇杆下压键
        /// </summary>
        /// <exception cref="ArgumentNullException">按钮设为null</exception>
        public KeyCodeButton MainJoystickDown
        {
            get => p_button_mainAxisDown;
            set => p_button_mainAxisDown = value ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// XBox手柄按键——副摇杆下压键
        /// </summary>
        /// <exception cref="ArgumentNullException">按钮设为null</exception>
        public KeyCodeButton DeputyJoystickDown
        {
            get => p_button_quAxisDown; 
            set => p_button_quAxisDown = value ?? throw new ArgumentNullException();
        }

        #endregion

        #endregion

        #region 派生

        public override HavingJoystick HavingJoysticks
        {
            get
            {
                return HavingJoystick.Joystick1 | HavingJoystick.Joystick2 | HavingJoystick.Joystick3;
            }
        }

        public override HavingButton HavingButtons
        {
            get
            {
                return HavingButton.Button1 | HavingButton.Button2 | HavingButton.Button3 | HavingButton.Button4 | HavingButton.Button5 | HavingButton.Button6 | HavingButton.Button7 | HavingButton.Button8 | HavingButton.Button9 | HavingButton.Button10 | HavingButton.Button11 | HavingButton.Button12;
            }
        }

        public override int JoystickCount => 3;

        public override int ButtonCount => 12;

        /// <summary>
        /// XBox手柄摇杆——左侧主摇杆
        /// </summary>
        public override BaseJoystick Joystick1 => p_mainJoystick;

        /// <summary>
        /// XBox手柄摇杆——右下副摇杆
        /// </summary>
        public override BaseJoystick Joystick2 => p_deputyJoystick;

        /// <summary>
        /// XBox手柄摇杆——十字键摇杆
        /// </summary>
        public override BaseJoystick Joystick3 => p_crossJoystick;

        /// <summary>
        /// XBox手柄按键——A
        /// </summary>
        public override BaseButton Button1 => p_buttonA;

        /// <summary>
        /// XBox手柄按键——B
        /// </summary>
        public override BaseButton Button2 => p_buttonB;

        /// <summary>
        /// XBox手柄按键——X
        /// </summary>
        public override BaseButton Button3 => p_buttonX;

        /// <summary>
        /// XBox手柄按键——Y
        /// </summary>
        public override BaseButton Button4 => p_buttonY;

        /// <summary>
        /// XBox手柄按键——L
        /// </summary>
        public override BaseButton Button5 => p_button_L;

        /// <summary>
        /// XBox手柄按键——LZ
        /// </summary>
        public override BaseButton Button6 => p_button_LZ;

        /// <summary>
        /// XBox手柄按键——R
        /// </summary>
        public override BaseButton Button7 => p_button_R;

        /// <summary>
        /// XBox手柄按键——RZ
        /// </summary>
        public override BaseButton Button8 => p_button_RZ;

        /// <summary>
        /// XBox手柄按键——右侧菜单键
        /// </summary>
        public override BaseButton Button9 => p_button_rightMenu;

        /// <summary>
        /// XBox手柄按键——左侧菜单键
        /// </summary>
        public override BaseButton Button10 => p_button_leftMenu;

        /// <summary>
        /// XBox手柄按键——主摇杆按压按键
        /// </summary>
        public override BaseButton Button11 => p_button_mainAxisDown;

        /// <summary>
        /// XBox手柄按键——副摇杆按压按键
        /// </summary>
        public override BaseButton Button12 => p_button_quAxisDown;


        public override bool HavingJoystickByIndex(int index)
        {
            return index >= 0 && index < 3;
        }

        public override bool HavingButtonByIndex(int index)
        {
            return index >= 0 && index < 12;
        }


        public override BaseJoystick GetJoystick(int index)
        {
            switch (index)
            {
                case 0:
                    return p_mainJoystick;
                case 1:
                    return p_deputyJoystick;
                case 2:
                    return p_crossJoystick;
                default:
                    throw new NotSupportedException();
            }
        }

        public override BaseButton GetButton(int index)
        {
            switch (index)
            {
                case 0:
                    return Button1;
                case 1:
                    return Button2;
                case 2:
                    return Button3;
                case 3:
                    return Button4;
                case 4:
                    return Button5;
                case 5:
                    return Button6;
                case 6:
                    return Button7;
                case 7:
                    return Button8;
                case 8:
                    return Button9;
                case 9:
                    return Button10;
                case 10:
                    return Button11;
                case 11:
                    return Button12;
                default:
                    throw new NotSupportedException();
            }
        }

        #endregion

        #endregion

    }

}
