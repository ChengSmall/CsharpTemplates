#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;
using UnityEditor;
using UnityEngine;
using Cheng.Unitys.Editors;

namespace Cheng.ButtonTemplates.Joysticks.Unitys.UnityEditors
{

    /// <summary>
    /// 绘制<see cref="TouchSlidingAxis"/> Unity GUI
    /// </summary>
    [CustomPropertyDrawer(typeof(TouchSlidingAxis))]
    public class TouchSlidingAxisEditorDraw : PropertyDrawer
    {

        #region 初始化

        public TouchSlidingAxisEditorDraw()
        {
            p_t = null;
            p_getting = false;
        }

        #endregion

        #region 参数

        #region tooltip

        private TooltipAttribute p_t;

        private bool p_getting;

        private TooltipAttribute Tooltip
        {
            get
            {
                if (p_getting) return p_t;
                p_getting = true;
                p_t = this.fieldInfo.GetCustomAttribute<TooltipAttribute>();                
                return p_t;
            }
        }

        #endregion

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

            OnGUIDraw(position, property, label);
        }

        #endregion

        #region 封装

        /// <summary>
        /// 获取字段GUI高度
        /// </summary>
        /// <param name="property">字段信息</param>
        /// <param name="label">标签</param>
        /// <returns>GUI高度</returns>
        public static float GetHeight(SerializedProperty property, GUIContent label)
        {            
            return EditorGUIParser.OnceHeight;
        }

        /// <summary>
        /// 绘制字段GUI
        /// </summary>
        /// <param name="position">绘制区域</param>
        /// <param name="property">字段信息</param>
        /// <param name="label">主标签</param>
        public static void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label)
        {

            var pro_speed = property.FindPropertyRelative(TouchSlidingAxis.FieldName_speed);

            Rect pos_label, pos_ver;

            position.SectionLength(0.4f, 0, out pos_label, out pos_ver);

            //绘制

            EditorGUI.LabelField(pos_label, label);

            pro_speed.vector2Value = EditorGUI.Vector2Field(pos_ver, GUIContent.none, pro_speed.vector2Value);
            
        }

        #endregion

    }

}

#endif