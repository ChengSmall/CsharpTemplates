#if UNITY_EDITOR

using System;

using UnityEngine;

using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using Cheng.Unitys.Editors;

namespace Cheng.Unitys.DataStructure.Editors
{

    /// <summary>
    /// 绘制<see cref="decimal"/>类型的十进制数到GUI的重绘脚本
    /// </summary>
    [CustomPropertyDrawer(typeof(UDecimal))]
    public sealed class DecimalUnityEditorDraw : PropertyDrawer
    {

        #region 绘制

        #region 构造

        public DecimalUnityEditorDraw()
        {
            p_isGettip = false;
            p_tip = null;
        }

        #endregion

        #region 参数

        private TooltipAttribute p_tip;
        private bool p_isGettip;
        private TooltipAttribute Tooltip
        {
            get
            {
                if (p_isGettip) return p_tip;
                p_isGettip = true;
                p_tip = this.fieldInfo.GetCustomAttribute<TooltipAttribute>();
                return p_tip;
            }
        }

        #endregion

        #region 派生

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var t = Tooltip;
            if (t != null) label.tooltip = t.tooltip;

            OnGUIDraw(position, property, label);
        }

        #endregion

        #region 封装绘制函数

        /// <summary>
        /// 获得高度
        /// </summary>
        /// <param name="property">字段存储信息</param>
        /// <param name="label">标签</param>
        /// <returns>此条GUI高度</returns>
        public static float GetHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIParser.AllFieldHeight;
        }

        /// <summary>
        /// 封装绘制函数
        /// </summary>
        /// <param name="position">绘制位置</param>
        /// <param name="property">字段储存信息</param>
        /// <param name="label">标签</param>
        public static void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label)
        {

            UDecimal ud;

            try
            {
                ud = GetPropertyValue(property);
                var nud = DrawDecimalValue(position, ud, label);
                if(nud != ud)
                {
                    SetPropertyValue(property, nud);
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder(32);
                sb.AppendLine("Decimal对象绘制失效");
                sb.Append("错误信息:");
                sb.AppendLine(ex.Message);
                sb.Append("错误类型:");
                sb.AppendLine(ex.GetType().FullName);
                sb.AppendLine("堆栈跟踪:");
                sb.Append(ex.StackTrace);
                Debug.LogWarning(sb.ToString());
            }
        }

        /// <summary>
        /// 绘制<see cref="UDecimal"/>，并将可能修改后的值返回
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="property">对象信息</param>
        /// <param name="label">标签；null或空标签表示不绘制标签</param>
        /// <returns>绘制后的值</returns>
        public static UDecimal OnGUIDrawValue(Rect position, SerializedProperty property, GUIContent label)
        {

            var pro_1 = property.FindPropertyRelative(UDecimal.fieldName_i1);
            var pro_2 = property.FindPropertyRelative(UDecimal.fieldName_i2);
            var pro_3 = property.FindPropertyRelative(UDecimal.fieldName_i3);
            var pro_4 = property.FindPropertyRelative(UDecimal.fieldName_i4);

            var v1 = pro_1.intValue;
            var v2 = pro_2.intValue;
            var v3 = pro_3.intValue;
            var v4 = pro_4.intValue;

            //初始化数据
            UDecimal ud = new UDecimal(v1, v2, v3, v4);

            return DrawDecimalValue(position, ud, label);

        }

        /// <summary>
        /// 获取<see cref="UDecimal"/>字段信息的值
        /// </summary>
        /// <param name="property">字段信息</param>
        /// <returns>获取的值</returns>
        public static UDecimal GetPropertyValue(SerializedProperty property)
        {

            var pro_1 = property.FindPropertyRelative(UDecimal.fieldName_i1);
            var pro_2 = property.FindPropertyRelative(UDecimal.fieldName_i2);
            var pro_3 = property.FindPropertyRelative(UDecimal.fieldName_i3);
            var pro_4 = property.FindPropertyRelative(UDecimal.fieldName_i4);

            var v1 = pro_1.intValue;
            var v2 = pro_2.intValue;
            var v3 = pro_3.intValue;
            var v4 = pro_4.intValue;

            return new UDecimal(v1, v2, v3, v4);
        }

        /// <summary>
        /// 设置<see cref="UDecimal"/>字段信息的值
        /// </summary>
        /// <param name="property">字段信息</param>
        /// <param name="value">要设置的值</param>
        public static void SetPropertyValue(SerializedProperty property, UDecimal value)
        {
            var pro_1 = property.FindPropertyRelative(UDecimal.fieldName_i1);
            var pro_2 = property.FindPropertyRelative(UDecimal.fieldName_i2);
            var pro_3 = property.FindPropertyRelative(UDecimal.fieldName_i3);
            var pro_4 = property.FindPropertyRelative(UDecimal.fieldName_i4);

            pro_1.intValue = value.i1;
            pro_2.intValue = value.i2;
            pro_3.intValue = value.i3;
            pro_4.intValue = value.i4;

        }

        /// <summary>
        /// 绘制一个 <see cref="UDecimal"/>
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="value">值</param>
        /// <param name="label">标签；null或空标签表示不绘制</param>
        /// <returns>可能修改后的值</returns>
        public static UDecimal DrawDecimalValue(Rect position, UDecimal value, GUIContent label)
        {
            var f = decimal.ToDouble(value.ToDec());
            if (label is null || GUIContent.none == label)
            {
                f = EditorGUI.DelayedDoubleField(position, f);
            }
            else
            {
                f = EditorGUI.DelayedDoubleField(position, label, f);
            }
          
            return new UDecimal(new decimal(f));
        }

        #endregion

        #endregion

    }

}

#endif
