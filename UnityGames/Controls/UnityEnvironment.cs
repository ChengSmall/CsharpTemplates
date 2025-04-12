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
        /// <remarks>根据环境退出unity，若处于unity编辑器中则退出播放；处于已打包环境则表示退出游戏进程</remarks>
        public static void ExitGame(int exitCode)
        {

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit(exitCode);
#endif

//#if UNITY_EDITOR
//            EditorApplication.isPlaying = false;

//#elif UNITY_STANDALONE
//            Application.Quit(exitCode);

//#elif UNITY_ANDROID
//            
//            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
//            using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
//            {
//                activity.Call("finish");
//            }

//            // 杀死当前进程
//            using (AndroidJavaClass process = new AndroidJavaClass("android.os.Process"))
//            {
//                int pid = process.CallStatic<int>("myPid");
//                process.CallStatic("killProcess", pid);
//            }

//#endif

        }

        #endregion

        #region Editor

#if UNITY_EDITOR

        private class ThreadLockStructure
        {
            public ThreadLockStructure()
            {
                isLockReload = false;
            }

            /// <summary>
            /// 是否关闭自动更新脚本机制
            /// </summary>
            public bool isLockReload;
        }

        #region 脚本自动更新控制

        private static readonly ThreadLockStructure p_threadLock = new ThreadLockStructure();

        [MenuItem("Cheng/系统/关闭脚本自动编译功能")]
        public static void LockReloadAssemblies()
        {
            lock (p_threadLock)
            {
                if (p_threadLock.isLockReload)
                {
                    //已经处于关闭自动更新机制状态

                    Debug.LogWarning("你已经关闭脚本自动编译功能，无法再次关闭！");
                    return;
                }
                //未关闭自动更新机制
                //设为关闭状态
                p_threadLock.isLockReload = true;

                //执行关闭更新代码
                EditorApplication.LockReloadAssemblies();
                
                Debug.Log("已关闭脚本自动编译功能");
            }
        }

        [MenuItem("Cheng/系统/打开脚本自动编译功能")]
        public static void UnlockReloadAssemblies()
        {
            lock (p_threadLock)
            {
                if (!p_threadLock.isLockReload)
                {
                    //未关闭自动更新机制
                    Debug.LogWarning("已经打开脚本自动编译功能，无法再次打开！");
                    return;
                }
                //处于关闭自动更新机制状态
                //设为未关闭状态
                p_threadLock.isLockReload = false;

                EditorApplication.UnlockReloadAssemblies();
                AssetDatabase.Refresh();
                Debug.Log("重新打开脚本自动编译功能");
            }
        }

        #endregion

        #region DEBUG

        #endregion

#endif

        #endregion

        #region GC

        /// <summary>
        /// 强制CG执行垃圾回收
        /// </summary>
#if UNITY_EDITOR
        [MenuItem("Cheng/GC/强制CG执行垃圾回收")]
#endif
        public static void GC_Collect()
        {
            GC.Collect();
#if UNITY_EDITOR
            Debug.Log("调用了一次GC.Collect");
#endif
        }

        #endregion

    }

}
#if UNITY_EDITOR

#endif
#if UNITY_EDITOR

#else

#endif