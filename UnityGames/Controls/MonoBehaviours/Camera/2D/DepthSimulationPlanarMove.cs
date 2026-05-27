using UnityEngine;
using UnityEngine.Serialization;
using Cheng.Algorithm;

#if UNITY_EDITOR
using Cheng.Unitys.Editors;
#endif

using UObj = UnityEngine.Object;
using GObj = UnityEngine.GameObject;

namespace Cheng.Unitys.Cameras._2D
{

    /// <summary>
    /// 2D平面移动时的z轴视差模拟
    /// </summary>
    /// <remarks>
    /// <para>脚本所在对象会根据参数<see cref="DepthSimulationPlanarMove.WorkingCameraTransform"/>的移动按<see cref="DepthSimulationPlanarMove.MovementRatio"/>比例移动，以造成z轴视觉偏差</para>
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
            moveType = Space.World;
            moveing = true;
            qukeRunUpdate = true;
        }

        #endregion

        #region 参数

        #region 外部参数

#if UNITY_EDITOR
        [Tooltip("要逐帧移动的对象变换")]
#endif
        [SerializeField] private Transform moveObject;

#if UNITY_EDITOR
        [Tooltip("要运行的2D正交摄像机变换")]
#endif
        [SerializeField] private Transform workingCamera;

#if UNITY_EDITOR
        [Tooltip("对象相对摄像机的移动比例\n移动比例越接近1，代表对象距离摄像机焦点越远；越接近0，表示离摄像机焦点越近\n等于0时表示物体的虚拟z轴处于摄像机焦点处\n若值小于0，则表示比摄像机焦点更接近摄像机")]
#endif
        [SerializeField] private Vector2 movementRatio;

#if UNITY_EDITOR
        [Tooltip("对象的移动空间类型")]
#endif
        [SerializeField] private Space moveType;

#if UNITY_EDITOR
        [Tooltip("是否启用移动\n只有在勾选该参数时脚本才会移动对象\n将该参数取消勾选时脚本会暂停移动对象，但是再次勾选后脚本会记录上次的移动状态继续以相对位置移动；\n如果是禁用脚本，再次运行时，脚本不会记录上一次运行的状态，而是以启动时对象的位置作为基准重新开始移动")]
#endif
        [SerializeField] private bool moveing;

#if UNITY_EDITOR
        [Tooltip("在运行时是否不进行检查参数为null\n参数为false时每帧运行前会检查参数是否为null，相对会消耗更多性能；true则不会检查")]
#endif
        [SerializeField] private bool qukeRunUpdate;

        #endregion

        #region 内部参数

        #if DEBUG
        /// <summary>
        /// 上一帧位置
        /// </summary>
        #endif
        private Vector3 p_lastCameraPos;
        
        #endregion

        #endregion

        #region 参数访问

        /// <summary>
        /// 要运行的2D正交摄像机所在变换
        /// </summary>
        /// <remarks>请确保在游戏脚本运行前该参数有实例</remarks>
        public Transform WorkingCameraTransform
        {
            get => workingCamera;
            set => workingCamera = value;
        }

        /// <summary>
        /// 访问或设置对象相对摄像机的移动比例
        /// </summary>
        /// <value>
        /// <para>移动比例越接近1，代表对象距离摄像机焦点越远；越接近0，表示离摄像机焦点越近</para>
        /// <para>等于0时表示物体的虚拟z轴处于摄像机焦点处</para>
        /// <para>若值小于0，则表示比摄像机焦点更接近摄像机</para>
        /// <para>绝对值如果超过1，则不会出现视差错觉效果</para>
        /// </value>
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

        /// <summary>
        /// 移动对象时的空间坐标类型
        /// </summary>
        public Space MoveType
        {
            get => moveType;
            set
            {
                moveType = value;
            }
        }

        /// <summary>
        /// 是否启用移动
        /// </summary>
        /// <value>
        /// <para>只有将参数设为true时脚本才会移动对象</para>
        /// <para>将该参数设为false时脚本会暂停移动对象，但是再次设为true后脚本会记录上次的移动状态继续以相对位置移动；如果是将<see cref="Behaviour.enabled"/>参数设为false或脚本因为其他原因停止运作，再次运行时，脚本不会记录上一次运行的状态，而是以启动时对象的位置作为基准重新开始移动</para>
        /// </value>
        public bool Moving
        {
            get => moveing;
            set
            {
                moveing = value;
            }
        }

        #endregion

        #region 运行

        /// <summary>
        /// 手动运行一帧移动
        /// </summary>
        /// <param name="cameraPosition">指定当前帧摄像机对象所在的场景坐标位置</param>
        /// <param name="moveObjectPosition">指定要移动的对象所在位置</param>
        /// <param name="movementRatio">对象相对摄像机的移动比例</param>
        /// <param name="lastPosition">上一帧摄像机对象所在的场景位置</param>
        /// <returns>要移动的对象所在的新位置</returns>
        public static Vector3 UpdateMove(Vector3 cameraPosition, Vector3 moveObjectPosition, Vector2 movementRatio, ref Vector3 lastPosition)
        {

            //获取camera位置
            //var camera_pos = cameraPosition;

            //计算摄像机的移动距离
            var moveCamera = cameraPosition - lastPosition;

            //计算对象移动距离
            Vector2 objMove = new Vector2(moveCamera.x * movementRatio.x, moveCamera.y * movementRatio.y);

            //保存当前camera位置
            lastPosition = cameraPosition;

            //移动
            return new Vector3(moveObjectPosition.x + objMove.x, moveObjectPosition.y + objMove.y, moveObjectPosition.z);
        }

        private void f_runUpdate()
        {
            //Transform cameraTrans = workingCamera.transform;
            if (moveing)
            {
                if (moveType == Space.Self)
                {
                    moveObject.localPosition = UpdateMove(workingCamera.transform.localPosition, moveObject.localPosition, this.movementRatio, ref p_lastCameraPos);
                }
                else
                {
                    moveObject.position = UpdateMove(workingCamera.transform.position, moveObject.position, this.movementRatio, ref p_lastCameraPos);
                }
            }
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
#if UNITY_EDITOR
#endif
#if DEBUG
#endif