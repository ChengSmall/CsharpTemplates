#if UNITY_EDITOR

using System;

using UnityEngine;

using UnityEditor;
using System.Reflection;
using Cheng.Unitys.Editors;
using Cheng.Unitys;
using Cheng.Unitys.DataStructure;
using Cheng.Unitys.DataStructure.Editors;

namespace Cheng.DataStructure.Receptacles.UnityEditors
{

    /// <summary>
    /// 针对<see cref="ReceptacleValue{T}"/>的泛型分支使用的GUI绘制函数
    /// </summary>
    public static class ReceptacleValueDrawing
    {

        /// <summary>
        /// 绘制字段 32位整数及以下值
        /// </summary>
        /// <param name="posValue">值位置</param>
        /// <param name="posMaxValue">最大值位置</param>
        /// <param name="valueProperty">值信息</param>
        /// <param name="maxValueProperty">最大值信息</param>
        public static void DrawingInt(Rect posValue, Rect posMaxValue, SerializedProperty valueProperty, SerializedProperty maxValueProperty)
        {
            var value = valueProperty.intValue;
            var maxValue = maxValueProperty.intValue;

            var newValue = EditorGUI.DelayedIntField(posValue, value);
            var newMax = EditorGUI.DelayedIntField(posMaxValue, maxValue);

            if (value != newValue)
            {
                //修改值
                //使其不超过最大值
                if (newValue > newMax)
                {
                    newMax = newValue;
                }
            }

            if(maxValue != newMax)
            {
                //修改最大值
                if(newMax < newValue)
                {
                    //设置值在范围内
                    newValue = newMax;
                }
            }

            valueProperty.intValue = newValue;
            maxValueProperty.intValue = newMax;

        }

        /// <summary>
        /// 绘制字段 64位整数
        /// </summary>
        /// <param name="posValue">值位置</param>
        /// <param name="posMaxValue">最大值位置</param>
        /// <param name="valueProperty">值信息</param>
        /// <param name="maxValueProperty">最大值信息</param>
        public static void DrawingLong(Rect posValue, Rect posMaxValue, SerializedProperty valueProperty, SerializedProperty maxValueProperty)
        {
            var value = valueProperty.longValue;
            var maxValue = maxValueProperty.longValue;

            var newValue = EditorGUI.LongField(posValue, value);
            var newMax = EditorGUI.LongField(posMaxValue, maxValue);

            if (value != newValue)
            {
                //修改值
                //使其不超过最大值
                if (newValue > newMax)
                {
                    newMax = newValue;
                }
            }

            if (maxValue != newMax)
            {
                //修改最大值
                if (newMax < newValue)
                {
                    //设置值在范围内
                    newValue = newMax;
                }
            }

            valueProperty.longValue = newValue;
            maxValueProperty.longValue = newMax;

        }

        /// <summary>
        /// 绘制字段 32位浮点数
        /// </summary>
        /// <param name="posValue">值位置</param>
        /// <param name="posMaxValue">最大值位置</param>
        /// <param name="valueProperty">值信息</param>
        /// <param name="maxValueProperty">最大值信息</param>
        public static void DrawingFloat(Rect posValue, Rect posMaxValue, SerializedProperty valueProperty, SerializedProperty maxValueProperty)
        {
            var value = valueProperty.floatValue;
            var maxValue = maxValueProperty.floatValue;

            var newValue = EditorGUI.DelayedFloatField(posValue, value);
            var newMax = EditorGUI.DelayedFloatField(posMaxValue, maxValue);

            if (value != newValue)
            {
                //修改值
                //使其不超过最大值
                if (newValue > newMax)
                {
                    newMax = newValue;
                }
            }

            if (maxValue != newMax)
            {
                //修改最大值
                if (newMax < newValue)
                {
                    //设置值在范围内
                    newValue = newMax;
                }
            }

            valueProperty.floatValue = newValue;
            maxValueProperty.floatValue = newMax;

        }

        /// <summary>
        /// 绘制字段 64位浮点数
        /// </summary>
        /// <param name="posValue">值位置</param>
        /// <param name="posMaxValue">最大值位置</param>
        /// <param name="valueProperty">值信息</param>
        /// <param name="maxValueProperty">最大值信息</param>
        public static void DrawingDouble(Rect posValue, Rect posMaxValue, SerializedProperty valueProperty, SerializedProperty maxValueProperty)
        {
            var value = valueProperty.doubleValue;
            var maxValue = maxValueProperty.doubleValue;

            var newValue = EditorGUI.DelayedDoubleField(posValue, value);
            var newMax = EditorGUI.DelayedDoubleField(posMaxValue, maxValue);

            if (value != newValue)
            {
                //修改值
                //使其不超过最大值
                if (newValue > newMax)
                {
                    newMax = newValue;
                }
            }

            if (maxValue != newMax)
            {
                //修改最大值
                if (newMax < newValue)
                {
                    //设置值在范围内
                    newValue = newMax;
                }
            }

            valueProperty.doubleValue = newValue;
            maxValueProperty.doubleValue = newMax;

        }

        /// <summary>
        /// 绘制字段 十进制数绘制封装类
        /// </summary>
        /// <param name="posValue">值位置</param>
        /// <param name="posMaxValue">最大值位置</param>
        /// <param name="valueProperty">值信息</param>
        /// <param name="maxValueProperty">最大值信息</param>
        public static void DrawingUDecimal(Rect posValue, Rect posMaxValue, SerializedProperty valueProperty, SerializedProperty maxValueProperty)
        {
            var value = DecimalUnityEditorDraw.GetPropertyValue(valueProperty);
            var maxValue = DecimalUnityEditorDraw.GetPropertyValue(maxValueProperty);

            //var value = uvalue.ToDec();
            //var maxValue = umaxValue.ToDec();

            var newValue = DecimalUnityEditorDraw.DrawDecimalValue(posValue, value, null);
            var newMax = DecimalUnityEditorDraw.DrawDecimalValue(posMaxValue, maxValue, null);

            if (value != newValue)
            {
                //修改值
                //使其不超过最大值
                if (newValue > newMax)
                {
                    newMax = newValue;
                }
            }

            if (maxValue != newMax)
            {
                //修改最大值
                if (newMax < newValue)
                {
                    //设置值在范围内
                    newValue = newMax;
                }
            }

            DecimalUnityEditorDraw.SetPropertyValue(valueProperty, newValue);
            DecimalUnityEditorDraw.SetPropertyValue(maxValueProperty, newMax);

        }

    }

    /// <summary>
    /// 容器结构 Unity GUI 绘制重写
    /// </summary>
    [CustomPropertyDrawer(typeof(ReceptacleValue<>))]
    public sealed class ReceptacleUnityEditorDraw : PropertyDrawer
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
            var fieldInfo = this.fieldInfo;
            var type = fieldInfo.FieldType;

            var tip = Tooltip;
            if(tip != null)
            {
                label.tooltip = tip.tooltip;
            }

            OnGUIDraw(position, property, label, p_fenLabel, type);

        }

        /// <summary>
        /// 封装绘制函数
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="property">容器字段数据</param>
        /// <param name="label">主标签</param>
        /// <param name="trimLabel">值和最大值的分隔符标签</param>
        /// <param name="fieldValueType">字段泛型类型</param>
        public static void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label, GUIContent trimLabel, Type fieldValueType)
        {

            Rect pos_label, pos_value;

            if(label == null || label == GUIContent.none)
            {
                pos_value = position;
            }
            else
            {
                //绘绘标签割割域
                position.SectionLength(0.5f, 0, out pos_label, out pos_value);
                EditorGUI.LabelField(pos_label, label);
            }

            OnGUIDrawValue(pos_value, property, trimLabel, fieldValueType);

        }


        /// <summary>
        /// 绘制<see cref="ReceptacleValue{T}"/>字段
        /// </summary>
        /// <param name="position">字段位置</param>
        /// <param name="property">字段值</param>
        /// <param name="trimLabel">分隔符标签</param>
        /// <param name="fieldValueType">字段泛型类型</param>
        public static void OnGUIDrawValue(Rect position, SerializedProperty property, GUIContent trimLabel, Type fieldValueType)
        {

            //获取流存储资源引用
            var pro_value = property.FindPropertyRelative(valueName);
            var pro_maxValue = property.FindPropertyRelative(maxValueName);

            //获取字段显示label
            //var gui_Label = createLabel(label);
            //GUI位置显示
            //Rect rect_label = new Rect(pos_x, pos_y, pos_guiLabelWidth, pos_height);

            //position.SectionLength(0.5f, 0, out rect_label, out Rect valueAll);

            Rect rect_value, rect_fenLabel, rect_maxValue;

            int trimTextLgn = trimLabel.text.Length;
            //分隔符长度
            float fenLen = trimTextLgn * 10;

            //切割两半作为值显示
            position.SectionLength(0.5f, fenLen + 1, out rect_value, out rect_maxValue);
            //中间用作分隔符

            rect_fenLabel = position.ShortenToLengthByCenter(fenLen);

            //valueAll.SectionLength(0.5f, 20, out rect_value, out rect_maxValue);
            //valueAll.ShortenLength(,);

            //分隔符绘制
            if(trimTextLgn != 0) EditorGUI.LabelField(rect_fenLabel, trimLabel);

            //总标签绘制
            //EditorGUI.LabelField(rect_label, label);

            if (typeof(ReceptacleValue<int>) == fieldValueType)
            {
                ReceptacleValueDrawing.DrawingInt(rect_value, rect_maxValue, pro_value, pro_maxValue);
            }
            else if (typeof(ReceptacleValue<long>) == fieldValueType)
            {
                ReceptacleValueDrawing.DrawingLong(rect_value, rect_maxValue, pro_value, pro_maxValue);
            }
            else if (typeof(ReceptacleValue<float>) == fieldValueType)
            {
                ReceptacleValueDrawing.DrawingFloat(rect_value, rect_maxValue, pro_value, pro_maxValue);
            }
            else if (typeof(ReceptacleValue<double>) == fieldValueType)
            {
                ReceptacleValueDrawing.DrawingDouble(rect_value, rect_maxValue, pro_value, pro_maxValue);
            }
            else if (typeof(ReceptacleValue<UDecimal>) == fieldValueType)
            {
                ReceptacleValueDrawing.DrawingUDecimal(rect_value, rect_maxValue, pro_value, pro_maxValue);
            }
            else if (typeof(ReceptacleValue<short>) == fieldValueType || typeof(ReceptacleValue<byte>) == fieldValueType)
            {
                ReceptacleValueDrawing.DrawingInt(rect_value, rect_maxValue, pro_value, pro_maxValue);
            }
            else
            {
                //value绘制
                //EditorGUI.PropertyField(rect_value, pro_value);
                ////maxValue绘制
                //EditorGUI.PropertyField(rect_maxValue, pro_maxValue);
                var defc = GUI.color;
                var defCont = GUI.contentColor;
                GUI.contentColor = Color.yellow;
                var temp = EditorGUIParser.NotTypeDrawingError;
                //temp.text = "ERROR";
                //temp.tooltip = "不是已知可绘制类";
                EditorGUI.LabelField(position, temp);
                GUI.color = defc;
                GUI.contentColor = defCont;
                //EditorGUI.LabelField(position, NotRecTypeError);

            }

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