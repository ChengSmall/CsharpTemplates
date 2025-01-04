#if UNITY_EDITOR

using System;

using UnityEngine;

using UnityEditor;
using System.Reflection;

namespace Cheng.DataStructure.Receptacles.UnityEditors
{

    /// <summary>
    /// 容器结构 Unity GUI 绘制重写
    /// </summary>
    [CustomPropertyDrawer(typeof(ReceptacleValue<>))]
    public class ReceptacleUnityEditorDraw : PropertyDrawer
    {

        public ReceptacleUnityEditorDraw()
        {
            p_isGettip = false;
            p_fenLabel = CreateDefaultTrim();
        }

        private const string valueName = ReceptacleValue<int>.guiName_value;

        private const string maxValueName = ReceptacleValue<int>.guiName_maxValue;

        private GUIContent p_fenLabel;

        private TooltipAttribute p_tip;
        private bool p_isGettip;
        private TooltipAttribute Tooltip
        {
            get
            {
                if (p_isGettip) return p_tip;
                p_tip = this.fieldInfo.GetCustomAttribute<TooltipAttribute>();
                p_isGettip = true;
                return p_tip;
            }
        }

        GUIContent createLabel(GUIContent label)
        {
            var t = Tooltip;
            if (t != null) label.tooltip = t.tooltip;
            return label;
            //return new GUIContent(label.text, label.image, tl.tooltip);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 20;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {            
            //获取字段显示label
            label = createLabel(label);
           
            OnGUIDraw(position, property, label, p_fenLabel);
        }

        /// <summary>
        /// 封装绘制函数
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="property">容器字段数据</param>
        /// <param name="label">主标签</param>
        /// <param name="trimLabel">值和最大值的分隔符标签</param>
        public static void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label,GUIContent trimLabel)
        {

            //获取流存储资源引用
            var pro_value = property.FindPropertyRelative(valueName);
            var pro_maxValue = property.FindPropertyRelative(maxValueName);

            //获取字段显示label
            //var gui_Label = createLabel(label);

            //获取总位置参数
            var pos_x = position.x;
            var pos_y = position.y;
            var pos_width = position.width;
            var pos_height = position.height;

            //字段标签长度
            var pos_guiLabelWidth = pos_width * 0.25f;

            //值框框长度
            var pos_banWidth = (pos_width * 0.380f) - 10;

            //分隔符长度
            float pos_fenWidth = 8f;

            //GUI位置显示
            Rect rect_label = new Rect(pos_x, pos_y, pos_guiLabelWidth, pos_height);

            Rect rect_value = new Rect(pos_x + pos_guiLabelWidth, pos_y, pos_banWidth, pos_height);

            //分隔符标签位置
            Rect rect_fenLabel = new Rect(rect_value.x + pos_banWidth, pos_y, pos_fenWidth, pos_height);

            Rect rect_maxValue = new Rect(rect_fenLabel.x + pos_fenWidth, pos_y, pos_banWidth, pos_height);

            //分隔符绘制
            EditorGUI.LabelField(rect_fenLabel, trimLabel);
            //总标签绘制
            EditorGUI.LabelField(rect_label, label);

            //value绘制
            EditorGUI.PropertyField(rect_value, pro_value, GUIContent.none);
            //maxValue绘制
            EditorGUI.PropertyField(rect_maxValue, pro_maxValue, GUIContent.none);

        }
        
        /// <summary>
        /// 创建默认的分隔符标签
        /// </summary>
        /// <returns></returns>
        public static GUIContent CreateDefaultTrim()
        {
            return new GUIContent("/");
        }

    }

}

#endif