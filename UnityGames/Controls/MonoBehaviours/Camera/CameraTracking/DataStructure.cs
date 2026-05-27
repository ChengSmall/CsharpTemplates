using UnityEngine;
using UnityEngine.Serialization;
using Cheng.Algorithm;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using Cheng.Unitys.Editors;
#endif

using UObj = UnityEngine.Object;
using GObj = UnityEngine.GameObject;

namespace Cheng.Unitys.Cameras
{

    partial class CameraTracking
    {

        #region 结构

        [Serializable]
        internal struct TransObjs
        {

            [SerializeField] public Transform camera;

            [SerializeField] public Transform obj;


#if UNITY_EDITOR

            public const string fieldName_camera = nameof(camera);

            public const string fieldName_obj = nameof(obj);
#endif

        }

        /// <summary>
        /// 坐标获取类型，false表示世界坐标，true表示本地坐标
        /// </summary>
        [Serializable]
        internal struct PosGetType
        {

            [SerializeField] public bool camera;

            [SerializeField] public bool obj;

#if UNITY_EDITOR

            public const string fieldName_camera = nameof(camera);
            public const string fieldName_obj = nameof(obj);
#endif

        }

        /// <summary>
        /// 移动参数
        /// </summary>
        [Serializable]
        internal struct MoveData
        {

            /// <summary>
            /// 移动速度
            /// </summary>
            [SerializeField] public float speed;

            /// <summary>
            /// true采用移动方式，false采用直接设置
            /// </summary>
            [SerializeField] public bool moveCamera;

            /// <summary>
            /// 使用2D方式移动
            /// </summary>
            [SerializeField] public bool type2D;


#if UNITY_EDITOR

            public const string fieldName_speed = nameof(speed);

            public const string fieldName_moveCamera = nameof(moveCamera);

            public const string fieldName_type2D = nameof(type2D);

#endif

        }


        #endregion

    }

}

#if UNITY_EDITOR
#endif