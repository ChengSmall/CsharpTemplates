#if UNITY_EDITOR

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cheng.Unitys.Editors;
using System.Text;

namespace Cheng.DataStructure.Collections.Unitys.UnityEditors
{

    /// <summary>
    /// 为<see cref="UList{T}"/>集合绘制 Editor GUI
    /// </summary>
    [CustomPropertyDrawer(typeof(UList<>))]
    public class UListGUIEditorDraw : PropertyDrawer
    {

        #region

        #region 构造

        public UListGUIEditorDraw()
        {
            p_isGettip = false;
            p_open = false;
        }

        #endregion

        #region 参数

        private TooltipAttribute p_tip;
        private bool p_open;
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

        #endregion

        #region 派生

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetHeight(property, label, p_open, EditorGUI.GetPropertyHeight);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = createLabel(label);

            p_open = OnGUIDraw(position, property, label, p_open, "item ", Convert.ToString, defDrawItem, EditorGUI.GetPropertyHeight);
        }

        static bool defDrawItem(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label);
            return false;
        }

        #endregion

        #region 封装

        /// <summary>
        /// 返回<see cref="UList{T}"/>的GUI的高度
        /// </summary>
        /// <param name="property">集合字段信息</param>
        /// <param name="label">字段的GUI标签</param>
        /// <param name="open">集合是否展开</param>
        /// <param name="getHeightFunc">集合内每个元素的高度获取方法</param>
        /// <returns>集合高度</returns>
        public static float GetHeight(SerializedProperty property, GUIContent label, bool open, Func<SerializedProperty, float> getHeightFunc)
        {
            float height = EditorGUIParser.AllFieldHeight;

            if (open)
            {
                height += EditorGUIParser.AllFieldHeight;

                var pro_arr = property.FindPropertyRelative(UList<int>.fieldName_array);
                var pro_count = property.FindPropertyRelative(UList<int>.fieldName_count);

                int count = pro_count.intValue;
                for (int i = 0; i < count; i++)
                {
                    var pro_arrItem = pro_arr.GetArrayElementAtIndex(i);
                    height += (getHeightFunc.Invoke(pro_arrItem) + EditorGUIParser.HeightInterval);
                }

            }

            return height;
        }

        /// <summary>
        /// 在 GUI 上绘制<see cref="UList{T}"/>集合
        /// </summary>
        /// <param name="position">要绘制的区域</param>
        /// <param name="property">集合字段的GUI信息</param>
        /// <param name="label">集合字段的主标签</param>        
        /// <param name="open">是否展开</param>
        /// <param name="countNamePrefix">集合内每个元素的标签前缀</param>
        /// <param name="numToString">一个将<see cref="int"/>转化为<see cref="string"/>的方法，用于绘制集合内每个元素的标签枚举数列</param>
        /// <param name="itemDrawFunc">集合内每个元素的绘制方法，第一个参数表示元素的绘制区域，第二个参数表示元素字段的GUI信息，第三个参数表示该字段的显示标签，返回值表示该值是否变动，有变动返回true，没有变动返回false</param>
        /// <param name="getHeightFunc">集合内每个元素的高度获取方法，参数为集合元素的字段信息，返回值将要绘制的字段的GUI高度</param>
        /// <returns>是否展开</returns>
        public static bool OnGUIDraw(Rect position, SerializedProperty property, GUIContent label, bool open, string countNamePrefix, Func<int, string> numToString, Func<Rect, SerializedProperty, GUIContent, bool> itemDrawFunc, Func<SerializedProperty, float> getHeightFunc)
        {
            //EditorGUI.LabelField(position, new GUIContent("暂无可用的GUI实现", ""));
            //return open;

            var pos_label = position.SetHeightFromTop(EditorGUIParser.OnceHeight);
            
            open = EditorGUI.Foldout(pos_label, open, label, true);
            try
            {
                if (open)
                {
                    //获取参数
                    var pro_arr = property.FindPropertyRelative(UList<int>.fieldName_array);
                    var pro_count = property.FindPropertyRelative(UList<int>.fieldName_count);
                    var pro_changeCount = property.FindPropertyRelative(UList<int>.fieldName_changeCount);
                    bool change = false;

                    //元素数量
                    int count = pro_count.intValue;

                    Rect pos_Count = pos_label.MoveHeight(EditorGUIParser.AllFieldHeight).ShortenLengthFormLeft(5f);

                    var newCount = EditorGUI.DelayedIntField(pos_Count, "size", count);

                    int lasc = Mathf.Min(newCount, count);

                    if (newCount != count)
                    {
                        change = true;
                    }

                    Rect pos_item = pos_Count.MoveHeight(EditorGUIParser.AllFieldHeight);
                    //修改数组容量
                    pro_arr.arraySize = newCount;

                    for (int i = 0; i < lasc; i++)
                    {
                        //遍历每一个元素绘制
                        var pro_arrItem = pro_arr.GetArrayElementAtIndex(i);

                        var itmHeight = getHeightFunc.Invoke(pro_arrItem);
                        pos_item = pos_item.SetHeightFromTop(itmHeight);

                        if (itemDrawFunc.Invoke(pos_item, pro_arrItem, new GUIContent(countNamePrefix + numToString.Invoke(i))))
                        {
                            change = true;
                        }
                        //下移绘制区域                       

                        pos_item = pos_item.MoveHeight(itmHeight + EditorGUIParser.HeightInterval);                        
                                                
                    }

                    //设置新的元素数
                    pro_count.intValue = pro_arr.arraySize;

                    if (change)
                    {
                        //推进变化值
                        pro_changeCount.intValue += 1;
                    }

                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("UList GUI错误:");
                sb.AppendLine(ex.Message);
                sb.Append("异常类型:");
                sb.AppendLine(ex.GetType().FullName);
                sb.Append("异常堆栈:");
                sb.AppendLine(ex.StackTrace);
                Debug.LogError(sb.ToString());
                
            }
            

            return open;
        }

        #endregion

        #endregion

    }

}

#endif
