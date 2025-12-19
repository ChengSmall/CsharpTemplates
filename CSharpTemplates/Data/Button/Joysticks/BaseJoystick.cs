using Cheng.Algorithm;
using Cheng.DataStructure.Cherrsdinates;
using System;

namespace Cheng.ButtonTemplates.Joysticks
{

    /// <summary>
    /// 表示一个摇杆控制器的基类
    /// </summary>
    /// <remarks>
    /// 一个集合了摇杆所有功能的基类，派生此类实现各种摇杆功能
    /// </remarks>
    public abstract class BaseJoystick
    {

        #region 权限判断

        /// <summary>
        /// 此摇杆是否允许获取摇杆水平数据分量
        /// </summary>
        public virtual bool CanGetHorizontalComponent => false;

        /// <summary>
        /// 此摇杆是否允许获取摇杆垂直数据分量
        /// </summary>
        public virtual bool CanGetVerticalComponent => false;

        /// <summary>
        /// 此摇杆是否允许设置摇杆水平数据分量
        /// </summary>
        public virtual bool CanSetHorizontalComponent => false;

        /// <summary>
        /// 此摇杆是否允许设置摇杆垂直数据分量
        /// </summary>
        public virtual bool CanSetVerticalComponent => false;

        /// <summary>
        /// 此摇杆是否允许获取摇杆偏移轴数据
        /// </summary>
        public virtual bool CanGetVector => false;

        /// <summary>
        /// 此摇杆是否允许设置摇杆偏移轴数据
        /// </summary>
        public virtual bool CanSetVector => false;

        /// <summary>
        /// 此摇杆是否使用摇杆数据改变事件
        /// </summary>
        public virtual bool CanChangeEvent => false;

        /// <summary>
        /// 是否允许将摇杆参数当作四向按钮获取参数
        /// </summary>
        public virtual bool CanGetFourwayButtons => false;

        /// <summary>
        /// 是否支持访问水平摇杆轴反转参数
        /// </summary>
        public virtual bool CanGetHorizontalReverse => false;

        /// <summary>
        /// 是否支持设置水平摇杆轴反转参数
        /// </summary>
        public virtual bool CanSetHorizontalReverse => false;

        /// <summary>
        /// 是否支持访问垂直摇杆轴反转参数
        /// </summary>
        public virtual bool CanGetVerticalReverse => false;

        /// <summary>
        /// 是否支持设置垂直摇杆轴反转参数
        /// </summary>
        public virtual bool CanSetVerticalReverse => false;

        /// <summary>
        /// 允许访问内部封装的摇杆参数
        /// </summary>
        public virtual bool CanGetInternalJoystick => false;

        /// <summary>
        /// 允许设置内部封装的摇杆参数
        /// </summary>
        public virtual bool CanSetInternalJoystick => false;

        /// <summary>
        /// 参数是否有独立的双精度浮点值
        /// </summary>
        /// <returns>true表示双精度版本的参数访问与修改是主要的独立数据，单精度值为次要或由双精度参数转化的兼容参数；false表示参数没有主要的双精度独立参数，主要参数集中在单精度浮点值参数</returns>
        public virtual bool CanDoubleValue => false;

        #endregion

        #region 参数

        /// <summary>
        /// 访问或设置该水平轴是否为反转操作
        /// </summary>
        /// <exception cref="NotSupportedException">没有此权限</exception>
        public virtual bool IsHorizontalReverse
        {
            get => ThrowNotSupportedException<bool>();
            set => ThrowNotSupportedException();
        }

        /// <summary>
        /// 访问或设置该垂直轴是否为反转操作
        /// </summary>
        /// <exception cref="NotSupportedException">没有此权限</exception>
        public virtual bool IsVerticalReverse
        {
            get => ThrowNotSupportedException<bool>();
            set => ThrowNotSupportedException();
        }

        /// <summary>
        /// 访问或设置水平方向的摇杆分量
        /// </summary>
        /// <exception cref="NotSupportedException">没有访问权限</exception>
        public virtual float Horizontal
        {
            get => ThrowNotSupportedException<float>();
            set
            {
                ThrowNotSupportedException();
            }
        }

        /// <summary>
        /// 访问或设置垂直方向的摇杆分量
        /// </summary>
        /// <exception cref="NotSupportedException">没有访问权限</exception>
        public virtual float Vertical
        {
            get => ThrowNotSupportedException<float>();
            set
            {
                ThrowNotSupportedException();
            }
        }

        /// <summary>
        /// 访问或设置水平方向的摇杆分量
        /// </summary>
        /// <exception cref="NotSupportedException">没有访问权限</exception>
        public virtual double HorizontalD
        {
            get => Horizontal;
            set
            {
                Horizontal = (float)value;
            }
        }

        /// <summary>
        /// 访问或设置垂直方向的摇杆分量
        /// </summary>
        /// <exception cref="NotSupportedException">没有访问权限</exception>
        public virtual double VerticalD
        {
            get => Vertical;
            set
            {
                Vertical = (float)value;
            }
        }


        /// <summary>
        /// 将摇杆左侧偏移当作按钮访问或设置参数
        /// </summary>
        /// <exception cref="NotSupportedException">没有此功能</exception>
        /// <exception cref="ArgumentNullException">设置的按钮是null</exception>
        public virtual BaseButton LeftButton
        {
            get => ThrowNotSupportedException<BaseButton>();
            set => ThrowNotSupportedException();
        }

        /// <summary>
        /// 将摇杆右侧偏移当作按钮访问或设置参数
        /// </summary>
        /// <exception cref="NotSupportedException">没有此功能</exception>
        /// <exception cref="ArgumentNullException">设置的按钮是null</exception>
        public virtual BaseButton RightButton
        {
            get => ThrowNotSupportedException<BaseButton>();
            set => ThrowNotSupportedException();
        }

        /// <summary>
        /// 将摇杆上方偏移当作按钮访问或设置参数
        /// </summary>
        /// <exception cref="NotSupportedException">没有此功能</exception>
        /// <exception cref="ArgumentNullException">设置的按钮是null</exception>
        public virtual BaseButton UpButton
        {
            get => ThrowNotSupportedException<BaseButton>();
            set => ThrowNotSupportedException();
        }

        /// <summary>
        /// 将摇杆下方偏移当作按钮访问或设置参数
        /// </summary>
        /// <exception cref="NotSupportedException">没有此功能</exception>
        /// <exception cref="ArgumentNullException">设置的按钮是null</exception>
        public virtual BaseButton DownButton
        {
            get => ThrowNotSupportedException<BaseButton>();
            set => ThrowNotSupportedException();
        }

        /// <summary>
        /// 访问或设置内部封装的摇杆对象
        /// </summary>
        /// <exception cref="NotSupportedException">没有此功能</exception>
        /// <exception cref="ArgumentNullException">设置的摇杆是null</exception>
        public virtual BaseJoystick InternalJoystick
        {
            get => ThrowNotSupportedException<BaseJoystick>();
            set => ThrowNotSupportedException();
        }

        #endregion

        #region 功能

        #region f

        /// <summary>
        /// 获取摇杆数据
        /// </summary>
        /// <param name="radian">摇杆偏移的弧度角；使用平面直角坐标系的弧度单位</param>
        /// <param name="length">摇杆位置距离摇杆原点的长度</param>
        /// <exception cref="NotSupportedException">没有指定权限</exception>
        public virtual void GetVector(out float radian, out float length)
        {
            ThrowNotSupportedException();
            throw new NotSupportedException();
        }

        /// <summary>
        /// 设置摇杆数据
        /// </summary>
        /// <param name="radian">摇杆偏移的弧度角；使用平面直角坐标系的弧度单位</param>
        /// <param name="length">摇杆位置距离摇杆原点的长度</param>
        /// <exception cref="NotSupportedException">没有指定权限</exception>
        /// <exception cref="ArgumentOutOfRangeException">超出范围</exception>
        public virtual void SetVector(float radian, float length)
        {
            ThrowNotSupportedException();
        }

        /// <summary>
        /// 获取摇杆数据
        /// </summary>
        /// <param name="angle">摇杆偏移的角度；使用平面直角坐标系的角度单位</param>
        /// <param name="length">摇杆位置距离摇杆原点的长度</param>
        /// <exception cref="NotSupportedException">没有指定权限</exception>
        public virtual void GetVectorAngle(out float angle, out float length)
        {
            GetVector(out angle, out length);
            angle = (float)(angle / (System.Math.PI / 180d));
        }

        /// <summary>
        /// 设置摇杆数据
        /// </summary>
        /// <param name="angle">摇杆偏移的角度；使用平面直角坐标系的角度单位</param>
        /// <param name="length">摇杆位置距离摇杆原点的长度</param>
        /// <exception cref="NotSupportedException">没有指定权限</exception>
        /// <exception cref="ArgumentOutOfRangeException">超出范围</exception>
        public virtual void SetVectorAngle(float angle, float length)
        {
            angle = (float)(angle * (System.Math.PI / 180d));
            SetVector(angle, length);
        }

        /// <summary>
        /// 设置摇杆数据
        /// </summary>
        /// <param name="horizontal">设置水平方向摇杆参数</param>
        /// <param name="vertical">设置垂直方向摇杆参数</param>
        /// <exception cref="ArgumentOutOfRangeException">范围超出</exception>
        /// <exception cref="NotSupportedException">没有权限</exception>
        public virtual void SetAxis(float horizontal, float vertical)
        {
            ThrowNotSupportedException();
        }

        /// <summary>
        /// 获取摇杆数据
        /// </summary>
        /// <param name="horizontal">水平方向摇杆参数</param>
        /// <param name="vertical">垂直方向摇杆参数</param>
        /// <exception cref="NotSupportedException">没有访问权限</exception>
        public virtual void GetAxis(out float horizontal, out float vertical)
        {
            ThrowNotSupportedException();
            throw new NotSupportedException();
        }

        #endregion

        #region d

        /// <summary>
        /// 获取摇杆数据
        /// </summary>
        /// <param name="radian">摇杆偏移的弧度角；使用平面直角坐标系的弧度单位</param>
        /// <param name="length">摇杆位置距离摇杆原点的长度</param>
        /// <exception cref="NotSupportedException">没有指定权限</exception>
        public virtual void GetVectorD(out double radian, out double length)
        {
            float r, l;
            GetVector(out r, out l);
            radian = r;
            length = l;
        }

        /// <summary>
        /// 设置摇杆数据
        /// </summary>
        /// <param name="radian">摇杆偏移的弧度角；使用平面直角坐标系的弧度单位</param>
        /// <param name="length">摇杆位置距离摇杆原点的长度</param>
        /// <exception cref="NotSupportedException">没有指定权限</exception>
        /// <exception cref="ArgumentOutOfRangeException">超出范围</exception>
        public virtual void SetVectorD(double radian, double length)
        {
            SetVector((float)radian, (float)length);
        }

        /// <summary>
        /// 获取摇杆数据
        /// </summary>
        /// <param name="angle">摇杆偏移的角度；使用平面直角坐标系的角度单位</param>
        /// <param name="length">摇杆位置距离摇杆原点的长度</param>
        /// <exception cref="NotSupportedException">没有指定权限</exception>
        public virtual void GetVectorAngleD(out double angle, out double length)
        {
            float a, l;
            GetVector(out a, out l);
            angle = (float)(a / (System.Math.PI / 180d));
            length = l;
        }

        /// <summary>
        /// 设置摇杆数据
        /// </summary>
        /// <param name="angle">摇杆偏移的角度；使用平面直角坐标系的角度单位</param>
        /// <param name="length">摇杆位置距离摇杆原点的长度</param>
        /// <exception cref="NotSupportedException">没有指定权限</exception>
        /// <exception cref="ArgumentOutOfRangeException">超出范围</exception>
        public virtual void SetVectorAngleD(double angle, double length)
        {
            angle = (float)(angle * (System.Math.PI / 180d));
            SetVectorD(angle, length);
        }

        /// <summary>
        /// 设置摇杆数据
        /// </summary>
        /// <param name="horizontal">设置水平方向摇杆参数</param>
        /// <param name="vertical">设置垂直方向摇杆参数</param>
        /// <exception cref="ArgumentOutOfRangeException">范围超出</exception>
        /// <exception cref="NotSupportedException">没有权限</exception>
        public virtual void SetAxisD(double horizontal, double vertical)
        {
            SetAxis((float)horizontal, (float)vertical);
        }

        /// <summary>
        /// 获取摇杆数据
        /// </summary>
        /// <param name="horizontal">水平方向摇杆参数</param>
        /// <param name="vertical">垂直方向摇杆参数</param>
        /// <exception cref="NotSupportedException">没有访问权限</exception>
        public virtual void GetAxisD(out double horizontal, out double vertical)
        {
            float h, v;
            GetAxis(out h, out v);
            horizontal = h;
            vertical = v;
        }

        #endregion

        #endregion

        #region 事件

        /// <summary>
        /// 当摇杆数据变化时引发的事件，参数表示变化前的摇杆，x表示横轴，y表示纵轴
        /// </summary>
        /// <exception cref="NotSupportedException">没有此功能</exception>
        public virtual event JoystickEvent<Point2> JoystickChangeEvent
        {
            add
            {
                ThrowNotSupportedException();
            }
            remove
            {
                ThrowNotSupportedException();
            }
        }

        #endregion

        #region 静态方法

        /// <summary>
        /// 求原点平面向量的长度和所在弧度
        /// </summary>
        /// <param name="x">从原点延伸的x分量</param>
        /// <param name="y">从原点延伸的y分量</param>
        /// <param name="radian">返回平面向量弧度</param>
        /// <param name="length">返回平面向量长度</param>
        public static void GetVectorRadionAndLength(float x, float y, out float radian, out float length)
        {
            // 计算长度
            length = (float)Math.Sqrt(x * x + y * y);

            // 计算平面向量与正x轴的夹角弧度
            radian = (float)Math.Atan2(y, x);

            // 弧度在0到2π范围内
            if (radian < 0) radian += (float)(2 * Math.PI);
        }

        /// <summary>
        /// 求原点平面向量的长度和所在弧度
        /// </summary>
        /// <param name="x">从原点延伸的x分量</param>
        /// <param name="y">从原点延伸的y分量</param>
        /// <param name="radian">返回平面向量弧度</param>
        /// <param name="length">返回平面向量长度</param>
        public static void GetVectorRadionAndLength(double x, double y, out double radian, out double length)
        {
            // 计算长度
            length = Math.Sqrt(x * x + y * y);

            // 计算平面向量与正x轴的夹角弧度
            radian = Math.Atan2(y, x);

            // 弧度在0到2π范围内
            if (radian < 0) radian += (2 * Math.PI);
        }

        /// <summary>
        /// 将向量分量的方向以中心点反转
        /// </summary>
        /// <param name="horizontal">原向量x分量</param>
        /// <param name="vertical">原向量y分量</param>
        /// <param name="revHorizontal">反转后的向量x分量</param>
        /// <param name="reVertical">反转后的向量y分量</param>
        public static void ToReverse(float horizontal, float vertical, out float revHorizontal, out float reVertical)
        {
            revHorizontal = -horizontal;
            reVertical = -vertical;
        }

        /// <summary>
        /// 将向量分量的方向以中心点反转
        /// </summary>
        /// <param name="horizontal">原向量x分量</param>
        /// <param name="vertical">原向量y分量</param>
        /// <param name="revHorizontal">反转后的向量x分量</param>
        /// <param name="reVertical">反转后的向量y分量</param>
        public static void ToReverse(double horizontal, double vertical, out double revHorizontal, out double reVertical)
        {
            revHorizontal = -horizontal;
            reVertical = -vertical;
        }

        /// <summary>
        /// 以圆心为点将指定角绕180度反转
        /// </summary>
        /// <param name="radian">指定弧度角</param>
        /// <returns>反转后的弧度角</returns>
        public static double ToReverseVector(double radian)
        {
            var r = radian - Math.PI;
            if (r < 0) return r + Math.PI * 2;
            return r;
        }

        /// <summary>
        /// 以圆心为点将指定角绕180度反转
        /// </summary>
        /// <param name="radian">指定弧度角</param>
        /// <returns>反转后的弧度角</returns>
        public static float ToReverseVector(float radian)
        {
            var r = radian - Math.PI;
            if (r < 0) return (float)(r + Math.PI * 2);
            return (float)r;
        }

        /// <summary>
        /// 根据向量参数获取向量分量
        /// </summary>
        /// <param name="radian">向量所在弧度</param>
        /// <param name="length">向量从原点向外延伸的长度</param>
        /// <param name="x">获取向量横坐标分量</param>
        /// <param name="y">获取向量纵坐标分量</param>
        public static void GetVectorComponent(float radian, float length, out float x, out float y)
        {
            x = (float)(length * Math.Cos(radian));
            y = (float)(length * Math.Sin(radian));
        }

        /// <summary>
        /// 根据向量参数获取向量分量
        /// </summary>
        /// <param name="radian">向量所在弧度</param>
        /// <param name="length">向量从原点向外延伸的长度</param>
        /// <param name="x">获取向量横坐标分量</param>
        /// <param name="y">获取向量纵坐标分量</param>
        public static void GetVectorComponent(double radian, double length, out double x, out double y)
        {
            x = (length * Math.Cos(radian));
            y = (length * Math.Sin(radian));
        }

        /// <summary>
        /// y轴做中心将给定弧度角对折到另一边
        /// </summary>
        /// <param name="radian">原角弧度</param>
        /// <returns>根据y轴做轴对称运动得到的角弧度</returns>
        public static double ToReverseHorizontal(float radian)
        {
            //水平反转
            //GetVectorComponent(radian, 1, out var x, out var y);
            var x = -(Math.Cos(radian));
            var y = (Math.Sin(radian));
            
            var d = Math.Atan2(y, x);

            // 弧度在0到2π范围内
            if (d < 0) d += (2 * Math.PI);

            return d;

        }

        /// <summary>
        /// x轴做中心将给定弧度角对折到另一边
        /// </summary>
        /// <param name="radian">原角弧度</param>
        /// <returns>根据x轴做轴对称运动得到的角弧度</returns>
        public static double ToReverseVertical(float radian)
        {
            var x = (Math.Cos(radian));
            var y = -(Math.Sin(radian));

            var d = Math.Atan2(y, x);

            // 弧度在0到2π范围内
            if (d < 0) d += (2 * Math.PI);

            return d;
        }

        /// <summary>
        /// y轴做中心将给定弧度角对折到另一边
        /// </summary>
        /// <param name="radian">原角弧度</param>
        /// <returns>根据y轴做轴对称运动得到的角弧度</returns>
        public static double ToReverseHorizontal(double radian)
        {
            //水平反转
            //GetVectorComponent(radian, 1, out var x, out var y);
            var x = -(Math.Cos(radian));
            var y = (Math.Sin(radian));

            var d = Math.Atan2(y, x);

            // 弧度在0到2π范围内
            if (d < 0) d += (2 * Math.PI);

            return d;

        }

        /// <summary>
        /// x轴做中心将给定弧度角对折到另一边
        /// </summary>
        /// <param name="radian">原角弧度</param>
        /// <returns>根据x轴做轴对称运动得到的角弧度</returns>
        public static double ToReverseVertical(double radian)
        {
            var x = (Math.Cos(radian));
            var y = -(Math.Sin(radian));

            var d = Math.Atan2(y, x);

            // 弧度在0到2π范围内
            if (d < 0) d += (2 * Math.PI);

            return d;
        }

        /// <summary>
        /// 调用此方法引发无法拥有某一项功能时引发的异常
        /// </summary>
        protected static void ThrowNotSupportedException()
        {
            throw new NotSupportedException(Cheng.Properties.Resources.Exception_NotSupportedText);
        }

        /// <summary>
        /// 调用此方法引发无法拥有某一项功能时引发的异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>预备返回值；因为会引发异常，所以不会真的返回</returns>
        protected static T ThrowNotSupportedException<T>()
        {
            throw new NotSupportedException(Cheng.Properties.Resources.Exception_NotSupportedText);
        }

        #endregion

    }

}
