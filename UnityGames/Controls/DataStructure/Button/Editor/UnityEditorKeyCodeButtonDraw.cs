#if UNITY_EDITOR
using Cheng.Unitys.Editors;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Cheng.ButtonTemplates.UnityButtons.UnityEditors
{

    /// <summary>
    /// 绘制 <see cref="KeyCodeButton"/> 类型参数的 Inspector 重绘脚本
    /// </summary>
    [CustomPropertyDrawer(typeof(KeyCodeButton))]
    public class UnityEditorKeyCodeButtonDraw : PropertyDrawer
    {

        private TooltipAttribute p_tip;
        private bool p_tipCreate = false;
        private TooltipAttribute Tooltip
        {
            get
            {
                if(!p_tipCreate)
                {
                    p_tipCreate = true;
                    p_tip = this.fieldInfo.GetCustomAttribute<TooltipAttribute>();
                }
                return p_tip;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var toolAt = Tooltip;

            if (toolAt != null) label.tooltip = toolAt.tooltip;

            OnGUIDraw(position, property, label);
        }

        /// <summary>
        /// 对<see cref="KeyCodeButton"/>绘制GUI函数封装
        /// </summary>
        /// <param name="position">绘制位置</param>
        /// <param name="property">字段储存信息</param>
        /// <param name="label">字段标签；null或空不绘制标签</param>
        public static void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect pos_label, pos_value;

            if (label is null || label == GUIContent.none)
            {
                pos_value = position;
            }
            else
            {
                position.SectionLength(0.5f, 0, out pos_label, out pos_value);
                EditorGUI.LabelField(pos_label, label);
            }

            OnGUIDrawing(pos_value, property);
        }

        /// <summary>
        /// 对<see cref="KeyCodeButton"/>绘制无标签GUI函数封装
        /// </summary>
        /// <param name="position">绘制位置</param>
        /// <param name="property">字段储存信息</param>
        public static void OnGUIDrawing(Rect position, SerializedProperty property)
        {
            var keyProperty = property.FindPropertyRelative(KeyCodeButton.EditorProperityFieldName);

            //var toolAt = Tooltip;
            //if (toolAt != null) label.tooltip = toolAt.tooltip;

            //keyProperty.intValue = (int)((KeyCode)EditorGUI.EnumPopup(position, (KeyCode)keyProperty.intValue));

            keyProperty.intValue = (int)((KeyCode)EditorGUI.EnumPopup(position, (KeyCode)keyProperty.intValue));
        }

    }

}

#endif