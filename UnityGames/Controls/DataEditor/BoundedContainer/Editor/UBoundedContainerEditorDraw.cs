#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;
using UnityEditor;
using Cheng.Unitys.Editors;
using System.Reflection;
using Cheng.Algorithm;
using Cheng.Unitys;

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
            p_temp = new GUIContent();
        }

        #region 参数

        private GUIContent p_temp;
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
            return GetHeight(property, label, null);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = createLabel(label);

            //OnGUIDraw(position, property, label);
            OnGUIDraw(position, property, label, fieldInfo.FieldType, p_temp);
        }

        #endregion

        #endregion

        #region 封装

        /// <summary>
        /// 获取控件高度
        /// </summary>
        /// <param name="property">字段</param>
        /// <param name="label">标签；可null</param>
        /// <param name="fieldGenericType">字段泛型类型；可null</param>
        /// <returns>GUI高度</returns>
        public static float GetHeight(SerializedProperty property, GUIContent label, Type fieldGenericType)
        {
            return EditorGUIParser.AllFieldHeight;

            //if (fieldGenericType == typeof(int) || fieldGenericType == typeof(long) || fieldGenericType == typeof(short) || fieldGenericType == typeof(float) || fieldGenericType == typeof(double) || fieldGenericType == typeof(UDecimal))
            //{
            //    return EditorGUIParser.AllFieldHeight;
            //}
            //else
            //{
            //    return EditorGUIUtility.singleLineHeight;
            //}

        }

        /// <summary>
        /// 对象绘制封装函数
        /// </summary>
        /// <param name="position">绘制的位置</param>
        /// <param name="property">绘制的字段储存器</param>
        /// <param name="label">主标签；null或空标签表示不绘制</param>
        /// <param name="fieldGenericType">字段泛型类型</param>
        /// <param name="temp">临时标签对象</param>
        public static void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label, Type fieldGenericType, GUIContent temp)
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

            OnGUIDrawing(pos_value, property, fieldGenericType, temp);

        }

        /// <summary>
        /// 绘制字段
        /// </summary>
        /// <param name="position">绘制的位置</param>
        /// <param name="property">绘制的字段储存器</param>
        /// <param name="fieldGenericType">字段泛型类型</param>
        /// <param name="temp">临时标签实例</param>
        public static void OnGUIDrawing(Rect position, SerializedProperty property, Type fieldGenericType, GUIContent temp)
        {
            if (fieldGenericType == typeof(int) || fieldGenericType == typeof(long) || fieldGenericType == typeof(short))
            {
                DrawingInt(position, property, temp);
            }
            else if (fieldGenericType == typeof(float) || fieldGenericType == typeof(double))
            {
                DrawingFloat(position, property, temp);
            }
            else if (fieldGenericType == typeof(UDecimal))
            {
                DrawingUDecimal(position, property, temp);
            }
            else
            {
                var defc = GUI.color;
                var defCont = GUI.contentColor;
                GUI.contentColor = Color.yellow;
                temp.text = "ERROR";
                temp.tooltip = "不是已知可绘制类";
                EditorGUI.LabelField(position, temp);
                GUI.color = defc;
                GUI.contentColor = defCont;
                //EditorGUI.HelpBox(position, "不是已知可绘制类", MessageType.Warning);
            }

        }

        /// <summary>
        /// 绘制字段 - 整数
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="property">字段信息</param>
        /// <param name="temp">临时标签对象</param>
        public static void DrawingInt(Rect position, SerializedProperty property, GUIContent temp)
        {
            var pro_value = property.FindPropertyRelative(UBoundedContainer<int>.cp_valueFieldName);
            var pro_max = property.FindPropertyRelative(UBoundedContainer<int>.cp_maxFieldName);
            var pro_min = property.FindPropertyRelative(UBoundedContainer<int>.cp_minFieldName);

            //分割标签区域和控件区域
            //position.SectionLength(0.5f, 0, out Rect pos_label, out Rect pos_field);

            Rect pos_field = position;
            Rect pos_min, pos_value, pos_max;

            pos_min = pos_field.LeftTrim(0.24f);
            pos_max = pos_field.RightTrim(0.24f);

            pos_value = pos_field.ShortenLengthToScale(0.25f, 0.25f);

            //获取值
            var min = pro_min.intValue;
            var max = pro_max.intValue;
            var value = pro_value.intValue;

            //绘制左右两端
            var newMin = EditorGUI.DelayedIntField(pos_min, min);
            var newMax = EditorGUI.DelayedIntField(pos_max, max);

            if (min != newMin)
            {
                //最小值被修改

                if(newMin > newMax) //限制最小值不超最大值
                {
                    newMin = newMax;
                }

            }

            if (max != newMax)
            {
                //最大值被修改

                if (newMax < newMin) //限制最大值不超最大值
                {
                    newMax = newMin;
                }
            }

            if (newMin < newMax)
            {
                pro_value.intValue = EditorGUI.IntSlider(pos_value, Maths.Clamp(value, newMin, newMax), min, max);
            }
            else
            {
                //范围超出
                value = Maths.Clamp(value, newMin, newMax);
                pro_value.intValue = value;
                //无修改绘制
                temp.text = value.ToString();
                temp.tooltip = "没有可修改的范围";
                temp.image = null;
                EditorGUI.LabelField(pos_value, temp);
            }

        }

        /// <summary>
        /// 绘制字段 - 浮点数
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="property">字段信息</param>
        /// <param name="temp">临时标签对象</param>
        public static void DrawingFloat(Rect position, SerializedProperty property, GUIContent temp)
        {
            var pro_value = property.FindPropertyRelative(UBoundedContainer<int>.cp_valueFieldName);
            var pro_max = property.FindPropertyRelative(UBoundedContainer<int>.cp_maxFieldName);
            var pro_min = property.FindPropertyRelative(UBoundedContainer<int>.cp_minFieldName);

            //分割标签区域和控件区域
            //position.SectionLength(0.5f, 0, out Rect pos_label, out Rect pos_field);

            Rect pos_field = position;
            Rect pos_min, pos_value, pos_max;

            pos_min = pos_field.LeftTrim(0.24f);
            pos_max = pos_field.RightTrim(0.24f);

            pos_value = pos_field.ShortenLengthToScale(0.25f, 0.25f);

            //获取值
            var min = pro_min.floatValue;
            var max = pro_max.floatValue;
            var value = pro_value.floatValue;

            //绘制左右两端
            var newMin = EditorGUI.DelayedFloatField(pos_min, min);
            var newMax = EditorGUI.DelayedFloatField(pos_max, max);

            if (min != newMin)
            {
                //最小值被修改

                if (newMin > newMax) //限制最小值不超最大值
                {
                    newMin = newMax;
                }

            }

            if (max != newMax)
            {
                //最大值被修改

                if (newMax < newMin) //限制最大值不超最大值
                {
                    newMax = newMin;
                }
            }

            if (newMin < newMax)
            {
                pro_value.floatValue = EditorGUI.Slider(pos_value, Maths.Clamp(value, newMin, newMax), min, max);
            }
            else
            {
                //范围超出
                value = Maths.Clamp(value, newMin, newMax);
                pro_value.floatValue = value;
                //无修改绘制
                temp.text = value.ToString();
                temp.tooltip = "没有可修改的范围";
                temp.image = null;
                EditorGUI.LabelField(pos_value, temp);
            }

        }

        /// <summary>
        /// 绘制字段 - 64位浮点数
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="property">字段信息</param>
        /// <param name="temp">临时标签对象</param>
        public static void DrawingDouble(Rect position, SerializedProperty property, GUIContent temp)
        {
            var pro_value = property.FindPropertyRelative(UBoundedContainer<int>.cp_valueFieldName);
            var pro_max = property.FindPropertyRelative(UBoundedContainer<int>.cp_maxFieldName);
            var pro_min = property.FindPropertyRelative(UBoundedContainer<int>.cp_minFieldName);

            //分割标签区域和控件区域
            //position.SectionLength(0.5f, 0, out Rect pos_label, out Rect pos_field);

            Rect pos_field = position;
            Rect pos_min, pos_value, pos_max;

            pos_min = pos_field.LeftTrim(0.24f);
            pos_max = pos_field.RightTrim(0.24f);

            pos_value = pos_field.ShortenLengthToScale(0.25f, 0.25f);

            //获取值
            var min = pro_min.doubleValue;
            var max = pro_max.doubleValue;
            var value = pro_value.doubleValue;

            //绘制左右两端
            var newMin = EditorGUI.DelayedDoubleField(pos_min, min);
            var newMax = EditorGUI.DelayedDoubleField(pos_max, max);

            if (min != newMin)
            {
                //最小值被修改

                if (newMin > newMax) //限制最小值不超最大值
                {
                    newMin = newMax;
                }

            }

            if (max != newMax)
            {
                //最大值被修改

                if (newMax < newMin) //限制最大值不超最大值
                {
                    newMax = newMin;
                }
            }

            if (newMin < newMax)
            {
                pro_value.doubleValue = EditorGUI.Slider(pos_value, (float)Maths.Clamp(value, newMin, newMax), (float)min, (float)max);
            }
            else
            {
                //范围超出
                value = Maths.Clamp(value, newMin, newMax);
                pro_value.doubleValue = value;
                //无修改绘制
                temp.text = value.ToString();
                temp.tooltip = "没有可修改的范围";
                temp.image = null;
                EditorGUI.LabelField(pos_value, temp);
            }

        }

        /// <summary>
        /// 绘制字段 - 十进制数
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="property">字段信息</param>
        /// <param name="temp">临时标签对象</param>
        public static void DrawingUDecimal(Rect position, SerializedProperty property, GUIContent temp)

        {
            var pro_value = property.FindPropertyRelative(UBoundedContainer<int>.cp_valueFieldName);
            var pro_max = property.FindPropertyRelative(UBoundedContainer<int>.cp_maxFieldName);
            var pro_min = property.FindPropertyRelative(UBoundedContainer<int>.cp_minFieldName);

            //分割标签区域和控件区域
            //position.SectionLength(0.5f, 0, out Rect pos_label, out Rect pos_field);

            Rect pos_field = position;
            Rect pos_min, pos_value, pos_max;

            pos_min = pos_field.LeftTrim(0.24f);
            pos_max = pos_field.RightTrim(0.24f);

            pos_value = pos_field.ShortenLengthToScale(0.25f, 0.25f);

            //获取值
            //var min = pro_min.doubleValue;
            //var max = pro_max.doubleValue;
            //var value = pro_value.doubleValue;
            var min = DecimalUnityEditorDraw.GetPropertyValue(pro_min);
            var max = DecimalUnityEditorDraw.GetPropertyValue(pro_max);
            var value = DecimalUnityEditorDraw.GetPropertyValue(pro_value);


            //绘制左右两端
            //var newMin = EditorGUI.DelayedDoubleField(pos_min, min);
            var newMin = DecimalUnityEditorDraw.DrawDecimalValue(pos_min, min, null);
            //var newMax = EditorGUI.DelayedDoubleField(pos_max, max);
            var newMax = DecimalUnityEditorDraw.DrawDecimalValue(pos_max, max, null);

            if (min != newMin)
            {
                //最小值被修改

                if (newMin > newMax) //限制最小值不超最大值
                {
                    newMin = newMax;
                }

            }

            if (max != newMax)
            {
                //最大值被修改

                if (newMax < newMin) //限制最大值不超最大值
                {
                    newMax = newMin;
                }
            }

            if (newMin < newMax)
            {
                var v = EditorGUI.Slider(pos_value, (float)Maths.Clamp(value.ToDec(), newMin.ToDec(), newMax.ToDec()), (float)min.ToDec(), (float)max.ToDec());
                DecimalUnityEditorDraw.SetPropertyValue(pro_value, new UDecimal((decimal)v));
            }
            else
            {
                //范围超出
                value = Maths.Clamp(value.ToDec(), newMin.ToDec(), newMax.ToDec());
                //pro_value.doubleValue = value;
                DecimalUnityEditorDraw.SetPropertyValue(pro_value, new UDecimal(value));
                //无修改绘制
                temp.text = value.ToString();
                temp.tooltip = "没有可修改的范围";
                temp.image = null;
                EditorGUI.LabelField(pos_value, temp);
            }

        }

        #endregion

    }

}

#endif