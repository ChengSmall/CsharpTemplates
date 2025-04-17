using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cheng.ButtonTemplates.UnityButtons
{

    /// <summary>
    /// 表示一个Unity游戏按钮基类
    /// </summary>
    public abstract class UnityButton : BaseButton
    {

        protected UnityButton()
        {
        }

        #region 参数访问

        /// <summary>
        /// UnityButton默认拥有的权限
        /// </summary>
        public const ButtonAvailablePermissions UnityButtonAvailablePromissions = ButtonAvailablePermissions.CanGetFrameValue;

        public override long NowFrame => Time.frameCount;

        public override ButtonAvailablePermissions AvailablePermissions
        {
            get
            {
                return UnityButtonAvailablePromissions;
            }
        }

        #endregion

    }

}
