#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using UnityEditor;
using UnityEngine;

using Cheng.Unitys.Editors;

namespace Cheng.Unitys.Cameras.Editor
{

    [CustomPropertyDrawer(typeof(CameraTracking.TransObjs))]
    internal sealed class CameraTrackingEditorDraw_TransObjs : PropertyDrawer
    {

        #region

        #region 初始化

        public CameraTrackingEditorDraw_TransObjs()
        {
            p_titleLabel = new GUIContent("操作对象");
            p_fenlabel = new GUIContent("->", "左侧是摄像机变换，右侧是跟踪对象变换");

        }

        private GUIContent p_titleLabel;
        private GUIContent p_fenlabel;

        private GUIStyle p_fenStyle;

        private GUIStyle FenStyle
        {
            get
            {
                if(p_fenStyle is null)
                {
                    //创建自定义样式（基于默认Label样式）
                    p_fenStyle = new GUIStyle(EditorStyles.label)
                    {
                        alignment = TextAnchor.MiddleCenter
                    };
                }

                return p_fenStyle;
            }
        }

        #endregion

        #region 派生

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIParser.AllFieldHeight * 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            var pro_camera = property.FindPropertyRelative(CameraTracking.TransObjs.fieldName_camera);
            var pro_obj = property.FindPropertyRelative(CameraTracking.TransObjs.fieldName_obj);

            //分隔符长度
            const float fenLength = 30;
            Rect pos_title = position.SetHeightFromTop(EditorGUIParser.OnceHeight);

            var move = pos_title.MoveHeight(EditorGUIParser.AllFieldHeight);
            move.SectionLength(0.5f, fenLength, out var pos_camera, out var pos_obj);

            Rect pos_fen;
            //分隔符长度
            pos_fen = new Rect(pos_camera.x + pos_camera.width, move.y, fenLength, move.height);

            EditorGUI.LabelField(pos_title, p_titleLabel);

            EditorGUI.LabelField(pos_fen, p_fenlabel, FenStyle);

            EditorGUI.ObjectField(pos_camera, pro_camera, GUIContent.none);
            EditorGUI.ObjectField(pos_obj, pro_obj, GUIContent.none);

        }

        #endregion

        #endregion

    }


    [CustomPropertyDrawer(typeof(CameraTracking.PosGetType))]
    internal sealed class CameraTrackingEditorDraw_PosGetType : PropertyDrawer
    {

        #region

        #region 初始化

        public CameraTrackingEditorDraw_PosGetType()
        {
            p_openLabel = new GUIContent("坐标获取类型");
            p_objLabel = new GUIContent("object", "获取跟踪对象坐标时的坐标类型");
            p_cameraLabel = new GUIContent("camera", "获取摄像机对象坐标时的坐标类型");

            p_open = true;
        }

        private GUIContent p_openLabel;

        private GUIContent p_cameraLabel;

        private GUIContent p_objLabel;

        private bool p_open;

        #endregion

        #region 派生

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //2个枚举+open折叠开关
            if (p_open)
            {
                //一个开关 + 2个控件
                return (EditorGUIParser.AllFieldHeight * 2) + EditorGUIParser.OnceHeight;
            }

            //1个开关
            return EditorGUIParser.OnceHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var pro_camera = property.FindPropertyRelative(CameraTracking.PosGetType.fieldName_camera);
            var pro_obj = property.FindPropertyRelative(CameraTracking.PosGetType.fieldName_obj);

            Rect pos_label, pos_camera, pos_obj;

            //开关位置
            pos_label = position.SetHeightFromTop(EditorGUIParser.OnceHeight);

            //绘制折叠开关
            p_open = EditorGUI.Foldout(pos_label, p_open, p_openLabel);

            if (p_open)
            {
                //右侧缩进
                pos_label = pos_label.RightTrim(0.9f);
                //从开关向下移动一个控件高度
                pos_camera = pos_label.MoveHeight(EditorGUIParser.AllFieldHeight);
                //从摄像机向下移动一个控件高度
                pos_obj = pos_camera.MoveHeight(EditorGUIParser.AllFieldHeight);

                //打开

                //转换参数
                Space cameraSapce, objSpace;
                cameraSapce = pro_camera.boolValue ? Space.Self : Space.World;
                objSpace = pro_obj.boolValue ? Space.Self : Space.World;

                //绘制参数
                cameraSapce = (Space)EditorGUI.EnumPopup(pos_camera, p_cameraLabel, cameraSapce);
                objSpace = (Space)EditorGUI.EnumPopup(pos_obj, p_objLabel, objSpace);

                //设置参数
                pro_camera.boolValue = cameraSapce == Space.Self ? true : false;
                pro_obj.boolValue = objSpace == Space.Self ? true : false;
            }

        }

        #endregion

        #region 封装

        #endregion

        #endregion

    }


    [CustomPropertyDrawer(typeof(CameraTracking.MoveData))]
    internal sealed class CameraTrackingEditorDraw_MoveData : PropertyDrawer
    {

        #region

        #region 初始化

        public CameraTrackingEditorDraw_MoveData()
        {
            p_moveLabel = new GUIContent("移动模式", "打开后摄像机将采用移动方式逐渐向追踪对象靠近，关闭则直接将摄像机设置到目标位置");
            p_speedLabel = new GUIContent("摄像机移动速度");
            p_type2Dlabel = new GUIContent("2D", "是否开启2D模式移动？\n打开选项后，在移动摄像机时摄像机的z轴参数将不会被脚本修改");
        }

        private GUIContent p_moveLabel;
        private GUIContent p_speedLabel;
        private GUIContent p_type2Dlabel;

        #endregion

        #region 派生

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIParser.AllFieldHeight * 3)/* - EditorGUIParser.HeightInterval*/;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //获取参数
            var pro_moveCamera = property.FindPropertyRelative(CameraTracking.MoveData.fieldName_moveCamera);
            var pro_speed = property.FindPropertyRelative(CameraTracking.MoveData.fieldName_speed);
            var pro_type2D = property.FindPropertyRelative(CameraTracking.MoveData.fieldName_type2D);
            bool wasEnabled = true;
            Rect pos_moveCamera, pos_speed, pos_type2D;

            //摄像机绘制区域
            pos_moveCamera = position.SetHeightFromTop(EditorGUIParser.OnceHeight);

            pos_speed = pos_moveCamera.MoveHeight(EditorGUIParser.AllFieldHeight);

            pos_type2D = pos_speed.MoveHeight(EditorGUIParser.AllFieldHeight);

            bool moveFlag;
            moveFlag = EditorGUI.ToggleLeft(pos_moveCamera, p_moveLabel, pro_moveCamera.boolValue);
            pro_moveCamera.boolValue = moveFlag;
            if (!moveFlag)
            {
                //关闭移动参数修改
                wasEnabled = GUI.enabled;
                GUI.enabled = false;
            }
            //速度参数
            pro_speed.floatValue = EditorGUI.FloatField(pos_speed, p_speedLabel, pro_speed.floatValue);

            if (!moveFlag) GUI.enabled = wasEnabled;

            //2D移动模式
            pro_type2D.boolValue = EditorGUI.Toggle(pos_type2D, p_type2Dlabel, pro_type2D.boolValue);


        }

        #endregion

        #endregion

    }


}
#endif