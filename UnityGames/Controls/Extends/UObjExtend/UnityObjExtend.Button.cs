using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine;
using Cheng.DataStructure;
using Cheng.DataStructure.Cherrsdinates;
using Cheng.DataStructure.Collections;
using Cheng.DataStructure.Colors;

using Cheng.ButtonTemplates.Joysticks.Unitys;
using Cheng.ButtonTemplates.Joysticks;
using Cheng.ButtonTemplates;
using Cheng.ButtonTemplates.UnityButtons;

using UObj = UnityEngine.Object;
using GObj = UnityEngine.GameObject;
using Cheng.Algorithm;

namespace Cheng.Unitys
{

    unsafe static partial class UnityObjExtend
    {

        #region 摇杆

        /// <summary>
        /// 获取摇杆的轴数据
        /// </summary>
        /// <param name="joystick">摇杆</param>
        /// <returns>使用<see cref="Vector2"/>获取摇杆的<see cref="BaseJoystick.GetAxis(out float, out float)"/>数据</returns>
        public static Vector2 GetAxis(this BaseJoystick joystick)
        {
            if (joystick is null) throw new ArgumentNullException();
            Vector2 v;
            joystick.GetAxis(out v.x, out v.y);
            return v;
        }

        #endregion

    }

}