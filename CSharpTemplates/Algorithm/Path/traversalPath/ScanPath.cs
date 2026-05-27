using Cheng.DataStructure.Collections;
using System;
using System.Collections.Generic;
using System.IO;

namespace Cheng.Algorithm.Paths
{

    /// <summary>
    /// 一个可反复遍历硬盘文件系统的扫描器
    /// </summary>
    public class ScanPath
    {

        #region 构造
        /// <summary>
        /// 使用指定目录实例化
        /// </summary>
        /// <param name="path">初始所在目录，若为null则表示为硬盘根目录</param>
        /// <exception cref="ArgumentException">目录包含无效字符</exception>
        /// <exception cref="PathTooLongException">指定的路径或文件名超过了系统定义的最大长度</exception>
        /// <exception cref="System.Security.SecurityException">没有权限</exception>
        public ScanPath(string path)
        {
            p_nowDire = (path is null) ? null : new DirectoryInfo(path);
            f_init();
            f_scaning();
        }
        /// <summary>
        /// 使用指定目录对象实例化
        /// </summary>
        /// <param name="nowDirectory">要初始化的目录对象，若为null则表示为硬盘根目录</param>
        public ScanPath(DirectoryInfo nowDirectory)
        {
            p_nowDire = nowDirectory;
            f_init();
            f_scaning();
        }
        /// <summary>
        /// 初始化一个硬盘扫描器，从访问所有逻辑驱动器开始
        /// </summary>
        public ScanPath()
        {
            p_nowDire = null;
            f_init();
            f_scaning();
        }

        private void f_init()
        {
            p_getfiles = new ArrayReadOnly<FileInfo>(p_filesBuffer);
            p_getdires = new ArrayReadOnly<DirectoryInfo>(p_diresBuffer);
        }
        #endregion

        #region 参数

        private List<FileInfo> p_filesBuffer = new List<FileInfo>();
        private List<DirectoryInfo> p_diresBuffer = new List<DirectoryInfo>();
        private Dictionary<string, FileInfo> p_fileDictBuffer = new Dictionary<string, FileInfo>();
        private Dictionary<string, DirectoryInfo> p_dictBuffer = new Dictionary<string, DirectoryInfo>();

        /// <summary>
        /// 当前所在目录
        /// </summary>
        protected DirectoryInfo p_nowDire;
        private ArrayReadOnly<FileInfo> p_getfiles;
        private ArrayReadOnly<DirectoryInfo> p_getdires;
        #endregion

        #region 访问
        /// <summary>
        /// 访问或设置当前所在目录
        /// </summary>
        /// <value>参数设置为null时，表示硬盘根目录</value>
        public DirectoryInfo NowDirectory
        {
            get => p_nowDire;
            set
            {
                p_nowDire = value;
            }
        }
        /// <summary>
        /// 访问当前目录的所有文件
        /// </summary>
        public ArrayReadOnly<FileInfo> Files
        {
            get => p_getfiles;
        }
        /// <summary>
        /// 访问当前目录的所有文件夹
        /// </summary>
        public ArrayReadOnly<DirectoryInfo> Diretorys
        {
            get => p_getdires;
        }

        #endregion

        #region 功能

        #region 封装

        private void f_addDire(DirectoryInfo info)
        {
            p_diresBuffer.Add(info);
            p_dictBuffer[info.Name] = info;
        }

        private void f_addfile(FileInfo info)
        {
            p_filesBuffer.Add(info);
            p_fileDictBuffer.Add(info.Name, info);
        }

        private void f_scanIsNull()
        {
            string[] gens = Environment.GetLogicalDrives();
            int length = gens.Length;
            for (int i = 0; i < length; i++)
            {
                f_addDire(new DirectoryInfo(gens[i]));
            }
            p_getdires = new ArrayReadOnly<DirectoryInfo>(p_diresBuffer.ToArray());
        }
        private void f_scaning()
        {

            f_clearBuffer();
            if (p_nowDire is null)
            {
                //根目录
                f_scanIsNull();
                return;
            }
            //所在目录所有文件系统

            var enr = p_nowDire.GetFileSystemInfos();

            int length = enr.Length;
            FileSystemInfo item;

            for (int i = 0; i < length; i++)
            {
                item = enr[i];

                if (item is FileInfo)
                {
                    f_addfile((FileInfo)item);
                }
                else if (item is DirectoryInfo)
                {
                    f_addDire((DirectoryInfo)item);
                }
            }

        }

        private void f_clearBuffer()
        {
            p_filesBuffer.Clear();
            p_diresBuffer.Clear();
            p_dictBuffer.Clear();
            p_fileDictBuffer.Clear();
        }
        #endregion

        /// <summary>
        /// 重新扫描当前目录的文件和文件夹
        /// </summary>
        /// <remarks>通常在手动设置<see cref="NowDirectory"/>属性后调用</remarks>
        public void Scaning()
        {
            lock (this) f_scaning();
        }
        /// <summary>
        /// 返回上一级目录并扫描
        /// </summary>
        /// <remarks>当目录是一个硬盘下的根目录，调用此方法可访问所有逻辑驱动器</remarks>
        public void Parent()
        {
            lock (this)
            {
                p_nowDire = p_nowDire?.Parent;
                f_scaning();
            }
        }
        /// <summary>
        /// 进入指定索引的子目录并扫描
        /// </summary>
        /// <param name="index">子目录的索引，可以使用<see cref="Diretorys"/>获取目录当前子目录集合</param>
        /// <returns>索引是否越界，越界则返回false，没有则返回true</returns>
        public bool Next(int index)
        {
            bool re = p_getdires.TryGetValue(index, out p_nowDire);
            if(re) lock(this) f_scaning();
            return re;
        }

        /// <summary>
        /// 进入指定文件夹的子目录并扫描
        /// </summary>
        /// <param name="folderName">文件夹名称（不是目录）</param>
        /// <returns>是否成功找到子文件夹名称，找到返回true，否则返回false</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public bool Next(string folderName)
        {
            lock (this)
            {
                if (p_dictBuffer.TryGetValue(folderName, out var now))
                {
                    p_nowDire = now;
                    f_scaning();
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 获取该目录下指定索引的文件
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>指定索引的文件，若超出索引范围，则返回一个null</returns>
        public FileInfo GetFile(int index)
        {
            lock(p_getfiles) return (index >= 0 && index < p_getfiles.Count) ? p_getfiles[index] : null;
        }
        /// <summary>
        /// 获取该目录下指定名称的文件
        /// </summary>
        /// <param name="fileName">文件名称，带后缀名</param>
        /// <returns>指定名称的文件，若无法找到文件，则返回一个null</returns>
        public FileInfo GetFile(string fileName)
        {
            FileInfo info;
            lock(p_fileDictBuffer) if (p_fileDictBuffer.TryGetValue(fileName, out info)) return info;
            return null;
        }
        #endregion

    }

}
