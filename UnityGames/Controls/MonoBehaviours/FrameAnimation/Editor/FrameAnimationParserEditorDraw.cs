#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Cheng.Unitys.Animators.FrameAnimations
{

    /// <summary>
    /// 重绘检查器动画脚本参数
    /// </summary>
    [CustomPropertyDrawer(typeof(FrameAnimationParser))]
    public class FrameAnimationParserEditorDraw : PropertyDrawer
    {

        #region 初始化绘制器
        public FrameAnimationParserEditorDraw()
        {
            p_toggleLabel = f_createToggleLoopLabel();
        }
        #endregion

        #region 参数

        private GUIContent p_toggleLabel;

        static GUIContent f_createToggleLoopLabel()
        {
            return new GUIContent("loop", "是否循环播放");
        }

        #endregion

        #region 重绘

        /// <summary>
        /// 创建检查器数据标签
        /// </summary>
        /// <param name="label"></param>
        /// <param name="field"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        static GUIContent f_createGUILabel(GUIContent label, FieldInfo field)
        {
            TooltipAttribute att;

            att = Attribute.GetCustomAttribute(field, typeof(TooltipAttribute)) as TooltipAttribute;
            //att = field.GetCustomAttribute<TooltipAttribute>();

            if (att is null) return label;

            return new GUIContent(label.text, label.image, att.tooltip);            
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            FieldInfo field = this.fieldInfo;

            label = f_createGUILabel(label, field);

            OnGUIDraw(position, property, label, p_toggleLabel);

        }

        #endregion

        #region 封装

        /// <summary>
        /// 绘制函数封装
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="property">绘制的字段数据</param>
        /// <param name="label">主标签</param>
        /// <param name="toggleLabel">循环播放开关标签</param>
        public static void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label, GUIContent toggleLabel)
        {

            //字段数据
            var frameTimePro = property.FindPropertyRelative(FrameAnimationParser.FrameTimeName);
            var loopPro = property.FindPropertyRelative(FrameAnimationParser.LoopName);

            //总长度
            float allWdith = position.width;

            //开关长度
            const float loopWidth = 35f + 18f;

            float y = position.y;
            float height = position.height;
            float x = position.x;

            //标签长度
            float labeLength;

            //labeLength = (drawLabel.text.Length * p_textAPixed) + 20f;
            labeLength = (allWdith * 0.38f);

            //浮点值框长度
            float frameTimeLength = (allWdith * 0.62f);

            //标签位置
            Rect labelPos = new Rect(x, y, labeLength, height);

            //浮点值框位置
            Rect frameTimePos = new Rect(x + labeLength, y, frameTimeLength - (loopWidth + 10f), height);

            //开关位置
            Rect loopBoolPos = new Rect(x + (allWdith - (loopWidth)), y, loopWidth, height);

            //-------绘制-----------

            //绘制字段标签

            EditorGUI.LabelField(labelPos, label, GUIContent.none);

            //左侧绘制浮点型数据

            var vre = EditorGUI.FloatField(frameTimePos, GUIContent.none, frameTimePro.floatValue);

            if (vre < 0) vre = 0;
            frameTimePro.floatValue = vre;

            //右侧绘制布尔开关

            loopPro.boolValue = EditorGUI.ToggleLeft(loopBoolPos, toggleLabel, loopPro.boolValue);

            //-------绘制-----------
        }

        #endregion

    }

}

#endif
