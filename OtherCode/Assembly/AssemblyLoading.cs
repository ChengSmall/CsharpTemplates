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
    /// <remarks>
    /// <para>允许程序集兼容性更新</para>
    /// </remarks>
    public static class AssemblyLoading
    {

        #region 封装

        private sealed class S_Obj
        {

            public S_Obj()
            {
                p_dict = new Dictionary<string, Assembly>(new StrEquals());
            }

            public Dictionary<string, Assembly> p_dict;
        }

        public sealed class StrEquals : EqualityComparer<string>
        {
            public override bool Equals(string x, string y)
            {
                return x == y;
            }

            public override int GetHashCode(string obj)
            {
                return obj is null ? 0 : obj.GetHashCode();
            }
        }

        static S_Obj p_lock;

        static string getSimpleAssemblyName(string fullName)
        {
            int i;
            for (i = 0; i < fullName.Length; i++)
            {
                if (fullName[i] == ',') return fullName.Substring(0, i);
            }
            return fullName;
            //return fullName.Substring(0, i + 1);
        }

        private static Assembly Dom_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assName = args.Name;
            assName = getSimpleAssemblyName(assName);
            lock (p_lock)
            {
                if (p_lock.p_dict.TryGetValue(assName, out var asses))
                {
                    return asses;
                }
            }
            return null;
        }

        static void f_addAss(Assembly ass, bool cover)
        {
            string name;

            //name = getSimpleAssemblyName(ass.FullName);
            name = ass.GetName().Name;
            lock (p_lock)
            {
                //获取name下的ass集合
                if (cover)
                {
                    p_lock.p_dict[name] = ass;
                }
                else
                {
                    if (!p_lock.p_dict.ContainsKey(name))
                    {
                        //没有直接加
                        p_lock.p_dict.Add(name, ass);
                    }
                }
                
            }
           
        }

        static int ReadBlock(Stream stream, byte[] buffer, int offset, int count)
        {
            int rsize;
            int re = 0;
            while (count != 0)
            {
                rsize = stream.Read(buffer, offset, count);
                if (rsize == 0) return re;
                offset += rsize;
                count -= rsize;
                re += rsize;
            }
            return re;
        }

        static void copyToStream(Stream stream, Stream toStream, byte[] buffer)
        {
            int length = buffer.Length;
            int rsize;

            BeginLoop:
            rsize = stream.Read(buffer, 0, length);
            if (rsize == 0) return;
            toStream.Write(buffer, 0, rsize);
            goto BeginLoop;
        }

        #endregion

        #region 功能

        /// <summary>
        /// 初始化动态程序集加载功能
        /// </summary>
        public static void InitLoading()
        {
            if(p_lock is null) p_lock = new S_Obj();
        }

        /// <summary>
        /// 对某个应用程序域注册程序集加载事件
        /// </summary>
        /// <param name="dom">要初始化的应用程序域</param>
        /// <exception cref="ArgumentNullException">参数是null或未初始化</exception>
        public static void RegisterEvent(AppDomain dom)
        {
            if (dom is null || p_lock is null) throw new ArgumentNullException();
            dom.AssemblyResolve += Dom_AssemblyResolve;
        }

        /// <summary>
        /// 对某个应用程序域注销程序集加载事件
        /// </summary>
        /// <param name="dom">要注销的应用程序域</param>
        /// <exception cref="ArgumentNullException">参数是null或未初始化</exception>
        public static void UnregisterEvent(AppDomain dom)
        {
            if (dom is null || p_lock is null) throw new ArgumentNullException();
            dom.AssemblyResolve -= Dom_AssemblyResolve;
        }

        /// <summary>
        /// 获取内部待加载程序集
        /// </summary>
        /// <returns>
        /// <para>使用键值对储存的程序集，Key表示程序集名称</para>
        /// <para>若未调用<see cref="InitLoading"/>进行初始化则返回null</para>
        /// </returns>
        public static Dictionary<string, Assembly> Assemblys
        {
            get => p_lock.p_dict;
        }

        /// <summary>
        /// 向动态加载程序集添加待加载模块
        /// </summary>
        /// <param name="raw">字节数组，表示包含程序集的基于 COFF 的映像</param>
        /// <param name="cover">若名字重复是否覆盖</param>
        /// <returns>出现的错误，没有错误则返回null</returns>
        /// <exception cref="NotSupportedException">未调用<see cref="RegisterEvent"/>进行初始化</exception>
        public static Exception AddAssembly(byte[] raw, bool cover)
        {
            if (p_lock is null) throw new NotSupportedException("未初始化加载模块");
            try
            {
                var ass = Assembly.Load(raw);
                f_addAss(ass, cover);
                return null;
                
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
        /// <exception cref="NotSupportedException">未调用<see cref="RegisterEvent"/>进行初始化</exception>
        public static Exception AddAssembly(byte[] raw)
        {
            return AddAssembly(raw, true);
        }

        /// <summary>
        /// 向动态加载程序集添加待加载模块
        /// </summary>
        /// <param name="stream">表示要加载的程序集数据，包含程序集的基于 COFF 的映像</param>
        /// <returns>出现的错误，没有错误则返回null</returns>
        /// <exception cref="NotSupportedException">未调用<see cref="RegisterEvent"/>进行初始化</exception>
        public static Exception AddAssembly(Stream stream)
        {
            return AddAssembly(stream, true, null);
        }

        /// <summary>
        /// 向动态加载程序集添加待加载模块
        /// </summary>
        /// <param name="stream">表示要加载的程序集数据，包含程序集的基于 COFF 的映像</param>
        /// <param name="cover">若程序集类型重复是否覆盖</param>
        /// <returns>出现的错误，没有错误则返回null</returns>
        /// <exception cref="NotSupportedException">未调用<see cref="RegisterEvent"/>进行初始化</exception>
        public static Exception AddAssembly(Stream stream, bool cover)
        {
            return AddAssembly(stream, cover, null);
        }

        /// <summary>
        /// 向动态加载程序集添加待加载模块
        /// </summary>
        /// <param name="stream">表示要加载的程序集数据，包含程序集的基于 COFF 的映像</param>
        /// <param name="cover">若程序集类型重复是否覆盖</param>
        /// <param name="buffer">在拷贝数据时的缓冲区，null表示采用默认大小的缓冲区（如果<paramref name="stream"/>的<see cref="Stream.CanSeek"/>为true，则该参数没有实际作用）</param>
        /// <returns>出现的错误，没有错误则返回null</returns>
        /// <exception cref="NotSupportedException">未调用<see cref="RegisterEvent"/>进行初始化</exception>
        public static Exception AddAssembly(Stream stream, bool cover, byte[] buffer)
        {
            if (p_lock is null) throw new NotSupportedException("未初始化加载模块");
            if (stream is null) return new ArgumentNullException();
            try
            {
                byte[] raw;
                if (stream.CanSeek)
                {
                    raw = new byte[stream.Length];
                    ReadBlock(stream, raw, 0, raw.Length);
                }
                else
                {
                    MemoryStream ms = new MemoryStream(1024);
                    if ((buffer is null) || (buffer.Length == 0)) buffer = new byte[1024 * 8];
                    copyToStream(stream, ms, buffer);
                    if(ms.TryGetBuffer(out var msBuf) && (msBuf.Offset == 0 && msBuf.Count == msBuf.Array.Length))
                    {
                        raw = msBuf.Array;
                    }
                    else
                    {
                        raw = ms.ToArray();
                    }
                }

                var ass = Assembly.Load(raw);
                f_addAss(ass, cover);
                return null;

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
        /// <param name="cover">若名字重复是否覆盖</param>
        /// <returns>出现的错误，没有错误则返回null</returns>
        /// <exception cref="ArgumentNullException">未调用<see cref="RegisterEvent"/>进行初始化</exception>
        public static Exception AddAssembly(string filePath, bool cover)
        {
            try
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    return AddAssembly(file, cover, null);
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
            if (p_lock is null) throw new ArgumentNullException();
            try
            {
                if (assembly is null) return new ArgumentNullException();
                    //p_asses[assembly.FullName] = assembly;
                f_addAss(assembly, cover);
                return null;

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
        /// <param name="searchOption">指示是否搜索子目录</param>
        /// <exception cref="Exception">错误</exception>
        public static void AddAssemblysByPath(string path, bool cover, SearchOption searchOption)
        {
            if (p_lock is null) throw new ArgumentNullException();
            DirectoryInfo info = new DirectoryInfo(path);

            var files = info.GetFiles("*.dll", searchOption);

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
        /// <param name="cover">若名字重复是否覆盖</param>
        /// <exception cref="Exception">错误</exception>
        public static void AddAssemblysByPath(string path, bool cover)
        {
            AddAssemblysByPath(path, cover, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// 将指定目录下的文件全部添加进待加载模块
        /// </summary>
        /// <param name="path">一个文件夹目录</param>
        /// <exception cref="Exception">错误</exception>
        public static void AddAssemblysByPath(string path)
        {
            AddAssemblysByPath(path, true, SearchOption.TopDirectoryOnly);
        }

        #endregion

    }

}
