using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Reflection;

using Cheng.Algorithm.Compressions;
using Cheng.Memorys;
using Cheng.Streams;
using Cheng.IO;

namespace Cheng.Algorithm.Compressions.ResourcePackages
{

    /// <summary>
    /// 资源打包器
    /// </summary>
    /// <remarks>
    /// 配合<see cref="ResourcePackageReader"/>使用的资源打包器，被该对象打包的数据能够被<see cref="ResourcePackageReader"/>读取
    /// </remarks>
    public unsafe class ResourcePackageWriter
    {

        #region 结构

        #endregion

        #region 构造

        /// <summary>
        /// 实例化资源打包器
        /// </summary>
        /// <param name="buffSize">指定数据打包和拷贝时的临时缓冲区，默认为16384</param>
        /// <exception cref="ArgumentOutOfRangeException">参数不得小于或等于0</exception>
        public ResourcePackageWriter(int buffSize)
        {
            if (buffSize <= 0) throw new ArgumentOutOfRangeException();

            f_init(buffSize);
        }

        /// <summary>
        /// 实例化资源打包器
        /// </summary>
        public ResourcePackageWriter()
        {
            f_init(1024 * 16);
        }

        private void f_init(int bufSize)
        {
            p_list = new List<FileInfoIndex>();
            p_transBuffer = new byte[16];

            p_notPaths = cp_notPaths;
            p_notFiles = cp_notFiles;

            p_notPathAndFiles = f_mergeAndRemoveDuplicates(p_notPaths, p_notFiles);

            p_copyBytesBuffer = new byte[bufSize];
        }

        static char[] f_mergeAndRemoveDuplicates(char[] array1, char[] array2)
        {

            HashSet<char> uniqueChars = new HashSet<char>();

            int length;

            length = System.Math.Max(array1.Length, array2.Length);

            for (int i = 0; i < length; i++)
            {
                if(i < array1.Length)
                {
                    uniqueChars.Add(array1[i]);
                }
                if (i < array2.Length)
                {
                    uniqueChars.Add(array2[i]);
                }
            }

            char[] result = new char[uniqueChars.Count];
            uniqueChars.CopyTo(result);

            return result;
        }

        #endregion

        #region 参数

        #region 常量

        const byte cp_notIndex = 0;

        const byte cp_haveIndex = byte.MaxValue;

        private static char[] cp_notPaths = Path.GetInvalidPathChars();

        private static char[] cp_notFiles = Path.GetInvalidFileNameChars();

        #endregion

        /// <summary>
        /// 待打包文件
        /// </summary>
        private List<FileInfoIndex> p_list;

        private byte[] p_transBuffer;

        /// <summary>
        /// 不允许在路径中出现的字符
        /// </summary>
        private char[] p_notPaths;

        /// <summary>
        /// 禁止在文件名中出现的字符
        /// </summary>
        private char[] p_notFiles;

        /// <summary>
        /// 禁止在文件名和路径名中出现的字符
        /// </summary>
        private char[] p_notPathAndFiles;

        /// <summary>
        /// 字节转化缓冲区
        /// </summary>
        private byte[] p_copyBytesBuffer;

        private Exception p_errorException;

        private bool p_packing;

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 当前对象是否正在打包资源
        /// </summary>
        /// <returns>
        /// <para>对象调用<see cref="ToPack(Stream)"/>时，或调用了<see cref="ToPackEnumator(Stream)"/>且没有运行完成，返回true；除此之外返回false</para>
        /// </returns>
        public bool Packing
        {
            get => p_packing;
        }

        /// <summary>
        /// 即将要打包的一些文件
        /// </summary>
        /// <remarks>
        /// <para>修改该集合，调整要打包的文件，准被完成后调用<see cref="ToPack(Stream)"/>或<see cref="ToPackEnumator(Stream)"/>开始打包</para>
        /// <para>这个集合内不要有重复的元素，否则可能会在打包时添加永远无法读取到的数据</para>
        /// </remarks>
        public List<FileInfoIndex> PackFiles
        {
            get => p_list;
        }

        /// <summary>
        /// 根据当前待打包文件计算数据包索引写入到流数据的长度
        /// </summary>
        /// <returns>计算数据头写入到流数据的长度</returns>
        public long CalculatedFileIndexByteSize()
        {
            return CalculatedFileIndexSize(this.p_list);
        }

        /// <summary>
        /// 根据指定的待打包文件计算数据包索引写入到流数据的长度
        /// </summary>
        /// <param name="list">待打包文件集合</param>
        /// <returns>写入到流数据的长度</returns>
        /// <exception cref="ArgumentNullException">集合为null</exception>
        public static long CalculatedFileIndexSize(IList<FileInfoIndex> list)
        {
            if (list is null) throw new ArgumentNullException();

            long re = 0;

            int i;

            int length = list.Count;
            for (i = 0; i < length; i++)
            {
                //+1头 +2长度数据 + 16固定包索引
                re += (1 + 2 + 16);

                var fi = list[i];

                //加字符数量
                if(fi.dataPath is null)
                {
                    throw new ArgumentException();
                }
                else
                {
                    re += fi.dataPath.Length * sizeof(char);
                }
                

                //re += 16;
            }
            re++;

            return re;
        }

        /// <summary>
        /// 上一次进行打包时可能产生的错误，若还没有打包或调用了<see cref="ResetLastException"/>后还没开始打包则为null
        /// </summary>
        public Exception LastThrowException
        {
            get => p_errorException;
        }

        /// <summary>
        /// 重置<see cref="LastThrowException"/>参数为null
        /// </summary>
        public void ResetLastException()
        {
            p_errorException = null;
        }

        #endregion

        #region 参数调整和判断

        /// <summary>
        /// 清除对象内部多余的缓冲区
        /// </summary>
        /// <remarks>
        /// 调用该函数会清空内部缓冲区，包括<see cref="PackFiles"/>中的所有元素
        /// </remarks>
        public void ClearBuffered()
        {
            p_list.Clear();
            if(p_list.Capacity > 16) p_list.Capacity = 4;
            
            if(p_transBuffer.Length > 64)
            {
                p_transBuffer = new byte[64];
            }

        }

        /// <summary>
        /// 验证当前即将要打包的文件是否全部存在
        /// </summary>
        /// <param name="exception">验证过程中发生的异常，没有则为null</param>
        /// <returns>即将要打包的文件全部存在返回true，有不存在的文件或其它错误返回false</returns>
        public bool IsAllFileExists(out Exception exception)
        {

            var list = p_list;
            int length = list.Count;
            try
            {
                exception = null;
                for (int i = 0; i < length; i++)
                {
                    var file = list[i];

                    if (file.file is null)
                    {
                        return false;
                    }

                    if (!file.file.Exists)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
           

        }

        /// <summary>
        /// 验证当前即将要打包的文件要打包的位置是否符合语法规定
        /// </summary>
        /// <remarks>
        /// <para>待打包集合<see cref="PackFiles"/>中，如果有元素的<see cref="FileInfoIndex.dataPath"/>不符合相对路径标准</para>
        /// </remarks>
        /// <returns>符合语法规定返回true，不符合或其它错误返回false</returns>
        public bool FileToPackPathIsTrue()
        {
            var list = p_list;
            int length = list.Count;
            
            for (int i = 0; i < length; i++)
            {
                var file = list[i];

                if (string.IsNullOrEmpty(file.dataPath))
                {
                    return false;
                }
                if(!f_isValidFolderPath(file.dataPath, p_notPathAndFiles))
                {
                    return false;
                }

            }

            return true;
                       
        }

        /// <summary>
        /// 判断全是空白字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static bool f_strIsNullOrEmpty(string str)
        {
            if (string.IsNullOrEmpty(str)) return true;

            int length = str.Length;
            for (int i = 0; i < length; i++)
            {
                char c = str[i];
                switch (c)
                {
                    case ' ':
                    case '\t':
                    case '\b':
                    case '\a':
                    case '\u3000':
                        //case '':
                        continue;
                    default: //有非空字符
                        return false;
                }
            }

            return false;

        }

        /// <summary>
        /// 检查字符串是否符合指定语法
        /// </summary>
        /// <param name="path">要检查的字符串，不可为null</param>
        /// <param name="noPathChars">不应存在字符串存在的字符组</param>
        /// <returns>符合返回true</returns>
        static bool f_isValidFolderPath(string path, char[] noPathChars)
        {

            string normalizedPath = path.Replace('/', '\\').Trim('\\');

            if (f_strIsNullOrEmpty(normalizedPath) || normalizedPath == ".")
            {
                return true;
            }

            if (normalizedPath.StartsWith("..\\", StringComparison.OrdinalIgnoreCase) ||
                normalizedPath.StartsWith("\\", StringComparison.Ordinal))
            {
                return false;
            }

            string[] segments = normalizedPath.Split('\\');
            foreach (var segment in segments)
            {

                if (f_strIsNullOrEmpty(segment))
                {
                    return false;
                }

                var idof = segment.IndexOfAny(noPathChars);

                if (idof >= 0)
                {
                    //找到无效字符
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 为所有待打包文件的打包到的路径添加根路径
        /// </summary>
        /// <remarks>
        /// <para>对<see cref="PackFiles"/>内的所有值，里边的<see cref="FileInfoIndex.dataPath"/>参数统一将<paramref name="baseDirectory"/>组合到前边</para>
        /// </remarks>
        /// <param name="baseDirectory">要添加的根路径</param>
        /// <exception cref="ArgumentException">参数格式不正确</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public void AddRootDirectory(string baseDirectory)
        {
            if (baseDirectory is null) throw new ArgumentNullException();
            if (baseDirectory.Length == 0) throw new ArgumentException();
            if(!f_isValidFolderPath(baseDirectory, p_notPaths))
            {
                throw new ArgumentException();
            }

            int length = p_list.Count;
            FileInfoIndex fi;
            for (int i = 0; i < length; i++)
            {
                fi = p_list[i];

                p_list[i] = new FileInfoIndex(fi.file, Path.Combine(baseDirectory, fi.dataPath));
            }

        }

        #endregion

        #region 打包

        private IEnumerable f_toPackenumator(Stream pack, long fileIndexSize)
        {

            long beginPos;
            //流的初始位置
            try
            {
                beginPos = pack.Position;
            }
            catch (Exception ex)
            {
                p_errorException = ex;
                p_packing = false;
                yield break;
            }
                
            //待打包文件
            var list = p_list;

            //数据所在位置起点
            long dataHand = beginPos + fileIndexSize;

            int i;

            FileInfoIndex fi;

            long nextSize;
            long allLen = 0;

            for (i = 0, nextSize = dataHand; i < list.Count; i++)
            {
                //获取第i个文件
                fi = list[i];

                if (fi.file is null || string.IsNullOrEmpty(fi.dataPath))
                {
                    //错误
                    p_packing = false;
                    yield break;
                }
                if (fi.dataPath.Length > ushort.MaxValue)
                {
                    //错误
                    p_packing = false;
                    yield break;
                }

                ushort strLen;
                int strSize;
                long fileSize;

                try
                {
                    //写入有数据头
                    pack.WriteByte(cp_haveIndex);

                    //写入字符串长度
                    strLen = (ushort)fi.dataPath.Length;

                    strLen.ToByteArray(p_transBuffer);

                    pack.Write(p_transBuffer, 0, 2);

                    //写入字符串
                    //字符串字节长度
                    strSize = strLen * 2;
                    if (p_transBuffer.Length < strSize)
                    {
                        var ns = p_transBuffer.Length * 2;
                        p_transBuffer = new byte[ns > strSize ? ns : strSize];
                    }

                    fi.dataPath.ToByteArray(0, p_transBuffer, 0);
                    //写入数据
                    pack.Write(p_transBuffer, 0, strSize);

                    //写入数据位置和长度

                    fileSize = fi.file.Length;

                    //位置
                    nextSize.ToByteArray(p_transBuffer);
                    pack.Write(p_transBuffer, 0, sizeof(long));

                    //长度
                    fileSize.ToByteArray(p_transBuffer);
                    pack.Write(p_transBuffer, 0, sizeof(long));

                }
                catch (Exception ex)
                {
                    p_errorException = ex;
                    yield break;
                }
                

                //推进下一个文件位置
                nextSize += fileSize;

                allLen += fileSize;

                yield return fi;

            }

            //写入终止符
            pack.WriteByte(cp_notIndex);

            FileStream readFile;

            //开始写入文件
            for (i = 0; i < list.Count; i++)
            {
                fi = list[i];
                //写入

                readFile = null;

                try
                {
                    readFile = fi.file.OpenRead();
                }
                catch (Exception ex)
                {
                    readFile?.Close();
                    p_packing = false;
                    p_errorException = ex;
                    yield break;
                }

                var enumators = readFile.CopyToStreamEnumator(pack, p_copyBytesBuffer);

                var enumator = enumators.GetEnumerator();

                while (true)
                {

                    try
                    {
                        if (!enumator.MoveNext())
                        {
                            break;
                        }

                    }
                    catch (Exception ex)
                    {
                        p_errorException = ex;
                        p_packing = false;
                        yield break;
                    }

                    yield return enumator.Current;

                }

                readFile.Close();

                yield return fi.file;

            }

            p_packing = false;
        }

        /// <summary>
        /// 返回一个资源打包迭代器
        /// </summary>
        /// <param name="stream">要打包到的流数据</param>
        /// <returns>
        /// <para>一个资源打包迭代器，将当前准备好的待打包文件逐步打包到指定的<paramref name="stream"/>中，每次迭代后会打包一部分数据</para>
        /// <para>返回不同实例对应不同操作，返回<see cref="FileInfoIndex"/>则表示正在写入索引；返回<see cref="long"/>或<see cref="int"/>类型数据则表示正在写入资源，值表示此次写入的资源大小；返回<see cref="FileInfo"/>表示已将这个文件写入完毕</para>
        /// <para>若打包时出现异常，则直接结束迭代，并将错误写入<see cref="LastThrowException"/></para>
        /// </returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException">流无法写入</exception>
        /// <exception cref="Exception">其它异常</exception>
        public IEnumerable ToPackEnumator(Stream stream)
        {
            if (stream is null) throw new ArgumentNullException();

            if (!(stream.CanWrite))
            {
                throw new NotSupportedException();
            }

            var size = CalculatedFileIndexByteSize();
            p_packing = true;
            return f_toPackenumator(stream, size);

        }

        /// <summary>
        /// 将当前准备好的待打包文件打包到指定的<paramref name="stream"/>中
        /// </summary>
        /// <param name="stream">
        /// 要打包到的流数据，会从流当前的位置开始写入，不会修改流之前的数据
        /// </param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException">流无法写入</exception>
        /// <exception cref="NotImplementedException">打包时出现错误</exception>
        /// <exception cref="Exception">其它异常</exception>
        public void ToPack(Stream stream)
        {
            if (stream is null) throw new ArgumentNullException();

            if (!(stream.CanWrite))
            {
                throw new NotSupportedException();
            }
            p_packing = true;

            var enr = f_toPackenumator(stream, CalculatedFileIndexByteSize()).GetEnumerator();
            while (enr.MoveNext())
            {
            }

        }

        /// <summary>
        /// 将当前准备好的待打包文件打包到指定路径的文件
        /// </summary>
        /// <param name="filePath">要创建的文件所在路径</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException">没有权限</exception>
        /// <exception cref="NotImplementedException">打包时出现错误</exception>
        /// <exception cref="PathTooLongException">路径太长</exception>
        /// <exception cref="DirectoryNotFoundException">目录不存在</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="Exception">其它异常</exception>
        public void ToPackFile(string filePath)
        {
            using (FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                ToPack(file);
            }
        }

        #endregion

        #region 集成功能

        /// <summary>
        /// 将指定目录下的文件写入待打包文件集合中
        /// </summary>
        /// <param name="directoryInfo">要搜索的根目录</param>
        /// <param name="searchPattern">
        /// 要与文件名匹配的搜索字符串
        /// <para>此参数可以包含有效的文本路径和通配符的组合（* 和 ?）字符</para>
        /// </param>
        /// <param name="searchOption">文件搜索模式</param>
        /// <param name="addBaseDirectory">是否将目录<paramref name="directoryInfo"/>表示的文件夹本身设置为所有文件的根</param>
        /// <exception cref="DirectoryNotFoundException">找不到目录</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="System.Security.SecurityException">没有权限</exception>
        /// <exception cref=""></exception>
        public void AddToPackagingFileList(DirectoryInfo directoryInfo, string searchPattern, SearchOption searchOption, bool addBaseDirectory)
        {
            if (directoryInfo is null) throw new ArgumentNullException();

            var enumator = directoryInfo.GetFiles(searchPattern, searchOption);
            var dire = directoryInfo.FullName;

            foreach (var item in enumator)
            {
                var filePath = item.FullName;

                string toPath;
                bool b;

                if (addBaseDirectory)
                {
                    b = f_getDirectoryAddBaseSubFilePath(dire, filePath, out toPath);
                }
                else
                {
                    b = f_getDirectorySubFilePath(dire, filePath, out toPath);
                }
                
                if (!b)
                {
                    //不匹配
                    continue;
                }

                p_list.Add(new FileInfoIndex(item, toPath));
            }
        }

        /// <summary>
        /// 将指定目录下的文件写入待打包文件集合中
        /// </summary>
        /// <param name="directoryInfo">要搜索的根目录</param>
        /// <param name="searchPattern">
        /// 要与文件名匹配的搜索字符串
        /// <para>此参数可以包含有效的文本路径和通配符的组合（* 和 ?）字符</para>
        /// </param>
        /// <param name="searchOption">文件搜索模式</param>
        /// <exception cref="DirectoryNotFoundException">找不到目录</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="System.Security.SecurityException">没有权限</exception>
        public void AddToPackagingFileList(DirectoryInfo directoryInfo, string searchPattern, SearchOption searchOption)
        {
            if (directoryInfo is null) throw new ArgumentNullException();

            var enumator = directoryInfo.GetFiles(searchPattern, searchOption);
            var dire = directoryInfo.FullName;

            foreach (var item in enumator)
            {
                var filePath = item.FullName;

                string toPath;
                bool b;
                b = f_getDirectorySubFilePath(dire, filePath, out toPath);
                if (!b)
                {
                    //不匹配
                    continue;
                }

                p_list.Add(new FileInfoIndex(item, toPath));
            }

        }

        /// <summary>
        /// 将指定目录下的所有文件写入待打包文件集合中
        /// </summary>
        /// <param name="directoryInfo">要搜索的根目录</param>
        /// <exception cref="DirectoryNotFoundException">找不到目录</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="System.Security.SecurityException">没有权限</exception>
        public void AddToPackagingFileList(DirectoryInfo directoryInfo)
        {
            AddToPackagingFileList(directoryInfo, "*", SearchOption.AllDirectories);
        }

        /// <summary>
        /// 将指定目录下的所有文件写入待打包文件集合中
        /// </summary>
        /// <param name="directory">要搜索的根目录</param>
        /// <exception cref="DirectoryNotFoundException">找不到目录</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="PathTooLongException">目录字符串超出范围</exception>
        /// <exception cref="System.Security.SecurityException">没有权限</exception>
        public void AddToPackagingFileList(string directory)
        {
            AddToPackagingFileList(new DirectoryInfo(directory), "*", SearchOption.AllDirectories);
        }

        /// <summary>
        /// 返回指定文件路径相对于指定父级文件夹目录的相对路径
        /// </summary>
        /// <param name="baseDire">指定文件夹目录</param>
        /// <param name="filePath">在<paramref name="baseDire"/>目录下的子路径</param>
        /// <param name="rePath">文件<paramref name="filePath"/>相对于<paramref name="baseDire"/>所在的相对路径</param>
        /// <returns>是否匹配</returns>
        static bool f_getDirectorySubFilePath(string baseDire, string filePath, out string rePath)
        {

            string baseDirectory = baseDire.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;

            string fullPath = filePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            //检测是否匹配
            if (!fullPath.StartsWith(baseDirectory, StringComparison.OrdinalIgnoreCase))
            {
                //throw new ArgumentException();
                rePath = null;
                return false;
            }

            // 获取相对路径
            rePath = fullPath.Substring(baseDirectory.Length);
            //relativePath = relativePath.Replace('\\', '/'); 
            return true;

        }


        static bool f_getDirectoryAddBaseSubFilePath(string baseDire, string filePath, out string rePath)
        {

            var parDire = Directory.GetParent(baseDire);

            if(parDire != null)
            {
                baseDire = parDire.FullName;
            }

            string baseDirectory = baseDire.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;

            string fullPath = filePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            //检测是否匹配
            if (!fullPath.StartsWith(baseDirectory, StringComparison.OrdinalIgnoreCase))
            {
                //throw new ArgumentException();
                rePath = null;
                return false;
            }

            // 获取相对路径
            rePath = fullPath.Substring(baseDirectory.Length);
            //relativePath = relativePath.Replace('\\', '/'); 
            return true;

        }

        #endregion

        #endregion

    }


}
