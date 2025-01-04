#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using UnityEditor;
using UnityEngine;

using Cheng.Unitys.Editors;

namespace Cheng.ButtonTemplates.Joysticks.Unitys.UnityEditors
{

    #region

    /// <summary>
    /// <see cref="UnityKeyCode4Joystick"/> 4键摇杆GUI重绘
    /// </summary>
    [CustomPropertyDrawer(typeof(UnityKeyCode4Joystick))]
    public class UnityKeyCode4JoystickEditorDraw : PropertyDrawer
    {

        #region 检查器重绘脚本

        #region 构造

        public UnityKeyCode4JoystickEditorDraw()
        {
            p_tipCreate = true;

            p_leftLabel = new GUIContent(cp_leftKeyLabelName);
            p_rightLabel = new GUIContent(cp_rightKeyLabelName);
            p_upLabel = new GUIContent(cp_upKeyLabelName);
            p_downLabel = new GUIContent(cp_downKeyLabelName);

            p_horRevLabel = new GUIContent(cp_horRevLabelName);
            p_verRevLabel = new GUIContent(cp_verRevLabelName);

            p_unfold = true;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 左键码标签
        /// </summary>
        private GUIContent p_leftLabel;

        /// <summary>
        /// 右键码标签
        /// </summary>
        private GUIContent p_rightLabel;

        /// <summary>
        /// 上键码标签
        /// </summary>
        private GUIContent p_upLabel;

        /// <summary>
        /// 下键码标签
        /// </summary>
        private GUIContent p_downLabel;

        /// <summary>
        /// 横轴反转开关标签
        /// </summary>
        private GUIContent p_horRevLabel;

        /// <summary>
        /// 纵轴反转开关标签
        /// </summary>
        private GUIContent p_verRevLabel;

        private TooltipAttribute p_tip;

        /// <summary>
        /// 特性捕获一次性开关
        /// </summary>
        private bool p_tipCreate;

        /// <summary>
        /// 脚本参数展开
        /// </summary>
        private bool p_unfold;

        /// <summary>
        /// 捕获特性扩展注释
        /// </summary>
        private TooltipAttribute Tooltip
        {
            get
            {
                if (p_tipCreate)
                {
                    p_tipCreate = false;
                    p_tip = this.fieldInfo.GetCustomAttribute<TooltipAttribute>();
                }
                return p_tip;
            }
        }

        #region 常量

        /// <summary>
        /// 默认值——左侧KeyCode按键标签名
        /// </summary>
        public const string cp_leftKeyLabelName = "左";

        /// <summary>
        /// 默认值——右侧KeyCode按键标签名
        /// </summary>
        public const string cp_rightKeyLabelName = "右";

        /// <summary>
        /// 默认值——上KeyCode按键标签名
        /// </summary>
        public const string cp_upKeyLabelName = "上";

        /// <summary>
        /// 默认值——下KeyCode按键标签名
        /// </summary>
        public const string cp_downKeyLabelName = "下";

        /// <summary>
        /// 默认值——横轴反转开关标签
        /// </summary>
        public const string cp_horRevLabelName = "横轴反转";

        /// <summary>
        /// 默认值——纵轴反转开关标签
        /// </summary>
        public const string cp_verRevLabelName = "纵轴反转";

        #endregion

        #endregion

        #region
        /*
        _______________________
        ↓[展开] fieldName:
             上:[ KeyCode ]
        左:[KeyCode] [KeyCode]:右
             下:[ KeyCode ]
        横轴反转:[ ]  纵轴反转:[ ]
                    ↑
                 中间分割
         */
        #endregion

        #region 重绘

        private GUIContent f_setLabel(GUIContent label)
        {
            var t = Tooltip;
            if (t != null) label.tooltip = t.tooltip;
            return label;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //关闭
            const float onH = (EditorGUIParser.OnceHeight + EditorGUIParser.HeightInterval);

            if (!p_unfold) return onH;

            //5个纵轴展开
            return onH * 5;
        }

        static int f_getStrToPixelLen(string str)
        {
            int length = str.Length;
            int re = 0;
            for (int i = 0; i < length; i++)
            {
                char c = str[i];
                if (c < 256)
                {
                    re += 14;
                }
                else
                {
                    re += 18;
                }
            }
            return re;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            label = f_setLabel(label);

            p_unfold = OnGUIDraw(position, property, label, p_unfold, p_leftLabel, p_rightLabel, p_upLabel, p_downLabel, p_horRevLabel, p_verRevLabel);

        }


        /// <summary>
        /// 计算<see cref="UnityKeyCode4Joystick"/>字段GUI高度
        /// </summary>
        /// <param name="open">是否为展开</param>
        /// <returns></returns>
        public static float GetHeight(bool open)
        {
            //关闭
            const float onH = (EditorGUIParser.OnceHeight + EditorGUIParser.HeightInterval);

            if (!open) return onH;

            //5个纵轴展开
            return onH * 5;
        }

        /// <summary>
        /// 封装绘制<see cref="UnityKeyCode4Joystick"/>字段GUI函数
        /// </summary>
        /// <param name="position">字段GUI位置</param>
        /// <param name="property">字段数据</param>
        /// <param name="label">字段GUI标签</param>
        /// <param name="open">是否展开</param>
        /// <param name="leftLabel">左侧按键标签</param>
        /// <param name="rightLabel">右侧按键标签</param>
        /// <param name="upLabel">上方按键标签</param>
        /// <param name="downLabel">下方按键标签</param>
        /// <param name="horRevLabel">横轴反转开挂标签</param>
        /// <param name="verRevLabel">纵轴反转开关标签</param>
        /// <returns>绘制完成后的展开情况</returns>
        public static bool OnGUIDraw(Rect position, SerializedProperty property, GUIContent label, bool open, GUIContent leftLabel, GUIContent rightLabel, GUIContent upLabel, GUIContent downLabel, GUIContent horRevLabel, GUIContent verRevLabel)
        {

            #region 获取字段

            SerializedProperty pro_left, pro_right, pro_up, pro_down;

            SerializedProperty pro_horRev, pro_verRev;

            pro_left = property.FindPropertyRelative(UnityKeyCode4Joystick.cp_leftFieldName);
            pro_right = property.FindPropertyRelative(UnityKeyCode4Joystick.cp_rightFieldName);
            pro_up = property.FindPropertyRelative(UnityKeyCode4Joystick.cp_upFieldName);
            pro_down = property.FindPropertyRelative(UnityKeyCode4Joystick.cp_downFieldName);

            pro_horRev = property.FindPropertyRelative(UnityKeyCode4Joystick.cp_horRevFieldName);
            pro_verRev = property.FindPropertyRelative(UnityKeyCode4Joystick.cp_verRevFieldName);

            #endregion

            #region 获取参数

            #region 调整位置信息

            const float compH = EditorGUIParser.HeightInterval + EditorGUIParser.OnceHeight;

            //float height = position.height;
            float width = position.width;

            const float och = EditorGUIParser.OnceHeight;

            Rect tepos;
            float x = position.x;
            //Debug.Log("height:" + height);

            //上下左右键码位置
            Rect pos_left, pos_right, pos_up, pos_down;
            //横轴和纵轴反转开关位置
            Rect pos_horRev, pos_verRev;

            //标签位置
            Rect pos_leftLabel, pos_rightLabel;
            Rect pos_upLabel, pos_downLabel;

            Rect pos_verRevLabel, pos_horRevLabel;

            Rect posBase = new Rect(x, position.y, width, och);

            tepos = new Rect(x, posBase.y + compH, width, och);
            //pos_up = tepos.ShortenLengthToScaleMid(0.2f, 0);
            //分割标签和字段

            tepos.SectionLength(0.2f, 0, out pos_upLabel, out pos_up);
            pos_up = pos_up.ShortenLengthFormRight(tepos.width * 0.2f);

            tepos = new Rect(x, tepos.y + compH, width, och);
            //分割中间
            tepos.SectionLength(0.5f, 3, out pos_left, out pos_right);
            //分割标签和字段
            pos_left.SectionLength(0.25f, 0, out pos_leftLabel, out pos_left);
            pos_right.SectionLength(0.775f, 10, out pos_right, out pos_rightLabel);

            tepos = new Rect(x, tepos.y + compH, width, och);
            //pos_down = tepos.ShortenLengthToScaleMid(0.2f, 0);

            tepos.SectionLength(0.25f, 0, out pos_downLabel, out pos_down);
            pos_down = pos_down.ShortenLengthFormRight(tepos.width * 0.2f);


            tepos = new Rect(x, tepos.y + compH, width, och);

            //左右切开
            tepos.SectionLength(0.5f, 5, out pos_horRev, out pos_verRev);

            //横轴开关和标签位置
            pos_horRev.SectionLength(0.8f, 0, out pos_horRevLabel, out pos_horRev);

            //var len = cp_horRevLabelName.Length;

            pos_horRevLabel = pos_horRevLabel.ShortenLengthFormLeft(pos_horRevLabel.width - (f_getStrToPixelLen(cp_horRevLabelName)));

            //纵轴开关和标签位置
            pos_verRev.SectionLength(0.2f, 0, out pos_verRev, out pos_verRevLabel);

            #endregion

            #endregion

            #region 绘制

            open = EditorGUI.Foldout(posBase, open, label, true);

            if (open)
            {
                EditorGUI.LabelField(pos_upLabel, upLabel);
                EditorGUI.PropertyField(pos_up, pro_up, GUIContent.none);

                EditorGUI.LabelField(pos_leftLabel, leftLabel);
                EditorGUI.LabelField(pos_rightLabel, rightLabel);
                EditorGUI.PropertyField(pos_left, pro_left, GUIContent.none);
                EditorGUI.PropertyField(pos_right, pro_right, GUIContent.none);

                EditorGUI.LabelField(pos_downLabel, downLabel);
                EditorGUI.PropertyField(pos_down, pro_down, GUIContent.none);

                EditorGUI.LabelField(pos_horRevLabel, horRevLabel);
                EditorGUI.LabelField(pos_verRevLabel, verRevLabel);

                pro_horRev.boolValue = EditorGUI.Toggle(pos_horRev, GUIContent.none, pro_horRev.boolValue);

                pro_verRev.boolValue = EditorGUI.Toggle(pos_verRev, GUIContent.none, pro_verRev.boolValue);
            }

            return open;

            #endregion

        }

        #endregion

        #endregion

    }

    #endregion

}

#endif