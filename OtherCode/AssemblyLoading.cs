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

        #region 封装

        static Dictionary<string, Assembly> p_asses;

        static string getSimpleAssemblyName(string fullName)
        {
            int i;
            for (i = 0; i < fullName.Length; i++)
            {
                if (fullName[i] == ',') return fullName.Substring(0, i + 1);
            }
            return fullName;
            //return fullName.Substring(0, i + 1);
        }

        private static Assembly Dom_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assName = args.Name;
            assName = getSimpleAssemblyName(assName);
            if(p_asses.TryGetValue(assName, out var asses))
            {
                return asses;
            }
            return null;
        }

        static void f_addAss(Assembly ass, bool cover)
        {
            string name;

            name = getSimpleAssemblyName(ass.FullName);
            //获取name下的ass集合
            bool b = p_asses.ContainsKey(name);
            if (b)
            {
                //有
                if (cover)
                {
                    p_asses[name] = ass;
                }
            }
            else
            {
                //没有直接加
                p_asses.Add(name, ass);
            }
        }

        #endregion

        #region 功能

        /// <summary>
        /// 初始化动态程序集加载功能
        /// </summary>
        public static void InitLoading()
        {
            if(p_asses is null) p_asses = new Dictionary<string, Assembly>();
        }

        /// <summary>
        /// 对某个应用程序域注册程序集加载事件
        /// </summary>
        /// <param name="dom">要初始化的应用程序域</param>
        /// <exception cref="ArgumentNullException">参数是null或未初始化</exception>
        public static void RegisterEvent(AppDomain dom)
        {
            if (dom is null || p_asses is null) throw new ArgumentNullException();
            dom.AssemblyResolve += Dom_AssemblyResolve;
        }

        /// <summary>
        /// 获取内部待加载程序集
        /// </summary>
        /// <returns>
        /// <para>使用键值对储存的程序集，Key表示程序集的<see cref="Assembly.FullName"/></para>
        /// <para>若未调用<see cref="RegisterEvent(AppDomain)"/>进行初始化则返回null</para>
        /// </returns>
        public static Dictionary<string, Assembly> Assemblys
        {
            get => p_asses;
        }

        /// <summary>
        /// 向动态加载程序集添加待加载模块
        /// </summary>
        /// <param name="raw">字节数组，表示包含程序集的基于 COFF 的映像</param>
        /// <param name="cover">若名字重复是否覆盖</param>
        /// <returns>出现的错误，没有错误则返回null</returns>
        /// <exception cref="ArgumentNullException">未调用<see cref="RegisterEvent"/>进行初始化</exception>
        public static Exception AddAssembly(byte[] raw, bool cover)
        {
            if (p_asses is null) throw new ArgumentNullException();
            try
            {
                lock (p_asses)
                {
                    var ass = Assembly.Load(raw);
                    f_addAss(ass, cover);
                    //p_asses[ass.FullName] = ass;
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
        /// <param name="raw">字节数组，表示包含程序集的基于 COFF 的映像</param>
        /// <returns>出现的错误，没有错误则返回null</returns>
        /// <exception cref="ArgumentNullException">未调用<see cref="RegisterEvent"/>进行初始化</exception>
        public static Exception AddAssembly(byte[] raw)
        {
            return AddAssembly(raw, true);
        }

        /// <summary>
        /// 向动态加载程序集添加待加载模块
        /// </summary>
        /// <param name="filePath">包含程序集清单的文件的名称或路径</param>
        /// <param name="cover">若名字重复是否覆盖</param>
        /// <returns>出现的错误，没有错误则返回null</returns>
        /// <exception cref="ArgumentNullException">未调用<see cref="RegisterEvent"/>进行初始化</exception>
        public static Exception AddAssembly(string filePath, bool cover)
        {
            if (p_asses is null) throw new ArgumentNullException();
            try
            {
                lock (p_asses)
                {
                    var ass = Assembly.LoadFrom(filePath);
                    f_addAss(ass, cover);
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
        /// <exception cref="ArgumentNullException">未调用<see cref="RegisterEvent"/>进行初始化</exception>
        public static Exception AddAssembly(string filePath)
        {
            return AddAssembly(filePath, true);
        }

        /// <summary>
        /// 向动态加载程序集添加待加载模块
        /// </summary>
        /// <param name="assembly">待加载的程序集模块</param>
        /// <param name="cover">若名字重复是否覆盖</param>
        /// <returns>出现的错误，没有错误则返回null</returns>
        /// <exception cref="ArgumentNullException">未调用<see cref="RegisterEvent"/>进行初始化</exception>
        public static Exception AddAssembly(Assembly assembly, bool cover)
        {
            if (p_asses is null) throw new ArgumentNullException();
            try
            {
                if (assembly is null) return new ArgumentNullException();
                lock (p_asses)
                {
                    //p_asses[assembly.FullName] = assembly;
                    f_addAss(assembly, cover);
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
        /// <exception cref="ArgumentNullException">未调用<see cref="RegisterEvent"/>进行初始化</exception>
        public static Exception AddAssembly(Assembly assembly)
        {
            return AddAssembly(assembly, true);
        }

        /// <summary>
        /// 将指定目录下的文件全部添加进待加载模块
        /// </summary>
        /// <param name="path">一个文件夹目录</param>
        /// <param name="cover">若名字重复是否覆盖</param>
        /// <exception cref="Exception">错误</exception>
        public static void AddAssemblysByPath(string path, bool cover)
        {
            if (p_asses is null) throw new ArgumentNullException();
            DirectoryInfo info = new DirectoryInfo(path);

            var files = info.GetFiles("*.dll");

            int length = files.Length;
            for (int i = 0; i < length; i++)
            {
                var fileInfo = files[i];

                if (fileInfo.Exists)
                {
                    
                    try
                    {
                        AddAssembly(fileInfo.FullName, cover);
                    }
                    catch (Exception)
                    {
                    }
                }

            }
        }

        /// <summary>
        /// 将指定目录下的文件全部添加进待加载模块
        /// </summary>
        /// <param name="path">一个文件夹目录</param>
        /// <exception cref="Exception">错误</exception>
        public static void AddAssemblysByPath(string path)
        {
            AddAssemblysByPath(path, false);
        }

        #endregion

    }

}
