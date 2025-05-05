#if UNITY_EDITOR

using Cheng.Unitys.Editors;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Cheng.ButtonTemplates.UnityButtons.UnityEditors
{

    /// <summary>
    /// MultKeyCodeButton 类型参数的 Inspector 重绘脚本
    /// </summary>
    [CustomPropertyDrawer(typeof(MultKeyCodeButton))]
    public class EditorMultKeyCodeDraw : PropertyDrawer
    {

        #region

        public EditorMultKeyCodeDraw()
        {
            p_andLabel = CreateIsAndStateGUILabel();
        }

        private GUIContent p_andLabel;

        private TooltipAttribute p_tip;

        private bool p_tipCreate = false;
        private TooltipAttribute Tooltip
        {
            get
            {
                
                if (!p_tipCreate)
                {
                    p_tipCreate = true;
                    p_tip = this.fieldInfo.GetCustomAttribute<TooltipAttribute>();
                }
                return p_tip;
            }
        }

        /// <summary>
        /// 创建<see cref="MultKeyCodeButton.IsAndState"/>对应字段的GUI默认指定标签
        /// </summary>
        /// <returns></returns>
        public static GUIContent CreateIsAndStateGUILabel()
        {
            return new GUIContent("键码映射方式", "此功能对应 MultKeyCodeButton 类中的 IsAndState 属性");
        }

        GUIContent createLabel(GUIContent label)
        {
            var tl = Tooltip;

            if (tl != null) label.tooltip = tl.tooltip;

            return label;
            //return new GUIContent(label.text, label.image, tl.tooltip);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var listPro = property.FindPropertyRelative(MultKeyCodeButton.cp_keyArrText);

            if (!listPro.isExpanded)
            {
                //不展开
                return 20f;
            }

            //展开
            return (listPro.arraySize * 20f) + 40f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //获取参数首标签
            GUIContent drawLabel = createLabel(label);

            OnGUIDraw(position, property, drawLabel, p_andLabel);
        }

        /// <summary>
        /// 封装<see cref="MultKeyCodeButton"/>字段GUI绘制函数
        /// </summary>
        /// <param name="position">字段GUI位置</param>
        /// <param name="property">字段储存数据</param>
        /// <param name="label">字段标签</param>
        /// <param name="toggleLabel">映射方法开关对应标签</param>
        public static void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label, GUIContent toggleLabel)
        {
            //获取两个字段
            var listPro = property.FindPropertyRelative(MultKeyCodeButton.cp_keyArrText);
            var andBoolPro = property.FindPropertyRelative(MultKeyCodeButton.cp_isAndText);

            //获取映射方式布尔值
            bool and = andBoolPro.boolValue;

            //标签位置
            Rect labelPos = new Rect(position.x, position.y, position.width, position.height);

            Rect andPos;

            float width = (label.text.Length * 15);

            andPos = new Rect(position.x + (position.width - (width + 10)), position.y, width, 20f);

            EditorGUI.PropertyField(labelPos, listPro, label, listPro.isExpanded);

            //绘制映射方式开关
            and = EditorGUI.ToggleLeft(andPos, toggleLabel, and);            

            andBoolPro.boolValue = and;
        }

        /// <summary>
        /// 返回字段对应GUI高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public static float GetHeight(SerializedProperty property, GUIContent label)
        {
            var listPro = property.FindPropertyRelative(MultKeyCodeButton.cp_keyArrText);

            if (!listPro.isExpanded)
            {
                //不展开
                return EditorGUIParser.AllFieldHeight;
            }

            //展开
            return (listPro.arraySize * EditorGUIParser.AllFieldHeight) + 40f;
        }

        #endregion

    }

}

#endif