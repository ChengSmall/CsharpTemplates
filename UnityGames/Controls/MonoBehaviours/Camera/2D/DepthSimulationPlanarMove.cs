using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using Cheng.Unitys.Editors;
#endif

namespace Cheng.Unitys.Cameras
{

    /// <summary>
    /// 2D平面移动时的z轴视差模拟
    /// </summary>
    /// <remarks>
    /// <para>脚本所在对象会根据摄像机<see cref="WorkingCamera"/>的移动按<see cref="MovementRatio"/>比例移动，以造成z轴视觉偏差</para>
    /// <para>
    /// 移动比例越接近1，代表对象距离摄像机焦点越远；越接近0，表示离摄像机焦点越近；<br/>
    /// 等于0时则表示物体的z轴处于摄像机焦点坐标；<br/>
    /// 若小于0，则表示比摄像机焦点更接近摄像机；<br/>
    /// 值的绝对值如果超过1，则不会出现视差错觉效果
    /// </para>
    /// <para>关闭脚本活动会停止运行和位置检测</para>
    /// </remarks>
#if UNITY_EDITOR
    [AddComponentMenu("Cheng/2D/摄像机/2D视差模拟")]
#endif
    [DisallowMultipleComponent]
    public sealed class DepthSimulationPlanarMove : MonoBehaviour
    {

        #region 构造

        public DepthSimulationPlanarMove()
        {
            movementRatio = new Vector2();
            workingCamera = null;
            moveObject = null;
            p_lastCameraPos = default;
        }

        #endregion

        #region 参数

        #region 外部参数

#if UNITY_EDITOR
        [Tooltip("要逐帧移动的对象变换")]
#endif
        [SerializeField] private Transform moveObject;

#if UNITY_EDITOR
        [Tooltip("要运行的2D正交摄像机")]
#endif
        [SerializeField] private Camera workingCamera;

#if UNITY_EDITOR
        [Tooltip("对象相对摄像机的移动比例")]
#endif
        [SerializeField] private Vector2 movementRatio;
        
#if UNITY_EDITOR
        [Tooltip("在运行时是否不进行检查参数为null\n参数为false时每帧运行前会检查参数是否为null，相对会消耗更多性能；true则不会检查")]
#endif
        [SerializeField] private bool qukeRunUpdate = true;

        #endregion

        #region 内部参数

        /// <summary>
        /// 上一帧位置
        /// </summary>
        private Vector3 p_lastCameraPos;
        
        #endregion

        #endregion

        #region 参数访问

        /// <summary>
        /// 要运行的2D正交摄像机
        /// </summary>
        /// <remarks>请确保在游戏脚本运行前该参数有摄像机实例</remarks>
        public Camera WorkingCamera
        {
            get => workingCamera;
            set => workingCamera = value;
        }

        /// <summary>
        /// 访问或设置对象相对摄像机的移动比例
        /// </summary>
        public Vector2 MovementRatio
        {
            get => movementRatio;
            set => movementRatio = value;
        }

        /// <summary>
        /// 要逐帧移动的对象的变换
        /// </summary>
        public Transform MoveObject
        {
            get => moveObject;
            set
            {
                moveObject = value;
            }
        }

        /// <summary>
        /// 是否快速运行
        /// </summary>
        /// <value>
        /// <para>在运行时是否不进行空引用检查</para>
        /// <para>参数设为false时每帧运行时会检查参数是否为null，这相对会消耗更多性能；true则不会检查</para>
        /// <para>参数默认为true</para>
        /// </value>
        public bool QukeRunUpdate
        {
            get => qukeRunUpdate;
            set
            {
                qukeRunUpdate = value;
            }
        }

        #endregion

        #region 运行

        private void f_runUpdate()
        {
            //Transform cameraTrans = workingCamera.transform;

            //获取camera位置
            var camera_pos = workingCamera.transform.position;

            //计算摄像机的移动距离
            var moveCamera = camera_pos - p_lastCameraPos;

            Vector2 objMove;

            //计算对象移动距离
            objMove = new Vector2(moveCamera.x * movementRatio.x, moveCamera.y * movementRatio.y);

            //保存当前camera位置
            p_lastCameraPos = camera_pos;
            
            if (objMove.x == 0 && objMove.y == 0) return; //如果是0则不移动

            //获得移动的对象位置
            Vector3 nowPos = moveObject.position;

            //移动
            moveObject.position = new Vector3(nowPos.x + objMove.x, nowPos.y + objMove.y, nowPos.z);
        }

        private void LateUpdate()
        {
            if (qukeRunUpdate) f_runUpdate();
            else
            {
                if (workingCamera != null && moveObject != null)
                {
                    f_runUpdate();
                }
            }
        }

        private void OnEnable()
        {
            if (workingCamera != null) p_lastCameraPos = workingCamera.transform.position;
        }

        #region

        private void OnDestroy()
        {
            moveObject = null;
            workingCamera = null;
        }

        #endregion

        #endregion

    }

}
