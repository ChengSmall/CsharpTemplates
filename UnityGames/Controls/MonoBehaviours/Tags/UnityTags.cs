using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using Cheng.Unitys.Editors;
#endif

namespace Cheng.Unitys.Tags
{

    /// <summary>
    /// Unity标签组，可在同一个对象上定义多个标签
    /// </summary>
#if UNITY_EDITOR
    [AddComponentMenu("Cheng/其它/标签组")]
#endif
    [DisallowMultipleComponent]
    public sealed class UnityTags : MonoBehaviour
    {

        #region 初始化
        public UnityTags()
        {
            tags = new List<string>();
        }
        #endregion

        #region 参数

#if UNITY_EDITOR
        [Tooltip("多个标签的集合")]
#endif
        [SerializeField]
        private List<string> tags;

        #endregion

        #region 功能

        /// <summary>
        /// 获取当前标签组的标签集合
        /// </summary>
        public List<string> Tags
        {
            get => tags;
        }

        /// <summary>
        /// 判断给定标签是否存在于标签组中
        /// </summary>
        /// <param name="tag">要判断的标签</param>
        /// <returns>存在返回true，不存在返回false</returns>
        public bool TagContains(string tag)
        {
            return tags.Contains(tag);
        }

        /// <summary>
        /// 判断给定标签组是否全部存在于标签组中
        /// </summary>
        /// <param name="tags"></param>
        /// <returns>如果任意一个标签存在标签组中返回true，否则返回true</returns>
        public bool TagContainsOr(IEnumerable<string> tags)
        {

            foreach (var item in tags)
            {
                if (this.tags.Contains(item)) return true;
            }

            return false;
        }

        /// <summary>
        /// 判断给定标签组是否全部存在于标签组中
        /// </summary>
        /// <param name="tags"></param>
        /// <returns>如果至少有一个标签不存在标签组中返回false，否则返回true</returns>
        public bool TagContainsAnd(IEnumerable<string> tags)
        {

            foreach (var item in tags)
            {
                if (!this.tags.Contains(item)) return false;
            }

            return true;
        }

        /// <summary>
        /// 当前标签组内的标签数量
        /// </summary>
        public int Count => tags.Count;

        /// <summary>
        /// 访问或设置指定索引的标签
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>标签</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public string this[int index]
        {
            get => tags[index];
            set => tags[index] = value;
        }

        /// <summary>
        /// 删除多余的重复标签
        /// </summary>
#if UNITY_EDITOR
        [ContextMenu("删除多余的重复标签")]
#endif
        public void Removexcess()
        {
            var list = tags;

            if (list.Count == 0) return;

            string temp;
            int index = 0;
            int i;

            while (index < list.Count)
            {

                temp = list[index];

                for (i = index + 1; i < list.Count;)
                {
                    if (list[i] == temp) list.RemoveAt(i);
                    else i++;
                }

                index++;
            }

        }

        #endregion

    }

}
