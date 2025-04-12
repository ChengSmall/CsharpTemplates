using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;
using UnityEngine.Serialization;


#if UNITY_EDITOR
using UnityEditor;
using Cheng.Unitys.Editors;
#endif

namespace Cheng.Unitys.Mouses
{

    /// <summary>
    /// 鼠标移动速率捕获脚本
    /// </summary>
#if UNITY_EDITOR
    [AddComponentMenu("Cheng/鼠标/鼠标移动速率捕获")]
#endif
    public sealed class MouseMoveSpeedAxis : MonoBehaviour
    {

        #region

        public MouseMoveSpeedAxis()
        {
            moveSpeed = new Vector2(1, 1);
        }

        #endregion

        #region 参数

        #region 外部参数
        
#if UNITY_EDITOR
        [Tooltip("鼠标的移动速率，x表示横轴，y表示纵轴；\r\n当值为1时，MoveValue 返回的移动值表示为像素/秒\r\n当值为负数时，值所在坐标轴的移动参数将会取反\r\n默认值为（1，1）")]
#endif
        [SerializeField] private Vector2 moveSpeed;

        #endregion

        #region 内部参数

        private Vector2 p_lastPos;

        private Vector2 p_nowPos;

        #endregion

        #endregion

        #region 功能

        /// <summary>
        /// 返回上一帧的移动值
        /// </summary>
        /// <returns>
        /// <para>上一帧的移动值</para>
        /// </returns>
        public Vector2 MoveValue
        {
            get
            {
                //帧移动值
                return (new Vector2(p_nowPos.x - p_lastPos.x, p_nowPos.y - p_lastPos.y)) * moveSpeed;
            }
        }

        /// <summary>
        /// 移动速率
        /// </summary>
        /// <value>
        /// <para>鼠标的移动速率，x表示横轴，y表示纵轴；</para>
        /// <para>当值为1时，<see cref="MoveValue"/>返回的移动值表示为像素/秒</para>
        /// <para>当值为负数时，值所在坐标轴的移动参数将会取反</para>
        /// <para>默认值为（1，1）</para>
        /// </value>
        public Vector2 MoveSpeed
        {
            get => moveSpeed;
            set
            {
                moveSpeed = value;
            }
        }

        #endregion

        #region 运行

        private void Awake()
        {
            p_lastPos = new Vector2(0, 0);
            p_nowPos = new Vector2(0, 0);
        }

        private void OnEnable()
        {
            p_nowPos = Input.mousePosition;
            p_lastPos = p_nowPos;
        }

        private void LateUpdate()
        {
            //当前鼠标位置
            p_lastPos = p_nowPos;
            p_nowPos = Input.mousePosition;
        }

        #endregion

    }

}
