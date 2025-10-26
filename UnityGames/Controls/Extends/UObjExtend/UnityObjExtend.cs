using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine;

using UObj = UnityEngine.Object;

namespace Cheng.Unitys
{

    /// <summary>
    /// Unity对象扩展
    /// </summary>
    public static unsafe partial class UnityObjExtend
    {

        #region 对象管理

        /// <summary>
        /// 将对象变成持久性对象
        /// </summary>
        /// <param name="obj"></param>
        public static void DontDestroyOnLoad(this UObj obj)
        {
            UObj.DontDestroyOnLoad(obj);
        }

        /// <summary>
        /// 将游戏对象移动至当前活动场景
        /// </summary>
        /// <param name="obj"></param>
        public static void MoveNowScene(this GameObject obj)
        {
            var scene = SceneManager.GetActiveScene();
            SceneManager.MoveGameObjectToScene(obj, scene);
        }

        /// <summary>
        /// 销毁对象实例
        /// </summary>
        /// <param name="obj"></param>
        public static void Destroy(this UObj obj)
        {
            UObj.Destroy(obj);
        }

        /// <summary>
        /// 克隆 <paramref name="original"/> 对象并返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original">源对象</param>
        /// <returns>克隆后的新对象</returns>
        public static T Instantiate<T>(this T original) where T : UObj
        {
            return UObj.Instantiate(original);
        }

        /// <summary>
        /// 克隆 <paramref name="original"/> 对象并返回
        /// </summary>
        /// <remarks>
        /// <para>
        /// 此函数会通过与编辑器中的复制命令类似的方式创建对象的副本。<br/>
        /// 如果要克隆<see cref="UnityEngine.GameObject"/>，则可以指定其位置和旋转，否则默认为原始的位置和旋转。<br/>
        /// 如果要克隆<see cref="UnityEngine.Component"/>，则也会克隆它附加到的 <see cref="UnityEngine.GameObject"/>，同样可指定可选的位置和旋转
        /// </para>
        /// <para>
        /// 默认情况下，新对象的父对象为null，表示最顶级的节点，直属于世界空间；但可使用<paramref name="parent"/>手动设置父对象<br/>
        /// 克隆时<see cref="UnityEngine.GameObject"/>的活动状态会维持，因此，如果原始对象处于非活动状态，则克隆对象也会在非活动状态下创建。<br/>
        /// </para>
        /// <para>
        /// 所有的对象，每个<see cref="UnityEngine.MonoBehaviour"/>和<see cref="UnityEngine.Component"/>都会仅当它们在游戏内真正处于活动状态时，才会调用其 Awake 和 OnEnable 方法
        /// </para>
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="original">要复制的现有对象</param>
        /// <param name="position">新对象的位置</param>
        /// <param name="rotation">新对象的方向</param>
        /// <param name="parent">将指定给新对象的父对象，null表示无父对象</param>
        /// <returns>克隆后的新对象</returns>
        public static T Instantiate<T>(this T original, Vector3 position, Quaternion rotation, Transform parent) where T : UObj
        {
            return UObj.Instantiate(original, position, rotation, parent);
        }

        /// <summary>
        /// 克隆 <paramref name="original"/> 对象并返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original">要复制的现有对象</param>
        /// <param name="position">新对象的位置</param>
        /// <param name="rotation">新对象的方向</param>
        /// <returns>克隆后的新对象</returns>
        public static T Instantiate<T>(this T original, Vector3 position, Quaternion rotation) where T : UObj
        {
            return UObj.Instantiate(original, position, rotation);
        }

        /// <summary>
        /// 克隆 <paramref name="original"/> 对象并返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original">要复制的现有对象</param>
        /// <param name="parent">将指定给新对象的父对象，null表示无父对象</param>
        /// <returns>克隆后的新对象</returns>
        public static T Instantiate<T>(this T original, Transform parent) where T : UObj
        {
            return UObj.Instantiate(original, parent);
        }

        /// <summary>
        /// 克隆 <paramref name="original"/> 对象并返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original">要复制的现有对象</param>
        /// <param name="parent">将指定给新对象的父对象，null表示无父对象</param>
        /// <param name="instantiateInWorldSpace">如果分配了父对象，true可直接在世界空间中定位新对象，false可相对于新父项来设置对象的位置</param>
        /// <returns>克隆后的新对象</returns>
        public static T Instantiate<T>(this T original, Transform parent, bool instantiateInWorldSpace) where T : UObj
        {
            return UObj.Instantiate(original, parent, instantiateInWorldSpace);
        }

        /// <summary>
        /// 判断对象的实例状态
        /// </summary>
        /// <remarks>
        /// <para>
        /// 对于 Unity 的每个<see cref="UObj"/>对象，都会关联一个UnityEngine内部句柄，因此所有派生于<see cref="UObj"/>的类都属于“非托管对象”；<br/>
        /// 当使用<see cref="UObj.Destroy(UObj)"/>销毁对象后，C#脚本的托管对象仍然可能存在一些时间，但是此时的对象已经失去实质作用了；<br/>
        /// 由于Unity内部判空的方法较为杂乱，因此该函数的作用就是更清晰的判断对象是否被销毁
        /// </para>
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>如果返回为true，则表示对象处于实例化场景中；返回为false，表示对象的Unity实例判空机制为false但脚本对象仍存在；返回null表示脚本对象的引用为null</returns>
        public static bool? IsNotNull<T>(this T obj) where T : UObj
        {
            if ((object)obj is null) return null;
            return (bool)obj;
        }

        #endregion

    }

}
