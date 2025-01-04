using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cheng.Unitys
{

    /// <summary>
    /// Unity引擎中的环境和功能
    /// </summary>
    public static class UnityEnvironment
    {

        #region 退出游戏

        /// <summary>
        /// 退出游戏
        /// </summary>
        /// <remarks>根据环境退出unity游戏，若处于unity编辑器中则退出播放；处于打包的windows，mac，linux游戏时则表示退出游戏进程</remarks>
        public static void ExitGame()
        {

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        /// <param name="exitCode">播放器应用程序在 Windows、Mac 和 Linux 上终止时返回的可选退出代码。默认值为0</param>
        /// <remarks>根据环境退出unity游戏，若处于unity编辑器中则退出播放；处于打包的Windows、Mac 和 Linux 游戏时则表示退出游戏</remarks>
        public static void ExitGame(int exitCode)
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit(exitCode);
#endif
        }

        #endregion

    }

}
