
using System;
using System.Diagnostics;
using UnityEngine;

namespace Cheng.Unitys.Debugs
{

    /// <summary>
    /// UnityEngine Debug扩展
    /// </summary>
    public static class UnityDebug
    {

        #region 打印

        /// <summary>
        /// 打印对象到日志控制台
        /// </summary>
        /// <param name="obj"></param>
        public static void print(this object obj)
        {
            if (obj is null) UnityEngine.Debug.Log("[Null]");
            else UnityEngine.Debug.Log(obj.ToString());
        }

        /// <summary>
        /// 打印文本到日志控制台
        /// </summary>
        /// <param name="str"></param>
        public static void print(this string str)
        {
            if (str is null) UnityEngine.Debug.Log("[Null]");
            else UnityEngine.Debug.Log(str);
        }

        /// <summary>
        /// 打印错误文本到控制台
        /// </summary>
        /// <param name="message"></param>
        public static void printError(this object message)
        {
            UnityEngine.Debug.LogError(message);
        }

        /// <summary>
        /// 打印错误文本到控制台
        /// </summary>
        /// <param name="message">文本</param>
        /// <param name="obj">错误对象</param>
        public static void printError(this object message, UnityEngine.Object obj)
        {
            UnityEngine.Debug.LogError(message, obj);
        }

        #endregion

        #region

        /// <summary>
        /// 计算当前游戏帧率（每秒帧数）
        /// </summary>
        public static int GameFPS
        {
            get
            {
                return (int)Math.Round(1 / (double)Time.unscaledDeltaTime);
            }
        }

        /// <summary>
        /// 计算当前游戏帧率（每秒帧数）
        /// </summary>
        public static float GameFPSFloat
        {
            get => 1 / Time.unscaledDeltaTime;
        }

        #endregion

    }

}
