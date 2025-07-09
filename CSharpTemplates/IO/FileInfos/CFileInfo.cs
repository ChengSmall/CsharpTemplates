using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using Cheng.Algorithm.Collections;
using System.Linq;
using Cheng.DataStructure;

namespace Cheng.IO
{

    /// <summary>
    /// 文件系统的访问权限
    /// </summary>
    [Flags]
    public enum FileInfoPermissions : byte
    {

        /// <summary>
        /// 无权限
        /// </summary>
        None = 0,

        /// <summary>
        /// 可读取
        /// </summary>
        Read = 0b1,

        /// <summary>
        /// 可写入
        /// </summary>
        Write = 0b10,

        /// <summary>
        /// 可任意查找
        /// </summary>
        Seek = 0b100,

        /// <summary>
        /// 可删除
        /// </summary>
        Delete = 0b1000,

        /// <summary>
        /// 可创建
        /// </summary>
        Create = 0b10000,

        /// <summary>
        /// 读写权限
        /// </summary>
        ReadWrite = Read | Write,

        /// <summary>
        /// 可任意访问的读取权限
        /// </summary>
        ReadSeek = Read | Seek,

        /// <summary>
        /// 可创建和删除
        /// </summary>
        CreateAndDelete = Create | Delete,

        /// <summary>
        /// 所有权限
        /// </summary>
        All = 0b11111,
    }

    /// <summary>
    /// 打开文件数据的方式
    /// </summary>
    [Flags]
    public enum CFileAccess
    {
        Read = 0b1,
        Write = 0b10,
        ReadWrite = 0b11
    }

    /// <summary>
    /// 用于控制其他对象对相同数据的共享权限
    /// </summary>
    [Flags]
    public enum CFileShare
    {
        /// <summary>
        /// 拒绝共享当前数据
        /// </summary>
        None = 0,

        /// <summary>
        /// 开放读取权限
        /// </summary>
        Read = 0b1,

        /// <summary>
        /// 开放写入权限
        /// </summary>
        Write = 0b10,

        /// <summary>
        /// 允许中途删除
        /// </summary>
        Delete = 0b100,

        /// <summary>
        /// 开放读写权限
        /// </summary>
        ReadWrite = 0b11,

        /// <summary>
        /// 共享所有权限
        /// </summary>
        All = 0b111,

    }


    /// <summary>
    /// 可扩展的文件系统基类
    /// </summary>
    public abstract class CFileSystemInfo
    {

        #region

        #endregion

        #region 参数访问权限

        /// <summary>
        /// 能否访问对象创建时间参数
        /// </summary>
        public virtual bool CanGetCreationTime => false;

        /// <summary>
        /// 能否设置对象创建时间参数
        /// </summary>
        public virtual bool CanSetCreationTime => false;

        /// <summary>
        /// 能否访问上次写入对象时间参数
        /// </summary>
        public virtual bool CanGetLastWriteTime => false;

        /// <summary>
        /// 能否设置上次写入对象时间参数
        /// </summary>
        public virtual bool CanSetLastWriteTime => false;

        /// <summary>
        /// 能否访问上次读取对象时间参数
        /// </summary>
        public virtual bool CanGetLastAccessTime => false;

        /// <summary>
        /// 能否设置上次读取对象时间参数
        /// </summary>
        public virtual bool CanSetLastAccessTime => false;

        /// <summary>
        /// 能否访问<see cref="Exists"/>参数
        /// </summary>
        /// <returns>如果该参数是true，则可访问<see cref="Exists"/>来判断当前对象是否实际存在于某个设施内；false则没有权限访问<see cref="Exists"/>参数</returns>
        public virtual bool CanGetExists => false;

        /// <summary>
        /// 当前对象是否为文件信息对象
        /// </summary>
        /// <returns>如果是true则该对象属于<see cref="CFileInfo"/>类派生对象，否则是<see cref="CDirectoryInfo"/>类派生对象</returns>
        public abstract bool IsFile { get; }

        /// <summary>
        /// 刷新对象状态函数<see cref="Refresh"/>是否有效
        /// </summary>
        public virtual bool CanRefresh => false;

        #endregion

        #region 派生

        /// <summary>
        /// 从根节点出发的绝对路径
        /// </summary>
        public abstract string FullPath { get; }

        /// <summary>
        /// 当前文件或节点的名称
        /// </summary>
        public virtual string Name
        {
            get
            {
                return Path.GetFileName(FullPath);
            }
        }
        /// <summary>
        /// 当前对象是否实际存在于硬盘、驱动器或其它硬件设施内，或者存在于某个封装器内的实际引用
        /// </summary>
        /// <exception cref="NotSupportedException">对象不是硬件连接器</exception>
        public virtual bool Exists => throw new NotSupportedException();

        /// <summary>
        /// 删除当前对象的实际引用
        /// </summary>
        /// <exception cref="NotSupportedException">没有删除权限</exception>
        /// <exception cref="Exception">其他错误</exception>
        public virtual void Delete() => throw new NotSupportedException();

        /// <summary>
        /// 以<see cref="CFileInfo"/>类型访问当前对象
        /// </summary>
        /// <exception cref="NotSupportedException">不是CFileInfo对象</exception>
        public virtual CFileInfo FileInfo
        {
            get => throw new NotSupportedException();
        }

        /// <summary>
        /// 以<see cref="CDirectoryInfo"/>类型访问当前对象
        /// </summary>
        /// <exception cref="NotSupportedException">不是CDirectoryInfo对象</exception>
        public virtual CDirectoryInfo DirectoryInfo
        {
            get => throw new NotSupportedException();
        }

        /// <summary>
        /// 获取或设置上次进行写入操作的当前对象的UTC时间
        /// </summary>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual DateTime LastWriteTimeUtc
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// 访问或设置创建该对象的UTC时间
        /// </summary>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual DateTime CreationTimeUtc
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// 访问或设置上次进行读取操作于当前对象的UTC时间
        /// </summary>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual DateTime LastAccessTimeUtc
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// 获取或设置上次进行写入操作的当前对象的本地时间
        /// </summary>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual DateTime LastWriteTime
        {
            get => new DateTime(LastWriteTimeUtc.Ticks, DateTimeKind.Utc);
            set
            {
                LastWriteTimeUtc = new DateTime(value.Ticks, DateTimeKind.Local);
            }
        }

        /// <summary>
        /// 访问或设置创建该对象的本地时间
        /// </summary>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual DateTime CreationTime
        {
            get => new DateTime(CreationTimeUtc.Ticks, DateTimeKind.Utc);
            set
            {
                CreationTimeUtc = new DateTime(value.Ticks, DateTimeKind.Local);
            }
        }

        /// <summary>
        /// 访问或设置上次进行读取操作于当前对象的本地时间
        /// </summary>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual DateTime LastAccessTime
        {
            get => new DateTime(LastAccessTimeUtc.Ticks, DateTimeKind.Utc);
            set
            {
                LastAccessTimeUtc = new DateTime(value.Ticks, DateTimeKind.Local);
            }
        }

        /// <summary>
        /// 刷新对象状态
        /// </summary>
        /// <remarks>
        /// <para>调用该函数用于刷新对象状态</para>
        /// <para>在派生类实现该函数，用于刷新对象的实际状态</para>
        /// </remarks>
        /// <exception cref="Exception">出现错误</exception>
        public virtual void Refresh()
        {
        }

        /// <summary>
        /// 返回对象的<see cref="Name"/>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion

    }

    /// <summary>
    /// 可扩展的目录系统
    /// </summary>
    public abstract class CDirectoryInfo : CFileSystemInfo
    {

        #region 权限

        /// <summary>
        /// 允许创建当前对象目录
        /// </summary>
        public virtual bool CanCreate => false;

        /// <summary>
        /// 允许将目录移动到另一个位置
        /// </summary>
        public virtual bool CanMove => false;

        /// <summary>
        /// 允许将目录拷贝到另一个位置
        /// </summary>
        public virtual bool CanCopy => false;

        /// <summary>
        /// 允许获取目录下的文件和目录
        /// </summary>
        public virtual bool CanGetFile => false;

        /// <summary>
        /// 允许获取父级目录或根目录
        /// </summary>
        public virtual bool CanGetParent => false;

        /// <summary>
        /// 在获取目录下的数据时，参数 searchPattern 是否允许使用正则表达式
        /// </summary>
        public virtual bool CanSearchPatternRegular => false;

        /// <summary>
        /// 在获取目录下的数据时，参数 searchPattern 是否有效
        /// </summary>
        public virtual bool SearchPatternEffective => false;

        /// <summary>
        /// 能否将当前目录删除
        /// </summary>
        /// <returns>
        /// <para>如果<see cref="CFileSystemInfo.CanGetExists"/>为false，则该参数也需要为false，因为如果对象不属于硬件连接器或某个封装器内的实际引用，则删除对象是无意义操作</para>
        /// </returns>
        public virtual bool CanDelete => false;

        /// <summary>
        /// 允许在当前目录下创建一个或多个子目录
        /// </summary>
        public virtual bool CanCreateSubdirectory => false;

        /// <summary>
        /// 允许在该目录下创建文件对象
        /// </summary>
        public virtual bool CanCreateFileInfo => false;

        /// <summary>
        /// 在目录下创建文件对象时是否能够创建实际存在的硬件资源
        /// </summary>
        public virtual bool CanCreateFileInfoExists => false;

        #endregion

        #region 功能

        #region

        public sealed override CFileInfo FileInfo => throw new NotSupportedException();

        public sealed override CDirectoryInfo DirectoryInfo => this;

        public sealed override bool IsFile => false;

        /// <summary>
        /// 获取当前目录的父级目录，如果没有父级目录则为null
        /// </summary>
        public virtual CDirectoryInfo Parent => null;

        /// <summary>
        /// 获取当前目录的根目录，如果该目录没有根目录则为null
        /// </summary>
        public virtual CDirectoryInfo Root => null;

        /// <summary>
        /// 创建当前目录
        /// </summary>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual void Create() => throw new NotSupportedException();

        /// <summary>
        /// 将目录移动到另一个位置
        /// </summary>
        /// <param name="directory">要将目录移动到的另一个位置目录</param>
        /// <returns>移动后的目录对象</returns>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual CDirectoryInfo MoveTo(string directory) => throw new NotSupportedException();

        /// <summary>
        /// 将目录拷贝到另一个位置
        /// </summary>
        /// <param name="directory">要将目录拷贝到的另一个位置目录</param>
        /// <returns>移动后的目录对象</returns>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual CDirectoryInfo CopyTo(string directory) => throw new NotSupportedException();

        /// <summary>
        /// 在指定路径上创建一个或多个子目录
        /// </summary>
        /// <param name="path">
        /// <para>表示当前目录下的子目录相对路径</para>
        /// </param>
        /// <returns>创建后的最后一层目录</returns>
        /// <exception cref="NotSupportedException">没有权限</exception>
        /// <exception cref="ArgumentNullException">路径是null</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual CDirectoryInfo CreateSubdirectory(string path)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 在当前目录下创建一个文件对象并返回
        /// </summary>
        /// <param name="fileName">要创建的文件名称</param>
        /// <param name="createExists">
        /// 在创建文件对象时是否创建实际引用，如果文件已存在则不会再创建；如果<see cref="CanCreateFileInfoExists"/>是false则该参数无效
        /// </param>
        /// <returns>创建的文件对象</returns>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="ArgumentNullException">文件名是null</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual CFileInfo CreateFileInfo(string fileName, bool createExists)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 返回当前目录下的一个文件对象
        /// </summary>
        /// <param name="fileName">要创建的文件名称</param>
        /// <returns>创建的文件对象</returns>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="ArgumentNullException">文件名是null</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual CFileInfo CreateFileInfo(string fileName)
        {
            return CreateFileInfo(fileName, false);
        }

        #endregion

        #region

        /// <summary>
        /// 获取当前目录下的文件和目录信息
        /// </summary>
        /// <param name="searchPattern">要与文件名匹配的搜索字符串，此参数包含有效的文本路径和通配符的组合（* 和 ?）字符，默认模式为“*”，该模式返回所有文件数据</param>
        /// <param name="searchOption">指示搜索当前目录还是所有子目录</param>
        /// <returns>一个可枚举访问文件信息的枚举器</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数有错误</exception>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual IEnumerable<CFileSystemInfo> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 获取当前目录下的目录信息
        /// </summary>
        /// <param name="searchPattern">要与文件名匹配的搜索字符串，此参数包含有效的文本路径和通配符的组合（* 和 ?）字符，默认模式为“*”，该模式返回所有数据</param>
        /// <param name="searchOption">指示搜索当前目录还是所有子目录</param>
        /// <returns>一个可枚举访问目录信息的枚举器</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数有错误</exception>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual IEnumerable<CDirectoryInfo> EnumerateDirectories(string searchPattern, SearchOption searchOption)
        {
            var enr = EnumerateFileSystemInfos(searchPattern, searchOption);
            return enr.ToOtherItemsByCondition<CFileSystemInfo, CDirectoryInfo>(toFunc);

            bool toFunc(CFileSystemInfo t_sys, out CDirectoryInfo t_dire)
            {
                if ((t_sys?.IsFile).GetValueOrDefault())
                {
                    t_dire = t_sys.DirectoryInfo;
                    return true;
                }
                t_dire = null;
                return false;
            }
        }

        /// <summary>
        /// 获取当前目录下的文件信息
        /// </summary>
        /// <param name="searchPattern">要与文件名匹配的搜索字符串，此参数包含有效的文本路径和通配符的组合（* 和 ?）字符，默认模式为“*”，该模式返回所有文件数据</param>
        /// <param name="searchOption">指示搜索当前目录还是所有子目录</param>
        /// <returns>一个可枚举访问文件信息的枚举器</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数有错误</exception>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual IEnumerable<CFileInfo> EnumerateFiles(string searchPattern, SearchOption searchOption)
        {
            var enr = EnumerateFileSystemInfos(searchPattern, searchOption);
            return enr.ToOtherItemsByCondition<CFileSystemInfo, CFileInfo>(toFunc);

            bool toFunc(CFileSystemInfo t_sys, out CFileInfo t_file)
            {
                if ((t_sys?.IsFile).GetValueOrDefault())
                {
                    t_file = t_sys.FileInfo;
                    return true;
                }
                t_file = null;
                return false;
            }
        }

        /// <summary>
        /// 获取当前目录下的文件和目录信息
        /// </summary>
        /// <param name="searchPattern">要与文件名匹配的搜索字符串，此参数包含有效的文本路径和通配符的组合（* 和 ?）字符，默认模式为“*”，该模式返回所有文件数据</param>
        /// <param name="searchOption">指示搜索当前目录还是所有子目录</param>
        /// <returns>获取的所有文件系统信息</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数有错误</exception>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual CFileSystemInfo[] GetFileSystemInfos(string searchPattern, SearchOption searchOption)
        {
            return EnumerateFileSystemInfos(searchPattern, searchOption).ToArray();
        }

        /// <summary>
        /// 获取当前目录下的文件信息
        /// </summary>
        /// <param name="searchPattern">要与文件名匹配的搜索字符串，此参数包含有效的文本路径和通配符的组合（* 和 ?）字符，默认模式为“*”，该模式返回所有文件数据</param>
        /// <param name="searchOption">指示搜索当前目录还是所有子目录</param>
        /// <returns>获取的所有文件信息</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数有错误</exception>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual CFileInfo[] GetFiles(string searchPattern, SearchOption searchOption)
        {
            return EnumerateFiles(searchPattern, searchOption).ToArray();
        }

        /// <summary>
        /// 获取当前目录下的目录信息
        /// </summary>
        /// <param name="searchPattern">要与文件名匹配的搜索字符串，此参数包含有效的文本路径和通配符的组合（* 和 ?）字符，默认模式为“*”，该模式返回所有数据</param>
        /// <param name="searchOption">指示搜索当前目录还是所有子目录</param>
        /// <returns>获取的所有目录信息</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数有错误</exception>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual CDirectoryInfo[] GetDirectories(string searchPattern, SearchOption searchOption)
        {
            return EnumerateDirectories(searchPattern, searchOption).ToArray();
        }

        /// <summary>
        /// 获取当前目录下的目录信息
        /// </summary>
        /// <param name="searchPattern">要与文件名匹配的搜索字符串，此参数包含有效的文本路径和通配符的组合（* 和 ?）字符，默认模式为“*”，该模式返回所有数据</param>
        /// <returns>一个可枚举访问目录信息的枚举器</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数有错误</exception>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual IEnumerable<CDirectoryInfo> EnumerateDirectories(string searchPattern)
        {
            return this.EnumerateDirectories(searchPattern, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// 获取当前目录下的文件信息
        /// </summary>
        /// <param name="searchPattern">要与文件名匹配的搜索字符串，此参数包含有效的文本路径和通配符的组合（* 和 ?）字符，默认模式为“*”，该模式返回所有文件数据</param>
        /// <returns>一个可枚举访问文件信息的枚举器</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数有错误</exception>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual IEnumerable<CFileInfo> EnumerateFiles(string searchPattern)
        {
            return this.EnumerateFiles(searchPattern, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// 获取当前目录下的文件和目录信息
        /// </summary>
        /// <param name="searchPattern">要与文件名匹配的搜索字符串，此参数包含有效的文本路径和通配符的组合（* 和 ?）字符，默认模式为“*”，该模式返回所有文件数据</param>
        /// <returns>一个可枚举访问文件信息的枚举器</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数有错误</exception>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual IEnumerable<CFileSystemInfo> EnumerateFileSystemInfos(string searchPattern)
        {
            return this.EnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// 获取当前目录下的目录信息
        /// </summary>
        /// <param name="searchPattern">要与文件名匹配的搜索字符串，此参数包含有效的文本路径和通配符的组合（* 和 ?）字符，默认模式为“*”，该模式返回所有数据</param>
        /// <returns>获取的所有目录信息</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数有错误</exception>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual CDirectoryInfo[] GetDirectories(string searchPattern)
        {
            return this.GetDirectories(searchPattern, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// 获取当前目录下的文件信息
        /// </summary>
        /// <param name="searchPattern">要与文件名匹配的搜索字符串，此参数包含有效的文本路径和通配符的组合（* 和 ?）字符，默认模式为“*”，该模式返回所有文件数据</param>
        /// <returns>获取的所有文件信息</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数有错误</exception>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual CFileInfo[] GetFiles(string searchPattern)
        {
            return this.GetFiles(searchPattern, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// 获取当前目录下的文件和目录信息
        /// </summary>
        /// <param name="searchPattern">要与文件名匹配的搜索字符串，此参数包含有效的文本路径和通配符的组合（* 和 ?）字符，默认模式为“*”，该模式返回所有文件数据</param>
        /// <returns>一个可枚举访问文件信息的枚举器</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数有错误</exception>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual CFileSystemInfo[] GetFileSystemInfos(string searchPattern)
        {
            return this.GetFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly);
        }


        /// <summary>
        /// 获取当前目录下的目录信息
        /// </summary>
        /// <returns>一个可枚举访问目录信息的枚举器</returns>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual IEnumerable<CDirectoryInfo> EnumerateDirectories()
        {
            return this.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// 获取当前目录下的文件信息
        /// </summary>
        /// <returns>一个可枚举访问文件信息的枚举器</returns>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual IEnumerable<CFileInfo> EnumerateFiles()
        {
            return this.EnumerateFiles("*", SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// 获取当前目录下的文件和目录信息
        /// </summary>
        /// <returns>一个可枚举访问文件信息的枚举器</returns>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual IEnumerable<CFileSystemInfo> EnumerateFileSystemInfos()
        {
            return this.EnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// 获取当前目录下的目录信息
        /// </summary>
        /// <returns>获取的所有目录信息</returns>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual CDirectoryInfo[] GetDirectories()
        {
            return this.GetDirectories("*", SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// 获取当前目录下的文件信息
        /// </summary>
        /// <returns>获取的所有文件信息</returns>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual CFileInfo[] GetFiles()
        {
            return this.GetFiles("*", SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// 获取当前目录下的文件和目录信息
        /// </summary>
        /// <returns>一个可枚举访问文件信息的枚举器</returns>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual CFileSystemInfo[] GetFileSystemInfos()
        {
            return this.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly);
        }

        #endregion

        #endregion

    }

    /// <summary>
    /// 可扩展的文件数据系统
    /// </summary>
    public abstract class CFileInfo : CFileSystemInfo, IGettingStream
    {

        #region 权限

        /// <summary>
        /// 能否访问数据长度
        /// </summary>
        public virtual bool CanGetLength => false;

        /// <summary>
        /// 在打开或创建数据时 FileShare 参数是有效的
        /// </summary>
        public virtual bool FileShareEffective => false;

        /// <summary>
        /// 文件系统的最大可用权限
        /// </summary>
        /// <returns>
        /// <para>该参数是一个位值枚举，指示当前对象的最大可用权限；最大可用权限是对象能够使用的最多权限，实际权限在一些影响下可能有波动，但不会超越该参数指定的权限；但是如果该参数没有指定某些权限，则该对象将永远不会拥有这些权限</para>
        /// </returns>
        public virtual FileInfoPermissions Permissions => FileInfoPermissions.None;

        /// <summary>
        /// 允许打开流数据
        /// </summary>
        public virtual bool CanOpenStream => false;

        /// <summary>
        /// 允许创建流数据
        /// </summary>
        public virtual bool CanCreateStream => false;

        /// <summary>
        /// 允许拷贝文件到其它位置
        /// </summary>
        public virtual bool CanCopyFile => false;

        /// <summary>
        /// 允许移动文件到其它位置
        /// </summary>
        public virtual bool CanMoveFile => false;

        /// <summary>
        /// 能否获取文件所在的目录对象
        /// </summary>
        public virtual bool CanCurrentDirectory => false;

        #endregion

        #region 功能

        public sealed override CDirectoryInfo DirectoryInfo => throw new NotSupportedException();

        public sealed override CFileInfo FileInfo => this;

        public sealed override bool IsFile => true;

        /// <summary>
        /// 获取当前文件所在的目录对象
        /// </summary>
        /// <exception cref="NotSupportedException">没有操作权限</exception>
        public virtual CDirectoryInfo CurrentDirectory
        {
            get => throw new NotSupportedException();
        }

        /// <summary>
        /// 访问对象的扩展名
        /// </summary>
        /// <exception cref="Exception">其它错误</exception>
        public virtual string Extension
        {
            get
            {
                return Path.GetExtension(FullPath);
            }
        }

        /// <summary>
        /// 获取对象数据长度
        /// </summary>
        /// <exception cref="NotSupportedException">没有访问权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual long Length => throw new NotSupportedException();

        /// <summary>
        /// 打开当前文件所在的流数据
        /// </summary>
        /// <param name="access">指定数据的打开方式</param>
        /// <param name="fileShare">指定共享权限</param>
        /// <returns>当前对象所在的数据</returns>
        /// <exception cref="ArgumentException">参数错误</exception>
        /// <exception cref="NotSupportedException">没有打开权限</exception>
        /// <exception cref="Exception">对象的其它错误</exception>
        public virtual Stream OpenStream(CFileAccess access, CFileShare fileShare) => throw new NotSupportedException();

        /// <summary>
        /// 创建当前文件所在的流数据
        /// </summary>
        /// <param name="access">指定数据的打开方式</param>
        /// <param name="fileShare">指定共享权限</param>
        /// <returns>当前对象所在的数据</returns>
        /// <exception cref="ArgumentException">参数错误</exception>
        /// <exception cref="NotSupportedException">没有创建权限</exception>
        /// <exception cref="Exception">对象的其它错误</exception>
        public virtual Stream CreateStream(CFileAccess access, CFileShare fileShare) => throw new NotSupportedException();

        /// <summary>
        /// 打开当前文件所在的流数据
        /// </summary>
        /// <param name="access">指定数据的打开方式</param>
        /// <returns>当前对象所在的数据</returns>
        /// <exception cref="ArgumentException">参数错误</exception>
        /// <exception cref="NotSupportedException">没有打开权限</exception>
        /// <exception cref="Exception">对象的其它错误</exception>
        public virtual Stream OpenStream(CFileAccess access) => OpenStream(access, CFileShare.ReadWrite);

        /// <summary>
        /// 创建当前文件所在的流数据
        /// </summary>
        /// <param name="access">指定数据的打开方式</param>
        /// <returns>当前对象所在的数据</returns>
        /// <exception cref="ArgumentException">参数错误</exception>
        /// <exception cref="NotSupportedException">没有创建权限</exception>
        /// <exception cref="Exception">对象的其它错误</exception>
        public virtual Stream CreateStream(CFileAccess access) => CreateStream(access, CFileShare.Read);

        /// <summary>
        /// 将文件移动到另一个路径
        /// </summary>
        /// <param name="toPath">要移动到的目标路径</param>
        /// <param name="overwrite">如果目标路径存在数据，是否进行覆盖</param>
        /// <returns>移动后的对象</returns>
        /// <exception cref="ArgumentNullException">路径是null</exception>
        /// <exception cref="NotSupportedException">没有权限</exception>
        /// <exception cref="Exception">移动对象时出现的其它异常</exception>
        public virtual CFileInfo MoveTo(string toPath, bool overwrite) => throw new NotSupportedException();

        /// <summary>
        /// 将文件拷贝到另一个路径
        /// </summary>
        /// <param name="toPath">要拷贝到的目标路径</param>
        /// <param name="overwrite">如果目标路径存在数据，是否进行覆盖</param>
        /// <returns>移动后的对象</returns>
        /// <exception cref="ArgumentNullException">路径是null</exception>
        /// <exception cref="NotSupportedException">没有权限</exception>
        /// <exception cref="Exception">移动对象时出现的其它异常</exception>
        public virtual CFileInfo CopyTo(string toPath, bool overwrite) => throw new NotSupportedException();

        /// <summary>
        /// 将文件移动到另一个路径
        /// </summary>
        /// <param name="toPath">要移动到的目标路径</param>
        /// <returns>移动后的对象</returns>
        /// <exception cref="ArgumentNullException">路径是null</exception>
        /// <exception cref="NotSupportedException">没有权限</exception>
        /// <exception cref="Exception">移动对象时出现的其它异常</exception>
        public virtual CFileInfo MoveTo(string toPath)
        {
            return MoveTo(toPath, false);
        }

        /// <summary>
        /// 将文件拷贝到另一个路径
        /// </summary>
        /// <param name="toPath">要拷贝到的目标路径</param>
        /// <returns>移动后的对象</returns>
        /// <exception cref="ArgumentNullException">路径是null</exception>
        /// <exception cref="NotSupportedException">没有权限</exception>
        /// <exception cref="Exception">移动对象时出现的其它异常</exception>
        public virtual CFileInfo CopyTo(string toPath)
        {
            return CopyTo(toPath, false);
        }

        long IGettingStream.StreamLength
        {
            get
            {
                return IGettingStreamLength;
            }
        }

        Stream IGettingStream.OpenStream()
        {
            return GettingOpenStream();
        }

        /// <summary>
        /// 实现<see cref="IGettingStream"/>接口的<see cref="IGettingStream.StreamLength"/>参数
        /// </summary>
        protected virtual long IGettingStreamLength
        {
            get
            {
                try
                {
                    if (CanGetLength)
                    {
                        if (CanRefresh)
                        {
                            Refresh();
                        }
                        return Length;
                    }
                }
                catch (Exception)
                {
                }
                return -1;
            }
        }

        /// <summary>
        /// 实现<see cref="IGettingStream"/>接口的<see cref="IGettingStream.OpenStream"/>可重写函数
        /// </summary>
        /// <returns><see cref="IGettingStream.OpenStream"/>函数的返回值</returns>
        protected virtual Stream GettingOpenStream()
        {
            try
            {
                if (CanOpenStream) return OpenStream(CFileAccess.Read);
            }
            catch (Exception)
            {
            }
            return null;
        }

        #endregion

    }

}
