using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Cheng.DataStructure.Streams;
using Cheng.IO;

namespace Cheng.Algorithm.Compressions.ResourcePackages
{

    /// <summary>
    /// 数据索引
    /// </summary>
    public struct StreamIndex : IEquatable<StreamIndex>
    {

        #region

        /// <summary>
        /// 初始化数据索引
        /// </summary>
        /// <param name="path">数据路径</param>
        /// <param name="block">数据索引</param>
        public StreamIndex(string path, StreamBlock block)
        {
            this.path = path;
            this.block = block;
        }

        /// <summary>
        /// 数据块
        /// </summary>
        public readonly StreamBlock block;

        /// <summary>
        /// 资源所在的索引路径
        /// </summary>
        /// <remarks>
        /// 资源所在的相对路径，格式为"file.ext","dire\dire2\file.ext"，不能有"..\"和"\path\file"此类语法
        /// </remarks>
        public readonly string path;

        #endregion

        #region 派生

        public bool Equals(StreamIndex other)
        {
            return this.path == other.path;
        }

        public override bool Equals(object obj)
        {
            if(obj is StreamIndex s)
            {
                return this.path == s.path;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (path?.GetHashCode()).GetValueOrDefault();
        }

        public override string ToString()
        {
            return path + " => " + block.ToString();
        }

        /// <summary>
        /// 比较相等
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool operator ==(StreamIndex s1, StreamIndex s2)
        {
            return s1.path == s2.path;
        }

        /// <summary>
        /// 比较不相等
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool operator !=(StreamIndex s1, StreamIndex s2)
        {
            return s1.path != s2.path;
        }

        #endregion

    }

    /// <summary>
    /// 表示打包文件的索引
    /// </summary>
    public struct FileInfoIndex : IEquatable<FileInfoIndex>
    {

        #region 构造和参数

        /// <summary>
        /// 初始化文件路径索引
        /// </summary>
        /// <param name="file">待存文件</param>
        /// <param name="dataPath">存储到的包索引，指定资源包相对位置</param>
        public FileInfoIndex(CFileInfo file, string dataPath)
        {
            this.file = file;
            this.dataPath = dataPath;
        }

        /// <summary>
        /// 要存储的文件数据
        /// </summary>
        public readonly CFileInfo file;

        /// <summary>
        /// 要存储到的资源包所在的索引路径
        /// </summary>
        /// <remarks>
        /// 资源所在的相对路径，格式为"file.ext","dire\dire2\file.ext"，不能有"..\"和"\path\file"此类语法
        /// </remarks>
        public readonly string dataPath;

        #endregion

        #region 功能

        #endregion

        #region 派生

        public bool Equals(FileInfoIndex other)
        {
            return this.dataPath == other.dataPath;
        }

        public override bool Equals(object obj)
        {
            if(obj is FileInfoIndex other)
            {
                return this.dataPath == other.dataPath;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (dataPath?.GetHashCode() ^ file?.GetHashCode()).GetValueOrDefault();
        }

        /// <summary>
        /// 比较两个文件索引是否相等
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static bool operator ==(FileInfoIndex f1, FileInfoIndex f2)
        {
            return f1.dataPath == f2.dataPath;
        }

        /// <summary>
        /// 比较两个文件索引是否不相等
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static bool operator !=(FileInfoIndex f1, FileInfoIndex f2)
        {
            return f1.dataPath != f2.dataPath;
        }

        /// <summary>
        /// 返回待打包文件和打包到的路径
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return file?.ToString() + " => " + dataPath;
        }

        #endregion

    }

}
