#if UNITY_EDITOR

using Cheng.Unitys.Editors;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Cheng.ButtonTemplates.UnityButtons.UnityEditors
{

    /// <summary>
    /// 绘制UnityNameButton类型参数的 Inspector 重绘脚本
    /// </summary>
    [CustomPropertyDrawer(typeof(UnityNameButton))]
    public class UnityEditorNameButtonDraw : PropertyDrawer
    {
        public UnityEditorNameButtonDraw()
        {
        }

        private TooltipAttribute p_tooltip;

        private TooltipAttribute ToolTip
        {
            get
            {
                if(p_tooltip is null) p_tooltip = this.fieldInfo.GetCustomAttribute<TooltipAttribute>();
                return p_tooltip;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //判断注释器特性
            var toolAt = ToolTip;

            if (toolAt != null) label.tooltip = toolAt.tooltip;

            OnGUIDraw(position, property, label);

        }

        /// <summary>
        /// 封装绘制<see cref="UnityNameButton"/>字段GUI函数
        /// </summary>
        /// <param name="position">字段带标签的GUI位置</param>
        /// <param name="property">字段存储信息</param>
        /// <param name="label">字段GUI标签</param>
        public static void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label)

        {
            //获取字段
            var nameProperty = property.FindPropertyRelative(UnityNameButton.EditorProperityFieldButtonName);
            var smoothPro = property.FindPropertyRelative(UnityNameButton.EditorProperityFieldAxisToolName);

            float height = position.height;

            //标签区域
            Rect labelRect = new Rect(position.x, position.y, 100f, height);

            //文本编写区域
            Rect textRect = new Rect(position.x + 105f, position.y, position.width - (120f + 10f), height);

            //开关区域
            Rect tookRect = new Rect(textRect.x + textRect.width + 5f, position.y, 35f, height);

            //绘制
            EditorGUI.LabelField(labelRect, label);
            nameProperty.stringValue = EditorGUI.DelayedTextField(textRect, nameProperty.stringValue);
            smoothPro.boolValue = EditorGUI.Toggle(tookRect, smoothPro.boolValue);

        }

        /// <summary>
        /// 封装绘制<see cref="UnityNameButton"/>字段无标签GUI函数
        /// </summary>
        /// <param name="position">字段的GUI位置</param>
        /// <param name="property">字段存储信息</param>
        public static void OnGUIDraw(Rect position, SerializedProperty property)

        {
            //获取字段
            var nameProperty = property.FindPropertyRelative(UnityNameButton.EditorProperityFieldButtonName);
            var smoothPro = property.FindPropertyRelative(UnityNameButton.EditorProperityFieldAxisToolName);

            float height = position.height;

            //标签区域
            //Rect labelRect = new Rect(position.x, position.y, 100f, height);

            //文本编写区域
            Rect textRect = new Rect(position.x, position.y, position.width - (EditorGUIParser.ToggleWidth + 5), height);

            //开关区域
            Rect tookRect = new Rect(textRect.x + ((EditorGUIParser.ToggleWidth + 8)), position.y, EditorGUIParser.ToggleWidth + 5, height);

            //绘制
            //EditorGUI.LabelField(labelRect, label);
            nameProperty.stringValue = EditorGUI.DelayedTextField(textRect, nameProperty.stringValue);
            smoothPro.boolValue = EditorGUI.Toggle(tookRect, smoothPro.boolValue);

        }


    }

}

#endif