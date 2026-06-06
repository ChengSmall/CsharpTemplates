using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Cheng.Memorys;
using Cheng.Streams;
using Cheng.Algorithm;

//using T = System.Byte;

namespace Cheng.DataStructure.Collections
{

    /// <summary>
    /// 字节缓存队列
    /// </summary>
    public unsafe class BufferQueue : System.Collections.Generic.IReadOnlyList<byte>
    {

        #region 构造

        /// <summary>
        /// 实例化一个空的字节缓存队列
        /// </summary>
        public BufferQueue()
        {
            p_buffer = Array.Empty<byte>();
            p_version = 0;
            p_head = 0;
            p_tail = 0;
            p_size = 0;
        }

        /// <summary>
        /// 实例化字节缓存队列，指定初始容量
        /// </summary>
        /// <param name="bufferSize">初始容量</param>
        /// <exception cref="ArgumentOutOfRangeException">容量小于0</exception>
        public BufferQueue(int bufferSize)
        {
            if (bufferSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize));
            }
            if (bufferSize == 0) p_buffer = Array.Empty<byte>();
            else p_buffer = new byte[bufferSize];
            p_head = 0;
            p_tail = 0;
            p_size = 0;
        }

        /// <summary>
        /// 使用集合初始化字节缓存队列
        /// </summary>
        /// <param name="collection">要初始化到队列的字节序列，null表示空队列初始化</param>
        public BufferQueue(IEnumerable<byte> collection)
        {
            p_size = 0;
            p_version = 0;
            if (collection is null)
            {
                p_buffer = Array.Empty<byte>();
            }
            else
            {
                if (collection is ICollection<byte> c)
                {
                    p_buffer = new byte[c.Count];
                }
                else
                {
                    p_buffer = new byte[cp_DefaultCapacity];
                }

                foreach (byte item in collection)
                {
                    Enqueue(item);
                }
            }

        }

        /// <summary>
        /// 实例化字节缓存队列，读取流数据初始化缓存队列
        /// </summary>
        /// <param name="stream"></param>
        public BufferQueue(Stream stream)
        {
            p_version = 0;
            if (stream is null)
            {
                throw new ArgumentNullException();
            }
            if (!stream.CanRead) throw new NotSupportedException();

            byte[] temp;
            int rec;
            if (stream.CanSeek && stream.Length <= int.MaxValue)
            {
                temp = new byte[stream.Length];
                rec = stream.ReadBlock(temp, 0, temp.Length);
                p_head = 0;
                p_tail = rec;
                p_size = rec;
            }
            else
            {
                temp = new byte[1024 * 4];
                p_buffer = new byte[1024 * 4];
                p_head = 0;
                p_tail = 0;
                p_size = 0;

                fixed (byte* bptr = temp)
                {
                    while (true)
                    {
                        var re = stream.Read(temp, 0, 1024 * 4);
                        if (re == 0) break;
                        EnqueueBuffer(bptr, re);
                    }
                }
            }
        }

        #endregion

        #region 参数

        private const int cp_DefaultCapacity = 1024 * 4;

        private byte[] p_buffer;

#if DEBUG
        /// <summary>
        /// 索引头
        /// </summary>
#endif
        private int p_head;

#if DEBUG
        /// <summary>
        /// 索引尾
        /// </summary>
#endif
        private int p_tail;

#if DEBUG
        /// <summary>
        /// 整体长度
        /// </summary>
#endif
        private int p_size;

        private uint p_version;

        #endregion

        #region 功能

        #region 封装

        internal byte f_getElement(int i)
        {
            return p_buffer[(p_head + i) % p_buffer.Length];
        }

        internal void f_setElement(int i, byte value)
        {
            p_buffer[(p_head + i) % p_buffer.Length] = value;
        }

        private void SetCapacity(int capacity)
        {
            var newbuf = new byte[capacity];
            p_version++;
            if (p_size > 0)
            {
                fixed (byte* bufPtr = p_buffer, newPtr = newbuf)
                {

                    if (p_head < p_tail)
                    {
                        Array.Copy(p_buffer, p_head, newbuf, 0, p_size);

                        MemoryOperation.MemoryCopy(bufPtr + p_head, newPtr, p_size);
                    }
                    else
                    {
                        //Array.Copy(p_buffer, p_head, newbuf, 0, p_buffer.Length - p_head);
                        //Array.Copy(p_buffer, 0, newbuf, p_buffer.Length - p_head, p_tail);

                        MemoryOperation.MemoryCopy(bufPtr + p_head, newPtr, p_buffer.Length - p_head);
                        MemoryOperation.MemoryCopy(bufPtr + 0, newPtr + (p_buffer.Length - p_head), p_tail);
                    }
                }

            }

            p_buffer = newbuf;
            p_head = 0;
            p_tail = ((p_size != capacity) ? p_size : 0);
        }

        #endregion

        #region 访问

        /// <summary>
        /// 缓存数据队列的字节数量
        /// </summary>
        public int Length
        {
            get => p_size;
        }

        /// <summary>
        /// 使用索引访问或设置已有元素
        /// </summary>
        /// <param name="index">从第一个元素到最后一个元素范围的索引</param>
        /// <returns>索引<paramref name="index"/>下的元素</returns>
        public byte this[int index]
        {
            get
            {
                return GetElement(index);
            }
        }

        /// <summary>
        /// 使用索引从队列头向后访问已有元素
        /// </summary>
        /// <param name="index">从队列头开始的元素索引，0表示最前端</param>
        /// <returns>索引<paramref name="index"/>下的元素</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public byte GetElement(int index)
        {
            if (index < 0 || index >= p_size) throw new ArgumentOutOfRangeException();
            return f_getElement(index);
        }

        /// <summary>
        /// 使用索引从队列头向后设置已有元素
        /// </summary>
        /// <param name="index">从队列头开始的元素索引，0表示最前端</param>
        /// <param name="value">要覆盖的新元素</param>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public void SetElement(int index, byte value)
        {
            if (index < 0 || index >= p_size) throw new ArgumentOutOfRangeException();
            f_setElement(index, value);
        }

        /// <summary>
        /// 使用索引从队尾向前访问已有元素
        /// </summary>
        /// <param name="index">反向索引，0表示队尾元素</param>
        /// <returns>索引<paramref name="index"/>下的元素</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public byte LastGetElement(int index)
        {
            if (index < 0 || index >= p_size) throw new ArgumentOutOfRangeException();
            return f_getElement((p_size - 1) - index);
        }

        /// <summary>
        /// 使用索引从队尾向前设置已有元素
        /// </summary>
        /// <param name="index">反向索引，0表示队尾元素</param>
        /// <param name="value">索引<paramref name="index"/>下的元素</param>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public void LastSetElement(int index, byte value)
        {
            if (index < 0 || index >= p_size) throw new ArgumentOutOfRangeException();
            f_setElement((p_size - 1) - index, value);
        }

        #endregion

        #region 队列操作

        /// <summary>
        /// 清除队列内所有元素
        /// </summary>
        public void Clear()
        {
            p_version++;

            p_head = 0;
            p_tail = 0;
            p_size = 0;
        }

        /// <summary>
        /// 将元素推入队列
        /// </summary>
        /// <param name="value">要推入的元素</param>
        public void Enqueue(byte value)
        {
            if (p_size == p_buffer.Length)
            {
                int num = (int)((long)p_buffer.Length * 200L / 100);
                if (num < p_buffer.Length + 4)
                {
                    num = p_buffer.Length + 4;
                }

                SetCapacity(num);
            }
            p_version++;

            p_buffer[p_tail] = value;
            p_tail = (p_tail + 1) % p_buffer.Length;
            p_size++;
        }

        /// <summary>
        /// 从队列清除并返回队尾的元素
        /// </summary>
        /// <returns>最前端元素</returns>
        /// <exception cref="InvalidOperationException">队列没有元素</exception>
        public byte Dequeue()
        {
            if (p_size == 0)
            {
                throw new InvalidOperationException();
            }
            p_version++;
            byte result = p_buffer[p_head];
            //p_buffer[p_head] = default;
            p_head = (p_head + 1) % p_buffer.Length;
            p_size--;
            return result;
        }

        /// <summary>
        /// 从队列清除并返回队尾元素
        /// </summary>
        /// <param name="result">返回的队尾元素</param>
        /// <returns>如果成功获取元素返回true，队列中不存在元素返回false</returns>
        public bool TryDequeue(out byte result)
        {
            if (p_size == 0)
            {
                result = default;
                return false;
            }
            p_version++;
            result = p_buffer[p_head];
            //p_buffer[p_head] = default;
            p_head = (p_head + 1) % p_buffer.Length;
            p_size--;
            return true;
        }

        /// <summary>
        /// 从队尾清除指定数量个元素
        /// </summary>
        /// <param name="count">要清除的元素数</param>
        /// <exception cref="ArgumentOutOfRangeException">参数小于0或大于当前字节数</exception>
        public void DequeueCount(int count)
        {
            if (count < 0 || count > p_size)
            {
                throw new ArgumentOutOfRangeException();
            }
            p_version++;

            if (count == p_size)
            {
                Clear();
                return;
            }

            //if (p_head < p_tail)
            //{
            //    Array.Clear(p_buffer, p_head, count);
            //}
            //else
            //{
            //    var nextC = p_buffer.Length - p_head;
            //    var c = Math.Min(count, nextC);
            //    Array.Clear(p_buffer, p_head, c);
            //    c = Math.Min(p_tail, count - c);
            //    if (c != 0) Array.Clear(p_buffer, 0, c);
            //}

            p_head = (p_head + count) % p_buffer.Length;
            p_size -= count;
        }

        /// <summary>
        /// 将指定数量个默认值的元素推入队列
        /// </summary>
        /// <param name="count">要推入的元素数</param>
        /// <exception cref="ArgumentOutOfRangeException">参数小于0</exception>
        public void EnqueueCount(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (count == 0) return;

            var newsize = p_size + count;
            if (newsize >= p_buffer.Length)
            {
                int num = (int)((long)p_buffer.Length * 200L / 100);
                if (num < p_buffer.Length + 4)
                {
                    num = p_buffer.Length + 4;
                }
                SetCapacity(Math.Max(num, newsize));
            }
            p_version++;

            p_tail = (p_tail + count) % p_buffer.Length;
            p_size = newsize;
        }

        /// <summary>
        /// 从队尾读取指定字节并清除指定字节的值
        /// </summary>
        /// <param name="buffer">要读取到的目标位置</param>
        /// <param name="size">要读取的字节数量</param>
        /// <returns>实际读取的字节数，0表示没有可读取的内容</returns>
        /// <exception cref="ArgumentOutOfRangeException">要读取的字节数量小于0</exception>
        public int DequeueBuffer(byte* buffer, int size)
        {
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            p_version++;
            var count = PeekBuffer(buffer, size);
            p_head = (p_head + count) % p_buffer.Length;
            p_size -= count;
            return count;
        }

        /// <summary>
        /// 将指定字节数据推入数据缓存队列
        /// </summary>
        /// <param name="buffer">要写入的字节值的起始位置</param>
        /// <param name="size">要写入的字节数量</param>
        /// <exception cref="ArgumentOutOfRangeException">写入的字节数小于0</exception>
        public void EnqueueBuffer(byte* buffer, int size)
        {
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (size == 0) return;

            var newsize = p_size + size;
            if (newsize >= p_buffer.Length)
            {
                int num = (int)((long)p_buffer.Length * 200L / 100);
                if (num < p_buffer.Length + 4)
                {
                    num = p_buffer.Length + 4;
                }
                SetCapacity(Math.Max(num, newsize));
            }
            p_version++;
            fixed (byte* bptr = p_buffer)
            {
                if (p_head < p_tail)
                {
                    //Array.Clear(p_buffer, p_head, p_size);
                    MemoryOperation.MemoryCopy(buffer + 0, bptr + p_head, size);

                }
                else
                {
                    //Array.Clear(p_buffer, p_head, p_buffer.Length - p_head);
                    //Array.Clear(p_buffer, 0, p_tail);
                    var lastC = p_buffer.Length - p_head;
                    int toOfc;
                    var cpc = Math.Min(lastC, size);
                    MemoryOperation.MemoryCopy(buffer, bptr + p_head, cpc);
                    toOfc = cpc;

                    if (cpc < size)
                    {
                        // 存在剩余量
                        cpc = size - cpc;
                        MemoryOperation.MemoryCopy(buffer + toOfc, bptr, cpc);
                    }

                }
            }

            p_tail = (p_tail + size) % p_buffer.Length;
            p_size = newsize;
        }

        /// <summary>
        /// 从队尾读取指定字节并清除
        /// </summary>
        /// <param name="buffer">要读取到的字节数组</param>
        /// <param name="offset">字节数组的起始偏移</param>
        /// <param name="count">要读取的字节数量</param>
        /// <returns>实际读取的字节数，0表示没有可读取的内容</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">字节数量小于0或超出参数范围</exception>
        public int DequeueBuffer(byte[] buffer, int offset, int count)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (count < 0 || offset < 0 || (offset + count > buffer.Length))
            {
                throw new ArgumentOutOfRangeException();
            }
            fixed (byte* bptr = buffer)
            {
                return DequeueBuffer(bptr + offset, count);
            }
        }

        /// <summary>
        /// 将指定数据推入数据缓存队列
        /// </summary>
        /// <param name="buffer">要写入的字节序列</param>
        /// <param name="offset">要写入的起始偏移</param>
        /// <param name="count">要写入的字节数量</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">写入的字节数小于0或超出参数范围</exception>
        public void EnqueueBuffer(byte[] buffer, int offset, int count)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (count < 0 || offset < 0 || (offset + count > buffer.Length))
            {
                throw new ArgumentOutOfRangeException();
            }
            fixed (byte* bptr = buffer)
            {
                EnqueueBuffer(bptr + offset, count);
            }
        }

        /// <summary>
        /// 从队尾读取指定字节但不清除读取的数据
        /// </summary>
        /// <param name="buffer">要读取到的目标位置</param>
        /// <param name="size">要读取的字节数量</param>
        /// <returns>实际读取的字节数，0表示没有可读取的内容</returns>
        /// <exception cref="ArgumentOutOfRangeException">要读取的字节数量小于0</exception>
        public int PeekBuffer(byte* buffer, int size)
        {
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (p_size == 0) return 0;

            int count = Math.Min(size, p_size);
            fixed (byte* bptr = p_buffer)
            {

                if (p_head < p_tail)
                {
                    //Array.Clear(p_buffer, p_head, count);
                    MemoryOperation.MemoryCopy(bptr + p_head, buffer, count);
                }
                else
                {
                    var nextC = p_buffer.Length - p_head;
                    var c = Math.Min(count, nextC);
                    int cpc;
                    MemoryOperation.MemoryCopy(bptr + p_head, buffer, c);
                    cpc = c;
                    //Array.Clear(p_buffer, p_head, c);

                    c = Math.Min(p_tail, count - c);
                    if (c != 0)
                    {
                        MemoryOperation.MemoryCopy(bptr, buffer + cpc, c);
                        //Array.Clear(p_buffer, 0, c);
                    }
                }
            }

            //p_head = (p_head + count) % p_buffer.Length;
            //p_size -= count;
            return count;
        }

        /// <summary>
        /// 从队尾读取指定字节但不清除读取的数据
        /// </summary>
        /// <param name="buffer">要读取到的字节数组</param>
        /// <param name="offset">字节数组的起始偏移</param>
        /// <param name="count">要读取的字节数量</param>
        /// <returns>实际读取的字节数，0表示没有可读取的内容</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">字节数量小于0或超出参数范围</exception>
        public int PeekBuffer(byte[] buffer, int offset, int count)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (count < 0 || offset < 0 || (offset + count > buffer.Length))
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                return PeekBuffer(bptr + offset, count);
            }
        }

        /// <summary>
        /// 获取队尾元素但不删除
        /// </summary>
        /// <returns>队尾元素</returns>
        /// <exception cref="InvalidOperationException">队列没有元素</exception>
        public byte Peek()
        {
            if (p_size == 0)
            {
                throw new InvalidOperationException();
            }

            return p_buffer[p_head];
        }

        /// <summary>
        /// 获取队尾元素但不删除
        /// </summary>
        /// <param name="result">获取的队尾元素</param>
        /// <returns>成功获取true，队列不存在元素返回false</returns>
        public bool TryPeek(out byte result)
        {
            if (p_size == 0)
            {
                result = default;
                return false;
            }
            result = p_buffer[p_head];
            return true;
        }

        /// <summary>
        /// 将当前队列的所有元素拷贝到新的数组中
        /// </summary>
        /// <returns>包含队列所有元素的数组</returns>
        public byte[] ToArray()
        {
            byte[] array = new byte[p_size];
            if (p_size == 0)
            {
                return array;
            }

            if (p_head < p_tail)
            {
                //Array.Copy(p_buffer, p_head, array, 0, p_size);
                MemoryOperation.MemoryCopy(p_buffer, p_head, array, 0, p_size);
            }
            else
            {
                //Array.Copy(p_buffer, p_head, array, 0, p_buffer.Length - p_head);
                //Array.Copy(p_buffer, 0, array, p_buffer.Length - p_head, p_tail);
                MemoryOperation.MemoryCopy(p_buffer, p_head, array, 0, p_buffer.Length - p_head);
                MemoryOperation.MemoryCopy(p_buffer, 0, array, p_buffer.Length - p_head, p_tail);
            }

            return array;
        }

        /// <summary>
        /// 清除队列的剩余空余容量
        /// </summary>
        public void TrimExcess()
        {
            int num = (p_buffer.Length);
            if (p_size < num)
            {
                SetCapacity(p_size);
            }
        }

        /// <summary>
        /// 访问或设置队列容器的总容量
        /// </summary>
        /// <value></value>
        /// <exception cref="ArgumentOutOfRangeException">设置的容量小于当前队列元素数量</exception>
        public int Capacity
        {
            get => p_buffer.Length;
            set
            {
                if (value < p_size) throw new ArgumentOutOfRangeException();
                if (value == p_buffer.Length) return;
                SetCapacity(value);
            }
        }

        /// <summary>
        /// 将当前数据提取并写入指定流
        /// </summary>
        /// <param name="stream">要写入的流</param>
        /// <param name="buffer">用作数据拷贝的缓冲区对象</param>
        /// <returns>拷贝的字节数量</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区对象大小是0</exception>
        /// <exception cref="NotSupportedException">流没有写入权限</exception>
        /// <exception cref="ObjectDisposedException">流对象已释放</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="Exception">操作流对象时出现的其他错误</exception>
        public long DequeueToStream(Stream stream, byte[] buffer)
        {
            if (stream is null || buffer is null) throw new ArgumentNullException();
            if (buffer.Length == 0) throw new ArgumentOutOfRangeException();
            int rec;
            long c = 0;
            Loop:
            rec = DequeueBuffer(buffer, 0, buffer.Length);
            if (rec == 0) return c;
            stream.Write(buffer, 0, rec);
            c += rec;
            goto Loop;
        }

        #endregion

        #region 派生

        #region 枚举器

        /// <summary>
        /// 队列枚举器
        /// </summary>
        public struct Enumerator : IEnumerator<byte>, IDisposable, IEnumerator
        {
            internal Enumerator(BufferQueue q)
            {
                p_queue = q;
                p_version = p_queue.p_version;
                p_index = -1;
                p_cut = default(byte);
            }

            private BufferQueue p_queue;

            private int p_index;

            private uint p_version;

            private byte p_cut;

            public byte Current
            {
                get
                {
                    return p_cut;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    //if (_index < 0)
                    //{
                    //    throw new InvalidOperationException();
                    //}

                    return p_cut;
                }
            }

            public bool MoveNext()
            {
                if ((p_queue is null) || p_version != p_queue.p_version)
                {
                    throw new InvalidOperationException();
                }

                if (p_index == -2)
                {
                    return false;
                }

                p_index++;
                if (p_index == p_queue.p_size)
                {
                    p_index = -2;
                    p_cut = default(byte);
                    return false;
                }

                p_cut = p_queue.f_getElement(p_index);
                return true;
            }

            public void Reset()
            {

                if ((p_queue is null) || p_version != p_queue.p_version)
                {
                    throw new InvalidOperationException();
                }

                p_index = -1;
                p_cut = default(byte);
            }

            void IDisposable.Dispose()
            {
                p_index = -2;
                p_cut = default(byte);
            }

        }

        /// <summary>
        /// 返回一个能够循环访问队列集合的枚举器
        /// </summary>
        /// <returns>用于循环访问的枚举器</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        #region 派生

        IEnumerator<byte> IEnumerable<byte>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        int IReadOnlyCollection<byte>.Count => p_size;

        #endregion

        #endregion

        #endregion

        #endregion

    }

}