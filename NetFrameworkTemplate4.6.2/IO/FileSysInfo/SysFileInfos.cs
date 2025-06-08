using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cheng.Algorithm.Collections;

namespace Cheng.IO.Systems
{

    /// <summary>
    /// .Net 4.0版系统目录对象封装
    /// </summary>
    public class NSysDirectoryInfo : SysDirectoryInfo
    {

        #region

        /// <summary>
        /// 实例化一个对象
        /// </summary>
        /// <param name="path">要封装的目录对象的路径</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">路径错误</exception>
        /// <exception cref="PathTooLongException">路径过长</exception>
        /// <exception cref="System.Security.SecurityException">没有权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public NSysDirectoryInfo(string path) : base(path)
        {
        }

        /// <summary>
        /// 实例化一个对象
        /// </summary>
        /// <param name="directoryInfo">要封装的目录对象</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public NSysDirectoryInfo(DirectoryInfo directoryInfo) : base(directoryInfo)
        {
        }

        #endregion

        #region 派生

        #region

        static CDirectoryInfo nsf_StoC(DirectoryInfo dire)
        {
            return new SysDirectoryInfo(dire);
        }

        static CFileInfo nsf_StoC(FileInfo file)
        {
            return new SysFileInfo(file);
        }

        static CFileSystemInfo nsf_StoCF(FileSystemInfo fsys)
        {
            if (fsys is FileInfo file)
            {
                return new SysFileInfo(file);
            }
            else return new SysDirectoryInfo((DirectoryInfo)fsys);
        }

        #endregion

        public override IEnumerable<CDirectoryInfo> EnumerateDirectories(string searchPattern, SearchOption searchOption)
        {
            return directoryInfo.EnumerateDirectories(searchPattern, searchOption).ToOtherItems(nsf_StoC);
        }

        public override IEnumerable<CFileInfo> EnumerateFiles(string searchPattern, SearchOption searchOption)
        {
            return directoryInfo.EnumerateFiles(searchPattern, searchOption).ToOtherItems(nsf_StoC);
        }

        public override IEnumerable<CFileSystemInfo> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption)
        {
            return directoryInfo.EnumerateFileSystemInfos(searchPattern, searchOption).ToOtherItems(nsf_StoCF);
        }

        public override CDirectoryInfo[] GetDirectories(string searchPattern, SearchOption searchOption)
        {
            return directoryInfo.GetDirectories(searchPattern, searchOption).ToOtherItems(nsf_StoC).ToArray();
        }

        public override CFileInfo[] GetFiles(string searchPattern, SearchOption searchOption)
        {
            return directoryInfo.GetFiles(searchPattern, searchOption).ToOtherItems(nsf_StoC).ToArray();
        }

        public override CFileSystemInfo[] GetFileSystemInfos(string searchPattern, SearchOption searchOption)
        {
            return directoryInfo.GetFileSystemInfos(searchPattern, searchOption).ToOtherItems(nsf_StoCF).ToArray();
        }

        #endregion

    }


}
