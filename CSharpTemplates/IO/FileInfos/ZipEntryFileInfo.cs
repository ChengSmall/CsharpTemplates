using System;
using System.Collections.Generic;
using System.Collections;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace Cheng.IO.Systems
{

    /// <summary>
    /// 对<see cref="ZipArchiveEntry"/>对象封装的文件对象
    /// </summary>
    public class ZipEntryFileInfo : CFileInfo
    {

        #region 构造

        /// <summary>
        /// 使用zip的一个<see cref="ZipArchiveEntry"/>对象封装文件对象
        /// </summary>
        /// <param name="entry">zip文档的项</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public ZipEntryFileInfo(ZipArchiveEntry entry)
        {
            p_entry = entry ?? throw new ArgumentNullException();
        }

        #endregion

        #region 参数

        private ZipArchiveEntry p_entry;

        #endregion

        #region 功能

        /// <summary>
        /// 获取封装的zip项
        /// </summary>
        public ZipArchiveEntry Entry
        {
            get => p_entry;
        }

        #endregion

        #region 派生

        public override string FullPath
        {
            get
            {
                return p_entry.FullName;
            }
        }

        public override string Name
        {
            get
            {
                return p_entry.Name;
            }
        }

        public override bool CanGetExists => true;

        public override bool Exists
        {
            get
            {
                return (object)p_entry.Archive != null;
            }
        }

        public override bool CanGetLastWriteTime
        {
            get
            {
                var zip = p_entry.Archive;
                if (zip is null) return false;
                return zip.Mode == ZipArchiveMode.Read || zip.Mode == ZipArchiveMode.Update;
            }
        }

        public override bool CanSetLastWriteTime
        {
            get
            {
                var zip = p_entry.Archive;
                if (zip is null) return false;
                return zip.Mode == ZipArchiveMode.Create || zip.Mode == ZipArchiveMode.Update;
            }
        }

        public override DateTime LastWriteTimeUtc 
        {
            get
            {
                try
                {
                    return p_entry.LastWriteTime.UtcDateTime;
                }
                catch (Exception ex)
                {
                    throw new NotSupportedException(ex.Message, ex);
                }
              
            }
            set
            {
                try
                {
                    p_entry.LastWriteTime = new DateTimeOffset(value);
                }
                catch (Exception ex)
                {
                    throw new NotSupportedException(ex.Message, ex);
                }
              
            }
        }

        public override bool CanGetLength
        {
            get
            {
                var zip = p_entry.Archive;
                if (zip is null) return false;
                return zip.Mode != ZipArchiveMode.Create;
            }
        }

        public override bool CanOpenStream
        {
            get => true;
        }

        public override long Length
        {
            get
            {
                try
                {
                    return p_entry.Length;
                }
                catch (Exception ex)
                {
                    throw new NotSupportedException(ex.Message, ex);
                }
            }
        }

        public override void Delete()
        {
            p_entry.Delete();
        }

        public override bool CanCreateStream
        {
            get => true;
        }

        public override Stream OpenStream(CFileAccess access, CFileShare fileShare)
        {
            return p_entry.Open();
        }

        public override Stream OpenStream(CFileAccess access)
        {
            return p_entry.Open();
        }

        public override Stream CreateStream(CFileAccess access)
        {
            return p_entry.Open();
        }

        public override Stream CreateStream(CFileAccess access, CFileShare fileShare)
        {
            return p_entry.Open();
        }

        public override FileInfoPermissions Permissions
        {
            get
            {
                return FileInfoPermissions.Read | FileInfoPermissions.CreateAndDelete;
            }
        }


        #endregion

    }

}
