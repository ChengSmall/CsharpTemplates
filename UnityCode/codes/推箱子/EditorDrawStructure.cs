using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Cheng.GameTemplates.PushingBoxes
{

    /// <summary>
    /// 推箱子图像绘制结构
    /// </summary>
    [Serializable]
    public struct SpriteDraw
    {

        /// <summary>
        /// 2D精灵图像
        /// </summary>
        [SerializeField] public Sprite sprite;

        /// <summary>
        /// 图像显示颜色
        /// </summary>
        [SerializeField] public Color color;

#if UNITY_EDITOR

        public const string fieldName_sprite = nameof(sprite);

        public const string fieldName_color = nameof(color);

#endif

    }

}