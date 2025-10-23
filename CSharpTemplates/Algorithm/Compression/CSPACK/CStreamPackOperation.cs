using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Cheng.Streams;
using Cheng.Memorys;
using Cheng.Algorithm.Sorts.Comparers;
using Cheng.Algorithm.Collections;
using Cheng.DataStructure;
using Cheng.DataStructure.Hashs;
using Cheng.DataStructure.Collections;
using Cheng.DataStructure.Streams;
using Cheng.IO;

namespace Cheng.Algorithm.Compressions.CSPACK
{

    /// <summary>
    /// 包数据项的打包信息
    /// </summary>
    public readonly struct PackItem : IEquatable<PackItem>
    {
        /// <summary>
        /// 初始化包数据项的打包信息
        /// </summary>
        /// <param name="path">路径索引</param>
        /// <param name="size">字节大小</param>
        public PackItem(string path, long size)
        {
            this.path = path;
            this.size = size;
        }

        /// <summary>
        /// 项的字节大小
        /// </summary>
        public readonly long size;

        /// <summary>
        /// 项的路径索引
        /// </summary>
        public readonly string path;

        /// <summary>
        /// 返回索引字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return path;
        }

        public override int GetHashCode()
        {
            if (path is null) return size.GetHashCode();
            return path.GetHashCode() ^ size.GetHashCode();
        }

        public bool Equals(PackItem other)
        {
            return EqualityStrNotPathSeparator.EqualPath(path, other.path, false, true);
        }

        public override bool Equals(object obj)
        {
            if(obj is PackItem other)
            {
                return EqualityStrNotPathSeparator.EqualPath(path, other.path, false, true);
            }
            return false;
        }

        /// <summary>
        /// 比较项路径是否一致
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(PackItem x, PackItem y)
        {
            return EqualityStrNotPathSeparator.EqualPath(x.path, y.path, false, true);
        }

        /// <summary>
        /// 比较项路径是否不一致
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(PackItem x, PackItem y)
        {
            return !EqualityStrNotPathSeparator.EqualPath(x.path, y.path, false, true);
        }

    }

    /// <summary>
    /// 打包完成一个数据项时调用的事件函数
    /// </summary>
    /// <param name="packOperation">事件源对象</param>
    /// <param name="item">打包完成的项数据信息</param>
    public delegate void ToPackItemFunction(CStreamPackOperation packOperation, in PackItem item);

    /// <summary>
    /// CSPACK包数据打包器
    /// </summary>
    public unsafe class CStreamPackOperation : PackagingOperation
    {

        #region 结构

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化CSPACK包打包器
        /// </summary>
        public CStreamPackOperation() : this(1024 * 8)
        {
        }

        /// <summary>
        /// 初始化CSPACK包打包器
        /// </summary>
        /// <param name="bufferSize">指定打包时使用的缓冲区长度</param>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区没有大于0</exception>
        public CStreamPackOperation(int bufferSize)
        {
            if (bufferSize <= 0) throw new ArgumentOutOfRangeException();

            p_packs = new Dictionary<string, IGettingStream>(new EqualityStrNotPathSeparator(false, true));

            var pathNot = Path.GetInvalidPathChars();
            var fileNameNot = Path.GetInvalidFileNameChars();

            HashSet<char> cs = new HashSet<char>();
            foreach (var nc in pathNot)
            {
                cs.Add(nc);
            }
            foreach (var nc in fileNameNot)
            {
                cs.Add(nc);
            }
            cs.Remove('\\');
            cs.Remove('/');
            cs.TrimExcess();

            p_notPathChars = cs;
            p_buf8 = new byte[8];
            p_buffer = new byte[bufferSize];

        }

        #endregion

        #region 参数

        private Dictionary<string, IGettingStream> p_packs;

        private byte[] p_buf8;

        private byte[] p_buffer;

        private HashSet<char> p_notPathChars;

        private bool p_packaging;

        #endregion

        #region 功能

        #region 参数

        /// <summary>
        /// CSPACK包的文件标准后缀名
        /// </summary>
        public const string StandardExtensionName = ".esp";

        #endregion

        #region 打包功能

        #region 打包封装

        static void f_writeHeader(Stream stream)
        {
            //0x43, 0x53, 0x50, 0x41, 0x43, 0x4B
            stream.WriteByte(0x43);
            stream.WriteByte(0x53);
            stream.WriteByte(0x50);
            stream.WriteByte(0x41);
            stream.WriteByte(0x43);
            stream.WriteByte(0x4B);
        }

        static void f_writerToStr(Stream stream, string str, int length)
        {
            for (int i = 0; i < length; i++)
            {
                ushort uv = str[i];
                stream.WriteByte((byte)(uv & 0xFF));
                stream.WriteByte((byte)((uv >> 8) & 0xFF));
            }
        }

#if DEBUG
        /// <summary>
        /// 打包到流数据
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer"></param>
        /// <returns>
        /// <para>打包过程枚举器：返回 int 或 long 表示正在拷贝数据，值表示此次拷贝的数据字节量；返回PackItem表示成功打包完成一个项数据，值表示项数据的路径</para>
        /// </returns>
#endif
        private IEnumerable f_topackEnumator(Stream stream, byte[] buffer)
        {
            //yield return null;
            //var pcount = p_packs.Count;
            PackItem pitem;
            foreach (var pair in p_packs)
            {
                if (!p_packaging) yield break;

                var data = pair.Value;

                try
                {
                    //路径长度
                    var keyLen = pair.Key.Length;
                    //写入长度
                    stream.WriteByte((byte)keyLen);
                    //写入字符串
                    f_writerToStr(stream, pair.Key, keyLen);

                }
                catch (Exception)
                {
                    p_packaging = false;
                    throw;
                }
                
                //写入数据长度
                var dataLen = data.StreamLength;
                using (var open = data.OpenStream())
                {
                    if(open is null)
                    {
                        dataLen = 0;
                    }
                    else
                    {
                        if (dataLen == -1)
                        {
                            if (open.CanSeek)
                            {
                                dataLen = open.Length;
                            }
                            else
                            {
                                //无法获知数据长度
                                dataLen = 0;
                            }
                        }
                    }

                    if (!p_packaging) yield break;
                    if (dataLen == 0)
                    {
                        try
                        {
                            //写入长度
                            Array.Clear(p_buf8, 0, 8);
                            stream.Write(p_buf8, 0, 8);
                        }
                        catch (Exception)
                        {
                            p_packaging = false;
                            throw;
                        }
                       
                    }
                    else
                    {
                        //写入长度
                        try
                        {
                            dataLen.OrderToByteArray(p_buf8, 0);
                            stream.Write(p_buf8, 0, 8);
                        }
                        catch (Exception)
                        {
                            p_packaging = false;
                            throw;
                        }
                        IEnumerator<int> copyEnumator = null;
                        try
                        {
                            copyEnumator = open.CopyToStreamEnumator(stream, buffer).GetEnumerator();
                        }
                        catch (Exception)
                        {
                            p_packaging = false;
                            copyEnumator.Dispose();
                            throw;
                        }

                        Loop:
                        bool next;
                        try
                        {
                            if (!p_packaging) yield break;
                            next = copyEnumator.MoveNext();
                        }
                        catch (Exception)
                        {
                            p_packaging = false;
                            copyEnumator.Dispose();
                            throw;
                        }
                        if (next)
                        {
                            //继续
                            yield return copyEnumator.Current;
                            goto Loop;
                        }
                        copyEnumator.Dispose();
                    }
                    if (!p_packaging) yield break;
                    pitem = new PackItem(pair.Key, dataLen);
                    try
                    {
                        //完成后返回完成路径
                        ToPackCompleteAItemEvent?.Invoke(this, in pitem);
                    }
                    catch (Exception)
                    {
                        p_packaging = false;
                        throw;
                    }

                    yield return pitem;
                }
            }
            if (!p_packaging) yield break;
            //数据终结符
            stream.WriteByte(0);
            p_packaging = false;
        }

        private void f_packTo(Stream stream, bool wrHeader)
        {
            p_packaging = true;
            try
            {
                if(wrHeader) f_writeHeader(stream);
            }
            catch (Exception)
            {
                p_packaging = false;
                throw;
            }
            
            foreach (var ret in f_topackEnumator(stream, p_buffer))
            {
            }
        }

        private void f_setData(IGettingStream data, string path)
        {
            //if (path is null) throw new ArgumentNullException();
            p_packs[path] = data;
        }

        #endregion

        /// <summary>
        /// 将当前待打包数据队列打包到指定流，使用枚举器逐步打包
        /// </summary>
        /// <param name="stream">要将数据写入到的流</param>
        /// <param name="buffer">拷贝数据时使用的缓冲区</param>
        /// <returns>
        /// <para>一个逐步打包的枚举器，每当推进一次打包器会逐步打包一些数据</para>
        /// <para>当枚举器当前返回值为<see cref="int"/>时，表示此次动作为打包的数据字节量；返回值为<see cref="PackItem"/>时，表示已经打包完成一项，参数为打包完成的项信息</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">缓冲区长度为0</exception>
        /// <exception cref="NotSupportedException">流没有写入权限</exception>
        /// <exception cref="NotImplementedException">当前打包器已经处于打包中</exception>
        public IEnumerable EnumeratePackTo(Stream stream, byte[] buffer)
        {
            if (stream is null || buffer is null) throw new ArgumentNullException();
            if (buffer.Length == 0) throw new ArgumentException();
            if (!stream.CanWrite) throw new NotSupportedException();
            if (p_packaging) throw new NotImplementedException();
            p_packaging = true;
            return f_topackEnumator(stream, buffer);
        }

        /// <summary>
        /// 将当前待打包数据队列打包到指定流，使用枚举器逐步打包
        /// </summary>
        /// <param name="stream">要将数据写入到的流</param>
        /// <returns>
        /// <para>一个逐步打包的枚举器，每当推进一次打包器会逐步打包一些数据</para>
        /// <para>当枚举器当前返回值为<see cref="int"/>时，表示此次动作为打包的数据字节量；返回值为<see cref="PackItem"/>时，表示已经打包完成一项，参数为打包完成的项信息</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">缓冲区长度为0</exception>
        /// <exception cref="NotSupportedException">流没有写入权限</exception>
        /// <exception cref="NotImplementedException">当前打包器已经处于打包中</exception>
        public IEnumerable EnumeratePackTo(Stream stream)
        {
            return EnumeratePackTo(stream, p_buffer);
        }

        /// <summary>
        /// 将当前待打包队列打包到指定流数据内
        /// </summary>
        /// <param name="stream">要写入数据的流</param>
        /// <param name="writeHeader">是否写入数据头</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">流没有写入权限</exception>
        /// <exception cref="NotImplementedException">已处于打包状态</exception>
        public void PackTo(Stream stream, bool writeHeader)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            if (!stream.CanWrite) throw new NotSupportedException();
            if (p_packaging) throw new NotImplementedException();
            f_packTo(stream, writeHeader);
        }

        /// <summary>
        /// 当前打包器是否处于打包过程
        /// </summary>
        public bool Packaging
        {
            get => p_packaging;
        }

        /// <summary>
        /// 强制停止当前打包器处于打包状态
        /// </summary>
        public void StopPackaging()
        {
            p_packaging = false;
        }

        #endregion

        #region 事件

        /// <summary>
        /// 在打包时打包完一个数据项时执行的事件
        /// </summary>
        public event ToPackItemFunction ToPackCompleteAItemEvent;

        /// <summary>
        /// 移除当前对象内注册的所有事件
        /// </summary>
        public void ClearAllEvent()
        {
            ToPackCompleteAItemEvent = null;
        }

        #endregion

        #region 派生

        public override int Count => p_packs.Count;

        public override bool CanRemoveData => true;

        public override bool CanContainsData => true;

        public override bool CanPassword => false;

        public override bool CanAlonePassword => false;

        public sealed override void PackTo(Stream stream)
        {
            PackTo(stream, true);
        }

        public override void PackTo(Stream stream, byte[] password)
        {
            PackTo(stream);
        }

        public override void PackTo(Stream stream, string password, Encoding encoding)
        {
            PackTo(stream);
        }

        public sealed override void SetData(IGettingStream data, string path)
        {
            if (path is null || data is null) throw new ArgumentNullException();
            int length = path.Length;
            if (length == 0 || length > byte.MaxValue) throw new ArgumentException();
            if(path ==  "..") throw new ArgumentException();
            for (int i = 0; i < length; i++)
            {
                if (p_notPathChars.Contains(path[i]))
                {
                    throw new ArgumentException();
                }
            }
            f_setData(data, path);
        }

        public override void SetData(IGettingStream data, string path, byte[] password)
        {
            SetData(data, path);
        }

        public override void SetData(IGettingStream data, string path, string password, Encoding encoding)
        {
            SetData(data, path);
        }

        public override bool ContainsData(string path)
        {
            return p_packs.ContainsKey(path);
        }

        public override void RemoveData(string path)
        {
            if (!p_packs.Remove(path))
            {
                throw new ArgumentException();
            }
        }

        public override bool TryRemoveData(string path)
        {
            return p_packs.Remove(path);
        }

        public override void PackToFile(string filePath)
        {
            using (FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                PackTo(file);
            }
        }

        #endregion

        #endregion

    }

}
#if DEBUG
#endif