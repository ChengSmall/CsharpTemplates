#if UNITY_EDITOR

using System;
using System.Text;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Cheng.Unitys.Editors;

namespace Cheng.Timers.Unitys.UnityEditors
{

    //UnityTimer

    /// <summary>
    /// 对<see cref="UnityTimer"/>重写GUI绘制
    /// </summary>
    [CustomPropertyDrawer(typeof(UnityTimer))]
    public class UnityTimerEditorDraw : PropertyDrawer
    {

        #region 初始化

        public UnityTimerEditorDraw()
        {
            p_isGettip = false;
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
            label.tooltip = Tooltip?.tooltip;

            OnGUIDraw(position, property, label);
        }

        #endregion

        #region 封装

        /// <summary>
        /// 返回GUI高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public static float GetHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIParser.OnceHeight;
        }

        /// <summary>
        /// 绘制<see cref="UnityTimer"/>GUI
        /// </summary>
        /// <param name="position">绘制区域</param>
        /// <param name="property">绘制的字段信息</param>
        /// <param name="label">主标签</param>
        public static void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label)
        {
            var t = property.FindPropertyRelative(UnityTimer.fieldName_type);
            
            t.intValue = (int)(UnityTimerType)EditorGUI.EnumPopup(position, label, (UnityTimerType)t.intValue);
        }

        #endregion

    }

}
#endif
