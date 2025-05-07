#if UNITY_EDITOR

using System;

using UnityEngine;

using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using Cheng.Unitys.Editors;

using UObj = UnityEngine.Object;

namespace Cheng.Unitys.DataStructure
{

    [CustomPropertyDrawer(typeof(Disabledable<>))]
    public sealed class DisabledableEditorDraw : PropertyDrawer
    {

        #region

        #region 构造

        public DisabledableEditorDraw()
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
            var tp = Tooltip;
            if (tp != null) label.tooltip = tp.tooltip;

            OnGUIDraw(position, property, label);
        }

        #endregion

        #region 绘制封装

        /// <summary>
        /// 获取高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public static float GetHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIParser.OnceHeight;
        }

        /// <summary>
        /// 对象绘制函数
        /// </summary>
        /// <param name="position">绘制区域</param>
        /// <param name="property">对象字段信息</param>
        /// <param name="label">标签；null或空标签表示没有</param>
        public static void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect pos_label, pos_value;

            if(label is null || label == GUIContent.none)
            {
                pos_value = position;
            }
            else
            {
                position.SectionLength(0.5f, 0, out pos_label, out pos_value);
                EditorGUI.LabelField(pos_label, label);
            }

            var pro_obj = property.FindPropertyRelative(Disabledable<UObj>.fieldName_object);
            var pro_active = property.FindPropertyRelative(Disabledable<UObj>.fieldName_activeBoolean);

            Rect pos_value_active, pos_value_obj;

            pos_value.SectionLengthByValue(pos_value.width - (EditorGUIParser.ToggleWidth), 2,
                out pos_value_obj, out pos_value_active);

            bool active = EditorGUI.Toggle(pos_value_active, pro_active.boolValue);

            if (active)
            {
                //var obj = pro_obj.objectReferenceValue;
                EditorGUI.ObjectField(pos_value_obj, pro_obj);
                //pro_obj.objectReferenceValue = obj;
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.ObjectField(pos_value_obj, pro_obj);
                EditorGUI.EndDisabledGroup();
            }

            pro_active.boolValue = active;
        }

        #endregion

        #endregion

    }

}
#endif