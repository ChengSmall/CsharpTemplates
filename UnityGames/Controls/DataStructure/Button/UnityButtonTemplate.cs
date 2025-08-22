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

        public override long NowFrame => Time.frameCount;

        public override bool CanGetFrameValue => true;

        #endregion

    }

}
