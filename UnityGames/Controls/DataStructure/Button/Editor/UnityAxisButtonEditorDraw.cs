#if UNITY_EDITOR

using Cheng.Unitys.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Cheng.ButtonTemplates.UnityButtons.UnityEditors
{

    /// <summary>
    /// <see cref="UnityAxisButton"/>GUI重绘
    /// </summary>
    [CustomPropertyDrawer(typeof(UnityAxisButton))]
    public class UnityAxisButtonEditorDraw : PropertyDrawer
    {

        #region

        #region 初始化

        public UnityAxisButtonEditorDraw()
        {
            p_getting = false;
            p_t = null;
        }

        #endregion

        #region 参数

        private TooltipAttribute p_t;

        private bool p_getting;

        private TooltipAttribute Tooltip
        {
            get
            {
                if (p_getting) return p_t;
                p_t = this.fieldInfo.GetCustomAttribute<TooltipAttribute>();
                p_getting = true;
                return p_t;
            }
        }

        #endregion

        #region 派生

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var t = Tooltip;
            if(t != null) label.tooltip = t.tooltip;

            OnGUIDraw(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetHeight();
        }

        #endregion

        #region 封装

        /// <summary>
        /// 获取<see cref="UnityAxisButton"/>字段的GUI高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public static float GetHeight()
        {
            return (EditorGUIParser.AllFieldHeight) * 2;
        }

        /// <summary>
        /// 封装绘制<see cref="UnityAxisButton"/>字段GUI方法
        /// </summary>
        /// <param name="position">字段绘制区域</param>
        /// <param name="property">字段存储信息</param>
        /// <param name="label">字段显示标签；null或空表示不绘制标签</param>
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

            OnGUIDrawing(pos_value, property);
        }

        /// <summary>
        /// 绘制<see cref="UnityAxisButton"/>字段
        /// </summary>
        /// <param name="position">字段绘制区域</param>
        /// <param name="property">字段存储信息</param>
        public static void OnGUIDrawing(Rect position, SerializedProperty property)
        {

            #region

            //虚拟按键名称字符串字段
            var pro_name = property.FindPropertyRelative(UnityAxisButton.cp_EditorProperityFieldButtonName);

            //虚拟轴Axis是否开启平滑布尔值字段
            var pro_smooth = property.FindPropertyRelative(UnityAxisButton.cp_EditorProperityFieldAxisToolName);

            //力度值映射到状态值的中间锚点浮点值字段
            var pro_mid = property.FindPropertyRelative(UnityAxisButton.cp_EditorMid_Name);

            #endregion

            #region
            //Rect pos_label;

            Rect pos_Name, pos_smooth, pos_mid;

            //float x = position.x, y = position.y;
            //标签区域
            //pos_label = default;

            Rect basePos = new Rect(position.x, position.y, position.width, EditorGUIParser.OnceHeight);

            pos_Name = basePos.ShortenLengthFormRight(EditorGUIParser.ToggleWidth + 3);


            pos_smooth = basePos.ShortenLengthFormLeft(pos_Name.width);

            //basePos = basePos.MoveHeight(EditorGUIParser.AllFieldHeight);

            pos_mid = basePos.MoveHeight(EditorGUIParser.AllFieldHeight);

            #endregion

            #region 绘制

            pro_name.stringValue = EditorGUI.DelayedTextField(pos_Name, pro_name.stringValue);

            //pro_name.stringValue = EditorGUI.DelayedTextField(pos_Name, pro_name.stringValue);

            pro_smooth.boolValue = EditorGUI.Toggle(pos_smooth, pro_smooth.boolValue);

            pro_mid.floatValue = EditorGUI.DelayedFloatField(pos_mid, "mid", pro_mid.floatValue);

            //EditorGUI.LabelField(pos_label, label);

            #endregion

        }

        #endregion

        #endregion

    }

}
#endif