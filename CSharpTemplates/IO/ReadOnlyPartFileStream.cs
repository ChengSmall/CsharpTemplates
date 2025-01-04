using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security;

using Cheng.Streams;
using Cheng.Memorys;

namespace Cheng.IO
{

    /// <summary>
    /// 对文件提供只读部分区域封装
    /// </summary>
    /// <remarks>
    /// 封装一个<see cref="FileStream"/>，并截取部分数据以只读的方式访问和读取
    /// </remarks>
    public class ReadOnlyPartFileStream : HEStream
    {

        #region 构造

        /// <summary>
        /// 实例化一个截取部分的只读文件流
        /// </summary>
        /// <param name="path">要打开的文件所在路径</param>
        /// <param name="position">要截取的文件数据的初始位置</param>
        /// <param name="length">要截取的文件长度</param>
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="NotSupportedException">无法指定权限</exception>
        /// <exception cref="FileNotFoundException">文件不存在</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="SecurityException">调用方没有所要求的权限</exception>
        /// <exception cref="DirectoryNotFoundException">指定的路径无效</exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException">指定的路径或文件名超过了系统定义的最大长度</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public ReadOnlyPartFileStream(string path, long position, long length) : this(path, position, length, FileShare.Read, 1024 * 4, 1024 * 4)
        {
        }

        /// <summary>
        /// 实例化一个截取部分的只读文件流
        /// </summary>
        /// <param name="path">要打开的文件所在路径</param>
        /// <param name="position">要截取的文件数据的初始位置</param>
        /// <param name="length">要截取的文件长度</param>
        /// <param name="share">指定其它句柄对该文件的共享访问权限</param>
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="NotSupportedException">无法指定权限</exception>
        /// <exception cref="FileNotFoundException">文件不存在</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="SecurityException">调用方没有所要求的权限</exception>
        /// <exception cref="DirectoryNotFoundException">指定的路径无效</exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException">指定的路径或文件名超过了系统定义的最大长度</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public ReadOnlyPartFileStream(string path, long position, long length, FileShare share) : this(path, position, length, share, 1024 * 4, 1024 * 4)
        {
        }

        /// <summary>
        /// 实例化一个截取部分的只读文件流
        /// </summary>
        /// <param name="path">要打开的文件所在路径</param>
        /// <param name="position">要截取的文件数据的初始位置</param>
        /// <param name="length">要截取的文件长度</param>
        /// <param name="share">指定其它句柄对该文件的共享访问权限</param>
        /// <param name="partFileStreamBufferSize">截取流的缓冲区长度</param>
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="NotSupportedException">无法指定权限</exception>
        /// <exception cref="FileNotFoundException">文件不存在</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="SecurityException">调用方没有所要求的权限</exception>
        /// <exception cref="DirectoryNotFoundException">指定的路径无效</exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException">指定的路径或文件名超过了系统定义的最大长度</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public ReadOnlyPartFileStream(string path, long position, long length, FileShare share, int partFileStreamBufferSize) : this(path, position, length, share, partFileStreamBufferSize, 1024 * 4)
        {
        }

        /// <summary>
        /// 实例化一个截取部分的只读文件流
        /// </summary>
        /// <param name="path">要打开的文件所在路径</param>
        /// <param name="position">要截取的文件数据的初始位置</param>
        /// <param name="length">要截取的文件长度</param>
        /// <param name="share">指定其它句柄对该文件的共享访问权限</param>
        /// <param name="partFileStreamBufferSize">截取流的缓冲区长度</param>
        /// <param name="fileStreamBufferSize">基本流的缓冲区长度</param>
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="NotSupportedException">无法指定权限</exception>
        /// <exception cref="FileNotFoundException">文件不存在</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="SecurityException">调用方没有所要求的权限</exception>
        /// <exception cref="DirectoryNotFoundException">指定的路径无效</exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException">指定的路径或文件名超过了系统定义的最大长度</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public ReadOnlyPartFileStream(string path, long position, long length, FileShare share, int partFileStreamBufferSize, int fileStreamBufferSize)
        {
            p_file = null;
            p_partFile = null;

            try
            {
                p_file = new FileStream(path, FileMode.Open, FileAccess.Read, share, fileStreamBufferSize);

                p_partFile = new TruncateStream(p_file, position, length, false, partFileStreamBufferSize);

                p_filePath = Path.GetFullPath(path);
            }
            catch (Exception)
            {
                p_file?.Close();
                p_partFile?.Close();
                throw;
            }

        }

        /// <summary>
        /// 实例化一个截取部分的只读文件流
        /// </summary>
        /// <param name="file">要指定截取的文件，该文件需要查找和读取权限</param>
        /// <param name="position">要截取的文件数据的初始位置</param>
        /// <param name="length">要截取的文件长度</param>
        /// <param name="partFileStreamBufferSize">截取流的缓冲区长度</param>
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="SecurityException">调用方没有所要求的权限</exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ObjectDisposedException">文件已释放</exception>
        public ReadOnlyPartFileStream(FileStream file, long position, long length, int partFileStreamBufferSize)
        {
            if (file is null) throw new ArgumentNullException();

            p_file = file;
            
            p_partFile = new TruncateStream(file, position, length, false, partFileStreamBufferSize);

            p_filePath = Path.GetFullPath(p_file.Name);
        }

        /// <summary>
        /// 实例化一个截取部分的只读文件流
        /// </summary>
        /// <param name="file">要指定截取的文件，该文件需要查找和读取权限</param>
        /// <param name="position">要截取的文件数据的初始位置</param>
        /// <param name="length">要截取的文件长度</param>
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="SecurityException">调用方没有所要求的权限</exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ObjectDisposedException">文件已释放</exception>
        public ReadOnlyPartFileStream(FileStream file, long position, long length) : this(file, position, length, 1024 * 4)
        {
        }

        #endregion

        #region 释放

        /// <summary>
        /// 重写该函数以释放资源和句柄
        /// </summary>
        /// <remarks>在派生类重写需要调用基实现</remarks>
        /// <param name="disposing">是否清理托管资源实例</param>
        /// <returns>回收完毕后是否取消对象终结器调用；true表示取消，false表示不取消</returns>
        protected override bool Disposing(bool disposing)
        {          
            if (disposing)
            {
                p_file.Close();
                p_partFile.Close();
            }            

            p_file = null;
            p_partFile = null;
            return true;
        }

        #endregion

        #region 参数

        private FileStream p_file;

        private TruncateStream p_partFile;

        private string p_filePath;

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 获取传递给构造函数的文件路径
        /// </summary>
        public string Name
        {
            get => p_file.Name;
        }

        /// <summary>
        /// 获取该文件的绝对路径
        /// </summary>
        public string FileFullPath
        {
            get => p_filePath;
        }

        /// <summary>
        /// 获取此实例内部封装的文件流
        /// </summary>
        /// <remarks>若先使用内部流读写再使用此实例读取数据，会影响封装流的读取效率，也可能会造成IO性能的额外浪费和损耗</remarks>
        public FileStream BaseFileStream
        {
            get => p_file;
        }

        #endregion

        #region 派生

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length
        {
            get
            {
                ThrowIsDispose();
                return p_partFile.Length;
            }
        }

        public override long Position 
        { 
            get
            {
                ThrowIsDispose();
                return p_partFile.Position;
            }
            set
            {
                ThrowIsDispose();
                p_partFile.Position = value;
            }
        }

        public override bool CanTimeout
        {
            get
            {
                return (p_partFile?.CanTimeout).GetValueOrDefault();
            }
        }

        public override int ReadTimeout
        {
            get => (p_partFile?.ReadTimeout).GetValueOrDefault();
            set
            {
                if (p_partFile != null) p_partFile.ReadTimeout = value;
            }
        }

        public override int WriteTimeout 
        { 
            get => p_partFile.WriteTimeout; 
            set
            {
                if(!IsDispose) p_partFile.WriteTimeout = value;
            }
        }

        /// <summary>
        /// 只读流中为无效操作
        /// </summary>
        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose();
            return p_partFile.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            ThrowIsDispose();
            return p_partFile.Seek(offset, origin);
        }

        public override int ReadByte()
        {
            ThrowIsDispose();
            return p_partFile.ReadByte();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void WriteByte(byte value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            ThrowIsDispose();
            return p_partFile.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }


        public override void EndWrite(IAsyncResult asyncResult)
        {
            ThrowIsDispose();
            p_partFile.EndWrite(asyncResult);
        }


        public override int EndRead(IAsyncResult asyncResult)
        {
            ThrowIsDispose();
            return p_partFile.EndRead(asyncResult);
        }

        #endregion

        #endregion

    }
}
