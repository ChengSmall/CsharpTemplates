using Cheng.DataStructure.BoundedContainers;
using Cheng.DataStructure;
using Cheng.Memorys;
using Cheng.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cheng.Algorithm;

namespace Cheng.Windows.Controls
{

    /// <summary>
    /// 可切换平滑条件的滑动条
    /// </summary>
    [ToolboxItem(true)]
    public partial class DragSlider : UserControl
    {

        public DragSlider()
        {
            f_lastInit();
            InitializeComponent();
            f_init();
        }

        #region code

        #region 释放

        private void f_disposing(bool disposed)
        {
            if (disposed)
            {
                p_grap?.Dispose();
                //p_pen_bar_origin?.Dispose();
                p_pen_bar_passing?.Dispose();
                p_pen_bar_Unpassing?.Dispose();
                p_pen_notDrawImageDefPen?.Dispose();
                //p_drawOrigin?.Dispose();
            }
            p_grap = null;
            //p_pen_bar_origin = null;
            p_pen_bar_Unpassing = null;
            p_pen_bar_passing = null;
            p_drawOrigin = null;
            p_pen_notDrawImageDefPen = null;
        }

        #endregion

        #region 初始化

        private void f_lastInit()
        {
            p_value = new BoundedContainerDouble(0, 0, 10);

            p_pen_bar_passing = new SolidBrush(Color.FromArgb(68, 168, 225));
            p_pen_bar_Unpassing = new SolidBrush(Color.FromArgb(215, 215, 215));
            p_drawOrigin = null;
            p_pen_notDrawImageDefPen = new SolidBrush(Color.FromArgb(56, 128, 215));
            p_front = DragSliderFront.Right;
            
        }

        private void f_init()
        {
            DoubleBuffered = true;

            BackColor = Color.FromArgb(0, 255, 255, 255);
            Size = new Size(120, 35);

            p_grap = this.CreateGraphics();

            p_defMouseSliderEvent = true;
        }

        #endregion

        #region 参数

        #region 绘制工具

        private Graphics p_grap;

#if DEBUG
        /// <summary>
        /// 绘制笔刷 - 采用图像绘制的滑动条进度点位
        /// </summary>
#endif
        private Image p_drawOrigin;

#if DEBUG
        /// <summary>
        /// 绘制笔刷 - 没有图像绘制的滑动条进度点位对象时，默认使用的进度点笔刷
        /// </summary>
#endif
        private SolidBrush p_pen_notDrawImageDefPen;

        //private Pen p_pen_bar_origin;

#if DEBUG
        /// <summary>
        /// 绘制笔刷 - 滑动条体，已划过的位置区域
        /// </summary>
#endif
        private SolidBrush p_pen_bar_passing;

#if DEBUG
        /// <summary>
        /// 绘制笔刷 - 滑动条体，未划过的位置区域
        /// </summary>
#endif
        private SolidBrush p_pen_bar_Unpassing;

        #endregion

        #region 控件参数

        private BoundedContainerDouble p_value;

        private DragSliderFront p_front;

        private bool p_defMouseSliderEvent;

        #endregion

        #endregion

        #region 控件

        #endregion

        #region 暴露参数

        /// <summary>
        /// 滑动条当前值
        /// </summary>
        [DefaultValue(0)]
        [Category("参数")]
        [Description("滑动条当前值")]
        public double Value
        {
            get => p_value.value;
            set
            {
                if (value == p_value.value) return;
                double oldv = p_value.value;
                p_value = p_value.SetValue(value);

                if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
                {
                    OnValueChangeEvent(new DragSliderValueChangeEventArgs(oldv, value));
                }
            }
        }

        /// <summary>
        /// 滑动条最大值
        /// </summary>
        [DefaultValue(10)]
        [Category("参数")]
        [Description("滑动条最大值")]
        public double MaxValue
        {
            get => p_value.max;
            set
            {
                double min = p_value.min;
                if(min > value)
                {
                    return;
                }

                double va = p_value.value;
                if(va > value)
                {
                    va = value;
                }

                p_value = new BoundedContainerDouble(va, min, value);

            }
        }

        /// <summary>
        /// 滑动条最小值
        /// </summary>
        [DefaultValue(0)]
        [Category("参数")]
        [Description("滑动条最小值")]
        public double MinValue
        {
            get => p_value.min;
            set
            {
                double max = p_value.max;
                if (max < value)
                {
                    //max = value;
                    return;
                }
                double va = p_value.value;
                if (va < value)
                {
                    va = value;
                }

                p_value = new BoundedContainerDouble(va, value, max);
            }
        }

        /// <summary>
        /// 滑动条运动方向
        /// </summary>
        [DefaultValue(DragSliderFront.Right)]
        [Category("参数")]
        [Description("滑动条运动方向")]
        public DragSliderFront SliderFront
        {
            get => p_front;
            set
            {
                p_front = value;
                //p_front = (DragSliderFront)Maths.Clamp((int)value, 0, (int)DragSliderFront.Down);
            }
        }

        /// <summary>
        /// 是否使用默认的滑动条鼠标拖动事件
        /// </summary>
        /// <value>
        /// true表示开启滑动条鼠标拖动事件，false表示不使用默认实现的鼠标拖动事件
        /// </value>
        [DefaultValue(true)]
        [Category("参数")]
        [Description("使用默认的滑动条鼠标拖动事件")]
        public bool SliderMouseDefaultEvent
        {
            get => p_defMouseSliderEvent;
            set
            {
                p_defMouseSliderEvent = value;
            }
        }

        /// <summary>
        /// 设置滑动条最值但不触发事件
        /// </summary>
        /// <param name="min">要设置的最小值</param>
        /// <param name="max">要设置的最大值</param>
        /// <exception cref="ArgumentOutOfRangeException">最小值大于最大值</exception>
        public void OnlySetMinMaxValue(double min, double max)
        {
            if (max < min) throw new ArgumentOutOfRangeException();
            double va = p_value.value.Clamp(min, max);
            p_value = new BoundedContainerDouble(va, min, max);
        }

        /// <summary>
        /// 设置滑动条当前值但不触发事件
        /// </summary>
        /// <param name="value">要设置的值</param>
        public void OnlySetValue(double value)
        {
            p_value = p_value.SetValue(value);
        }

        #endregion

        #region 事件

        /// <summary>
        /// <see cref="Value"/>更改时的事件
        /// </summary>
        public event EventHandler<DragSliderValueChangeEventArgs> ValueChangeEvent;

        /// <summary>
        /// <see cref="Value"/>更改事件的触发器
        /// </summary>
        /// <param name="value">事件参数</param>
        protected virtual void OnValueChangeEvent(DragSliderValueChangeEventArgs value)
        {
            ValueChangeEvent?.Invoke(this, value);
        }

        #endregion

        #region 功能封装

        /// <summary>
        /// 指定可用区相对坐标，返回坐标所在对应的值
        /// </summary>
        /// <param name="pixelPoint">进度条可用区相对坐标</param>
        /// <param name="value">坐标所在表示的值</param>
        /// <returns>坐标是否存在于可用区内</returns>
        public bool GetPixelLocationToValue(Point pixelPoint, out double value)
        {
            value = default;

            //控件本地矩形
            Rectangle crect = new Rectangle(new Point(0, 0), ClientSize);
            //crect = base.ClientRectangle;
            Rectangle orect;

            var fr = p_front;
            double last, next;
            //bool cfb;
            if(fr < DragSliderFront.Up)
            {
                //cfb = fr == DragSliderFront.Right;

                orect = new Rectangle(crect.X + crect.Height, crect.Y, crect.Width - ((crect.Height) * 2), crect.Height);


                if (!orect.Contains(pixelPoint))
                {
                    //不包含
                    return false;
                }
                //左右
                //左右两端长度各缩短一个高

                last = crect.Height;
                next = crect.Width - (crect.Height);
                //实操长度
                var len = (next - last);
                double leftLen = pixelPoint.X - last;

                value = p_value.min + ((p_value.max - p_value.min) * (leftLen / len));
                return true;
            }
            else
            {
                orect = new Rectangle(crect.X, crect.Y + crect.Width, crect.Width, crect.Height - (crect.Width * 2));

                if (!orect.Contains(pixelPoint))
                {
                    //不包含
                    return false;
                }

                //上下
                //上下两端长度各缩短一个长
                last = crect.Width;
                next = crect.Height - (crect.Width);
                //实操长度
                var len = (next - last);

                //上侧长度
                double leftLen = pixelPoint.Y - last;

                if (fr == DragSliderFront.Down)
                {
                    // ↓
                    value = p_value.min + ((p_value.max - p_value.min) * (leftLen / len));
                }
                else
                {
                    // ↑
                    value = p_value.min + ((p_value.max - p_value.min) * ((len - leftLen) / len));
                }
                return true;
            }


            return true;
        }

        #region UI绘制

        private void f_getRects(in RectangleF rect, out Rectangle lastBar, out Rectangle nextBar, out Rectangle nowPoint)
        {
            //前半段值
            var lastBarValue = p_value.value - p_value.min;

            //后半段值
            var nextBarValue = p_value.max - p_value.value;

            //总长度
            var allBarValue = p_value.max - p_value.min;

            if(allBarValue <= 0)
            {
                allBarValue = 1E-7;
            }

            //进度条全身范围
            RectangleF allbody;
            if (p_front == DragSliderFront.Right)
            {

                //上下缩短一半
                allbody = new RectangleF(rect.X + rect.Height, rect.Y + (rect.Height / 4), rect.Width - (rect.Height * 2), rect.Height / 2);

                //前半段区域
                lastBar = new Rectangle((int)allbody.X, (int)allbody.Y, (int)(allbody.Width * (lastBarValue / allBarValue)), (int)allbody.Height);

                //后半
                nextBar = new Rectangle((int)(allbody.X + lastBar.Width), (int)allbody.Y, (int)(allbody.Width * (nextBarValue / allBarValue)), (int)allbody.Height);

                //中间点矩形
                //高度为边长

                nowPoint = new Rectangle((int)(nextBar.X - (rect.Height / 2)), (int)(rect.Y), (int)(rect.Height), (int)(rect.Height));

            }
            else if (p_front == DragSliderFront.Left)
            {
                allbody = new RectangleF(rect.X + rect.Height, rect.Y + (rect.Height / 4), rect.Width - (rect.Height * 2), rect.Height / 2);


                nextBar = new Rectangle((int)allbody.X, (int)allbody.Y, (int)(allbody.Width * (lastBarValue / allBarValue)), (int)allbody.Height);

                lastBar = new Rectangle((int)(allbody.X + nextBar.Width), (int)allbody.Y, (int)(allbody.Width * (nextBarValue / allBarValue)), (int)allbody.Height);

                //中间点矩形
                //高度为边长
                nowPoint = new Rectangle((int)(lastBar.X - (rect.Height / 2)), (int)(rect.Y), (int)(rect.Height), (int)(rect.Height));

            }
            else if (p_front == DragSliderFront.Down)
            {
                //左右缩一半
                allbody = new RectangleF(rect.X + (rect.Width / 4), rect.Y + rect.Width, rect.Width / 2, rect.Height - (rect.Width * 2));

                //从上往下
                //前半
                lastBar = new Rectangle((int)(allbody.X), (int)(allbody.Y), (int)(allbody.Width), (int)(allbody.Height * (lastBarValue / allBarValue)));

                nextBar = new Rectangle((int)(allbody.X), (int)(allbody.Y + lastBar.Height), (int)(allbody.Width), (int)(allbody.Height * (nextBarValue / allBarValue)));

                //中间点矩形
                //长度为边长
                nowPoint = new Rectangle((int)(rect.X), (int)(nextBar.Y - (rect.Width / 2)), (int)(rect.Width), (int)(rect.Width));
            }
            else
            {
                allbody = new RectangleF(rect.X + (rect.Width / 4), rect.Y + rect.Width, rect.Width / 2, rect.Height - (rect.Width * 2));

                nextBar = new Rectangle((int)(allbody.X), (int)(allbody.Y), (int)(allbody.Width), (int)(allbody.Height * (nextBarValue / allBarValue)));

                lastBar = new Rectangle((int)(allbody.X), (int)(allbody.Y + nextBar.Height), (int)(allbody.Width), (int)(allbody.Height * (lastBarValue / allBarValue)));

                nowPoint = new Rectangle((int)(rect.X), (int)(lastBar.Y - (rect.Width / 2)), (int)(rect.Width), (int)(rect.Width));
            }

        }

        private void f_drawControl(Graphics graphics, RectangleF clipRectangle)
        {
            //经过的滑动条区域
            Rectangle lastBar;
            //未经过的滑动条区域
            Rectangle nextBar;

            //当前点位
            Rectangle nowPoint;

            f_getRects(in clipRectangle, out lastBar, out nextBar, out nowPoint);

            //grap.FillRectangle;
            graphics.FillRectangle(p_pen_bar_passing, lastBar);
            graphics.FillRectangle(p_pen_bar_Unpassing, nextBar);
            if (p_drawOrigin == null)
            {
                graphics.FillEllipse(p_pen_notDrawImageDefPen, nowPoint);
            }
            else
            {
                graphics.DrawImage(p_drawOrigin, nowPoint);
            }

        }

        private void f_OnPointLocation(Point location)
        {
            if (!p_defMouseSliderEvent)
            {
                return;
            }

            if (!GetPixelLocationToValue(location, out double value))
            {
                return; //不在其上
            }
            //修改
            Value = value;
            Invalidate(false);
            Update();

            //if (p_grap != null) f_drawControl(p_grap, ClientRectangle);
        }

        #endregion

        #endregion

        #region 派生

        public override void ResetBackColor()
        {
            BackColor = Color.FromArgb(0, 255, 255, 255);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            var but = e.Button;
            switch (but)
            {
                case MouseButtons.Left:
                case MouseButtons.Right:
                case MouseButtons.Middle:
                    f_OnPointLocation(e.Location);
                    break;
                default:
                    break;
            }

            base.OnMouseClick(e);
        }

#if DEBUG
        /// <summary>
        /// 控件首次显示之前
        /// </summary>
        /// <param name="e"></param>
#endif
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            f_drawControl(e.Graphics, e.ClipRectangle);
            base.OnPaint(e);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            //Invalidate(false);
            //Update();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            //Invalidate(false);
            //Update();
        }


        #endregion

        #endregion

    }
}
#if DEBUG

#endif