using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cheng.ButtonTemplates.Joysticks.Unitys
{

    /// <summary>
    /// unity摇杆类扩展
    /// </summary>
    public static class UnityJoystickExtend
    {

        #region 摇杆数据获取

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
