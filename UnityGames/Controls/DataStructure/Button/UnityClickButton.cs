using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Cheng.ButtonTemplates.UnityButtons
{

    /// <summary>
    /// 一个Unity可点击按钮封装
    /// </summary>
    /// <remarks>
    /// 封装<see cref="UnityEngine.UIElements.Button"/>按钮
    /// </remarks>
    public sealed class UnityClickButton : UnityButton
    {

        #region 构造
        /// <summary>
        /// 封装一个Unity的可点击按钮
        /// </summary>
        /// <param name="button"></param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public UnityClickButton(Button button)
        {
            p_button = button ?? throw new ArgumentNullException();
            p_eventList = new List<Pars>();
        }
        /// <summary>
        /// 封装一个Unity的可点击按钮
        /// </summary>
        /// <param name="gobj">按钮所在的游戏对象</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public UnityClickButton(UnityEngine.GameObject gobj)
        {
            p_button = gobj?.GetComponent<Button>();
            if(p_button is null) throw new ArgumentNullException();
            p_eventList = new List<Pars>();
        }
        #endregion

        #region 参数

        private List<Pars> p_eventList;

        private Button p_button;

        #endregion

        #region 功能

        #region 权限重写

        public override bool CanButtonClick => true;

        #endregion

        #region 按钮

        private class Pars
        {
            public Pars(BaseButton button, ButtonEvent<BaseButton> action)
            {
                this.button = button;
                this.action = action;
                Invoke = EventFunc;
            }
            private BaseButton button;
            public ButtonEvent<BaseButton> action;

            public Action Invoke;

            private void EventFunc()
            {
                action.Invoke(button);
            }
        }

        /// <summary>
        /// Unity按钮被点击时引发的事件
        /// </summary>
        public sealed override event ButtonEvent<BaseButton> ButtonClickEvent
        {
            add
            {
                Pars p = new Pars(this, value);
                p_eventList.Add(p);

                p_button.clicked += p.Invoke;
            }
            remove
            {                
                var p = p_eventList.Find(Finds);
                if (p is null) return;
                int index = p_eventList.IndexOf(p);

                p_button.clicked -= p.Invoke;
                p_eventList.RemoveAt(index);

                bool Finds(Pars ps)
                {
                    return ps.action == value;
                }
            }
        }

        /// <summary>
        /// 获取封装的按钮
        /// </summary>
        public Button BaseButon => p_button;

        #endregion

        #endregion

    }

}
