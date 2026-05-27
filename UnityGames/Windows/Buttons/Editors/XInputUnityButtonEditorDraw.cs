#if UNITY_EDITOR && UNITY_STANDALONE_WIN

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;

using Cheng.Unitys.Editors;
using System.Reflection;
using Cheng.Algorithm;
using Cheng.Unitys.Windows.XInput;
using Cheng.ButtonTemplates.Joysticks.Unitys;

namespace Cheng.ButtonTemplates.UnityButtons.Windows.Editors
{

    /// <summary>
    /// UnityEditor 绘制<see cref="XInputPadUnityButton"/> 字段
    /// </summary>
    [CustomPropertyDrawer(typeof(XInputPadUnityButton))]
    public class XInputPadUnityButtonEditorDraw : PropertyDrawer
    {

        #region draw

        #region 参数

        public XInputPadUnityButtonEditorDraw()
        {
            p_isGettip = false;
            p_tip = null;
            p_drawPar = new DrawPar();
            p_drawPar.indexEnumTexts = f_getIndexEnumText();
            p_drawPar.butEnumTexts = f_getButtonEnumText();
        }

        private TooltipAttribute p_tip;
        private DrawPar p_drawPar;
        private bool p_isGettip;
        private TooltipAttribute Tooltip
        {
            get
            {
                if (p_isGettip) return p_tip;
                p_isGettip = true;
                p_tip = this.fieldInfo.GetCustomAttribute<TooltipAttribute>();
                return p_tip;
            }
        }

        internal static string[] f_getIndexEnumText()
        {
            return new string[4]
            {
                "手柄 0",
                "手柄 1",
                "手柄 2",
                "手柄 3"
            };
        }

        static string[] f_getButtonEnumText()
        {
            return new string[15]
            {
                "无",
                "十字键 - 上",
                "十字键 - 下",
                "十字键 - 左",
                "十字键 - 右",
                "菜单键",
                "返回键",
                "左摇杆按压",
                "右摇杆按压",
                "左肩键(LS)",
                "右肩键(RS)",
                "A",
                "B",
                "X",
                "Y"
            };
        }

        #endregion

        #region 派生

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var t = Tooltip;
            if (t != null) label.tooltip = t.tooltip;

            OnGUIDraw(position, property, label, p_drawPar);
        }

        #endregion

        #region 封装

        /// <summary>
        /// 绘制参数
        /// </summary>
        public struct DrawPar
        {

            /// <summary>
            /// 4个手柄索引的文本
            /// </summary>
            public string[] indexEnumTexts;

            /// <summary>
            /// 15个手柄按钮文本
            /// </summary>
            public string[] butEnumTexts;

        }

        /// <summary>
        /// 获取控件高度
        /// </summary>
        /// <param name="property">字段</param>
        /// <param name="label">标签；可null</param>
        /// <returns>GUI高度</returns>
        public static float GetHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIParser.AllFieldHeight;
        }

        public static void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label, DrawPar par)
        {
            Rect pos_label, pos_field;
            if (position.width < 50)
            {
                position.SectionLength(0.35f, 2, out pos_label, out pos_field);
            }
            else
            {
                position.SectionLength(0.5f, 2, out pos_label, out pos_field);
            }
            EditorGUI.LabelField(pos_label, label);
            OnGUIDrawField(pos_field, property, in par);
        }

        public static void OnGUIDrawField(Rect position, SerializedProperty property, in DrawPar par)
        {
            var pro_index = property.FindPropertyRelative(XInputPadUnityButton.cp_fieldName_index);
            var pro_button = property.FindPropertyRelative(XInputPadUnityButton.cp_fieldName_button);

            //切割左右
            Rect pos_index, pos_button;
            position.SectionLength(0.4f, 2, out pos_index, out pos_button);

            //绘制手柄索引枚举
            pro_index.intValue = EditorGUI.Popup(pos_index, pro_index.intValue, par.indexEnumTexts);

            //绘制按钮枚举
            var bvi = EditorGUI.Popup(pos_button, GetValueToNum(pro_button.intValue), par.butEnumTexts);

            pro_button.intValue = GetNumToValue(bvi);
        }

        static int GetValueToNum(int type)
        {
            if (type <= (int)XInputPadButtonType.RightShoulder)
            {
                return (int)type;
            }
            return (int)type - 2;
        }

        static int GetNumToValue(int index)
        {
            if (index <= (int)XInputPadButtonType.RightShoulder)
            {
                return index;
            }
            return (index + 2);
        }

        #endregion

        #endregion

    }

    /// <summary>
    /// UnityEditor 绘制<see cref="XInputTriggerUnityButton"/> 字段
    /// </summary>
    [CustomPropertyDrawer(typeof(XInputTriggerUnityButton))]
    public class XInputTriggerUnityButtonEditorDraw : PropertyDrawer
    {

        #region draw

        #region 参数

        public XInputTriggerUnityButtonEditorDraw()
        {
            p_isGettip = false;
            p_tip = null;
            p_drawPar = new DrawPar();
            p_drawPar.indexEnumTexts = XInputPadUnityButtonEditorDraw.f_getIndexEnumText();
            p_drawPar.butEnumTexts = f_getButtonEnumText();
        }

        private TooltipAttribute p_tip;
        private DrawPar p_drawPar;
        private bool p_isGettip;
        private TooltipAttribute Tooltip
        {
            get
            {
                if (p_isGettip) return p_tip;
                p_isGettip = true;
                p_tip = this.fieldInfo.GetCustomAttribute<TooltipAttribute>();
                return p_tip;
            }
        }

        static string[] f_getButtonEnumText()
        {
            return new string[3]
            {
                "无",
                "左扳机",
                "右扳机"
            };
        }

        #endregion

        #region 派生

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var t = Tooltip;
            if (t != null) label.tooltip = t.tooltip;

            OnGUIDraw(position, property, label, p_drawPar);
        }

        #endregion

        #region 封装

        /// <summary>
        /// 绘制参数
        /// </summary>
        public struct DrawPar
        {

            /// <summary>
            /// 4个手柄索引的文本
            /// </summary>
            public string[] indexEnumTexts;

            /// <summary>
            /// 3个扳机按钮文本
            /// </summary>
            public string[] butEnumTexts;

        }

        /// <summary>
        /// 获取控件高度
        /// </summary>
        /// <param name="property">字段</param>
        /// <param name="label">标签；可null</param>
        /// <returns>GUI高度</returns>
        public static float GetHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIParser.AllFieldHeight;
        }

        public static void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label, DrawPar par)
        {
            Rect pos_label, pos_field;
            if (position.width < 50)
            {
                position.SectionLength(0.35f, 2, out pos_label, out pos_field);
            }
            else
            {
                position.SectionLength(0.5f, 2, out pos_label, out pos_field);
            }

            EditorGUI.LabelField(pos_label, label);
            OnGUIDrawField(pos_field, property, in par);
        }

        public static void OnGUIDrawField(Rect position, SerializedProperty property, in DrawPar par)
        {
            var pro_index = property.FindPropertyRelative(XInputTriggerUnityButton.cp_fieldName_index);
            var pro_button = property.FindPropertyRelative(XInputTriggerUnityButton.cp_fieldName_button);

            //切割左右
            Rect pos_index, pos_button;
            position.SectionLength(0.5f, 2, out pos_index, out pos_button);

            //绘制手柄索引枚举
            pro_index.intValue = EditorGUI.Popup(pos_index, pro_index.intValue, par.indexEnumTexts);

            //绘制按钮枚举
            pro_button.intValue = EditorGUI.Popup(pos_button, (pro_button.intValue), par.butEnumTexts);
        }

        #endregion

        #endregion

    }

    /// <summary>
    /// UnityEditor 绘制<see cref="XInputUnityJoystick"/> 字段
    /// </summary>
    [CustomPropertyDrawer(typeof(XInputUnityJoystick))]
    public class XInputUnityJoystickEditorDraw : PropertyDrawer
    {

        #region draw

        #region 参数

        public XInputUnityJoystickEditorDraw()
        {
            p_isGettip = false;
            p_tip = null;
            p_drawPar = new DrawPar();
            p_drawPar.indexEnumTexts = XInputPadUnityButtonEditorDraw.f_getIndexEnumText();
            p_drawPar.butEnumTexts = f_getButtonEnumText();
        }

        private TooltipAttribute p_tip;
        private DrawPar p_drawPar;
        private bool p_isGettip;
        private TooltipAttribute Tooltip
        {
            get
            {
                if (p_isGettip) return p_tip;
                p_isGettip = true;
                p_tip = this.fieldInfo.GetCustomAttribute<TooltipAttribute>();
                return p_tip;
            }
        }

        static string[] f_getButtonEnumText()
        {
            return new string[3]
            {
                "无",
                "左摇杆",
                "右摇杆"
            };
        }

        #endregion

        #region 派生

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var t = Tooltip;
            if (t != null) label.tooltip = t.tooltip;

            OnGUIDraw(position, property, label, p_drawPar);
        }

        #endregion

        #region 封装

        /// <summary>
        /// 绘制参数
        /// </summary>
        public struct DrawPar
        {

            /// <summary>
            /// 4个手柄索引的文本
            /// </summary>
            public string[] indexEnumTexts;

            /// <summary>
            /// 3个摇杆类型文本
            /// </summary>
            public string[] butEnumTexts;

        }

        /// <summary>
        /// 获取控件高度
        /// </summary>
        /// <param name="property">字段</param>
        /// <param name="label">标签；可null</param>
        /// <returns>GUI高度</returns>
        public static float GetHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIParser.AllFieldHeight;
        }

        public static void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label, DrawPar par)
        {
            Rect pos_label, pos_field;
            if (position.width < 50)
            {
                position.SectionLength(0.35f, 2, out pos_label, out pos_field);
            }
            else
            {
                position.SectionLength(0.5f, 2, out pos_label, out pos_field);
            }

            EditorGUI.LabelField(pos_label, label);
            OnGUIDrawField(pos_field, property, in par);
        }

        public static void OnGUIDrawField(Rect position, SerializedProperty property, in DrawPar par)
        {
            var pro_index = property.FindPropertyRelative(XInputUnityJoystick.cp_fieldName_index);
            var pro_button = property.FindPropertyRelative(XInputUnityJoystick.cp_fieldName_button);

            //切割左右
            Rect pos_index, pos_button;
            position.SectionLength(0.5f, 2, out pos_index, out pos_button);

            //绘制手柄索引枚举
            pro_index.intValue = EditorGUI.Popup(pos_index, pro_index.intValue, par.indexEnumTexts);

            //绘制按钮枚举
            pro_button.intValue = EditorGUI.Popup(pos_button, (pro_button.intValue), par.butEnumTexts);
        }

        #endregion

        #endregion

    }

}

#endif