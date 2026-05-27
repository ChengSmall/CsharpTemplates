using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using UnityEngine;

namespace Cheng.Unitys.Tags
{

    /// <summary>
    /// Unity标签组扩展功能
    /// </summary>
    public static class TagsExtend
    {

        /// <summary>
        /// 判断游戏对象的指定标签是否存在于标签组中
        /// </summary>
        /// <param name="gobj">游戏对象，参数不得为null</param>
        /// <param name="tag">判断的标签</param>
        /// <returns>存在为true，不存在则false</returns>
        public static bool TagContains(this GameObject gobj, string tag)
        {
            return gobj.GetComponent<UnityTags>().TagContains(tag);
        }

        /// <summary>
        /// 判断游戏对象的指定标签是否存在于标签组中
        /// </summary>
        /// <param name="gobj">游戏对象，参数不得为null</param>
        /// <param name="tag">判断的标签</param>
        /// <returns>存在为true，不存在则false；若无法获取标签组则为null</returns>
        public static bool? TryTagContains(this GameObject gobj, string tag)
        {
            var tags = gobj.GetComponent<UnityTags>();
            if (tags) return tags.TagContains(tag);
            return null;
        }

        /// <summary>
        /// 判断组件所在对象的指定标签是否存在于标签组中
        /// </summary>
        /// <param name="component">组件，参数不得为null</param>
        /// <param name="tag">判断的标签</param>
        /// <returns>存在为true，不存在则false</returns>
        public static bool TagContains(this Component component, string tag)
        {
            return component.GetComponent<UnityTags>().TagContains(tag);
        }

        /// <summary>
        /// 判断组件所在对象的指定标签是否存在于标签组中
        /// </summary>
        /// <param name="component">组件，参数不得为null</param>
        /// <param name="tag">判断的标签</param>
        /// <returns>存在为true，不存在则false；若无法获取标签组则为null</returns>
        public static bool? TryTagContains(this Component component, string tag)
        {
            var tags = component.GetComponent<UnityTags>();
            if (tags is null) return null;
            return tags.TagContains(tag);
        }

        /// <summary>
        /// 向上寻找存在指定标签的标签组
        /// </summary>
        /// <param name="transform">要寻找的根变换</param>
        /// <param name="tag">要匹配的标签</param>
        /// <param name="find">返回找到匹配的变换，若没有找到匹配的标签，则为null</param>
        /// <returns>
        /// 从参数<paramref name="transform"/>开始，向父类寻找附加标签组对象并且有匹配<paramref name="tag"/>的变换，找到返回true，没有找到返回false
        /// </returns>
        public static bool ParentagContains(this Transform transform, string tag, out Transform find)
        {
            Transform tras = transform;
            bool flag;
            find = null;

            while ((object)tras != null)
            {
                flag = tras.TagContains(tag);
                if (flag)
                {
                    find = tras;
                    return true;
                }
                tras = tras.parent;
            }

            return false;
        }


    }
}
