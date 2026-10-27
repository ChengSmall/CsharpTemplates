using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Runtime;
using System.Runtime.InteropServices;

namespace Cheng.Systems
{

    /// <summary>
    /// 程序集动态加载器
    /// </summary>
    /// <remarks>
    /// <para>可直接加载或懒加载指定路径或目录的程序集</para>
    /// <para>从内存中加载或直接添加<see cref="Assembly"/>对象会在解析时直接返回对应的程序集，而使用AddLazy前缀添加的待加载程序集，会在解析时再从文件系统中读取数据并加载</para>
    /// </remarks>
    public static class LazyAssemblyLoading
    {

        #region 封装

        internal enum AssLoadType
        {
            /// <summary>
            /// 直接获取程序集的内存分离程序集
            /// </summary>
            AssemblyData = 1,

            /// <summary>
            /// 从文件路径懒加载到数据分离程序集
            /// </summary>
            LazyFileData,

            /// <summary>
            /// 直接获取程序集的文件系统程序集
            /// </summary>
            AssemblyFile,

            /// <summary>
            /// 从文件路径懒加载文件系统程序集
            /// </summary>
            LazyFile
        }

        internal struct c_loadAssPath
        {
            internal c_loadAssPath(AssLoadType type, string path)
            {
                loadAssType = type;
                this.path = path;
                assembly = null;
            }

            internal c_loadAssPath(AssLoadType type, Assembly assembly)
            {
                this.assembly = assembly;
                loadAssType = type;
                path = null;
            }

            internal string path;
            internal Assembly assembly;
            internal AssLoadType loadAssType;
        }

        private sealed class S_Obj
        {

            internal S_Obj()
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    p_fileNameEqual = StringComparer.Ordinal;
                }
                else
                {
                    p_fileNameEqual = StringComparer.OrdinalIgnoreCase;
                }

                p_dict = new Dictionary<string, c_loadAssPath>(p_fileNameEqual);
                p_pathList = new List<c_loadAssPath>();
                p_loadedAssDict = new Dictionary<string, Assembly>(p_fileNameEqual);
                p_registerFunc = Dom_AssemblyResolve;
                p_registerOtherFunc = null;
                p_runningAssembly = null;
            }

            internal IEqualityComparer<string> p_fileNameEqual;
            internal Dictionary<string, c_loadAssPath> p_dict;
            internal List<c_loadAssPath> p_pathList;
            internal Assembly p_runningAssembly;
            internal Dictionary<string, Assembly> p_loadedAssDict;

            internal ResolveEventHandler p_registerFunc;
            internal ResolveEventHandler p_registerOtherFunc;

            static Assembly f_loadAssToFileData(string filePath, bool isOnlyRef)
            {
                byte[] raw;
                using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 4, FileOptions.SequentialScan))
                {
                    raw = streamToBytes(file, null);
                }
                if (isOnlyRef)
                {
                    return Assembly.ReflectionOnlyLoad(raw);
                }
                else return Assembly.Load(raw);

            }

            private Assembly f_resolveAss(object sender, ResolveEventArgs args, string ename, bool isOnlyRef)
            {
                Assembly ass = null;

                c_loadAssPath asses;
                // 搜索精准程序集
                if (p_dict.TryGetValue(ename, out asses))
                {

                    // 存在精准程序集
                    try
                    {
                        if (asses.loadAssType == AssLoadType.AssemblyData || asses.loadAssType == AssLoadType.AssemblyFile)
                        {
                            ass = asses.assembly;
                        }
                        else
                        {
                            if (asses.loadAssType == AssLoadType.LazyFile)
                            {
                                // 懒加载文件系统
                                if (isOnlyRef)
                                {
                                    ass = Assembly.ReflectionOnlyLoadFrom(asses.path);
                                }
                                else
                                {
                                    ass = Assembly.LoadFrom(asses.path);
                                }
                            }
                            else
                            {
                                // 懒加载文件数据分离
                                ass = f_loadAssToFileData(asses.path, isOnlyRef);
                            }

                        }
                    }
                    catch (Exception)
                    {
                        ass = null;
                    }
                }

                if (ass is null)
                {
                    foreach (var path in p_pathList)
                    {
                        try
                        {
                            // 扫描目录

                            var direInfo = new DirectoryInfo(path.path);
                            var filesEnumator = direInfo.EnumerateFiles("*", SearchOption.TopDirectoryOnly);

                            foreach (var fileInfo in filesEnumator)
                            {
                                try
                                {
                                    // 当前扫描的文件路径
                                    var fileFullPath = fileInfo.FullName;
                                    // 要搜索的文件名
                                    var sfileName = Path.GetFileNameWithoutExtension(fileFullPath);
                                    // 对比文件名属于要解析的ass名
                                    if (p_fileNameEqual.Equals(sfileName, ename))
                                    {
                                        // 比较成功
                                        if (path.loadAssType == AssLoadType.LazyFile)
                                        {
                                            // 加载文件系统程序集
                                            if (isOnlyRef)
                                            {
                                                ass = Assembly.ReflectionOnlyLoadFrom(fileFullPath);
                                            }
                                            else
                                            {
                                                ass = Assembly.LoadFrom(fileFullPath);
                                            }

                                            goto Over;
                                        }
                                        else if (path.loadAssType == AssLoadType.LazyFileData)
                                        {
                                            // 加载内存分离程序集
                                            f_loadAssToFileData(fileFullPath, isOnlyRef);

                                            goto Over;
                                        }


                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }

                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                Over:
                if (ass is null) ass = p_registerOtherFunc?.Invoke(sender, args);
                return ass;
            }

            private Assembly Dom_AssemblyResolve(object sender, ResolveEventArgs args)
            {
                lock (this)
                {
                    Assembly ass;
                    var assName = getSimpleAssemblyName(args.Name);
                    if(p_loadedAssDict.TryGetValue(assName, out ass))
                    {
                        // 返回已缓存的程序集
                        return ass;
                    }
                    ass = f_resolveAss(sender, args, assName, false);
                    p_loadedAssDict[assName] = ass;
                    return ass;
                }
            }

        }

        static S_Obj p_lock;

        static bool f_assIsMemoryLoad(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            // 动态生成
            // 必然是纯内存对象，且访问 Location 会抛 NotSupportedException，必须先判断
            if (assembly.IsDynamic)
                return true;

            string location;
            try
            {
                location = assembly.Location;
            }
            catch (NotSupportedException)
            {
                return true;
            }
            // Location 为空 通过 byte[] 加载
            return string.IsNullOrEmpty(location);
        }

        static string getSimpleAssemblyName(string fullName)
        {
            int i;
            for (i = 0; i < fullName.Length; i++)
            {
                if (fullName[i] == ',') return fullName.Substring(0, i);
            }
            return fullName;
        }

        static void f_addAss(c_loadAssPath assp, bool cover)
        {
            string name;
            name = Path.GetFileNameWithoutExtension(assp.path);
            //lock (p_lock)

            //获取name下的ass集合
            if (cover)
            {
                p_lock.p_dict[name] = assp;
            }
            else
            {
                if (!p_lock.p_dict.ContainsKey(name))
                {
                    //没有直接加
                    p_lock.p_dict.Add(name, assp);
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

        static byte[] streamToBytes(Stream stream, byte[] buffer)
        {
            byte[] raw;
            if (stream.CanSeek)
            {
                if (stream.Length > int.MaxValue) throw new OutOfMemoryException("要加载的数据量超出int类型大小");
                raw = new byte[stream.Length];
                ReadBlock(stream, raw, 0, raw.Length);
            }
            else
            {
                MemoryStream ms = new MemoryStream(1024);
                if ((buffer is null) || (buffer.Length == 0)) buffer = new byte[1024 * 8];
                copyToStream(stream, ms, buffer);
                if (ms.TryGetBuffer(out var msBuf) && (msBuf.Offset == 0 && msBuf.Count == msBuf.Array.Length))
                {
                    raw = msBuf.Array;
                }
                else
                {
                    raw = ms.ToArray();
                }
            }
            return raw;
        }

        #endregion

        #region 功能

        #region 参数

        /// <summary>
        /// 为程序域注册程序集解析事件的内部委托对象
        /// </summary>
        internal static ResolveEventHandler PrivateAssemblyResolveEvent
        {
            get => p_lock?.p_registerFunc;
        }

        /// <summary>
        /// 加载器内待加载程序集名称集合
        /// </summary>
        internal static IReadOnlyCollection<string> AssemblyLoadingNames
        {
            get
            {
                if (p_lock is null) return null;
                return p_lock.p_dict.Keys;
            }
        }

        /// <summary>
        /// 外部解析程序集方法，在未查找到程序集时调用该函数解析程序集
        /// </summary>
        /// <value>null表示不使用外部解析，默认为null</value>
        internal static ResolveEventHandler OtherResolveFunc
        {
            get
            {
                if (p_lock is null) throw new ArgumentNullException();
                lock (p_lock)
                {
                    return p_lock.p_registerOtherFunc;
                }
            }
            set
            {
                if (p_lock is null) throw new ArgumentNullException();
                lock (p_lock)
                {
                    p_lock.p_registerOtherFunc = value;
                }
            }
        }

        /// <summary>
        /// 已成功解析的程序集对象
        /// </summary>
        internal static IReadOnlyCollection<Assembly> LoadedAssemblys
        {
            get
            {
                return p_lock.p_loadedAssDict.Values;
            }
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化动态程序集加载功能
        /// </summary>
        /// <param name="assembly">当前的运行时模块程序集</param>
        public static void InitLoading(Assembly assembly)
        {
            p_lock = new S_Obj();
            p_lock.p_runningAssembly = assembly;
            if (p_lock.p_runningAssembly is null)
            {
                p_lock.p_runningAssembly = Assembly.GetEntryAssembly();
            }
            if (p_lock.p_runningAssembly is null)
            {
                p_lock.p_runningAssembly = Assembly.GetExecutingAssembly();
            }
            if (p_lock.p_runningAssembly is null)
            {
                p_lock.p_runningAssembly = typeof(LazyAssemblyLoading).Assembly;
            }
        }

        /// <summary>
        /// 对某个应用程序域注册程序集加载事件
        /// </summary>
        /// <param name="dom">要初始化的应用程序域</param>
        /// <exception cref="ArgumentNullException">参数是null或未初始化</exception>
        internal static void RegisterEvent(AppDomain dom)
        {
            if (dom is null || p_lock is null) throw new ArgumentNullException();
            lock (p_lock) dom.AssemblyResolve += p_lock.p_registerFunc;
        }

        /// <summary>
        /// 对某个应用程序域注销程序集加载事件
        /// </summary>
        /// <param name="dom">要注销的应用程序域</param>
        /// <exception cref="ArgumentNullException">参数是null或未初始化</exception>
        internal static void UnregisterEvent(AppDomain dom)
        {
            if (dom is null || p_lock is null) throw new ArgumentNullException();
            lock (p_lock) dom.AssemblyResolve -= p_lock.p_registerFunc;
        }

        #endregion

        #region 加载 Assembly

        /// <summary>
        /// 向程序集直接添加待加载模块
        /// </summary>
        /// <param name="assembly">待加载的程序集模块</param>
        /// <param name="cover">若名字重复是否覆盖</param>
        /// <returns>出现的错误，没有错误则返回null</returns>
        /// <exception cref="ArgumentNullException">未调用<see cref="RegisterEvent"/>进行初始化</exception>
        internal static Exception AddAssembly(Assembly assembly, bool cover)
        {
            if (p_lock is null) throw new ArgumentNullException();
            try
            {
                if (assembly is null) return new ArgumentNullException();
                var ismem = f_assIsMemoryLoad(assembly);
                f_addAss(new c_loadAssPath(ismem ? AssLoadType.AssemblyData : AssLoadType.AssemblyFile, assembly), cover);
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>
        /// 向程序集直接添加待加载模块
        /// </summary>
        /// <param name="assembly">待加载的程序集模块</param>
        /// <returns>出现的错误，没有错误则返回null</returns>
        /// <exception cref="ArgumentNullException">未调用<see cref="RegisterEvent"/>进行初始化</exception>
        internal static Exception AddAssembly(Assembly assembly)
        {
            return AddAssembly(assembly, true);
        }

        #region 数据隔离

        /// <summary>
        /// 向程序集直接添加待加载模块
        /// </summary>
        /// <param name="stream">表示要加载的程序集数据，包含程序集的基于 COFF 的映像</param>
        /// <param name="cover">若程序集类型重复是否覆盖</param>
        /// <param name="buffer">在拷贝数据时的缓冲区，null表示采用默认大小的缓冲区（如果<paramref name="stream"/>的<see cref="Stream.CanSeek"/>为true，则该参数没有实际作用）</param>
        /// <returns>出现的错误，没有错误则返回null</returns>
        /// <exception cref="NotSupportedException">未调用<see cref="RegisterEvent"/>进行初始化</exception>
        internal static Exception AddDataAssembly(Stream stream, bool cover, byte[] buffer)
        {
            if (p_lock is null) throw new NotSupportedException("未初始化加载模块");
            if (stream is null) return new ArgumentNullException();
            try
            {
                byte[] raw;
                if (stream.CanSeek)
                {
                    if (stream.Length > int.MaxValue) throw new OutOfMemoryException("要加载的数据量超出int类型大小");
                    raw = new byte[stream.Length];
                    ReadBlock(stream, raw, 0, raw.Length);
                }
                else
                {
                    MemoryStream ms = new MemoryStream(1024);
                    if ((buffer is null) || (buffer.Length == 0)) buffer = new byte[1024 * 8];
                    copyToStream(stream, ms, buffer);
                    if (ms.TryGetBuffer(out var msBuf) && (msBuf.Offset == 0 && msBuf.Count == msBuf.Array.Length))
                    {
                        raw = msBuf.Array;
                    }
                    else
                    {
                        raw = ms.ToArray();
                    }
                }

                var ass = Assembly.Load(raw);
                f_addAss(new c_loadAssPath(AssLoadType.AssemblyData, ass), cover);
                return null;

            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>
        /// 向程序集直接添加待加载模块
        /// </summary>
        /// <param name="stream">表示要加载的程序集数据，包含程序集的基于 COFF 的映像</param>
        /// <param name="cover">若程序集类型重复是否覆盖</param>
        /// <returns>出现的错误，没有错误则返回null</returns>
        /// <exception cref="NotSupportedException">未调用<see cref="RegisterEvent"/>进行初始化</exception>
        internal static Exception AddDataAssembly(Stream stream, bool cover)
        {
            return AddDataAssembly(stream, cover, null);
        }

        /// <summary>
        /// 添加字节数组的待加载程序集
        /// </summary>
        /// <param name="raw">字节数组，表示包含程序集的基于 COFF 的映像</param>
        /// <param name="cover">若名字重复是否覆盖</param>
        /// <returns>出现的错误，没有错误则返回null</returns>
        /// <exception cref="NotSupportedException">未调用<see cref="RegisterEvent"/>进行初始化</exception>
        internal static Exception AddDataAssembly(byte[] raw, bool cover)
        {
            if (p_lock is null) throw new NotSupportedException("未初始化加载模块");
            try
            {
                var ass = Assembly.Load(raw);
                f_addAss(new c_loadAssPath(AssLoadType.AssemblyData, ass), cover);
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>
        /// 添加字节数组的待加载程序集
        /// </summary>
        /// <param name="raw">字节数组，表示包含程序集的基于 COFF 的映像</param>
        /// <returns>出现的错误，没有错误则返回null</returns>
        /// <exception cref="NotSupportedException">未调用<see cref="RegisterEvent"/>进行初始化</exception>
        internal static Exception AddDataAssembly(byte[] raw)
        {
            return AddDataAssembly(raw, true);
        }

        #endregion

        #region 懒加载数据隔离

        /// <summary>
        /// 向程序集添加懒加载模块文件路径
        /// </summary>
        /// <param name="filePath">包含程序集清单的文件的名称或路径</param>
        /// <param name="cover">若名字重复是否覆盖</param>
        /// <returns>出现的错误，没有错误则返回null</returns>
        /// <exception cref="ArgumentNullException">未调用<see cref="RegisterEvent"/>进行初始化</exception>
        internal static Exception AddLazyDataAssembly(string filePath, bool cover)
        {
            if (p_lock is null) throw new ArgumentNullException();
            try
            {
                lock (p_lock)
                {
                    f_addAss(new c_loadAssPath(AssLoadType.LazyFileData, filePath), cover);
                }
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>
        /// 向程序集添加懒加载模块文件路径
        /// </summary>
        /// <param name="filePath">包含程序集清单的文件的名称或路径</param>
        /// <returns>出现的错误，没有错误则返回null</returns>
        /// <exception cref="ArgumentNullException">未调用<see cref="RegisterEvent"/>进行初始化</exception>
        internal static Exception AddLazyDataAssembly(string filePath)
        {
            return AddLazyDataAssembly(filePath, true);
        }

        /// <summary>
        /// 将指定目录设置为懒加载模块
        /// </summary>
        /// <param name="path">一个文件夹目录</param>
        /// <param name="cover">若名字重复是否覆盖</param>
        /// <param name="searchOption">指示是否搜索子目录</param>
        /// <exception cref="Exception">错误</exception>
        internal static void AddLazyDataAssemblysByPath(string path, bool cover, SearchOption searchOption)
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
                        AddLazyDataAssembly(fileInfo.FullName, cover);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 将指定目录设置为懒加载模块
        /// </summary>
        /// <param name="path">一个文件夹目录</param>
        /// <param name="cover">若名字重复是否覆盖</param>
        /// <exception cref="Exception">错误</exception>
        internal static void AddLazyDataAssemblysByPath(string path, bool cover)
        {
            AddLazyDataAssemblysByPath(path, cover, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// 将指定目录设置为懒加载模块
        /// </summary>
        /// <param name="path">一个文件夹目录</param>
        /// <exception cref="Exception">错误</exception>
        internal static void AddLazyDataAssemblysByPath(string path)
        {
            AddLazyDataAssemblysByPath(path, true, SearchOption.TopDirectoryOnly);
        }

        #endregion

        #region 懒加载

        /// <summary>
        /// 向程序集添加懒加载模块文件路径
        /// </summary>
        /// <param name="filePath">包含程序集清单的文件的名称或路径</param>
        /// <param name="cover">若名字重复是否覆盖</param>
        /// <returns>出现的错误，没有错误则返回null</returns>
        /// <exception cref="ArgumentNullException">未调用<see cref="RegisterEvent"/>进行初始化</exception>
        internal static Exception AddLazyAssembly(string filePath, bool cover)
        {
            if (p_lock is null) throw new ArgumentNullException();
            try
            {
                lock (p_lock)
                {
                    f_addAss(new c_loadAssPath(AssLoadType.LazyFile, filePath), cover);
                }
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>
        /// 向程序集添加懒加载模块文件路径
        /// </summary>
        /// <param name="filePath">包含程序集清单的文件的名称或路径</param>
        /// <returns>出现的错误，没有错误则返回null</returns>
        /// <exception cref="ArgumentNullException">未调用<see cref="RegisterEvent"/>进行初始化</exception>
        internal static Exception AddLazyAssembly(string filePath)
        {
            return AddLazyAssembly(filePath, true);
        }

        /// <summary>
        /// 将指定目录设置为懒加载模块
        /// </summary>
        /// <param name="path">一个文件夹目录</param>
        /// <param name="cover">若名字重复是否覆盖</param>
        /// <param name="searchOption">指示是否搜索子目录</param>
        /// <exception cref="Exception">错误</exception>
        internal static void AddLazyAssemblysByPath(string path, bool cover, SearchOption searchOption)
        {
            if (p_lock is null) throw new ArgumentNullException();
            DirectoryInfo info = new DirectoryInfo(path);

            var files = info.GetFiles("*", searchOption);

            int length = files.Length;
            for (int i = 0; i < length; i++)
            {
                var fileInfo = files[i];

                if (fileInfo.Exists)
                {

                    try
                    {
                        AddLazyAssembly(fileInfo.FullName, cover);
                    }
                    catch (Exception)
                    {
                    }
                }

            }
        }

        /// <summary>
        /// 将指定目录设置为懒加载模块
        /// </summary>
        /// <param name="path">一个文件夹目录</param>
        /// <param name="cover">若名字重复是否覆盖</param>
        /// <exception cref="Exception">错误</exception>
        internal static void AddLazyAssemblysByPath(string path, bool cover)
        {
            AddLazyAssemblysByPath(path, cover, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// 将指定目录设置为懒加载模块
        /// </summary>
        /// <param name="path">一个文件夹目录</param>
        /// <exception cref="Exception">错误</exception>
        internal static void AddLazyAssemblysByPath(string path)
        {
            AddLazyAssemblysByPath(path, true, SearchOption.TopDirectoryOnly);
        }

        #endregion

        #endregion

        #region 其它功能

        /// <summary>
        /// 裁切多余的缓存容量
        /// </summary>
        /// <exception cref="NotImplementedException">未初始化</exception>
        internal static void TrimExcessBuffer()
        {
            if (p_lock is null) throw new NotImplementedException();
            lock (p_lock)
            {
                p_lock.p_pathList.Capacity = p_lock.p_pathList.Count;
                var dict = new Dictionary<string, c_loadAssPath>(p_lock.p_dict, p_lock.p_dict.Comparer);
                p_lock.p_dict = dict;
                var ddict = new Dictionary<string, Assembly>(p_lock.p_loadedAssDict, p_lock.p_loadedAssDict.Comparer);
                p_lock.p_loadedAssDict = ddict;
            }
        }

        #endregion

        #endregion

    }

}
