#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;
using UnityEditor;
using Cheng.Unitys.Editors;
using System.Reflection;

namespace Cheng.DataStructure.BoundedContainers.UnityEditors
{

    /// <summary>
    /// 对<see cref="UBoundedContainer{T}"/>的GUI绘制
    /// </summary>
    [CustomPropertyDrawer(typeof(UBoundedContainer<>))]
    public class UBoundedContainerEditorDraw : PropertyDrawer
    {

        #region 绘制派生

        #region 初始化

        public UBoundedContainerEditorDraw()
        {
            
        }

        #region 参数

        private TooltipAttribute p_tip;
        private bool p_isGettip = false;

        private TooltipAttribute Tooltip
        {
            get
            {
                if (p_isGettip) return p_tip;
                p_isGettip = true;
                try
                {
                    p_tip = this.fieldInfo.GetCustomAttribute<TooltipAttribute>();
                }
                catch (Exception ex)
                {
                    p_tip = null;
                    Debug.LogError(ex.Message);
                }
                              
                return p_tip;
            }
        }

        /// <summary>
        /// 创建主标签
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        GUIContent createLabel(GUIContent label)
        {
            var t = Tooltip;
            if (t != null) label.tooltip = t.tooltip;
            return label;
            //return new GUIContent(label.text, label.image, tl.tooltip);
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
            label = createLabel(label);

            OnGUIDraw(position, property, label);
        }

        #endregion

        #endregion

        #region 封装

        /// <summary>
        /// 获取控件高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns>GUI高度</returns>
        public static float GetHeight(SerializedProperty property, GUIContent label)
        {
            //if (open)
            //{
            //    //展开高度 两个控件高度
            //    return EditorGUIParser.AllFieldHeight * 2;
            //}
            return EditorGUIParser.AllFieldHeight;
        }

        /// <summary>
        /// 封装绘制器
        /// </summary>
        /// <param name="position">绘制的位置</param>
        /// <param name="property">绘制的字段储存器</param>
        /// <param name="label">主标签</param>
        public static void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label)
        {
            var pro_value = property.FindPropertyRelative(UBoundedContainer<int>.cp_valueFieldName);
            var pro_max = property.FindPropertyRelative(UBoundedContainer<int>.cp_maxFieldName);
            var pro_min = property.FindPropertyRelative(UBoundedContainer<int>.cp_minFieldName);

            //分割标签区域和控件区域
            position.SectionLength(0.32f, 0, out Rect pos_label, out Rect pos_field);

            Rect pos_min, pos_value, pos_max;

            pos_min = pos_field.LeftTrim(0.23f);
            pos_max = pos_field.RightTrim(0.23f);

            pos_value = pos_field.ShortenLengthToScale(0.25f, 0.25f);

            //绘制标签
            EditorGUI.LabelField(pos_label, label);

            //绘制左右两端极值
            EditorGUI.PropertyField(pos_min, pro_min, GUIContent.none);
            EditorGUI.PropertyField(pos_max, pro_max, GUIContent.none);

            var pt = pro_value.propertyType;

            //按类型绘制滑动条
            if(pt == SerializedPropertyType.Integer)
            {
                var min = pro_min.intValue;
                var max = pro_max.intValue;
                pro_value.intValue = EditorGUI.IntSlider(pos_value, Mathf.Clamp(pro_value.intValue, min, max), min, max);
            }
            else if (pt == SerializedPropertyType.Float)
            {
                var min = pro_min.floatValue;
                var max = pro_max.floatValue;
                pro_value.floatValue = EditorGUI.Slider(pos_value, Mathf.Clamp(pro_value.floatValue, min, max), min, max);
            }
            else
            {                
                EditorGUI.PropertyField(pos_value, pro_value, GUIContent.none);
            }

        }

        #endregion


    }

}

#endif