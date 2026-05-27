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
using Cheng.DataStructure.Collections;

namespace Cheng.Unitys.Cameras
{

    /// <summary>
    /// 控制摄像机追踪对象位置
    /// </summary>
    /// <remarks>
    /// <para>这是一个控制摄像机追踪对象的脚本，并且可以设置摄像机追踪的速度，追踪对象位置的偏移数据，指定获取的坐标类型</para>
    /// <para>理论上你可以将任意一个对象变换设置并追踪另一个变换，但是最具有实用价值的是摄像机追踪</para>
    /// </remarks>
#if UNITY_EDITOR
    [AddComponentMenu("Cheng/摄像机/对象追踪")]
#endif
    [DisallowMultipleComponent]
    public sealed partial class CameraTracking : MonoBehaviour
    {

        #region 外部参数

        public CameraTracking()
        {
            up_trans = default;
            positionOffset = default;
            up_spaces.camera = false;
            up_spaces.obj = false;
            up_move.moveCamera = false;
            up_move.type2D = false;
            up_move.speed = 0;
        }

        [SerializeField] private TransObjs up_trans;

        [SerializeField] private PosGetType up_spaces;

        [SerializeField] private MoveData up_move;

#if UNITY_EDITOR
        [Tooltip("摄像机追踪对象目标的位置偏移")]
#endif
        [SerializeField] private Vector3 positionOffset;

        #endregion

        #region

        #endregion

        #region 功能

        /// <summary>
        /// 在获取或设置摄像机坐标参数时的坐标类型
        /// </summary>
        /// <value>参数为<see cref="UnityEngine.Space.World"/>表示使用世界空间坐标；<see cref="Space.Self"/>表示使用变换本地坐标</value>
        public UnityEngine.Space MoveCameraSpace
        {
            get => up_spaces.camera ? Space.Self : Space.World;
            set
            {
                switch (value)
                {
                    case Space.Self:
                        up_spaces.camera = true;
                        break;
                    case Space.World:
                    default:
                        up_spaces.camera = false;
                        break;
                }
            }
        }

        /// <summary>
        /// 在获取追踪对象的坐标时，获取的变换坐标类型
        /// </summary>
        /// <value>参数为<see cref="UnityEngine.Space.World"/>表示使用世界空间坐标；<see cref="Space.Self"/>表示使用变换本地坐标</value>
        public UnityEngine.Space GetObjectPositionSpace
        {
            get => up_spaces.obj ? Space.Self : Space.World;
            set
            {
                switch (value)
                {
                    case Space.Self:
                        up_spaces.obj = true;
                        break;
                    case Space.World:
                    default:
                        up_spaces.obj = false;
                        break;
                }
            }
        }

        /// <summary>
        /// 指定摄像机是移动到目标还是直接设置到目标位置
        /// </summary>
        /// <value>
        /// <para>参数为false时，摄像机每帧都会将位置设置到目标位置；参数为true时，摄像机会以一定速度移动到目标位置</para>
        /// </value>
        public bool MoveCamera
        {
            get => up_move.moveCamera;
            set
            {
                up_move.moveCamera = value;
            }
        }

        /// <summary>
        /// 在移动摄像机时是否忽略z轴变化
        /// </summary>
        /// <value>
        /// <para>当该参数为true时，摄像机的位置变化将忽略z轴，z轴参数将永远不会被脚本更改</para>
        /// <para>该参数默认为false</para>
        /// </value>
        public bool MoveType2D
        {
            get => up_move.type2D;
            set
            {
                up_move.type2D = value;
            }
        }

        /// <summary>
        /// 摄像机追踪对象时的移动速度
        /// </summary>
        /// <value>
        /// <para>
        /// 该参数仅在<see cref="MoveCamera"/>参数为true时有效，该参数指定摄像机追踪对象时移动的速度，表示为每秒移动的距离；<br/>
        /// 参数设为0时摄像机将不再移动；<br/>
        /// 你可以在摄像机追踪的过程中设置该参数来动态调整摄像机的追踪速度
        /// </para>
        /// <para>该参数默认是0</para>
        /// </value>
        public float CameraMoveSpeed
        {
            get => up_move.speed;
            set
            {
                up_move.speed = value;
            }
        }

        /// <summary>
        /// 要追踪的对象所在变换
        /// </summary>
        public Transform TrackedObj
        {
            get => up_trans.obj;
            set
            {
                up_trans.obj = value;
            }
        }

        /// <summary>
        /// 要控制的摄像机所在变换
        /// </summary>
        public Transform ControlledCamera2D
        {
            get => up_trans.camera;
            set => up_trans.camera = value;
        }

        /// <summary>
        /// 摄像机追踪对象目标的位置偏移
        /// </summary>
        /// <value>
        /// <para>
        /// 当摄像机进行对象追踪时，脚本会将<see cref="TrackedObj"/>对象的位置坐标与该参数相加的结果作为摄像机移动到的目标位置；
        /// </para>
        /// <para>该参数默认为(0,0,0)</para>
        /// </value>
        public Vector3 PositionOffset
        {
            get => positionOffset;
            set
            {
                positionOffset = value;
            }
        }

        #endregion

        #region 运行

        #region 初始化

        private void Awake()
        {

        }

        #endregion

        private void OnDestroy()
        {
            up_trans = default;
        }

        private void OnEnable()
        {

        }

        private void OnDisable()
        {

        }

        #region 

        static void f_update(TransObjs trans, PosGetType spaces, MoveData move, Vector3 offset, float deltaTime)
        {
            Vector3 cameraPos, objPos;
            Vector2 v2;
            if (spaces.camera)
            {
                cameraPos = trans.camera.localPosition;
            }
            else
            {
                cameraPos = trans.camera.position;
            }

            if (spaces.obj)
            {
                objPos = trans.obj.localPosition;
            }
            else
            {
                objPos = trans.obj.position;
            }

            if (move.moveCamera)
            {
                //移动
                if (move.speed != 0)
                {
                    objPos = objPos + offset;
                    if (move.type2D)
                    {
                        v2 = Vector2.MoveTowards(new Vector2(cameraPos.x, cameraPos.y), new Vector2(objPos.x, objPos.y), move.speed * deltaTime);
                        cameraPos = new Vector3(v2.x, v2.y, cameraPos.z);
                    }
                    else
                    {

                        cameraPos = Vector3.MoveTowards(cameraPos, objPos, move.speed * deltaTime);
                    }

                }
                else
                {
                    //cameraPos = cameraPos;
                }
            }
            else
            {
                if (move.type2D)
                {
                    cameraPos = new Vector3(objPos.x + offset.x, objPos.y + offset.y, cameraPos.z);
                }
                else
                {
                    cameraPos = new Vector3(objPos.x + offset.x, objPos.y + offset.y, objPos.z + offset.z);
                }
            }

            if (spaces.camera)
            {
                trans.camera.localPosition = cameraPos;
            }
            else
            {
                trans.camera.position = cameraPos;
            }
        }

        #endregion

        private void Update()
        {

            f_update(up_trans, up_spaces, up_move, positionOffset, UnityEngine.Time.unscaledDeltaTime);

        }

        #endregion

    }

}
#if UNITY_EDITOR

#endif
