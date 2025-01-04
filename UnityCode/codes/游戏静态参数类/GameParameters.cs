
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cheng.Algorithm.Randoms;
using UnityEngine;

namespace Cheng.Unitys
{

    /// <summary>
    /// 游戏基本参数合集
    /// </summary>
    public sealed class GameParameters
    {

        #region 静态参数

        static GameParameters p_par;

        //[RuntimeInitializeOnLoadMethod]

        /// <summary>
        /// 初始化参数配置
        /// </summary>
        public static void InitUnityGame()
        {
            if (p_par is null)
            {
                p_par = new GameParameters();
            }
        }

        /// <summary>
        /// 游戏参数
        /// </summary>
        public static GameParameters Parmaeters
        {
            get
            {
                if(p_par is null)
                {
                    p_par = new GameParameters();
                }
                return p_par;
            }
        }

        #endregion

        #region 构造

        private GameParameters()
        {

            #region 目录检测

            if (!Cheng.Systems.SystemEnvironment.TryGetCommandArgs(out commandArgs))
            {
                commandArgs = null;
            }

            try
            {
                appDomain = AppDomain.CurrentDomain;
                baseDirectory = appDomain.BaseDirectory;
            }
            catch (Exception)
            {
                appDomain = null;
                baseDirectory = null;
            }

            unityDataPath = Application.dataPath;

            unityStreamingAssetsPath = Application.streamingAssetsPath;

            unityPersistentDataPath = Application.persistentDataPath;

            #region 动态目录初始化

#if UNITY_EDITOR

            //编辑器

#elif UNITY_STANDALONE

            //PC个人电脑

#elif UNITY_ANDROID
            
            //安卓

#else
            
            //其它

#endif

            #endregion

            #endregion

            #region 平台检测

            isX64 = Cheng.Systems.SystemEnvironment.X64;

            #endregion

            #region 随机器参数

            random = null;
            //random = new LICRandom(0);

            #endregion

        }

        #endregion

        #region 参数

        /// <summary>
        /// 当前应用程序域，没有则为null
        /// </summary>
        public readonly AppDomain appDomain;

        /// <summary>
        /// 程序命令行参数，没有则为空数组；若没有程序命令行功能则为null
        /// </summary>
        public readonly string[] commandArgs;

        /// <summary>
        /// 当前执行程序根目录，若无法获取程序根目录则为null
        /// </summary>
        public readonly string baseDirectory;

        /// <summary>
        /// <see cref="Application.dataPath"/>参数，项目数据目录
        /// </summary>
        public readonly string unityDataPath;

        /// <summary>
        /// <see cref="Application.streamingAssetsPath"/>参数，基础文件目录
        /// </summary>
        public readonly string unityStreamingAssetsPath;

        /// <summary>
        /// <see cref="Application.persistentDataPath"/>参数，持久数据目录
        /// </summary>
        public readonly string unityPersistentDataPath;

        /// <summary>
        /// 随机数生成器
        /// </summary>
        public BaseRandom random;

        /// <summary>
        /// 获取随机数生成器
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <returns>随机器，类型不对返回null</returns>
        public T GetRandom<T>() where T : BaseRandom
        {
            return random as T;
        }

        /// <summary>
        /// 可动态调整的资源目录，用于存储游戏资源文件，未初始化前为null
        /// </summary>
        public string dataAssetsDire;

        /// <summary>
        /// 可动态调整的配置目录，存储配置文件，未初始化前为null
        /// </summary>
        public string configDire;

        /// <summary>
        /// 可动态调整的存档文件目录，拥有存储游戏存档，未初始化前为null
        /// </summary>
        public string saveDire;

        /// <summary>
        /// 该程序是否为64位
        /// </summary>
        public readonly bool isX64;

        #endregion

        #region 方法


        #endregion

    }

}

#if UNITY_EDITOR


#elif UNITY_STANDALONE


#elif UNITY_ANDROID


#else


#endif