using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cheng.ButtonTemplates;

namespace Cheng.DEBUG
{

    /// <summary>
    /// 封装按钮进行DEBUG
    /// </summary>
    public sealed class Debutton : BaseButton
    {

        /// <summary>
        /// 实例化调试按钮
        /// </summary>
        /// <param name="button">封装按钮</param>
        /// <param name="print">打印日志的输出流</param>
        public Debutton(BaseButton button, TextWriter print)
        {
            p_but = button ?? throw new ArgumentNullException("button");
            p_print = print;
        }

        private BaseButton p_but;
        private TextWriter p_print;

        /// <summary>
        /// 日志打印流
        /// </summary>
        public TextWriter PrintLog
        {
            get => p_print;
            set
            {
                p_print = value;
            }
        }

        public override event ButtonEvent<BaseButton> ButtonDownEvent
        {
            add
            {
                p_but.ButtonDownEvent += value;
            }
            remove
            {
                p_but.ButtonDownEvent -= value;
            }
        }

        public override event ButtonEvent<BaseButton> ButtonUpEvent
        {
            add => p_but.ButtonUpEvent += value;
            remove => p_but.ButtonUpEvent -= value;
        }

        public override event ButtonEvent<BaseButton> ButtonClickEvent
        {
            add => p_but.ButtonClickEvent += value;
            remove => p_but.ButtonClickEvent -= value;
        }

        public override ButtonAvailablePermissions AvailablePermissions
        {
            get
            {
                var re = ButtonAvailablePermissions.CanSetAndGetInternalButton |
                 p_but.AvailablePermissions;
                p_print?.WriteLine("权限:" + re.ToString());
                return re;
            }
        }

        public override bool ButtonState
        {
            get
            {
                var re = p_but.ButtonState;
                p_print?.WriteLine("获取参数" + nameof(ButtonState) + ":" + re.ToString());
                return re;
            }
            set
            {
                p_print?.WriteLine("设置参数" + nameof(ButtonState) + ":" + value.ToString());
                p_but.ButtonState = value;
            }
        }

        public override float Power
        {
            get
            {
                var re = p_but.Power;
                p_print?.WriteLine("获取参数" + nameof(Power) + ":" + re.ToString());
                return re;
            }
            set
            {
                p_print?.WriteLine("设置参数" + nameof(Power) + ":" + value.ToString());
                p_but.Power = value;
            }
        }

        public override float MaxPower
        {
            get
            {
                var re = p_but.MaxPower;
                p_print?.WriteLine("获取参数" + nameof(MaxPower) + ":" + re.ToString());
                return re;
            }
            set
            {
                p_but.MaxPower = value;
                p_print?.WriteLine("设置参数" + nameof(MaxPower) + ":" + value.ToString());
                
            }
        }

        public override float MinPower
        {
            get
            {
                var re = p_but.MinPower;
                p_print?.WriteLine("获取参数" + nameof(MinPower) + ":" + re.ToString());
                return re;
            }
            set
            {                
                p_but.MaxPower = value;
                p_print?.WriteLine("设置参数" + nameof(MaxPower) + ":" + value.ToString());
            }
        }

        public override bool ButtonDown
        {
            get
            {
                var re = p_but.ButtonDown;
                p_print?.WriteLine("获取参数" + nameof(ButtonDown) + ":" + re.ToString());
                return re;
            }
        }

        public override bool ButtonUp
        {
            get
            {
                var re = p_but.ButtonUp;
                p_print?.WriteLine("获取参数" + nameof(ButtonUp) + ":" + re.ToString());
                return re;
            }
        }

        public override long NowFrame
        {
            get
            {
                var re = p_but.NowFrame;
                p_print?.WriteLine("获取参数" + nameof(NowFrame) + ":" + re.ToString());
                return re;
            }
        }

        public override BaseButton InternalButton
        {
            get => p_but;
            set => p_but = value;
        }

    }

}
