#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Collections;

using UnityEngine;
using Cheng.ButtonTemplates.Joysticks.Unitys;
using Cheng.ButtonTemplates.Joysticks;
using Cheng.ButtonTemplates.UnityButtons;
using UnityEditor;
using System.Reflection;
using Cheng.Unitys.Editors;
using Cheng.ButtonTemplates.UnityButtons.UnityEditors;
using Cheng.ButtonTemplates.Joysticks.Unitys.UnityEditors;

namespace Cheng.Controllers.Unitys.UnityEditors
{

    /// <summary>
    /// Unity XBox 手柄 <see cref="UnityXBoxGameController"/> GUI 重绘
    /// </summary>
    [CustomPropertyDrawer(typeof(UnityXBoxGameController))]
    public class UnityXBoxGameControllerEditorDraw : PropertyDrawer
    {

        #region 绘制

        #region 初始化

        public UnityXBoxGameControllerEditorDraw()
        {
            p_par = DefaultInitParter();
            p_labels = DefaultInitLabels();
            p_par.open = sp_open;
            p_getTooltip = false;
            p_tip = null;
        }

        /// <summary>
        /// 返回默认的GUI额外参数初始值
        /// </summary>
        /// <returns>默认的GUI额外参数初始值</returns>
        public static OnGUIParter DefaultInitParter()
        {
            OnGUIParter parter;
            parter.open = true;
            parter.joy1_open = false;
            parter.joy2_open = false;
            parter.joy3_open = false;
            parter.button_keyCode_open = false;
            parter.button_LZ_open = false;
            parter.button_RZ_open = false;
            return parter;
        }

        public static LabelGUIParter DefaultInitLabels()
        {
            LabelGUIParter p;
            p.label_joy1 = new GUIContent("主摇杆");
            p.label_joy2 = new GUIContent("副摇杆");
            p.label_joy3 = new GUIContent("十字键");
            p.label_LZ = new GUIContent("左肩键");
            p.label_RZ = new GUIContent("右肩键");
            p.label_Keys = new GUIContent("按键集");

            p.label_A = new GUIContent("A");
            p.label_B = new GUIContent("B");
            p.label_X = new GUIContent("X");
            p.label_Y = new GUIContent("Y");

            p.label_L = new GUIContent("L");
            p.label_R = new GUIContent("R");

            p.label_LeftMenu = new GUIContent("Selet");
            p.label_RightMenu = new GUIContent("Menu");

            p.label_joy1Down = new GUIContent("主摇杆按压");
            p.label_joy2Down = new GUIContent("副摇杆按压");

            return p;
        }

        #endregion

        #region 参数

        private static bool sp_open = true;

        private LabelGUIParter p_labels;

        private OnGUIParter p_par;

        private TooltipAttribute p_tip;
        private bool p_getTooltip;
        private TooltipAttribute f_getTooltip()
        {
            if (p_getTooltip) return p_tip;
            p_tip = this.fieldInfo.GetCustomAttribute<TooltipAttribute>();
            p_getTooltip = true;
            return p_tip;
        }

        #endregion

        #region 派生

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var t = f_getTooltip();

            if (t != null) label.tooltip = t.tooltip;

            p_par = OnGUIDraw(position, property, label, p_labels, p_par);
            sp_open = p_par.open;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetHeight(property, label, p_par);
        }

        #endregion

        #region 封装

        /// <summary>
        /// OnGUI 收展参数
        /// </summary>
        public struct OnGUIParter
        {
            /// <summary>
            /// 字段是否展开
            /// </summary>
            public bool open;

            /// <summary>
            /// 主摇杆展开
            /// </summary>
            public bool joy1_open;

            /// <summary>
            /// 副摇杆展开
            /// </summary>
            public bool joy2_open;

            /// <summary>
            /// 十字键摇杆展开
            /// </summary>
            public bool joy3_open;

            /// <summary>
            /// 左按压式肩键展开
            /// </summary>
            public bool button_LZ_open;

            /// <summary>
            /// 右按压式肩键展开
            /// </summary>
            public bool button_RZ_open;

            /// <summary>
            /// KeyCode按键集合是否展开
            /// </summary>
            public bool button_keyCode_open;

        }

        /// <summary>
        /// OnGUI 子节点标签参数
        /// </summary>
        public struct LabelGUIParter
        {

            /// <summary>
            /// 主摇杆标签
            /// </summary>
            public GUIContent label_joy1;

            /// <summary>
            /// 副摇杆标签
            /// </summary>
            public GUIContent label_joy2;

            /// <summary>
            /// 十字摇杆标签
            /// </summary>
            public GUIContent label_joy3;

            /// <summary>
            /// 左侧按压肩键名称字段标签
            /// </summary>
            public GUIContent label_LZ;

            /// <summary>
            /// 右侧按压肩键名称字段标签
            /// </summary>
            public GUIContent label_RZ;

            /// <summary>
            /// 按键集合标签
            /// </summary>
            public GUIContent label_Keys;

            #region

            /// <summary>
            /// 手柄按钮 A 标签
            /// </summary>
            public GUIContent label_A;

            /// <summary>
            /// 手柄按钮 B 标签
            /// </summary>
            public GUIContent label_B;

            /// <summary>
            /// 手柄按钮 X 标签
            /// </summary>
            public GUIContent label_X;

            /// <summary>
            /// 手柄按钮 Y 标签
            /// </summary>
            public GUIContent label_Y;

            /// <summary>
            /// 左肩键
            /// </summary>
            public GUIContent label_L;

            /// <summary>
            /// 右肩键
            /// </summary>
            public GUIContent label_R;

            /// <summary>
            /// 左侧菜单键
            /// </summary>
            public GUIContent label_LeftMenu;

            /// <summary>
            /// 右侧菜单键
            /// </summary>
            public GUIContent label_RightMenu;

            /// <summary>
            /// 主摇杆按压按键标签
            /// </summary>
            public GUIContent label_joy1Down;

            /// <summary>
            /// 副摇杆按压按键标签
            /// </summary>
            public GUIContent label_joy2Down;

            #endregion

        }

        /// <summary>
        /// 获取<see cref="UnityXBoxGameController"/>字段GUI高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <param name="parter">收展状态</param>
        /// <returns></returns>
        public static float GetHeight(SerializedProperty property, GUIContent label, OnGUIParter parter)
        {
            //非展开
            if (!parter.open)
            {
                return EditorGUIParser.AllFieldHeight;
            }

            //展开

            float height;

            //一个标签字段+间隔
            height = EditorGUIParser.AllFieldHeight;

            //累加一个三个摇杆的高度
            height += UnityEditorAxisDraw.GetHeight(parter.joy1_open) + UnityEditorAxisDraw.GetHeight(parter.joy2_open) + UnityEditorAxisDraw.GetHeight(parter.joy3_open);

            //左右肩键判断并累加高度
            if (parter.button_LZ_open)
            {
                height += UnityAxisButtonEditorDraw.GetHeight() + EditorGUIParser.AllFieldHeight;
            }
            else
            {
                height += EditorGUIParser.AllFieldHeight;
            }

            if (parter.button_RZ_open)
            {
                height += UnityAxisButtonEditorDraw.GetHeight() + EditorGUIParser.AllFieldHeight;
            }
            else
            {
                height += EditorGUIParser.AllFieldHeight;
            }

            //判断按键集并累加高度

            if (parter.button_keyCode_open)
            {
                //一个标签+10个字段
                height += (EditorGUIParser.AllFieldHeight * 11);
            }
            else
            {
                //1个标签
                height += EditorGUIParser.AllFieldHeight;
            }

            return height;
        }

        /// <summary>
        /// 绘制<see cref="UnityXBoxGameController"/>字段GUI
        /// </summary>
        /// <param name="position">GUI绘制区域</param>
        /// <param name="property">字段存储信息</param>
        /// <param name="label">字段标签</param>
        /// <param name="labels">GUI子节点标签</param>
        /// <param name="parter">GUI收展状态</param>
        /// <returns>GUI收展状态</returns>
        public static OnGUIParter OnGUIDraw(Rect position, SerializedProperty property, GUIContent label, LabelGUIParter labels, OnGUIParter parter)
        {

            #region

            #region 参数获取

            SerializedProperty pro_joy1, pro_joy2, pro_joy3;
            SerializedProperty pro_A, pro_B, pro_X, pro_Y, pro_L, pro_LZ, pro_R, pro_RZ;
            SerializedProperty pro_leftMenu, pro_rightMenu;
            SerializedProperty pro_joy1Down, pro_joy2Down;

            pro_joy1 = property.FindPropertyRelative(UnityXBoxGameController.cp_mainJoystick_Name);

            pro_joy2 = property.FindPropertyRelative(UnityXBoxGameController.cp_deputyJoystick_Name);

            pro_joy3 = property.FindPropertyRelative(UnityXBoxGameController.cp_crossJoystick_Name);

            pro_A = property.FindPropertyRelative(UnityXBoxGameController.cp_B_A_Name);

            pro_B = property.FindPropertyRelative(UnityXBoxGameController.cp_B_B_Name);

            pro_X = property.FindPropertyRelative(UnityXBoxGameController.cp_B_X_Name);

            pro_Y = property.FindPropertyRelative(UnityXBoxGameController.cp_B_Y_Name);

            pro_L = property.FindPropertyRelative(UnityXBoxGameController.cp_B_L_Name);

            pro_R = property.FindPropertyRelative(UnityXBoxGameController.cp_B_R_Name);

            pro_LZ = property.FindPropertyRelative(UnityXBoxGameController.cp_B_LZ_Name);

            pro_RZ = property.FindPropertyRelative(UnityXBoxGameController.cp_B_RZ_Name);

            pro_leftMenu = property.FindPropertyRelative(UnityXBoxGameController.cp_B_LeftMenu_Name);

            pro_rightMenu = property.FindPropertyRelative(UnityXBoxGameController.cp_B_RightMenu_Name);

            pro_joy1Down = property.FindPropertyRelative(UnityXBoxGameController.cp_B_mainAxisDown_Name);

            pro_joy2Down = property.FindPropertyRelative(UnityXBoxGameController.cp_B_qeAxisDown_Name);

            #endregion

            #region 位置

            Rect pos_mainLabel;

            Rect pos_joy1, pos_joy2, pos_joy3;
            Rect pos_A, pos_B, pos_X, pos_Y, pos_L, pos_LZ, pos_R, pos_RZ;
            Rect pos_leftMenu, pos_rightMenu;
            Rect pos_joy1Down, pos_joy2Down;

            //Rect pos_joy1_label, pos_joy2_label, pos_joy3_label;
            //Rect pos_A_label, pos_B_label, pos_X_label, pos_Y_label,
            //Rect pos_L_label, pos_R_label;
            Rect pos_LZ_label, pos_RZ_label;
            //Rect pos_leftMenu_label, pos_rightMenu_label;
            //Rect pos_joy1Down_label, pos_joy2Down_label;

            Rect pos_keys_label;

            pos_mainLabel = position.SetHeightFromTop(EditorGUIParser.OnceHeight);


            #endregion

            #region 绘制


            //LZ判断打开
            float Z_moveHeight;

            // UnityAxis 实例内部标签
            parter.open = EditorGUI.Foldout(pos_mainLabel, parter.open, label, true);


            if (parter.open)
            {

                var height_joy1 = UnityEditorAxisDraw.GetHeight(parter.joy1_open);
                var height_joy2 = UnityEditorAxisDraw.GetHeight(parter.joy2_open);
                var height_joy3 = UnityEditorAxisDraw.GetHeight(parter.joy3_open);

                //下移一个字段单位
                pos_joy1 = pos_mainLabel.MoveHeight(EditorGUIParser.AllFieldHeight);
                //设置高度
                pos_joy1 = pos_joy1.SetHeightFromTop(height_joy1);
                pos_joy1 = pos_joy1.ShortenLengthFormLeft(5);

                pos_joy2 = pos_joy1.MoveHeight(height_joy1);
                pos_joy2 = pos_joy2.SetHeightFromTop(height_joy2);

                pos_joy3 = pos_joy2.MoveHeight(height_joy2);
                pos_joy3 = pos_joy3.SetHeightFromTop(height_joy3);

                //设置LZ收放标签区域
                pos_LZ_label = pos_joy3.MoveHeight(height_joy3);
                pos_LZ_label = pos_LZ_label.SetHeightFromTop(EditorGUIParser.OnceHeight);


                var label_uAxis_her = new GUIContent("横轴");
                var label_uAxis_ver = new GUIContent("纵轴");

                //三个摇杆绘制            

                parter.joy1_open = UnityEditorAxisDraw.OnGUIDraw(
                    pos_joy1, pro_joy1, labels.label_joy1,
                    label_uAxis_her, label_uAxis_ver, parter.joy1_open);

                parter.joy2_open = UnityEditorAxisDraw.OnGUIDraw(
                    pos_joy2, pro_joy2, labels.label_joy2,
                    label_uAxis_her, label_uAxis_ver, parter.joy2_open);

                parter.joy3_open = UnityEditorAxisDraw.OnGUIDraw(
                    pos_joy3, pro_joy3, labels.label_joy3,
                    label_uAxis_her, label_uAxis_ver, parter.joy3_open);

                //两个肩键绘制

                GUIContent label_Z_nameField;
                label_Z_nameField = new GUIContent("按键名称");


                parter.button_LZ_open = EditorGUI.Foldout(pos_LZ_label, parter.button_LZ_open, labels.label_LZ, true);

                if (parter.button_LZ_open)
                {
                    //移动一个字段单位
                    pos_LZ = pos_LZ_label.MoveHeight(EditorGUIParser.AllFieldHeight);
                    //设置字段高度
                    Z_moveHeight = UnityAxisButtonEditorDraw.GetHeight();
                    pos_LZ = pos_LZ.SetHeightFromTop(Z_moveHeight);
                }
                else
                {
                    //一个标签隔断
                    Z_moveHeight = EditorGUIParser.AllFieldHeight;
                    pos_LZ = pos_LZ_label;
                }

                if (parter.button_LZ_open)
                {
                    UnityAxisButtonEditorDraw.OnGUIDraw(pos_LZ, pro_LZ, label_Z_nameField);
                }

                //下移到RZ位置
                pos_RZ_label = pos_LZ.MoveHeight(Z_moveHeight).SetHeightFromTop(EditorGUIParser.OnceHeight);


                parter.button_RZ_open = EditorGUI.Foldout(pos_RZ_label, parter.button_RZ_open, labels.label_RZ, true);

                if (parter.button_RZ_open)
                {
                    //移动一个字段单位
                    pos_RZ = pos_RZ_label.MoveHeight(EditorGUIParser.AllFieldHeight);
                    //设置字段高度
                    Z_moveHeight = UnityAxisButtonEditorDraw.GetHeight();
                    pos_RZ = pos_RZ.SetHeightFromTop(Z_moveHeight);
                }
                else
                {
                    Z_moveHeight = EditorGUIParser.AllFieldHeight;
                    pos_RZ = pos_RZ_label;
                }


                if (parter.button_RZ_open)
                {
                    UnityAxisButtonEditorDraw.OnGUIDraw(pos_RZ, pro_RZ, label_Z_nameField);
                }

                //移动到KeyCode集
                pos_keys_label = pos_RZ.MoveHeight(Z_moveHeight).SetHeightFromTop(EditorGUIParser.OnceHeight);


                // 按键集
                parter.button_keyCode_open = EditorGUI.Foldout(pos_keys_label, parter.button_keyCode_open, labels.label_Keys, true);

                if (parter.button_keyCode_open)
                {
                    pos_A = pos_keys_label.MoveHeight(EditorGUIParser.AllFieldHeight).SetHeightFromTop(EditorGUIParser.AllFieldHeight);

                    pos_B = pos_A.MoveHeight(EditorGUIParser.AllFieldHeight);
                    pos_X = pos_B.MoveHeight(EditorGUIParser.AllFieldHeight);
                    pos_Y = pos_X.MoveHeight(EditorGUIParser.AllFieldHeight);
                    pos_L = pos_Y.MoveHeight(EditorGUIParser.AllFieldHeight);
                    pos_R = pos_L.MoveHeight(EditorGUIParser.AllFieldHeight);
                    pos_joy1Down = pos_R.MoveHeight(EditorGUIParser.AllFieldHeight);
                    pos_joy2Down = pos_joy1Down.MoveHeight(EditorGUIParser.AllFieldHeight);
                    pos_leftMenu = pos_joy2Down.MoveHeight(EditorGUIParser.AllFieldHeight);
                    pos_rightMenu = pos_leftMenu.MoveHeight(EditorGUIParser.AllFieldHeight);

                }
                else
                {
                    pos_A = default;
                    pos_B = default;
                    pos_X = default;
                    pos_Y = default;
                    pos_L = default;
                    pos_R = default;
                    pos_leftMenu = default;
                    pos_rightMenu = default;
                    pos_joy1Down = default;
                    pos_joy2Down = default;

                }


                if (parter.button_keyCode_open)
                {

                    UnityEditorKeyCodeButtonDraw.OnGUIDraw(pos_A, pro_A, labels.label_A);
                    UnityEditorKeyCodeButtonDraw.OnGUIDraw(pos_B, pro_B, labels.label_B);
                    UnityEditorKeyCodeButtonDraw.OnGUIDraw(pos_X, pro_X, labels.label_X);
                    UnityEditorKeyCodeButtonDraw.OnGUIDraw(pos_Y, pro_Y, labels.label_Y);

                    UnityEditorKeyCodeButtonDraw.OnGUIDraw(pos_L, pro_L, labels.label_L);
                    UnityEditorKeyCodeButtonDraw.OnGUIDraw(pos_R, pro_R, labels.label_R);

                    UnityEditorKeyCodeButtonDraw.OnGUIDraw(pos_joy1Down, pro_joy1Down, labels.label_joy1Down);
                    UnityEditorKeyCodeButtonDraw.OnGUIDraw(pos_joy2Down, pro_joy2Down, labels.label_joy2Down);

                    UnityEditorKeyCodeButtonDraw.OnGUIDraw(pos_leftMenu, pro_leftMenu, labels.label_LeftMenu);
                    UnityEditorKeyCodeButtonDraw.OnGUIDraw(pos_rightMenu, pro_rightMenu, labels.label_RightMenu);

                }

            }


            #endregion

            return parter;

            #endregion

        }

        #endregion

        #endregion

    }

}


#endif