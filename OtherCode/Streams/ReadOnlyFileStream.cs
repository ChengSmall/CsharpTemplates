using Cheng.Memorys;
using Cheng.Streams;
using System;
using System.IO;


namespace Cheng.IO
{

    /// <summary>
    /// 封装只读文件流以更加便利读取文件数据
    /// </summary>
    public sealed class ReadOnlyFileStream : ReleaseDestructor
    {

        #region 构造

        /// <summary>
        /// 初始化只读文件访问
        /// </summary>
        /// <param name="filePath">访问的文件路径</param>
        /// <param name="disposeFileStream">释放时是否释放基本流</param>
        public ReadOnlyFileStream(string filePath, bool disposeFileStream)
        {
            if (filePath is null) throw new ArgumentNullException();

            FileStream fs = null;

            try
            {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (Exception)
            {
                fs?.Close();
                throw;
            }

            p_filePath = Path.GetFullPath(filePath);
            p_fileName = Path.GetFileName(filePath);

            p_notCloseFile = new RegulateDisposeStream(fs, false);
            p_isDispose = disposeFileStream;
        }

        /// <summary>
        /// 初始化只读文件访问
        /// </summary>
        /// <param name="filePath">访问的文件路径</param>
        public ReadOnlyFileStream(string filePath) : this(filePath, true)
        {
        }

        #endregion

        #region 释放

        protected override bool Disposeing(bool disposeing)
        {
            if (p_isDispose && disposeing)
            {
                p_file?.Close();
            }

            p_file = null;
            return true;
        }

        /// <summary>
        /// 调用此方法关闭内部文件流
        /// </summary>
        public override void Close()
        {
            Dispose(true);
        }

        #endregion

        #region 参数

        private FileStream p_file;

        private RegulateDisposeStream p_notCloseFile;

        private string p_filePath;

        private string p_fileName;

        private bool p_isDispose;

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 获取基本封装流
        /// </summary>
        /// <returns>释放则返回null</returns>
        public FileStream BaseFileStream
        {
            get => p_file;
        }

        /// <summary>
        /// 文件所在的绝对路径
        /// </summary>
        public string FilePath
        {
            get
            {
                ThrowObjectDisposeException();
                return p_filePath;
            }
        }

        /// <summary>
        /// 获取文件名和后缀组成的文件名称
        /// </summary>
        public string FileName
        {
            get
            {
                ThrowObjectDisposeException();
                return p_fileName;
            }
        }

        /// <summary>
        /// 从基本文件流封装的不会关闭的流
        /// </summary>
        /// <returns></returns>
        public RegulateDisposeStream NotCloseStream
        {
            get
            {
                ThrowObjectDisposeException();
                return p_notCloseFile;
            }
        }

        #endregion

        #region 派生流

        /// <summary>
        /// 创建一个新的只读文件流
        /// </summary>
        /// <returns>同文件下的新的只读流</returns>
        public FileStream CreateNewFileStream()
        {
            return new FileStream(this.p_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        /// <summary>
        /// 创建一个新的截断一部分的只读文件流
        /// </summary>
        /// <param name="startPos">截断的起始位置</param>
        /// <param name="length">截断的长度</param>
        /// <returns>新的截断一部分的只读流</returns>
        public ReadOnlyPartFileStream CreatePartFileStream(long startPos, long length)
        {
            return new ReadOnlyPartFileStream(p_filePath, startPos, length, FileShare.ReadWrite);
        }

        /// <summary>
        /// 返回该实例封装的只读文件路径
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return p_filePath;
        }

        #endregion

        #endregion

    }

}
