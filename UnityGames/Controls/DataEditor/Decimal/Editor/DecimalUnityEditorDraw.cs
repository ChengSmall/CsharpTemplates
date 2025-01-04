#if UNITY_EDITOR

using System;

using UnityEngine;

using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace Cheng.Unitys.Editors
{

    /// <summary>
    /// 绘制<see cref="decimal"/>类型的十进制数到GUI的重绘脚本
    /// </summary>
    [CustomPropertyDrawer(typeof(UDecimal))]
    public class DecimalUnityEditorDraw : PropertyDrawer
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

            var pro_1 = property.FindPropertyRelative("i1");
            var pro_2 = property.FindPropertyRelative("i2");
            var pro_3 = property.FindPropertyRelative("i3");
            var pro_4 = property.FindPropertyRelative("i4");

            var v1 = pro_1.intValue;
            var v2 = pro_2.intValue;
            var v3 = pro_3.intValue;
            var v4 = pro_4.intValue;

            //初始化数据
            UDecimal ud = new UDecimal(v1, v2, v3, v4);
            var dec = ud.ToDec();

            var f = decimal.ToDouble(dec);

            f = EditorGUI.DoubleField(position, label, f);

            ////返回数值字符串
            //var dec_str = dec.ToString();
            ////用字符串绘制值
            //dec_str = EditorGUI.DelayedTextField(position, label, dec_str);

            try
            {
                dec = new decimal(f);

                //有效则重写写入
                ud = new UDecimal(dec);
                ud.GetValue(out v1, out v2, out v3, out v4);
                pro_1.intValue = v1;
                pro_2.intValue = v2;
                pro_3.intValue = v3;
                pro_4.intValue = v4;
            }
            catch (Exception ex)
            {
                Debug.LogError("Decimal GUI 错误:" + ex.Message + Environment.NewLine + ex.GetType().FullName);
            }

            //解析用户输入
            //if(decimal.TryParse(dec_str, out dec))
            //{
            //    //有效则重写写入
            //    ud = new UDecimal(dec);
            //    ud.GetValue(out v1, out v2, out v3, out v4);
            //    pro_1.intValue = v1;
            //    pro_2.intValue = v2;
            //    pro_3.intValue = v3;
            //    pro_4.intValue = v4;
            //}

        }

        #endregion

        #endregion

    }

}

#endif
