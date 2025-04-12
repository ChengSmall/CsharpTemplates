using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine;

using UObj = UnityEngine.Object;

namespace Cheng.Unitys.Scenes
{

    /// <summary>
    /// 将保持对象全局存在的脚本
    /// </summary>
    /// <remarks>
    /// <para>在初始化时，将对象设置为全局存在，不受场景影响而销毁</para>
    /// </remarks>
#if UNITY_EDITOR
    [AddComponentMenu("Cheng/场景/全局对象初始化")]
#endif
    [DisallowMultipleComponent]
    public class GlobalPersistenceObject : MonoBehaviour
    {

        #region

        public GlobalPersistenceObject()
        {
        }

        #endregion

        #region 运行

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        #endregion

    }

}
