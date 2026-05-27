#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Cheng.ButtonTemplates.Joysticks.Unitys.UnityEditors
{

    /// <summary>
    /// <see cref="UnityAxis"/> 对象 Inspector 重绘脚本
    /// </summary>
    [CustomPropertyDrawer(typeof(UnityAxis))]
    public class UnityEditorAxisDraw : PropertyDrawer
    {

        #region

        public UnityEditorAxisDraw()
        {
            p_open = true;

            p_herLabel = new GUIContent(horizontalInspectorName);
            p_verLabel = new GUIContent(verticalInspectorName);
            p_t = null;
            p_getting = false;
        }

        const float OnceHeight = 18f;

        private GUIContent p_herLabel;
        private GUIContent p_verLabel;
        private TooltipAttribute p_t;

        private bool p_open;

        private bool p_getting;

        private TooltipAttribute Tooltip
        {
            get
            {
                if (p_getting) return p_t;
                p_t = this.fieldInfo.GetCustomAttribute<TooltipAttribute>();
                p_getting = true;
                return p_t;
            }
        }

        const string horizontalInspectorName = "横轴名称";
        const string verticalInspectorName = "纵轴名称";

        /// <summary>
        /// 展开时字段大小
        /// </summary>
        const float cp_openSize = ((18 * 3) + (2 * 3));

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return p_open ? cp_openSize : (20f);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            var att = Tooltip;
            if (att != null) label.tooltip = att.tooltip;

            p_open = OnGUIDraw(position, property, label, p_herLabel, p_verLabel, p_open);

        }


        /// <summary>
        /// <see cref="UnityAxis"/>字段绘制GUI方法封装
        /// </summary>
        /// <param name="position">GUI字段位置</param>
        /// <param name="property">GUI字段储存信息</param>
        /// <param name="label">要绘制的字段名称和显示标签信息</param>
        /// <param name="herLabel">横轴参数标签</param>
        /// <param name="verLabel">纵轴参数标签</param>
        /// <param name="open">折叠条是否展开</param>
        /// <returns><paramref name="open"/>参数变化</returns>
        public static bool OnGUIDraw(Rect position, SerializedProperty property, GUIContent label, GUIContent herLabel, GUIContent verLabel, bool open)
        {

            const string xname = UnityAxis.FieldName_Horizontal;
            const string yname = UnityAxis.FieldName_Vertical;
            const string x_smoothTook = UnityAxis.FieldName_XSmooth;
            const string y_smoothTook = UnityAxis.FieldName_YSmooth;
            const string isXRevTook = UnityAxis.FieldName_IsXReversed;
            const string isYRevTook = UnityAxis.FieldName_IsYReversed;

            //获取字段
            var xStrPro = property.FindPropertyRelative(xname);
            var yStrPro = property.FindPropertyRelative(yname);
            var x_axisPro = property.FindPropertyRelative(x_smoothTook);
            var y_axisPro = property.FindPropertyRelative(y_smoothTook);
            var x_revPro = property.FindPropertyRelative(isXRevTook);
            var y_revPro = property.FindPropertyRelative(isYRevTook);

            //获取标签
            //GUIContent drawLabel = label;
            //var att = fieldInfo.GetCustomAttribute<TooltipAttribute>();
            //if (att != null) drawLabel.tooltip = att.tooltip;

            //两轴注释文本长度
            const float labelAxisTextLength = 100f;

            //标签位置
            Rect labelPos = new Rect(position.x, position.y, position.width, OnceHeight);

            //当前y轴位置
            float yThisPos;
            //当前名称填写x位置
            float textPos;
            //当前名称填写长度
            float textLength;
            //平滑开关x位置
            float togglePos;
            //反转开关位置
            float toggleRevPos;

            //开关的长度
            const float togglength = 20f;

            yThisPos = position.y + 20;
            textPos = position.x + (labelAxisTextLength);
            textLength = position.width - (labelAxisTextLength + 50f);
            //togglePos = position.x + position.width - 40f;
            togglePos = textPos + textLength + 2;
            toggleRevPos = togglePos + togglength + 5f;

            //横轴标签位置
            Rect xLabel = new Rect(position.x, yThisPos, labelAxisTextLength, OnceHeight);
            //横轴名称填写位置
            Rect xPos = new Rect(textPos, yThisPos, textLength, OnceHeight);
            //横轴平滑开关位置
            Rect x_axisPos = new Rect(togglePos, yThisPos, togglength, OnceHeight);
            //横轴反转开关位置
            Rect x_axisRevPos = new Rect(toggleRevPos, yThisPos, togglength, OnceHeight);

            yThisPos += 20;
            //textPos = position.x + (labelAxisTextLength + 5);
            //textLength = position.width - (labelAxisTextLength + 5 + 20f);
            //togglePos = position.x + position.width - 15f;
            //toggleRevPos = togglePos + 25f;

            //纵轴标签位置
            Rect yLabel = new Rect(position.x, yThisPos, labelAxisTextLength, OnceHeight);
            //纵轴名称填写位置
            Rect yPos = new Rect(textPos, yThisPos, textLength, OnceHeight);
            //纵轴平滑开关位置
            Rect y_axisPos = new Rect(togglePos, yThisPos, togglength, OnceHeight);
            //纵轴反转开关位置
            Rect y_axisRevPos = new Rect(toggleRevPos, yThisPos, togglength, OnceHeight);

            //绘制

            //EditorGUI.LabelField(labelPos, drawLabel);
            open = EditorGUI.Foldout(labelPos, open, label, true);

            if (open)
            {
                EditorGUI.LabelField(xLabel, herLabel);
                xStrPro.stringValue = EditorGUI.DelayedTextField(xPos, GUIContent.none, xStrPro.stringValue);
                x_axisPro.boolValue = EditorGUI.Toggle(x_axisPos, GUIContent.none, x_axisPro.boolValue);

                x_revPro.boolValue = EditorGUI.Toggle(x_axisRevPos, GUIContent.none, x_revPro.boolValue);

                EditorGUI.LabelField(yLabel, verLabel);
                yStrPro.stringValue = EditorGUI.DelayedTextField(yPos, GUIContent.none, yStrPro.stringValue);
                y_axisPro.boolValue = EditorGUI.Toggle(y_axisPos, GUIContent.none, y_axisPro.boolValue);

                y_revPro.boolValue = EditorGUI.Toggle(y_axisRevPos, GUIContent.none, y_revPro.boolValue);
            }

            //EditorGUI.EndFoldoutHeaderGroup();

            return open;
        }

        /// <summary>
        /// 获取<see cref="UnityAxis"/>字段绘制时的GUI高度
        /// </summary>
        /// <param name="fold">获取的字段高度是否为展开，展开为true，折叠为false</param>
        /// <returns>字段GUI高度</returns>
        public static float GetHeight(bool fold)
        {
            return fold ? cp_openSize : (20f);
        }

        #endregion

    }

}
#endif