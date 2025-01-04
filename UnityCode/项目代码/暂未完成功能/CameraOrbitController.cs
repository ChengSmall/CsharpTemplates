using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using GObj = UnityEngine.GameObject;
using UObj = UnityEngine.Object;

namespace Cheng.Unitys.Cameras
{

    /// <summary>
    /// （未完成）控制3D摄像机在指定对象周围旋转
    /// </summary>
    /// <remarks>
    /// <para>
    /// 这是一个控制摄像机<see cref="controlledCamera"/>在指定对象<see cref="targetObject"/>周围旋转的脚本，可以指定旋转速度和距离对象的长度
    /// </para>
    /// </remarks>
    [Obsolete("未完成", true)]
    internal class CameraOrbitController : MonoBehaviour
    {

        #region 参数

        public CameraOrbitController()
        {
            controlledCamera = null;
            targetObject = null;
        }

#if UNITY_EDITOR
        [Tooltip("要控制的3D摄像机")]
#endif
        [SerializeField] private Camera controlledCamera;

#if UNITY_EDITOR
        [Tooltip("要以此为主要中心旋转的变换对象")]
#endif
        [SerializeField] private Transform targetObject;

#if UNITY_EDITOR
        [Tooltip("摄像机与对象的距离")]
        [Min(0)]
#endif
        [SerializeField] private float cameraToObjDistance;

        #endregion

        #region 功能

        /// <summary>
        /// 转动摄像机角度，使用四元数
        /// </summary>
        /// <param name="quaternion">要转动的角度</param>
        public void RollCamera(Quaternion quaternion)
        {
            throw new NotImplementedException("功能未实现");
        }

        /// <summary>
        /// 转动摄像机角度，使用欧拉角模式
        /// </summary>
        /// <param name="eulerAngles">要转动的角度</param>
        public void RollCamera(Vector3 eulerAngles)
        {
            throw new NotImplementedException("功能未实现");
        }


        public void SetCameraAngle(Quaternion quaternion)
        {
            throw new NotImplementedException("功能未实现");
        }


        public void SetCameraAngle(Vector3 eulerAngles)
        {
            throw new NotImplementedException("功能未实现");
        }

        #endregion

        #region 运行

        private void Awake()
        {

        }


        private void OnDestroy()
        {

        }

        #endregion

    }

}

#if UNITY_EDITOR

#endif