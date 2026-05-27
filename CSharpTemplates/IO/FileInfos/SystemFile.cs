using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using Cheng.Algorithm.Collections;
using System.Linq;

namespace Cheng.IO.Systems
{

    /// <summary>
    /// 将<see cref="FileInfo"/>对象封装到<see cref="CFileInfo"/>
    /// </summary>
    public class SysFileInfo : CFileInfo
    {

        #region 构造

        /// <summary>
        /// 实例化一个对象
        /// </summary>
        /// <param name="fileName">要封装的文件对象的文件路径</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">路径错误</exception>
        /// <exception cref="PathTooLongException">路径过长</exception>
        /// <exception cref="System.Security.SecurityException">没有权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public SysFileInfo(string fileName)
        {
            this.fileInfo = new FileInfo(fileName);
        }

        /// <summary>
        /// 实例化一个对象
        /// </summary>
        /// <param name="fileInfo">要封装的文件对象</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public SysFileInfo(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo ?? throw new ArgumentNullException();
        }

        #endregion

        #region 参数

        /// <summary>
        /// 封装的系统文件对象
        /// </summary>
        protected readonly FileInfo fileInfo;

        #endregion

        #region 派生

        #region 权限

        public override bool CanGetExists => true;

        public override FileInfoPermissions Permissions
        {
            get
            {
                return FileInfoPermissions.All;
            }
        }

        public override string FullPath => fileInfo.FullName;

        public override string Name => fileInfo.Name;

        public override bool CanSetCreationTime
        {
            get
            {
                return true;
            }
        }

        public override bool CanGetLastWriteTime => true;

        public override bool CanSetLastWriteTime => true;

        public override bool CanGetLastAccessTime => true;

        public override bool CanSetLastAccessTime => true;

        public override bool CanGetLength => true;

        public override bool FileShareEffective => true;

        public override bool CanGetCreationTime => true;

        public override bool CanCopyFile => true;

        public override bool CanCreateStream
        {
            get => true;
        }

        public override bool CanMoveFile => true;

        public override bool CanOpenStream => true;

        public override bool CanCurrentDirectory => true;

        #endregion

        #region

        public override CDirectoryInfo CurrentDirectory
        {
            get
            {
                return new SysDirectoryInfo(fileInfo.Directory);
            }
        }

        /// <summary>
        /// 获取内部封装的文件对象
        /// </summary>
        public FileInfo SystemFileInfo => fileInfo;

        public override bool Exists => fileInfo.Exists;

        public override DateTime LastWriteTimeUtc 
        {
            get => fileInfo.LastWriteTimeUtc; 
            set => fileInfo.LastWriteTimeUtc = value; 
        }

        public override DateTime CreationTimeUtc 
        {
            get => fileInfo.CreationTimeUtc; 
            set => fileInfo.CreationTimeUtc = value;
        }

        public override DateTime LastAccessTimeUtc
        {
            get => fileInfo.LastAccessTimeUtc; 
            set => fileInfo.LastAccessTimeUtc = value;
        }

        public override DateTime LastWriteTime 
        {
            get => fileInfo.LastWriteTime; 
            set => fileInfo.LastWriteTime = value;
        }

        public override DateTime CreationTime 
        {
            get => fileInfo.CreationTime;
            set => fileInfo.CreationTime = value; 
        }

        public override DateTime LastAccessTime
        {
            get => fileInfo.LastAccessTime; 
            set => fileInfo.LastAccessTime = value;
        }

        public override string Extension => fileInfo.Extension;

        public override long Length => fileInfo.Length;

        public override void Delete()
        {
            fileInfo.Delete();
        }

        public override Stream OpenStream(CFileAccess access, CFileShare fileShare)
        {
            FileAccess acc = 0;
            FileShare fs = 0;
            if ((access & CFileAccess.Read) == CFileAccess.Read) acc |= FileAccess.Read;
            if ((access & CFileAccess.Write) == CFileAccess.Write) acc |= FileAccess.Write;

            if ((fileShare & CFileShare.Read) == CFileShare.Read) fs |= FileShare.Read;
            if ((fileShare & CFileShare.Write) == CFileShare.Write) fs |= FileShare.Write;
            if ((fileShare & CFileShare.Delete) == CFileShare.Delete) fs |= FileShare.Delete;

            return fileInfo.Open(FileMode.Open, acc, fs);
        }

        public override Stream CreateStream(CFileAccess access, CFileShare fileShare)
        {
            FileAccess acc = 0;
            FileShare fs = 0;
            if ((access & CFileAccess.Read) == CFileAccess.Read) acc |= FileAccess.Read;
            if ((access & CFileAccess.Write) == CFileAccess.Write) acc |= FileAccess.Write;

            if ((fileShare & CFileShare.Read) == CFileShare.Read) fs |= FileShare.Read;
            if ((fileShare & CFileShare.Write) == CFileShare.Write) fs |= FileShare.Write;
            if ((fileShare & CFileShare.Delete) == CFileShare.Delete) fs |= FileShare.Delete;
            return new FileStream(fileInfo.FullName, FileMode.Create, acc, fs);
        }

        public override Stream OpenStream(CFileAccess access)
        {
            FileAccess acc = 0;
            if ((access & CFileAccess.Read) == CFileAccess.Read) acc |= FileAccess.Read;
            if ((access & CFileAccess.Write) == CFileAccess.Write) acc |= FileAccess.Write;

            return fileInfo.Open(FileMode.Open, acc, FileShare.ReadWrite);
        }

        public override Stream CreateStream(CFileAccess access)
        {
            return CreateStream(access, CFileShare.ReadWrite);
        }

        public override CFileInfo MoveTo(string toPath, bool overwrite)
        {
            if (File.Exists(toPath))
            {
                if (overwrite)
                {
                    File.Delete(toPath);
                }
                else
                {
                    throw new IOException();
                }
            }
            fileInfo.MoveTo(toPath);

            return this;
        }

        public override CFileInfo CopyTo(string toPath, bool overwrite)
        {
            fileInfo.CopyTo(toPath, overwrite);
            return this;
        }

        public override CFileInfo MoveTo(string toPath)
        {
            fileInfo.MoveTo(toPath);
            return this;
        }

        public override CFileInfo CopyTo(string toPath)
        {
            fileInfo.CopyTo(toPath);
            return this;
        }

        public override bool CanRefresh => true;

        public override void Refresh()
        {
            fileInfo.Refresh();
        }

        #endregion

        #endregion

    }

    /// <summary>
    /// 将<see cref="DirectoryInfo"/>对象封装到<see cref="CDirectoryInfo"/>
    /// </summary>
    public class SysDirectoryInfo : CDirectoryInfo
    {

        #region 构造

        /// <summary>
        /// 实例化一个对象
        /// </summary>
        /// <param name="directory">指定目录路径</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="System.Security.SecurityException">没有权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public SysDirectoryInfo(string directory)
        {
            this.directoryInfo = new System.IO.DirectoryInfo(directory);
        }

        /// <summary>
        /// 实例化一个对象
        /// </summary>
        /// <param name="directoryInfo">要封装的目录对象</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public SysDirectoryInfo(DirectoryInfo directoryInfo)
        {
            this.directoryInfo = directoryInfo ?? throw new ArgumentNullException();
        }

        #endregion

        #region 参数

        /// <summary>
        /// 内部封装的系统文件夹对象
        /// </summary>
        protected readonly DirectoryInfo directoryInfo;

        #endregion

        #region 派生

        #region 权限

        public override bool CanGetExists => true;

        public override bool CanCreate => true;

        public override bool CanDelete => true;

        public override bool CanGetFile => true;

        public override bool CanGetParent => true;

        public override bool CanMove => true;


        public override bool CanGetCreationTime => true;

        public override bool CanGetLastAccessTime => true;

        public override bool CanGetLastWriteTime => true;

        public override bool CanSetCreationTime => true;

        public override bool CanSetLastAccessTime => true;

        public override bool CanSetLastWriteTime => true;

        public override bool CanSearchPatternRegular => false;

        public override bool CanCopy => false;

        public override bool CanCreateFileInfo => true;

        public override bool CanCreateSubdirectory => true;

        public override bool CanCreateFileInfoExists => true;
        #endregion

        #region 功能

        public override bool CanRefresh => true;

        public override void Refresh()
        {
            directoryInfo.Refresh();
        }

        public override CFileInfo CreateFileInfo(string fileName, bool createExists)
        {
            var path = Path.Combine(directoryInfo.FullName, fileName);
            if (createExists)
            {
                if (!File.Exists(path)) File.Create(path).Close();
            }
            return new SysFileInfo(path);
        }

        public override CDirectoryInfo CreateSubdirectory(string path)
        {
            return new SysDirectoryInfo(directoryInfo.CreateSubdirectory(path));
        }

        /// <summary>
        /// 获取内部封装的系统目录对象
        /// </summary>
        public DirectoryInfo SystemDirectoryInfo => directoryInfo;

        public override string FullPath => directoryInfo.FullName;

        public override string Name => directoryInfo.Name;

        public override bool Exists => directoryInfo.Exists;

        public override DateTime LastWriteTimeUtc { get => directoryInfo.LastWriteTimeUtc; set => directoryInfo.LastWriteTimeUtc = value; }

        public override DateTime CreationTimeUtc { get => directoryInfo.CreationTimeUtc; set => directoryInfo.CreationTimeUtc = value; }

        public override DateTime LastAccessTimeUtc { get => directoryInfo.LastAccessTimeUtc; set => directoryInfo.LastAccessTimeUtc = value; }

        public override DateTime LastWriteTime { get => directoryInfo.LastWriteTime; set => directoryInfo.LastWriteTime = value; }

        public override DateTime CreationTime { get => directoryInfo.CreationTime; set => directoryInfo.CreationTime = value; }

        public override DateTime LastAccessTime 
        {
            get => base.LastAccessTime; 
            set => base.LastAccessTime = value; 
        }

        public override CDirectoryInfo Parent => new SysDirectoryInfo(directoryInfo.Parent);

        public override CDirectoryInfo Root => new SysDirectoryInfo(directoryInfo.Root);

        public override CDirectoryInfo MoveTo(string directory)
        {
            directoryInfo.MoveTo(directory);
            return this;
        }

        public override void Delete()
        {
            directoryInfo.Delete();
        }

        public override void Create()
        {
            directoryInfo.Create();
        }

        static CDirectoryInfo sf_StoC(DirectoryInfo dire)
        {
            return new SysDirectoryInfo(dire);
        }

        static CFileInfo sf_StoC(FileInfo file)
        {
            return new SysFileInfo(file);
        }

        static CFileSystemInfo sf_StoCF(FileSystemInfo fsys)
        {
            if (fsys is FileInfo file)
            {
                return new SysFileInfo(file);
            }
            else return new SysDirectoryInfo((DirectoryInfo)fsys);
        }

        public override IEnumerable<CDirectoryInfo> EnumerateDirectories(string searchPattern, SearchOption searchOption)
        {
            return directoryInfo.EnumerateDirectories(searchPattern, searchOption).ToOtherItems(sf_StoC);
        }

        public override IEnumerable<CFileInfo> EnumerateFiles(string searchPattern, SearchOption searchOption)
        {
            return directoryInfo.EnumerateFiles(searchPattern, searchOption).ToOtherItems(sf_StoC);
        }

        public override IEnumerable<CFileSystemInfo> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption)
        {
            return directoryInfo.EnumerateFileSystemInfos(searchPattern, searchOption).ToOtherItems(sf_StoCF);
            //if (searchOption == SearchOption.TopDirectoryOnly)
            //{
            //    return directoryInfo.GetFileSystemInfos(searchPattern).ToOtherItems(sf_StoCF);
            //}
            //else if (searchOption == SearchOption.AllDirectories)
            //{
            //    var dires = directoryInfo.GetDirectories(searchPattern, SearchOption.AllDirectories).ToOtherItems(sf_StoCF);
            //    var files = directoryInfo.GetFiles(searchPattern, SearchOption.AllDirectories).ToOtherItems(sf_StoCF);

            //    return System.Linq.Enumerable.Concat(dires, files);
            //}
        }

        public override CDirectoryInfo[] GetDirectories(string searchPattern, SearchOption searchOption)
        {
            return directoryInfo.EnumerateDirectories(searchPattern, searchOption).ToOtherItems(sf_StoC).ToArray();
        }

        public override CFileInfo[] GetFiles(string searchPattern, SearchOption searchOption)
        {
            return directoryInfo.EnumerateFiles(searchPattern, searchOption).ToOtherItems(sf_StoC).ToArray();
        }

        public override CFileSystemInfo[] GetFileSystemInfos(string searchPattern, SearchOption searchOption)
        {
            return directoryInfo.EnumerateFileSystemInfos(searchPattern, searchOption).ToOtherItems(sf_StoCF).ToArray();
            //if (searchOption == SearchOption.TopDirectoryOnly)
            //{
            //    return directoryInfo.GetFileSystemInfos(searchPattern).ToOtherItems(sf_StoCF).ToArray();
            //}
            //else if(searchOption == SearchOption.AllDirectories)
            //{
            //    var dires = directoryInfo.GetDirectories(searchPattern, SearchOption.AllDirectories).ToOtherItems(sf_StoCF);
            //    var files = directoryInfo.GetFiles(searchPattern, SearchOption.AllDirectories).ToOtherItems(sf_StoCF);

            //    return System.Linq.Enumerable.Concat(dires, files).ToArray();
            //}
        }

        #endregion

        #endregion

    }

}
