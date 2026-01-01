using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Cheng.Memorys;
using Cheng.DataStructure;
using Cheng.DataStructure.Texts;

namespace Cheng.Texts
{

    /// <summary>
    /// 暴露内部缓冲区参数的可变字符串
    /// </summary>
    public unsafe sealed class CMStringBuilder : IEquatable<CMStringBuilder>
    {

        #region 初始化

        /// <summary>
        /// 实例化可变字符串
        /// </summary>
        public CMStringBuilder()
        {
            f_init(0);
            p_length = 0;
        }

        /// <summary>
        /// 实例化可变字符串
        /// </summary>
        /// <param name="capacity">指定初始缓冲区容量</param>
        /// <exception cref="ArgumentOutOfRangeException">容量小于0</exception>
        public CMStringBuilder(int capacity)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException();
            f_init(capacity);
            p_length = 0;
        }

        /// <summary>
        /// 实例化可变字符串
        /// </summary>
        /// <param name="value">指定初始字符串内容</param>
        public CMStringBuilder(string value)
        {
            if(string.IsNullOrEmpty(value))
            {
                f_init(0);
                p_length = 0;
            }
            else
            {
                f_init(value.Length);
                p_length = value.Length;
                Append(value);
            }
        }

        /// <summary>
        /// 实例化可变字符串
        /// </summary>
        /// <param name="value">指定初始字符串内容</param>
        /// <param name="startIndex">初始字符串的起始位置</param>
        /// <param name="length">要添加的字符数量</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">超出范围</exception>
        public CMStringBuilder(string value, int startIndex, int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException();
            
            f_init(length);
            p_length = 0;
            Append(value, startIndex, length);
        }

        /// <summary>
        /// 实例化可变字符串
        /// </summary>
        /// <param name="stringBuilder">将可变字符串添加到当前内容</param>
        public CMStringBuilder(StringBuilder stringBuilder)
        {
            if(stringBuilder is null)
            {
                throw new ArgumentNullException();
            }
            f_init(stringBuilder.Length);
            p_length = 0;
            Append(stringBuilder, 0, stringBuilder.Length);
        }

        private void f_init(int capacity)
        {
            if (capacity == 0) p_charBuffer = cp_emptyChar;
            else p_charBuffer = new char[capacity];
            p_newLine = Environment.NewLine;
        }

        #endregion

        #region 参数

        private static char[] cp_emptyChar = new char[0];

        private string p_newLine;

        private char[] p_charBuffer;

        #if DEBUG
        /// <summary>
        /// 当前长度
        /// </summary>
        #endif
        private int p_length;

        #endregion

        #region 功能

        #region 封装

#if DEBUG
        /// <summary>
        /// 设置新容量并将旧容量按参数拷贝
        /// </summary>
        /// <param name="newCapacity">新的容器字符数</param>
        /// <param name="copyLen">旧容器拷贝长度</param>
#endif
        private void f_setNewCapacity(int newCapacity, int copyLen)
        {
            if (newCapacity == 0)
            {
                p_charBuffer = cp_emptyChar;
                return;
            }

            char[] newBuf;
            newBuf = new char[newCapacity];
            Array.ConstrainedCopy(p_charBuffer, 0, newBuf, 0, Math.Min(newCapacity, copyLen));
            p_charBuffer = newBuf;
        }

#if DEBUG
        /// <summary>
        /// 传入即将添加的字符数，判断是否需要扩充容量
        /// </summary>
        /// <param name="addCount">即将添加的字符数</param>
#endif
        private void f_ifAddCapacity(int addCount)
        {
            //新长度
            int newLen = p_length + addCount;
            if (newLen <= p_charBuffer.Length) return;

            //默认扩容长度
            int defBufLen = p_charBuffer.Length == 0 ? 16 : p_charBuffer.Length * 2;

            //新的容量
            f_setNewCapacity(Math.Max(defBufLen, newLen), p_length);

        }

#if DEBUG
        /// <summary>
        /// 在保证容量足够的情况下添加一个换行文本
        /// </summary>
#endif
        private void f_addnewLine()
        {
            fixed (char* bufPtr = p_charBuffer, newLinePtr = p_newLine)
            {
                MemoryOperation.MemoryCopy(newLinePtr, bufPtr + p_length, p_newLine.Length * sizeof(char));
            }
            p_length += p_newLine.Length;
        }

        #endregion

        #region 参数访问

        /// <summary>
        /// 访问或设置指定索引下的字符
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>索引<paramref name="index"/>所在的字符</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引超出当前字符串长度范围</exception>
        public char this[int index]
        {
            get
            {
                if (index < 0 || index >= p_length) throw new ArgumentOutOfRangeException();
                return p_charBuffer[index];
            }
            set
            {
                if (index < 0 || index >= p_length) throw new ArgumentOutOfRangeException();
                p_charBuffer[index] = value;
            }
        }

        /// <summary>
        /// 访问或设置字符串长度
        /// </summary>
        /// <value>
        /// <para>要设置的字符串长度</para>
        /// <para>设置的新长度如果小于旧长度，则截断多余长度；如果大于旧长度，则直接扩充到新长度；如果扩充时缓冲区不足则会申请新缓冲区，如果缓冲区充足则仅调整长度参数</para>
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">参数小于0</exception>
        public int Length
        {
            get => p_length;
            set
            {
                if (value == p_length) return;

                if(value > p_length)
                {
                    //大于旧长
                    f_ifAddCapacity(value - p_length);
                }

                p_length = value;
            }
        }

        /// <summary>
        /// 访问或设置当前实例内的文本换行符
        /// </summary>
        /// <value>当前实例内的文本换行符，默认使用<see cref="Environment.NewLine"/>参数</value>
        /// <exception cref="ArgumentException">设置的换行符为空或null</exception>
        public string NewLine
        {
            get => p_newLine;
            set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentException();
                p_newLine = value;
            }
        }

        #endregion

        #region 内部参数

        /// <summary>
        /// 返回当前可变字符串内部的字符数组缓冲区
        /// </summary>
        /// <returns>
        /// <para>实例内部用于存储字符数组的缓冲区</para>
        /// <para>返回的值是实例当前用于存储字符文本的字符数组，当调用函数获取数组后，在实例修改<see cref="Capacity"/>参数或因其它情况扩容之前，返回的字符数组会一直属于该实例的字符缓冲区参数；可配合<see cref="OnlySetLength(int)"/>函数手动修改对象所存的文本和长度</para>
        /// </returns>
        public ArraySegment<char> GetCharBuffer()
        {
            return new ArraySegment<char>(p_charBuffer, 0, p_length);
        }

        /// <summary>
        /// 访问或设置可变字符内部缓冲区的容量
        /// </summary>
        /// <returns>可变字符内部缓冲区的实际容量，本质上获取的是内部缓冲区对象的<see cref="Array.Length"/>参数</returns>
        /// <value>
        /// <para>新的缓冲区容量</para>
        /// <para>实例会根据新的缓冲区容量重新申请指定长度的字符数组，并将旧的数据拷贝到新缓冲区内</para>
        /// <para>当值小于当前实例内存储的字符数量时，会截断多余的长度</para>
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">值小于0</exception>
        public int Capacity
        {
            get => p_charBuffer.Length;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException();
                if (value == p_charBuffer.Length) return;

                f_setNewCapacity(value, p_length);

                if (value < p_length)
                {
                    p_length = value;
                }
            }
        }

        /// <summary>
        /// 仅设置长度参数
        /// </summary>
        /// <remarks>
        /// <para>实例内部使用一个长度参数区分当前文本的长度和实际缓冲区长度，使用该函数可仅修改长度参数，除此之外不做其它任何动作</para>
        /// <para>调用该函数需要明确需求和理解该对象的内部结构，一般用于配合<see cref="GetCharBuffer"/>函数和非托管库做交互读写内存</para>
        /// </remarks>
        /// <param name="length">要设置到长度参数的值</param>
        public void OnlySetLength(int length)
        {
            p_length = length;
        }

        #endregion

        #region 字符串添加

        /// <summary>
        /// 确保该可变字符串的最小容量并尝试扩容
        /// </summary>
        /// <remarks>
        /// <para>如果内部的缓冲区容量比传入的值小，则将会扩容以确保不会小于参数；若参数小于或等于实际容量，则不会扩容</para>
        /// </remarks>
        /// <param name="capacity">确保的最小容量</param>
        /// <returns>尝试扩容后的实际容量</returns>
        /// <exception cref="ArgumentOutOfRangeException">参数小于0</exception>
        public int EnsureCapacity(int capacity)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException();
            if(capacity > p_charBuffer.Length)
            {
                f_setNewCapacity(capacity, p_length);
            }

            return p_charBuffer.Length;
        }

        /// <summary>
        /// 追加一段字符串
        /// </summary>
        /// <param name="value">待添加字符串</param>
        public CMStringBuilder Append(string value)
        {
            if (string.IsNullOrEmpty(value)) return this;
            
            fixed (char* bufPtr = value)
            {
                Append(bufPtr, value.Length);
            }
            return this;
        }

        /// <summary>
        /// 追加一段字符串
        /// </summary>
        /// <param name="value">待添加字符串</param>
        /// <param name="startIndex">添加的字符串起始位置</param>
        /// <param name="count">添加的字符数量</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定参数超出范围</exception>
        public CMStringBuilder Append(string value, int startIndex, int count)
        {
            if (value is null) throw new ArgumentNullException();
            if (startIndex < 0 || count < 0 || (startIndex + count) > value.Length) throw new ArgumentException();

            fixed (char* bufPtr = value)
            {
                return Append(bufPtr + startIndex, count);
            }
        }

        /// <summary>
        /// 追加一段字符串
        /// </summary>
        /// <param name="buffer">待添加缓冲区</param>
        public CMStringBuilder Append(char[] buffer)
        {
            if (buffer is null || buffer.Length == 0) return this;

            fixed (char* bufPtr = buffer)
            {
                return Append(bufPtr, buffer.Length);
            }
        }

        /// <summary>
        /// 追加一段字符串
        /// </summary>
        /// <param name="buffer">待添加缓冲区</param>
        /// <param name="index">添加的缓冲区起始位置</param>
        /// <param name="count">添加的字符数量</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定参数超出范围</exception>
        public CMStringBuilder Append(char[] buffer, int index, int count)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (index < 0 || count < 0 || (index + count) > buffer.Length) throw new ArgumentException();

            fixed (char* bufPtr = buffer)
            {
                return Append(bufPtr + index, count);
            }
        }

        /// <summary>
        /// 追加一段字符串
        /// </summary>
        /// <param name="charBuffer">待添加的字符串首地址</param>
        /// <param name="count">要添加的字符数量</param>
        public CMStringBuilder Append(char* charBuffer, int count)
        {
            if (count == 0) return this;
            f_ifAddCapacity(count);

            fixed (char* bufPtr = p_charBuffer)
            {
                MemoryOperation.MemoryCopy(charBuffer, bufPtr + p_length, count * sizeof(char));
            }
            p_length += count;
            return this;
        }

        /// <summary>
        /// 追加一个字符
        /// </summary>
        /// <param name="value">要追加的一个字符</param>
        public CMStringBuilder Append(char value)
        {
            f_ifAddCapacity(1);
            p_charBuffer[p_length++] = value;
            return this;
        }

        /// <summary>
        /// 追加一个字符
        /// </summary>
        /// <param name="value">要追加的字符</param>
        /// <returns></returns>
        public CMStringBuilder Append(Unichar value)
        {
            value.AppendString(this);
            return this;
        }

        /// <summary>
        /// 追加多个相同字符
        /// </summary>
        /// <param name="value">要追加的字符</param>
        /// <param name="count">要追加的字符数量</param>
        /// <returns></returns>
        public CMStringBuilder AppendMultChar(char value, int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException();
            if (count == 0) return this;
            int end = p_length + count;
            f_ifAddCapacity(count);
            for (int i = p_length; i < end; i++)
            {
                p_charBuffer[i] = value;
            }
            p_length = end;
            return this;
        }

        /// <summary>
        /// 追加一段字符串
        /// </summary>
        /// <param name="stringBuilder">待添加字符串缓冲区</param>
        /// <param name="index">添加的缓冲区起始位置</param>
        /// <param name="count">添加的字符数量</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定参数超出范围</exception>
        public CMStringBuilder Append(StringBuilder stringBuilder, int index, int count)
        {
            if (stringBuilder is null) throw new ArgumentNullException();
            if (index < 0 || count < 0 || (index + count) > stringBuilder.Length) throw new ArgumentException();

            if (count == 0) return this;
            f_ifAddCapacity(count);

            stringBuilder.CopyTo(index, p_charBuffer, 0, count);

            p_length += count;
            return this;
        }

        /// <summary>
        /// 追加一段字符串
        /// </summary>
        /// <param name="stringBuilder">待添加字符串缓冲区</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public CMStringBuilder Append(StringBuilder stringBuilder)
        {
            if (stringBuilder is null) throw new ArgumentNullException();
            int count = stringBuilder.Length;
            if (count == 0) return this;
            f_ifAddCapacity(count);

            stringBuilder.CopyTo(0, p_charBuffer, 0, count);
            p_length += count;
            return this;
        }

        /// <summary>
        /// 将追加一段指定对象的文本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">要添加的对象</param>
        /// <param name="format">
        /// <para>要使用的格式</para>
        /// <para>传入null（在 Visual Basic 中为 Nothing），表示为<see cref="System.IFormattable"/>实现的类型定义的默认格式</para>
        /// </param>
        /// <param name="formatProvider">
        /// <para>要用于对值设置格式的提供程序</para>
        /// <para>传入null，表示从操作系统的当前区域设置获取数字格式信息</para>
        /// </param>
        public CMStringBuilder Append<T>(T value, string format, IFormatProvider formatProvider) where T : IFormattable
        {
            return Append(value?.ToString(format, formatProvider));
        }

        /// <summary>
        /// 将追加一段指定对象的文本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">要添加的对象</param>
        public CMStringBuilder Append<T>(T value)
        {
            return Append(value?.ToString());
        }

        /// <summary>
        /// 从另一个可变字符串追加一段字符串
        /// </summary>
        /// <param name="cmStringBuilder">待添加的内容</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public CMStringBuilder Append(CMStringBuilder cmStringBuilder)
        {
            if (cmStringBuilder is null) throw new ArgumentNullException();
            Append(cmStringBuilder.p_charBuffer, 0, cmStringBuilder.p_length);
            return this;
        }

        /// <summary>
        /// 追加一个换行符
        /// </summary>
        public CMStringBuilder AppendLine()
        {
            f_ifAddCapacity(p_newLine.Length);
            fixed (char* bufPtr = p_newLine)
            {
                return Append(bufPtr, p_newLine.Length);
            }
        }

        /// <summary>
        /// 追加一段字符串并在结尾追加换行符
        /// </summary>
        /// <param name="value">待添加字符串</param>
        public CMStringBuilder AppendLine(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return AppendLine();
            }

            fixed (char* bufPtr = value)
            {
                return AppendLine(bufPtr, value.Length);
            }
        }

        /// <summary>
        /// 追加一段字符串并在结尾追加换行符
        /// </summary>
        /// <param name="value">待添加字符串</param>
        /// <param name="startIndex">添加的字符串起始位置</param>
        /// <param name="count">添加的字符数量</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定参数超出范围</exception>
        public CMStringBuilder AppendLine(string value, int startIndex, int count)
        {
            if (value is null) throw new ArgumentNullException();
            if (startIndex < 0 || count < 0 || (startIndex + count) > value.Length) throw new ArgumentException();

            fixed (char* bufPtr = value)
            {
                return AppendLine(bufPtr + startIndex, count);
            }
        }

        /// <summary>
        /// 追加一段字符串并在结尾追加换行符
        /// </summary>
        /// <param name="buffer">待添加缓冲区</param>
        public CMStringBuilder AppendLine(char[] buffer)
        {
            if (buffer is null)
            {
                return AppendLine();
            }

            fixed (char* bufPtr = buffer)
            {
                return AppendLine(bufPtr, buffer.Length);
            }
        }

        /// <summary>
        /// 追加一段字符串并在结尾追加换行符
        /// </summary>
        /// <param name="buffer">待添加缓冲区</param>
        /// <param name="index">添加的缓冲区起始位置</param>
        /// <param name="count">添加的字符数量</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定参数超出范围</exception>
        public CMStringBuilder AppendLine(char[] buffer, int index, int count)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (index < 0 || count < 0 || (index + count) > buffer.Length) throw new ArgumentException();

            fixed (char* bufPtr = buffer)
            {
                return AppendLine(bufPtr + index, count);
            }
        }

        /// <summary>
        /// 追加一段字符串并在结尾追加换行符
        /// </summary>
        /// <param name="charBuffer">待添加的字符串首地址</param>
        /// <param name="count">要添加的字符数量</param>
        public CMStringBuilder AppendLine(char* charBuffer, int count)
        {
            int addCount = count + p_newLine.Length;
            f_ifAddCapacity(addCount);

            fixed (char* bufPtr = p_charBuffer, newLinePtr = p_newLine)
            {
                MemoryOperation.MemoryCopy(charBuffer, bufPtr + p_length, count * sizeof(char));
                p_length += count;
                MemoryOperation.MemoryCopy(newLinePtr, bufPtr + p_length, p_newLine.Length * sizeof(char));
                p_length += p_newLine.Length;
            }
            return this;
        }

        /// <summary>
        /// 追加一段字符串并在结尾追加换行符
        /// </summary>
        /// <param name="stringBuilder">待添加字符串缓冲区</param>
        /// <param name="index">添加的缓冲区起始位置</param>
        /// <param name="count">添加的字符数量</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定参数超出范围</exception>
        public CMStringBuilder AppendLine(StringBuilder stringBuilder, int index, int count)
        {
            if (stringBuilder is null) throw new ArgumentNullException();
            if (index < 0 || count < 0 || (index + count) > stringBuilder.Length) throw new ArgumentException();

            //if (count == 0) return;
            int addCount = count + p_newLine.Length;
            f_ifAddCapacity(addCount);

            stringBuilder.CopyTo(index, p_charBuffer, 0, count);
            p_length += count;
            f_addnewLine();
            return this;
        }

        /// <summary>
        /// 从另一个可变字符串追加一段字符串并在结尾追加换行符
        /// </summary>
        /// <param name="cmStringBuilder">待添加的内容</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public CMStringBuilder AppendLine(CMStringBuilder cmStringBuilder)
        {
            if (cmStringBuilder is null) throw new ArgumentNullException();
            return AppendLine(cmStringBuilder.p_charBuffer, 0, cmStringBuilder.p_length);
        }

        /// <summary>
        /// 将追加一段指定对象的文本并在结尾追加换行符
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">要添加的对象</param>
        /// <param name="format">
        /// <para>要使用的格式</para>
        /// <para>传入null（在 Visual Basic 中为 Nothing），表示为<see cref="System.IFormattable"/>实现的类型定义的默认格式</para>
        /// </param>
        /// <param name="formatProvider">
        /// <para>要用于对值设置格式的提供程序</para>
        /// <para>传入null，表示从操作系统的当前区域设置获取数字格式信息</para>
        /// </param>
        public CMStringBuilder AppendLine<T>(T value, string format, IFormatProvider formatProvider) where T : IFormattable
        {
            return AppendLine(value?.ToString(format, formatProvider));
        }

        /// <summary>
        /// 将追加一段指定对象的文本并在结尾追加换行符
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">要添加的对象</param>
        public CMStringBuilder AppendLine<T>(T value)
        {
            return AppendLine(value?.ToString());
        }

        #endregion

        #region 字符串修改

        /// <summary>
        /// 清空内容
        /// </summary>
        /// <returns>实例自身</returns>
        public CMStringBuilder Clear()
        {
            p_length = 0;
            return this;
        }

        /// <summary>
        /// 删除指定范围的字符串
        /// </summary>
        /// <param name="startIndex">要删除的起始字符串</param>
        /// <param name="count">要删除的字符串数量</param>
        /// <returns>实例自身</returns>
        public CMStringBuilder Remove(int startIndex, int count)
        {
            int leftIndex = startIndex + count;

            if (startIndex < 0 || count < 0 || (leftIndex) > p_length) throw new ArgumentOutOfRangeException();
            if (count == 0) return this;

            if(leftIndex < p_length) Array.Copy(p_charBuffer, leftIndex, p_charBuffer, startIndex, count);

            p_length -= count;
            return this;
        }

        /// <summary>
        /// 从末尾开始向前删除指定数量的字符
        /// </summary>
        /// <param name="count">删除的字符数量</param>
        /// <returns>实例自身</returns>
        /// <exception cref="ArgumentOutOfRangeException">参数<paramref name="count"/>小于0或大于当前字符串长度</exception>
        public CMStringBuilder RemoveEnd(int count)
        {
            if (count < 0 || (count) > p_length) throw new ArgumentOutOfRangeException();
            p_length -= count;
            return this;
        }

        /// <summary>
        /// 将字符数组插入到指定索引的位置
        /// </summary>
        /// <param name="index">要插入的索引位置</param>
        /// <param name="value">待插入的字符串，null表示不插入</param>
        public CMStringBuilder Insert(int index, string value)
        {
            if (string.IsNullOrEmpty(value)) return this;
            Insert(index, value, 0, value.Length);
            return this;
        }

        /// <summary>
        /// 将字符数组插入到指定索引的位置
        /// </summary>
        /// <param name="index">要插入的索引位置</param>
        /// <param name="value">待插入的字符串</param>
        /// <param name="startIndex">要插入的缓冲区起始索引</param>
        /// <param name="count">要插入的字符串数量</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">参数超出范围</exception>
        public CMStringBuilder Insert(int index, string value, int startIndex, int count)
        {
            if (value is null) throw new ArgumentNullException();
            if (index < 0 || count < 0 || (startIndex < 0) || count < 0 || (startIndex + count > value.Length) || index > p_length)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (count == 0) return this;

            f_ifAddCapacity(count);

            if (index == p_length)
            {
                return Append(value, startIndex, count);
            }
            else
            {

                int endi = index + count;
                //var size = count + sizeof(char);

                Array.Copy(p_charBuffer, index, p_charBuffer, endi, count);
                value.CopyTo(startIndex, p_charBuffer, index, count);
                //Array.Copy(buffer, startIndex, p_charBuffer, index, count);

                p_length += count;
                return this;
            }
        }

        /// <summary>
        /// 将字符数组插入到指定索引的位置
        /// </summary>
        /// <param name="index">要插入的索引位置</param>
        /// <param name="buffer">待插入的字符缓冲区</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public CMStringBuilder Insert(int index, char[] buffer)
        {
            if (buffer is null) throw new ArgumentNullException();
            return Insert(index, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 将字符数组插入到指定索引的位置
        /// </summary>
        /// <param name="index">要插入的索引位置</param>
        /// <param name="buffer">待插入的字符缓冲区</param>
        /// <param name="startIndex">要插入的缓冲区起始索引</param>
        /// <param name="count">要插入的字符串数量</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">参数超出范围</exception>
        public CMStringBuilder Insert(int index, char[] buffer, int startIndex, int count)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (index < 0 || count < 0 || (startIndex < 0) || count < 0 || (startIndex + count > buffer.Length) || index > p_length)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (count == 0) return this;

            f_ifAddCapacity(count);

            if (index == p_length)
            {
                return Append(buffer, startIndex, count);
            }
            else
            {
                
                int endi = index + count;

                Array.Copy(p_charBuffer, index, p_charBuffer, endi, count);

                Array.Copy(buffer, startIndex, p_charBuffer, index, count);

                p_length += count;
                return this;
            }
        }

        /// <summary>
        /// 将可变字符串内的字符串插入到指定索引的位置
        /// </summary>
        /// <param name="index">要插入的索引位置</param>
        /// <param name="stringBuilder">待插入的可变字符串</param>
        /// <param name="startIndex">要插入的可变字符串起始索引</param>
        /// <param name="count">要插入的字符串数量</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">参数超出范围</exception>
        public CMStringBuilder Insert(int index, StringBuilder stringBuilder, int startIndex, int count)
        {
            if (stringBuilder is null) throw new ArgumentNullException();
            if (index < 0 || count < 0 || (startIndex < 0) || count < 0 || (startIndex + count > stringBuilder.Length) || index > p_length)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (count == 0) return this;

            f_ifAddCapacity(count);

            if (index == p_length)
            {
                return Append(stringBuilder, startIndex, count);
            }
            else
            {
                int endi = index + count;

                Array.Copy(p_charBuffer, index, p_charBuffer, endi, count);
                //Array.Copy(buffer, startIndex, p_charBuffer, index, count);
                stringBuilder.CopyTo(startIndex, p_charBuffer, index, count);

                p_length += count;

                return this;
            }
        }

        private void f_insert(int index, char* charBuffer, int count)
        {
            f_ifAddCapacity(count);

            if (index == p_length)
            {
                Append(charBuffer, count);
            }
            else
            {
                fixed(char* thisBuf = p_charBuffer)
                {
                    int endi = index + count;
                    var size = count + sizeof(char);
                    MemoryOperation.MemoryCopyWhole(thisBuf + index, thisBuf + endi, size);
                    MemoryOperation.MemoryCopy(charBuffer, thisBuf + index, size);
                    p_length += count;
                }
              
            }
        }

        /// <summary>
        /// 将字符串插入到指定索引的位置
        /// </summary>
        /// <param name="index">要插入的索引位置</param>
        /// <param name="charBuffer">待插入字符串的首地址</param>
        /// <param name="count">要插入的字符串数量</param>
        /// <exception cref="ArgumentOutOfRangeException">参数超出范围</exception>
        public CMStringBuilder Insert(int index, char* charBuffer, int count)
        {
            if (index < 0 || count < 0 || index > p_length)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (count == 0) return this;
            f_insert(index, charBuffer, count);
            return this;
        }

        #endregion

        #region 导出

        /// <summary>
        ///返回缓冲区内的字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (p_length == 0) return string.Empty;
            return new string(p_charBuffer, 0, p_length);
        }

        /// <summary>
        /// 返回缓冲区内指定区域的字符串
        /// </summary>
        /// <param name="startIndex">要返回的缓冲区字符串起始索引</param>
        /// <param name="length">要返回的字符数量</param>
        /// <returns>指定区域的字符串</returns>
        /// <exception cref="ArgumentOutOfRangeException">参数超出范围</exception>
        public string ToString(int startIndex, int length)
        {
            if (startIndex < 0 || length < 0 || (startIndex + length) > p_length) throw new ArgumentOutOfRangeException();
            return new string(p_charBuffer, startIndex, length);
        }

        /// <summary>
        /// 将缓冲区内指定区域的字符串拷贝到字符数组
        /// </summary>
        /// <param name="index">缓冲区内的起始索引</param>
        /// <param name="count">要拷贝到数组的字符数量</param>
        /// <param name="toArray">拷贝的目标数组</param>
        /// <param name="toIndex">目标数组接受时的起始索引</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">参数超出范围</exception>
        public void CopyToArray(int index, int count, char[] toArray, int toIndex)
        {
            if (toArray is null) throw new ArgumentNullException();

            if (index < 0 || count < 0 || (index + count) > p_length) throw new ArgumentOutOfRangeException();

            if (toIndex + count > toArray.Length) throw new ArgumentOutOfRangeException();

            Array.Copy(p_charBuffer, index, toArray, toIndex, count);

        }

        /// <summary>
        /// 将缓冲区内指定区域的字符串拷贝到指定内存
        /// </summary>
        /// <param name="index">缓冲区内的起始索引</param>
        /// <param name="count">要拷贝的字符数量</param>
        /// <param name="charBuffer">拷贝到的目标内存起始位</param>
        /// <exception cref="ArgumentOutOfRangeException">参数超出范围</exception>
        public void CopyToBuffer(int index, int count, char* charBuffer)
        {
            if (index < 0 || count < 0 || (index + count) > p_length) throw new ArgumentOutOfRangeException();

            fixed (char* thisBuf = p_charBuffer)
            {
                MemoryOperation.MemoryCopyWhole(thisBuf + index, charBuffer, sizeof(char) * count);
            }

        }

        /// <summary>
        /// 将当前缓冲区的字符串添加到<see cref="StringBuilder"/>可变字符串
        /// </summary>
        /// <param name="append">待添加可变字符串</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void AppendTo(StringBuilder append)
        {
            if (append is null) throw new ArgumentNullException();
            append.Append(p_charBuffer, 0, p_length);
        }

        /// <summary>
        /// 将当前缓冲区的字符串插入到<see cref="StringBuilder"/>可变字符串
        /// </summary>
        /// <param name="append">待插入可变字符串</param>
        /// <param name="index">插入到可变字符串的位置</param>
        public void InsertTo(StringBuilder append, int index)
        {
            if (append is null) throw new ArgumentNullException();
            append.Insert(index, p_charBuffer, 0, p_length);
        }

        /// <summary>
        /// 将此实例的指定段中的字符复制到目标字符数组的指定段中
        /// </summary>
        /// <param name="sourceIndex">此实例中开始复制字符的位置</param>
        /// <param name="destination">将从中复制字符的数组</param>
        /// <param name="destinationIndex"><paramref name="destination"/>中将从其开始复制字符的起始位置</param>
        /// <param name="count">要复制的字符数</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定范围超出索引</exception>
        /// <exception cref="ArgumentException">要拷贝的数组缓冲区长度不够</exception>
        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
        {
            if (destination is null) throw new ArgumentNullException();

            if (destinationIndex < 0 || sourceIndex < 0 || count < 0 || (sourceIndex + count) > p_length) throw new ArgumentOutOfRangeException();

            if (destinationIndex + count > destination.Length)
            {
                throw new System.ArgumentException();
            }

            Array.Copy(p_charBuffer, sourceIndex, destination, destinationIndex, count);
        }

        #endregion

        #region 派生

        /// <summary>
        /// 对比字符串是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(CMStringBuilder other)
        {
            if (other is null) return false;

            if (p_length != other.p_length) return false;

            fixed (char* cthis = p_charBuffer, cother = other.p_charBuffer)
            {
                return MemoryOperation.EqualsMemory(cthis, cother, p_length * sizeof(char));
            }
        }

        /// <summary>
        /// 比较内容是否相等
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool operator ==(CMStringBuilder s1, CMStringBuilder s2)
        {
            if ((object)s1 == (object)s2) return true;
            if (s1 is null || s2 is null) return false;

            var len = s1.Length;
            if (len != s2.Length) return false;

            fixed (char* cp1 = s1.p_charBuffer, cp2 = s2.p_charBuffer)
            {
                return MemoryOperation.EqualsMemory(cp1, cp2, len * sizeof(char));
            }
        }

        /// <summary>
        /// 比较内容是否不相等
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool operator !=(CMStringBuilder s1, CMStringBuilder s2)
        {
            if ((object)s1 == (object)s2) return false;
            if (s1 is null || s2 is null) return true;

            var len = s1.Length;
            if (len != s2.Length) return true;

            fixed (char* cp1 = s1.p_charBuffer, cp2 = s2.p_charBuffer)
            {
                return !MemoryOperation.EqualsMemory(cp1, cp2, len * sizeof(char));
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is CMStringBuilder s) return this == s;
            return false;
        }

        public override int GetHashCode()
        {
            fixed (char* p = p_charBuffer)
            {
                return Cheng.Algorithm.HashCodes.HashCode64.GetHashCode64ByPointer(p, p_length).GetHashCode();
            }
        }

        #endregion

        #endregion

    }

}
#if DEBUG
#endif