using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Cheng.Systems
{

    /// <summary>
    /// 一个动态加载程序集的加载方法
    /// </summary>
    public static class AssemblyLoading
    {

        static Dictionary<string, Assembly> p_asses;

        private static Assembly Dom_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assName = args.Name;
            if (p_asses.TryGetValue(assName, out var ass))
            {
                return ass;
            }
            return null;
        }

        /// <summary>
        /// 初始化动态程序集加载模块和参数
        /// </summary>
        public static void Init()
        {
            //var currAss = Assembly.GetExecutingAssembly();
            var dom = AppDomain.CurrentDomain;
            dom.AssemblyResolve += Dom_AssemblyResolve;
            p_asses = new Dictionary<string, Assembly>();
        }

        /// <summary>
        /// 获取内部待加载程序集
        /// </summary>
        /// <returns>
        /// <para>使用键值对储存的程序集，Key表示程序集的<see cref="Assembly.FullName"/></para>
        /// <para>若未调用<see cref="Init"/>进行初始化则返回null</para>
        /// </returns>
        public static Dictionary<string, Assembly> Assemblys
        {
            get => p_asses;
        }

        /// <summary>
        /// 向动态加载程序集添加待加载模块
        /// </summary>
        /// <param name="raw">字节数组，表示包含程序集的基于 COFF 的映像</param>
        /// <returns>出现的错误，没有错误则返回null</returns>
        /// <exception cref="ArgumentNullException">未调用<see cref="Init"/>进行初始化</exception>
        public static Exception AddAssembly(byte[] raw)
        {
            if (p_asses is null) throw new ArgumentNullException();
            try
            {
                lock (p_asses)
                {
                    var ass = Assembly.Load(raw);
                    p_asses[ass.FullName] = ass;
                    return null;
                }
                
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>
        /// 向动态加载程序集添加待加载模块
        /// </summary>
        /// <param name="filePath">包含程序集清单的文件的名称或路径</param>
        /// <returns>出现的错误，没有错误则返回null</returns>
        /// <exception cref="ArgumentNullException">未调用<see cref="Init"/>进行初始化</exception>
        public static Exception AddAssembly(string filePath)
        {
            if (p_asses is null) throw new ArgumentNullException();
            try
            {
                lock (p_asses)
                {
                    var ass = Assembly.LoadFrom(filePath);
                    p_asses[ass.FullName] = ass;
                    return null;
                }
                
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>
        /// 向动态加载程序集添加待加载模块
        /// </summary>
        /// <param name="assembly">待加载的程序集模块</param>
        /// <returns>出现的错误，没有错误则返回null</returns>
        /// <exception cref="ArgumentNullException">未调用<see cref="Init"/>进行初始化</exception>
        public static Exception AddAssembly(Assembly assembly)
        {
            if (p_asses is null) throw new ArgumentNullException();
            try
            {
                if (assembly is null) return new ArgumentNullException();
                lock (p_asses)
                {
                    p_asses[assembly.FullName] = assembly;
                    return null;
                }

            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>
        /// 将指定目录下的文件全部添加进待加载模块
        /// </summary>
        /// <param name="path">一个文件夹目录</param>
        /// <exception cref="Exception">错误</exception>
        public static void AddAssemblysByPath(string path)
        {
            if (p_asses is null) throw new ArgumentNullException();
            DirectoryInfo info = new DirectoryInfo(path);

            var files = info.GetFiles();

            int length = files.Length;
            for (int i = 0; i < length; i++)
            {
                var fileInfo = files[i];

                if (fileInfo.Exists)
                {
                    if (fileInfo.Extension == ".dll")
                    {
                        try
                        {
                            AddAssembly(fileInfo.FullName);
                        }
                        catch (Exception)
                        {
                        }
                        
                    }
                }

            }
        }

        /// <summary>
        /// 获取已添加到动态加载集合中的程序集
        /// </summary>
        /// <param name="fullName">程序集名称</param>
        /// <returns>要获取的程序集，若未找到则返回null</returns>
        public static Assembly GetAssembly(string fullName)
        {
            if (p_asses is null) throw new ArgumentNullException();
            lock (p_asses)
            {
                if(p_asses.TryGetValue(fullName, out var ass))
                {
                    return ass;
                }
            }
            return null;
        }

    }

}
