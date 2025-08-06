#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

using Cheng.Unitys.Editors;

namespace Cheng.Unitys.Animators.FrameAnimations
{

    /// <summary>
    /// 重绘检查器动画脚本参数
    /// </summary>
    [CustomPropertyDrawer(typeof(FrameAnimationParser))]
    public sealed class FrameAnimationParserEditorDraw : PropertyDrawer
    {

        #region 初始化绘制器
        public FrameAnimationParserEditorDraw()
        {
            p_toggleLabel = f_createToggleLoopLabel();
        }
        #endregion

        #region 参数

        private GUIContent p_toggleLabel;

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
        GUIContent f_createGUILabel(GUIContent label)
        {
            var att = Tooltip;
            if(att != null) label.tooltip = att.tooltip;
            return label;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            label = f_createGUILabel(label);

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
            Rect pos_value, pos_label;

            if (label is null || label == GUIContent.none)
            {
                pos_value = position;
            }
            else
            {
                position.SectionLength(0.4f, 0, out pos_label, out pos_value);
                EditorGUI.LabelField(pos_label, label);
            }


            OnGUIDrawing(pos_value, property, toggleLabel);

        }

        /// <summary>
        /// 绘制一个<see cref="FrameAnimationParser"/>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="toggleLabel"></param>
        public static void OnGUIDrawing(Rect position, SerializedProperty property, GUIContent toggleLabel)
        {
            //字段数据
            var frameTimePro = property.FindPropertyRelative(FrameAnimationParser.Field_FrameTimeName);
            var loopPro = property.FindPropertyRelative(FrameAnimationParser.Field_LoopName);

            Rect pos_frameTime, pos_toggle, pos_toggleLabel;

            //开关长度
            const float loopWidth_Label = 30f;

            //切割frametime和循环开关
            position.SectionRightLengthByValue(loopWidth_Label + EditorGUIParser.ToggleWidth + 2, 0,
                out pos_frameTime, out pos_toggle);

            pos_toggle.SectionRightLengthByValue(EditorGUIParser.ToggleWidth + 2, 0,
                out pos_toggleLabel, out pos_toggle);

            //-------绘制-----------

            //绘制字段标签

            //EditorGUI.LabelField(labelPos, label, GUIContent.none);

            //左侧绘制浮点型数据

            var vre = EditorGUI.FloatField(pos_frameTime, frameTimePro.floatValue);

            if (vre < 0) vre = 0;
            frameTimePro.floatValue = vre;

            //开关标签
            EditorGUI.LabelField(pos_toggleLabel, toggleLabel);

            //右侧绘制布尔开关
            loopPro.boolValue = EditorGUI.Toggle(pos_toggle, loopPro.boolValue);

            //-------绘制-----------
        }

        #endregion

    }

}

#endif
