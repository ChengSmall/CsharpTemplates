#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Collections;

using UnityEngine;

using UnityEditor;
using System.Reflection;
using Cheng.Unitys.Editors;

namespace Cheng.GameTemplates.PushingBoxes.UnityEditors
{

    /// <summary>
    /// 对<see cref="SpriteDraw"/>的U3D检查器绘制
    /// </summary>
    [CustomPropertyDrawer(typeof(SpriteDraw))]
    public class SpriteDrawEditorGUI : PropertyDrawer
    {

        #region 绘制

        #region 构造

        public SpriteDrawEditorGUI()
        {
            p_isGettip = false;
            p_tip = null;
        }

        #endregion

        #region 参数

        private TooltipAttribute p_tip;
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

        #region 封装绘制函数

        /// <summary>
        /// 获得高度
        /// </summary>
        /// <param name="property">字段存储信息</param>
        /// <param name="label">标签</param>
        /// <returns>此条GUI高度</returns>
        public static float GetHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIParser.AllFieldHeight;
        }

        /// <summary>
        /// 封装绘制函数
        /// </summary>
        /// <param name="position">绘制位置</param>
        /// <param name="property">字段储存信息</param>
        /// <param name="label">标签</param>
        public static void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label)
        {

            var pro_sprite = property.FindPropertyRelative(SpriteDraw.fieldName_sprite);
            var pro_color = property.FindPropertyRelative(SpriteDraw.fieldName_color);

            position.SectionLength(0.45f, 0, out Rect label_rect, out Rect control);

            Rect pos_sprite, pos_color;

            control.SectionLength(0.5f, 3, out pos_sprite, out pos_color);

            EditorGUI.PropertyField(pos_sprite, pro_sprite, GUIContent.none);

            var color = pro_color.colorValue;

            pro_color.colorValue = EditorGUI.ColorField(pos_color, GUIContent.none, color);

            EditorGUI.LabelField(label_rect, label);

        }

        #endregion

        #endregion

    }

}
#endif