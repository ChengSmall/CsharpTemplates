using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.Windows.Controls
{

    /// <summary>
    /// 滑动条的走向
    /// </summary>
    [ComVisible(true)]
    public enum DragSliderFront
    {
        /// <summary>
        /// 从左往右
        /// </summary>
        Right = 0,

        /// <summary>
        /// 从右往左
        /// </summary>
        Left = 1,

        /// <summary>
        /// 从下往上
        /// </summary>
        Up = 2,

        /// <summary>
        /// 从上往下
        /// </summary>
        Down = 3
    }

    /// <summary>
    /// <see cref="DragSlider.ValueChangeEvent"/> 事件参数
    /// </summary>
    public readonly struct DragSliderValueChangeEventArgs : IEquatable<DragSliderValueChangeEventArgs>
    {

        #region 初始化

        /// <summary>
        /// 初始化时间参数
        /// </summary>
        /// <param name="oldValue">修改前的值</param>
        /// <param name="newValue">修改后的新值</param>
        public DragSliderValueChangeEventArgs(double oldValue, double newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 修改前的值
        /// </summary>
        public readonly double oldValue;

        /// <summary>
        /// 修改后的新值
        /// </summary>
        public readonly double newValue;

        #endregion

        #region 功能

        public override bool Equals(object obj)
        {
            if(obj is DragSliderValueChangeEventArgs ds) return Equals(ds);
            return false;
        }

        public bool Equals(DragSliderValueChangeEventArgs other)
        {
            return this.newValue == other.newValue && this.oldValue == other.oldValue;
        }

        public override int GetHashCode()
        {
            return oldValue.GetHashCode() ^ newValue.GetHashCode();
        }

        public override string ToString()
        {
            return oldValue.ToString() + " => " + newValue.ToString();
        }

        #endregion

    }


}
