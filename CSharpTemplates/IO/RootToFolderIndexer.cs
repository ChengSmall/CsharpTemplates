using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Collections;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.ComponentModel;
using System.Security;
using Cheng.Texts;
using Cheng.Memorys;
using Cheng.Algorithm.Compressions;

namespace Cheng.IO
{

    /// <summary>
    /// 使用某个文件夹作为根目录的文件操作器
    /// </summary>
    public class RootToFolderIndexer : BaseCompressionParser
    {

        #region 结构
        
        /// <summary>
        /// 文件信息结构
        /// </summary>
        protected class FileInfomation : DataInformation, IEquatable<FileInfomation>
        {

            public FileInfomation(FileInfo fileInfo, DirectoryInfo root)
            {
                if (fileInfo is null || root is null) throw new ArgumentNullException();
                p_file = fileInfo;
                p_path = TextManipulation.Path.GetRelativeDirectoryByPath(root.FullName, p_file.FullName);
            }

            protected readonly FileInfo p_file;
            private string p_path;

            public override long BeforeSize
            {
                get
                {
                    try
                    {
                        if (p_file.Exists)
                        {
                            return p_file.Length;
                        }
                    }
                    catch (Exception)
                    {
                    }
                    return -1;
                }
            }

            public override string DataName
            {
                get
                {
                    try
                    {
                        if (p_file.Exists)
                        {
                            return p_file.Name;
                        }
                    }
                    catch (Exception)
                    {
                    }
                    return null;
                }
            }

            public override string DataPath
            {
                get
                {
                    try
                    {
                        if (p_file.Exists)
                        {
                            return p_path;
                        }
                    }
                    catch (Exception)
                    {
                    }
                    return null;
                }
            }

            public override DateTime? ModifiedTime
            {
                get
                {
                    try
                    {
                        if (p_file.Exists)
                        {
                            return p_file.LastWriteTime;
                        }
                    }
                    catch (Exception)
                    {
                    }
                    return null;
                }
            }

            public override string ToString()
            {
                return DataName;
            }

            public bool Equals(FileInfomation other)
            {
                if (other is null) return false;
                return (p_path == other.p_path);
            }
        }

        #endregion

        #region

        /// <summary>
        /// 实例化文件操作器，使用某个文件夹作为根目录
        /// </summary>
        /// <param name="directory">根目录</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="DirectoryNotFoundException">目录不存在</exception>
        /// <exception cref="IOException">初始化时IO错误</exception>
        /// <exception cref="SecurityException">没有权限</exception>
        public RootToFolderIndexer(string directory)
        {
            if (directory is null) throw new ArgumentNullException();
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException();
            }
            p_root = new DirectoryInfo(directory);
            f_init();
        }

        /// <summary>
        /// 实例化文件操作器，使用某个文件夹作为根目录
        /// </summary>
        /// <param name="directoryInfo">根目录</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="DirectoryNotFoundException">目录不存在</exception>
        /// <exception cref="IOException">初始化时IO错误</exception>
        /// <exception cref="SecurityException">没有权限</exception>
        public RootToFolderIndexer(DirectoryInfo directoryInfo)
        {
            if (directoryInfo is null) throw new ArgumentNullException();
            if (!directoryInfo.Exists)
            {
                throw new DirectoryNotFoundException();
            }
            p_root = directoryInfo;
            f_init();
        }

        private void f_init()
        {
            p_rootfullPath = p_root.FullName;
            p_list = new List<FileInfomation>();
            //FileSystemWatcher;
            p_buffer = new byte[1024 * 4];
            Refresh();
        }

        #endregion

        #region 参数

        /// <summary>
        /// 根目录
        /// </summary>
        protected DirectoryInfo p_root;

        /// <summary>
        /// 根目录全路径
        /// </summary>
        protected string p_rootfullPath;

        /// <summary>
        /// 文件集合
        /// </summary>
        protected List<FileInfomation> p_list;

        private byte[] p_buffer;

        #endregion

        #region 功能

        #region 文件夹

        /// <summary>
        /// 刷新当前文件夹内所有的文件和文件夹状态到数据缓冲区
        /// </summary>
        /// <exception cref="SecurityException">没有权限</exception>
        /// <exception cref="DirectoryNotFoundException">目录已不存在</exception>
        public virtual void Refresh()
        {
            lock (p_list)
            {
                var filenums = p_root.GetFiles("*", SearchOption.AllDirectories);
                p_list.Clear();

                foreach (var item in filenums)
                {
                    var finfo = new FileInfomation(item, p_root);
                    p_list.Add(finfo);
                }
            }

        }

        #endregion

        #region 权限重写

        public override bool CanBeCompressed => false;

        public override bool CanAddData => true;

        public override bool CanCreatePath => true;

        public override bool CanDeCompressionByPath => true;

        public override bool CanDeCompressionByIndex => true;

        public override bool CanDeCompression => true;

        public override bool CanOpenCompressedStreamByPath => true;

        public override bool CanSetData => true;

        public override bool CanIndexOf => true;

        public override bool CanRemoveData => true;

        /// <summary>
        /// 不需要释放资源
        /// </summary>
        public override bool IsNeedToReleaseResources => false;

        public override bool CanOpenCompressedStreamByIndex => true;

        public override bool CanProbePath => true;

        #endregion

        #region 派生

        public override DataInformation this[int index]
        {
            get
            {
                return p_list[index];
            }
        }

        public override DataInformation this[string dataPath]
        {
            get
            {
                var path = Path.Combine(p_rootfullPath, dataPath);
                if (File.Exists(path))
                {
                    try
                    {
                        return new FileInfomation(new FileInfo(path), p_root);
                    }
                    catch (Exception)
                    {
                        return new DataInformation.EmptyInformation();
                    }
                    
                }
                return new DataInformation.EmptyInformation();
            }
        }

        public override int Count => p_list.Count;

        public override void Add(byte[] data, string dataPath)
        {
            if (data is null) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(dataPath)) throw new ArgumentException(Cheng.Properties.Resources.Exception_PathIsEmpty);

            var path = Path.Combine(p_rootfullPath, dataPath);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (FileStream file = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                file.Write(data, 0, data.Length);
            }
        }

        public override void DeCompressionTo(string dataPath, Stream stream)
        {
            if (stream is null) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(dataPath)) throw new ArgumentException(Cheng.Properties.Resources.Exception_PathIsEmpty);
            var path = Path.Combine(p_rootfullPath, dataPath);

            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                file.CopyToStream(stream, p_buffer);
            }
        }

        public override void DeCompressionTo(int index, Stream stream)
        {
            if (stream is null) throw new ArgumentNullException();
            var inf = p_list[index];
            var path = inf.DataPath;
            if (path is null) throw new FileNotFoundException();

            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                file.CopyToStream(stream, p_buffer);
            }

        }

        public override void SetData(Stream stream, string dataPath)
        {
            if (stream is null) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(dataPath)) throw new ArgumentException(Cheng.Properties.Resources.Exception_PathIsEmpty);
            var path = Path.Combine(p_rootfullPath, dataPath);
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                stream.CopyToStream(file, p_buffer);
            }

        }

        public override void SetData(byte[] data, string dataPath)
        {
            if (data is null) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(dataPath)) throw new ArgumentException(Cheng.Properties.Resources.Exception_PathIsEmpty);
            var path = Path.Combine(p_rootfullPath, dataPath);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                file.Write(data, 0, data.Length);
            }
        }

        public override bool CreatePath(string dataPath)
        {
            if (string.IsNullOrEmpty(dataPath)) throw new ArgumentException(Cheng.Properties.Resources.Exception_PathIsEmpty);
            var path = Path.Combine(p_rootfullPath, dataPath);
            
            if (File.Exists(path))
            {
                return false;
            }
            File.Create(path);
            return true;
        }

        public override Stream OpenCompressedStream(int index)
        {
            var inf = p_list[index];

            var path = inf.DataPath;
            if (path is null) throw new FileNotFoundException();

            try
            {
                return new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            }
            catch (System.Security.SecurityException)
            {
                return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }

        }

        public override Stream OpenCompressedStream(string dataPath)
        {
            if (string.IsNullOrEmpty(dataPath)) throw new ArgumentException(Cheng.Properties.Resources.Exception_PathIsEmpty);
            var path = Path.Combine(p_rootfullPath, dataPath);
            if (File.Exists(path)) return null;

            try
            {
                return new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            }
            catch (System.Security.SecurityException)
            {
                return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }

        }

        public override Stream CreateOrOpenStream(string dataPath)
        {
            if (string.IsNullOrEmpty(dataPath)) throw new ArgumentException(Cheng.Properties.Resources.Exception_PathIsEmpty);
            var path = Path.Combine(p_rootfullPath, dataPath);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            try
            {
                return new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            }
            catch (System.Security.SecurityException)
            {
                return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
            }
        }

        public override void Add(Stream stream, string dataPath)
        {
            if (string.IsNullOrEmpty(dataPath)) throw new ArgumentException(Cheng.Properties.Resources.Exception_PathIsEmpty);
            var path = Path.Combine(p_rootfullPath, dataPath);
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (FileStream file = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                stream.CopyToStream(file, p_buffer);
            }
        }

        public override void DeCompressionToText(int index, Encoding encoding, TextWriter writer)
        {
            var inf = p_list[index];
            var path = inf.DataPath;
            if (File.Exists(path)) throw new FileNotFoundException();

            DeCompressionToText(path, encoding, writer);
        }

        public override void DeCompressionToText(string dataPath, Encoding encoding, TextWriter writer)
        {
            if (string.IsNullOrEmpty(dataPath)) throw new ArgumentException(Cheng.Properties.Resources.Exception_PathIsEmpty);
            var path = Path.Combine(p_rootfullPath, dataPath);

            using (StreamReader sreader = new StreamReader(path, encoding, false, 1024 * 2))
            {
                sreader.CopyToText(writer, new char[1024 * 2]);
            }
        }

        public override bool ContainsData(string dataPath)
        {
            if (string.IsNullOrEmpty(dataPath)) throw new ArgumentException(Cheng.Properties.Resources.Exception_PathIsEmpty);
            var path = Path.Combine(p_rootfullPath, dataPath);
            return File.Exists(path);
        }

        /// <summary>
        /// 删除该文件夹下所有文件和文件夹
        /// </summary>
        public override void Clear()
        {
            var dires = p_root.GetDirectories();
            foreach (var item in dires)
            {
                try
                {
                    item.Delete();
                }
                catch (Exception)
                {
                }
            }
            var files = p_root.GetFiles("*");
            foreach (var item in files)
            {
                try
                {
                    item.Delete();
                }
                catch (Exception)
                {
                }
            }
        }

        public override bool Remove(string dataPath)
        {
            if (string.IsNullOrEmpty(dataPath)) throw new ArgumentException(Cheng.Properties.Resources.Exception_PathIsEmpty);
            var path = Path.Combine(p_rootfullPath, dataPath);

            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            return false;
        }

        public override IEnumerable<string> EnumatorFilePath()
        {

            var enr = p_root.GetFiles("*", SearchOption.TopDirectoryOnly);
            foreach (var item in enr)
            {

                string path;
                try
                {
                    
                    path = TextManipulation.Path.GetRelativeDirectoryByPath(p_rootfullPath, item.FullName);
                }
                catch (Exception)
                {
                    path = null;
                }

                if(path != null) yield return path;

            }
        }

        public override IEnumerable<string> EnumatorFileName()
        {

            var enr = p_root.GetFiles("*", SearchOption.TopDirectoryOnly);
            foreach (var item in enr)
            {
                string path;
                try
                {
                    path = item.Name;
                }
                catch (Exception)
                {
                    path = null;
                }

                if (path != null) yield return path;

            }

        }


        #endregion

        #region

        /// <summary>
        /// 清除当前目录下的所有空文件夹
        /// </summary>
        /// <remarks>
        /// <para>检测当前目录下的子文件夹，如果存在空文件夹则将其从硬盘删除；在删除子文件夹后如果父文件夹同样成为了空文件夹也会被删除，直到当前目录</para>
        /// <para>当前目录即使是空文件夹也不会被删除</para>
        /// </remarks>
        public void ClearEmptyFolder()
        {
            f_clearEmptyDire(p_root);
        }

        private static void f_clearEmptyDire(DirectoryInfo info)
        {
            foreach (DirectoryInfo subDir in info.GetDirectories())
            {
                f_clearEmptyDire(subDir); // 递归处理子目录

                // 处理完子目录后检查是否存在及空状态
                if (subDir.Exists && f_isDireEmpty(subDir))
                {
                    subDir.Delete(); // 删除空目录
                }
            }
        }

        static bool f_isDireEmpty(DirectoryInfo info)
        {
            try
            {
                var arr = info.GetDirectories("*", SearchOption.TopDirectoryOnly);
                var fs = info.GetFiles("*", SearchOption.TopDirectoryOnly);
                return arr.Length == 0 && fs.Length == 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #endregion

    }

}
