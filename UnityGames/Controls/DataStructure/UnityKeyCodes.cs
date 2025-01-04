using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace Cheng.Unitys
{

    /// <summary>
    /// <see cref="KeyCode"/>按键消息提取器
    /// </summary>
    public sealed class UnityKeyCodes
    {

        #region 构造

        /// <summary>
        /// 实例化一个按键消息提取器
        /// </summary>
        public UnityKeyCodes()
        {

            KeyCode[] ks = (KeyCode[])Enum.GetValues(typeof(KeyCode));

            keys = new KeyCode[ks.Length - 1];

            //Array.Copy(ks, 1, p_keys, 0, p_keys.Length);

            for (int i = 0, j = 0; i < ks.Length; i++)
            {
                if (ks[i] == KeyCode.None) continue;

                keys[j] = ks[i];
                j++;
            }

        }

        #endregion

        #region 参数

        /// <summary>
        /// 所有<see cref="KeyCode"/>枚举的值
        /// </summary>
        public readonly KeyCode[] keys;

        #endregion

        #region 功能

        #region 参数

        #endregion

        #region AllKeys

        /// <summary>
        /// 获取当前帧按下的键
        /// </summary>
        /// <returns>当前帧按下的按键，若没有按下任何按键返回<see cref="KeyCode.None"/></returns>
        public KeyCode KeyDown()
        {
            if (Input.anyKeyDown)
            {
                int length = keys.Length;
                for (int i = 0; i < length; i++)
                {
                    if (Input.GetKeyDown(keys[i])) return keys[i];
                }
            }

            return KeyCode.None;
        }

        /// <summary>
        /// 获取当前帧按下的键
        /// </summary>
        /// <returns>反向遍历当前帧按下的按键，若没有按下任何按键返回<see cref="KeyCode.None"/></returns>
        public KeyCode KeyDownLast()
        {
            if (Input.anyKeyDown)
            {
                KeyCode k;
                for (int i = keys.Length - 1; i >= 0; i--)
                {
                    k = keys[i];
                    if (Input.GetKeyDown(k)) return k;
                }
            }

            return KeyCode.None;
        }

        /// <summary>
        /// 返回当前帧按下的所有按键
        /// </summary>
        /// <param name="list">要将按键添加到的集合</param>
        /// <exception cref="ArgumentNullException">集合为null</exception>
        public void GetAllKeyDown(IList<KeyCode> list)
        {
            if (list is null) throw new ArgumentNullException();

            if (Input.anyKeyDown)
            {
                int length = keys.Length;
                for (int i = 0; i < length; i++)
                {
                    if (Input.GetKeyDown(keys[i])) list.Add(keys[i]);
                }
            }
        }

        /// <summary>
        /// 返回当前帧按下的所有按键
        /// </summary>
        /// <returns>当前帧按下的所有按键；若没有则为一个空数组</returns>
        public KeyCode[] GetAllKeyDown()
        {
            List<KeyCode> list = new List<KeyCode>();
            if (Input.anyKeyDown)
            {
                int length = keys.Length;
                for (int i = 0; i < length; i++)
                {
                    if (Input.GetKeyDown(keys[i])) list.Add(keys[i]);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 返回当前帧松开的按键
        /// </summary>
        /// <returns>当前帧松开的按键，若没有松开任何按键返回<see cref="KeyCode.None"/></returns>
        public KeyCode KeyUp()
        {
            int length = keys.Length;
            for (int i = 0; i < length; i++)
            {
                if (Input.GetKeyUp(keys[i])) return keys[i];
            }

            return KeyCode.None;
        }

        /// <summary>
        /// 返回当前帧松开的所有按键
        /// </summary>
        /// <param name="list">要将按键添加到的集合</param>
        /// <exception cref="ArgumentNullException">集合为null</exception>
        public void KeyAllUp(IList<KeyCode> list)
        {
            if (list is null) throw new ArgumentNullException();
            int length = keys.Length;
            for (int i = 0; i < length; i++)
            {
                if (Input.GetKeyUp(keys[i])) list.Add(keys[i]);
            }
        }

        /// <summary>
        /// 返回当前帧松开的所有按键
        /// </summary>
        /// <returns>当前帧松开的所有按键；若没有则为一个空数组</returns>
        public KeyCode[] KeyAllUp()
        {
            List<KeyCode> list = new List<KeyCode>();
            int length = keys.Length;
            for (int i = 0; i < length; i++)
            {
                if (Input.GetKeyUp(keys[i])) list.Add(keys[i]);
            }
            return list.ToArray();
        }

        #endregion

        #region joysticks

        /// <summary>
        /// 访问任意摇杆键码按下的按键
        /// </summary>
        /// <returns>当前帧按下的摇杆按键，没有则返回<see cref="KeyCode.None"/></returns>
        public KeyCode JoystickButtonDown()
        {
            if (Input.anyKeyDown)
            {

                int i = (int)KeyCode.JoystickButton0;
                int length = (int)KeyCode.JoystickButton19;

                for ( ; i <= length; i++)
                {
                    if (Input.GetKeyDown((KeyCode)i)) return (KeyCode)i;
                }

            }

            return KeyCode.None;

        }

        /// <summary>
        /// 访问1号摇杆键码按下的按键
        /// </summary>
        /// <returns>当前帧按下的按键，没有则返回<see cref="KeyCode.None"/></returns>
        public KeyCode Joystick1ButtonDown()
        {
            if (Input.anyKeyDown)
            {

                int i = (int)KeyCode.Joystick1Button0;
                int length = (int)KeyCode.Joystick1Button19;

                for (; i <= length; i++)
                {
                    if (Input.GetKeyDown((KeyCode)i)) return (KeyCode)i;
                }

            }

            return KeyCode.None;

        }

        /// <summary>
        /// 访问2号摇杆键码按下的按键
        /// </summary>
        /// <returns>当前帧按下的按键，没有则返回<see cref="KeyCode.None"/></returns>
        public KeyCode Joystick2ButtonDown()
        {
            if (Input.anyKeyDown)
            {

                int i = (int)KeyCode.Joystick2Button0;
                int length = (int)KeyCode.Joystick2Button19;

                for (; i <= length; i++)
                {
                    if (Input.GetKeyDown((KeyCode)i)) return (KeyCode)i;
                }

            }

            return KeyCode.None;

        }

        /// <summary>
        /// 访问3号摇杆键码按下的按键
        /// </summary>
        /// <returns>当前帧按下的按键，没有则返回<see cref="KeyCode.None"/></returns>
        public KeyCode Joystick3ButtonDown()
        {
            if (Input.anyKeyDown)
            {

                int i = (int)KeyCode.Joystick3Button0;
                int length = (int)KeyCode.Joystick3Button19;

                for (; i <= length; i++)
                {
                    if (Input.GetKeyDown((KeyCode)i)) return (KeyCode)i;
                }

            }

            return KeyCode.None;

        }

        /// <summary>
        /// 访问4号摇杆键码按下的按键
        /// </summary>
        /// <returns>当前帧按下的按键，没有则返回<see cref="KeyCode.None"/></returns>
        public KeyCode Joystick4ButtonDown()
        {
            if (Input.anyKeyDown)
            {

                int i = (int)KeyCode.Joystick4Button0;
                int length = (int)KeyCode.Joystick4Button19;

                for (; i <= length; i++)
                {
                    if (Input.GetKeyDown((KeyCode)i)) return (KeyCode)i;
                }

            }

            return KeyCode.None;

        }

        /// <summary>
        /// 访问5号摇杆键码按下的按键
        /// </summary>
        /// <returns>当前帧按下的按键，没有则返回<see cref="KeyCode.None"/></returns>
        public KeyCode Joystick5ButtonDown()
        {
            if (Input.anyKeyDown)
            {

                int i = (int)KeyCode.Joystick5Button0;
                int length = (int)KeyCode.Joystick5Button19;

                for (; i <= length; i++)
                {
                    if (Input.GetKeyDown((KeyCode)i)) return (KeyCode)i;
                }

            }

            return KeyCode.None;

        }

        /// <summary>
        /// 访问6号摇杆键码按下的按键
        /// </summary>
        /// <returns>当前帧按下的按键，没有则返回<see cref="KeyCode.None"/></returns>
        public KeyCode Joystick6ButtonDown()
        {
            if (Input.anyKeyDown)
            {

                int i = (int)KeyCode.Joystick6Button0;
                int length = (int)KeyCode.Joystick6Button19;

                for (; i <= length; i++)
                {
                    if (Input.GetKeyDown((KeyCode)i)) return (KeyCode)i;
                }

            }

            return KeyCode.None;

        }

        /// <summary>
        /// 访问7号摇杆键码按下的按键
        /// </summary>
        /// <returns>当前帧按下的按键，没有则返回<see cref="KeyCode.None"/></returns>
        public KeyCode Joystick7ButtonDown()
        {
            if (Input.anyKeyDown)
            {

                int i = (int)KeyCode.Joystick7Button0;
                int length = (int)KeyCode.Joystick7Button19;

                for (; i <= length; i++)
                {
                    if (Input.GetKeyDown((KeyCode)i)) return (KeyCode)i;
                }

            }

            return KeyCode.None;

        }

        /// <summary>
        /// 访问8号摇杆键码按下的按键
        /// </summary>
        /// <returns>当前帧按下的按键，没有则返回<see cref="KeyCode.None"/></returns>
        public KeyCode Joystick8ButtonDown()
        {
            if (Input.anyKeyDown)
            {

                int i = (int)KeyCode.Joystick8Button0;
                int length = (int)KeyCode.Joystick8Button19;

                for (; i <= length; i++)
                {
                    if (Input.GetKeyDown((KeyCode)i)) return (KeyCode)i;
                }

            }

            return KeyCode.None;
        }

        /// <summary>
        /// 访问指定编号下的摇杆键码按下的按键
        /// </summary>
        /// <param name="num">指定摇杆的编号，范围在[1,8]分别对应1-8编号的摇杆</param>
        /// <returns>当前帧按下的按键，没有则返回<see cref="KeyCode.None"/>；如果编号超出范围则返回<see cref="KeyCode.None"/></returns>
        public KeyCode JoystickButtonDown(int num)
        {

            if (Input.anyKeyDown)
            {
                int i, length;

                i = (int)KeyCode.Joystick1Button0;
                //length = (int)KeyCode.Joystick1Button19;

                //int addnum = ((num - 1) * 20);
                
                i = i + ((num - 1) * 20);

                length = i + 20;

                for (; i < length; i++)
                {
                    if (Input.GetKeyDown((KeyCode)i)) return (KeyCode)i;
                }

            }

            return KeyCode.None;

        }

        #endregion

        #region boards

        /// <summary>
        /// 获取当前帧按下的键，除去摇杆按键
        /// </summary>
        /// <returns>当前帧按下的按键，若没有按下任何按键返回<see cref="KeyCode.None"/></returns>
        public KeyCode KeyDownNotJoystick()
        {
            if (Input.anyKeyDown)
            {
                int length = keys.Length;
                for (int i = 0; i < length; i++)
                {
                    if (keys[i] >= KeyCode.JoystickButton0)
                    {
                        continue;
                    }
                    if (Input.GetKeyDown(keys[i])) return keys[i];
                }
            }

            return KeyCode.None;
        }

        /// <summary>
        /// 获取当前帧按下的键，除去摇杆按键
        /// </summary>
        /// <returns>反向遍历当前帧按下的按键，若没有按下任何按键返回<see cref="KeyCode.None"/></returns>
        public KeyCode KeyDownLastNotJoystick()
        {
            if (Input.anyKeyDown)
            {
                KeyCode k;
                for (int i = keys.Length - 1; i >= 0; i--)
                {
                    if (keys[i] >= KeyCode.JoystickButton0)
                    {
                        continue;
                    }
                    k = keys[i];
                    if (Input.GetKeyDown(k)) return k;
                }
            }

            return KeyCode.None;
        }

        /// <summary>
        /// 返回当前帧按下的所有按键，除去摇杆按键
        /// </summary>
        /// <param name="list">要将按键添加到的集合</param>
        /// <exception cref="ArgumentNullException">集合为null</exception>
        public void GetAllKeyDownNotJoystick(IList<KeyCode> list)
        {
            if (list is null) throw new ArgumentNullException();

            if (Input.anyKeyDown)
            {
                int length = keys.Length;
                for (int i = 0; i < length; i++)
                {
                    if (keys[i] >= KeyCode.JoystickButton0)
                    {
                        continue;
                    }
                    if (Input.GetKeyDown(keys[i])) list.Add(keys[i]);
                }
            }
        }

        /// <summary>
        /// 返回当前帧按下的所有按键，除去摇杆按键
        /// </summary>
        /// <returns>当前帧按下的所有按键；若没有则为一个空数组</returns>
        public KeyCode[] GetAllKeyDownNotJoystick()
        {
            List<KeyCode> list = new List<KeyCode>();
            if (Input.anyKeyDown)
            {
                int length = keys.Length;
                for (int i = 0; i < length; i++)
                {
                    if (keys[i] >= KeyCode.JoystickButton0)
                    {
                        continue;
                    }
                    if (Input.GetKeyDown(keys[i])) list.Add(keys[i]);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 返回当前帧松开的按键，除去摇杆按键
        /// </summary>
        /// <returns>当前帧松开的按键，若没有松开任何按键返回<see cref="KeyCode.None"/></returns>
        public KeyCode KeyUpNotJoystick()
        {
            int length = keys.Length;
            for (int i = 0; i < length; i++)
            {
                if (keys[i] >= KeyCode.JoystickButton0)
                {
                    continue;
                }
                if (Input.GetKeyUp(keys[i])) return keys[i];
            }

            return KeyCode.None;
        }

        /// <summary>
        /// 返回当前帧松开的所有按键，除去摇杆按键
        /// </summary>
        /// <param name="list">要将按键添加到的集合</param>
        /// <exception cref="ArgumentNullException">集合为null</exception>
        public void KeyAllUpNotJoystick(IList<KeyCode> list)
        {
            if (list is null) throw new ArgumentNullException();
            int length = keys.Length;
            for (int i = 0; i < length; i++)
            {
                if (keys[i] >= KeyCode.JoystickButton0)
                {
                    continue;
                }
                if (Input.GetKeyUp(keys[i])) list.Add(keys[i]);
            }
        }

        /// <summary>
        /// 返回当前帧松开的所有按键，除去摇杆按键
        /// </summary>
        /// <returns>当前帧松开的所有按键；若没有则为一个空数组</returns>
        public KeyCode[] KeyAllUpNotJoystick()
        {
            List<KeyCode> list = new List<KeyCode>();
            int length = keys.Length;
            for (int i = 0; i < length; i++)
            {
                if (keys[i] >= KeyCode.JoystickButton0)
                {
                    continue;
                }
                if (Input.GetKeyUp(keys[i])) list.Add(keys[i]);
            }
            return list.ToArray();
        }

        #endregion

        #endregion

    }

}
