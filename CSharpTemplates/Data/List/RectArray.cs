using Cheng.Algorithm.Collections;
using System;
using System.Collections.Generic;

namespace Cheng.DataStructure.Collections
{

    /// <summary>
    /// 表示一个二维数组的公共接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITwoDimensionalArray<T>
    {

        /// <summary>
        /// 获取二维数组中所有的元素数
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 获取二维数组中的1维长度
        /// </summary>
        int Width { get; }

        /// <summary>
        /// 获取二维数组中的2维长度
        /// </summary>
        int Height { get; }

        /// <summary>
        /// 使用索引访问或修改元素
        /// </summary>
        /// <param name="x">1维索引</param>
        /// <param name="y">2维索引</param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException">参数超出范围</exception>
        T this[int x, int y] { get; set; }

    }

    /// <summary>
    /// 表示一个只读的二维集合公共接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadOnlyTwoDimesionalList<T>
    {

        /// <summary>
        /// 获取二维数组中所有的元素数
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 获取二维数组中的1维长度
        /// </summary>
        int Width { get; }

        /// <summary>
        /// 获取二维数组中的2维长度
        /// </summary>
        int Height { get; }

        /// <summary>
        /// 使用索引访问元素数量
        /// </summary>
        /// <param name="x">1维索引</param>
        /// <param name="y">2维索引</param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException">参数超出范围</exception>
        T this[int x, int y] { get; }
    }

    /// <summary>
    /// 表示一个矩形的二维数组
    /// </summary>
    /// <remarks>
    /// <para>该数组使用x作为长度索引，y使用宽度索引，以二维数组<typeparamref name="T"/>[,]做矩形计算</para>
    /// <para>
    /// 图示：<br/>
    /// <code>
    /// y
    ///  ^
    ///  | [□□□□□□□]
    ///  | [□□□□□□□]
    ///  | [□□□□□□□]
    ///  | [□□□□□□□]
    ///  | [□□□□□□□]
    ///  | [□□□□□□□]
    ///  ---------------------> x<br/>
    /// </code>
    /// </para>
    /// </remarks>
    /// <typeparam name="T">数组元素类型</typeparam>
    public class RectArray<T> : ITwoDimensionalArray<T>, IReadOnlyTwoDimesionalList<T>
    {

        #region 构图

        /*
         *
         * 
         *  y
         * ^
         * | [□□□□□□□]
         * | [□□□□□□□]
         * | [□□□□□□□]
         * | [□□□□□□□]
         * | [□□□□□□□]
         * | [□□□□□□□]
         * ---------------------> x
         * 
         *  y
         * ^
         * | [□□□□□□]
         * | [□□□□□□]
         * | [□□□□□□]
         * | [□□□□■□] => [4,2]
         * | [□□□□□□]
         * | [□□□□□□]
         * ---------------------> x
         * 
         *  y
         * ^
         * | [□□□□□□]
         * | [□□□□□□]
         * | [□■□□□□]
         * | [□□□□□□] => [1,3]
         * | [□□□□□□]
         * | [□□□□□□]
         * ---------------------> x
         * 
         */

        #endregion

        #region 构造

        /// <summary>
        /// 实例化一个空二维数组
        /// </summary>
        public RectArray()
        {
            p_arr = new T[0,0];
        }

        /// <summary>
        /// 实例化一个二维数组
        /// </summary>
        /// <param name="width">长度</param>
        /// <param name="height">宽度</param>
        public RectArray(int width, int height)
        {
            if (width < 0 || height < 0) throw new ArgumentOutOfRangeException();
            p_arr = new T[width, height];
        }

        /// <summary>
        /// 使用已有数组拷贝实例化
        /// </summary>
        /// <param name="array">要拷贝的数组</param>
        public RectArray(T[,] array)
        {
            if (array is null) throw new ArgumentNullException();
            p_arr = (T[,])array.Clone();
        }

        #endregion

        #region 参数

        #region 实例

        /// <summary>
        /// 当前实例内封装的数组
        /// </summary>
        protected T[,] p_arr;

        #endregion

        #endregion

        #region API

        #region 属性访问

        /// <summary>
        /// 获取当前实例内封装的数组
        /// </summary>
        public T[,] BaseArray
        {
            get => p_arr;
        }

        /// <summary>
        /// 获取矩形数据中所有的元素数
        /// </summary>
        public int Count
        {
            get => p_arr.Length;
        }

        /// <summary>
        /// 获取当前矩形数组的长度
        /// </summary>
        public int Width
        {
            get => p_arr.GetLength(0);
        }

        /// <summary>
        /// 获取当前矩形数组的高度
        /// </summary>
        public int Height
        {
            get => p_arr.GetLength(1);
        }

        /// <summary>
        /// 使用二维坐标访问或设置矩形数组的值
        /// </summary>
        /// <param name="x">长度坐标，范围在[0, <see cref="Width"/>)</param>
        /// <param name="y">宽度坐标，范围在[0, <see cref="Height"/>)</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public T this[int x, int y]
        {
            get
            {
                if (x < 0 || y < 0 || x >= Width || y >= Height) throw new ArgumentOutOfRangeException();
                return p_arr[x, y];
            }
            set
            {
                if (x < 0 || y < 0 || x >= Width || y >= Height) throw new ArgumentOutOfRangeException();
                p_arr[x, y] = value;
            }
        }

        #endregion

        #region 功能

        #region 静态功能

        #region 封装

        private bool f_tryMoveArg(int beginX, int endX, int beginY, int endY, int toBeginX, int toBeginY)
        {
            if (beginX < 0 || beginY < 0 || toBeginX < 0 || toBeginY < 0) return false;
            if (endX < beginX || endY < beginY) return false;
            if (endX >= Width || endY >= Height) return false;
            return true;
        }

        #endregion

        #endregion

        #region 实例函数

        /// <summary>
        /// 判断给定索引是否在范围内
        /// </summary>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <returns>处于数组范围内返回true，不在范围内返回false</returns>
        public bool IndexInScope(int x, int y)
        {
            return (x >= 0 && y >= 0) && (x < Width && y < Height);
        }

        /// <summary>
        /// 访问指定索引下的值
        /// </summary>
        /// <param name="x">x索引</param>
        /// <param name="y">y索引</param>
        /// <param name="value">要获取的值</param>
        /// <returns>是否成功获取；索引处于范围内则返回true，否则返回false</returns>
        public bool TryGet(int x, int y, out T value)
        {
            if(x < 0 || y < 0 || x >= Width || y >= Height)
            {
                value = default;
                return false;
            }

            value = p_arr[x, y];
            return true;
        }

        /// <summary>
        /// 设置指定索引下的值
        /// </summary>
        /// <param name="x">x索引</param>
        /// <param name="y">y索引</param>
        /// <param name="value">要设置的值</param>
        /// <returns>是否成功设置；索引处于范围内则返回true，否则返回false</returns>
        public bool TrySet(int x, int y, T value)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
            {
                return false;
            }

            p_arr[x, y] = value;
            return true;
        }

        /// <summary>
        /// 将二维数组中的某一块数据内容拷贝移动到另一块数据中
        /// </summary>
        /// <param name="beginX">原数据块的长度起始索引</param>
        /// <param name="countX">原数据块的长度</param>
        /// <param name="beginY">原数据块的高度起始索引</param>
        /// <param name="countY">原数据块的高度</param>
        /// <param name="toBeginX">目标数据块的起始长度索引</param>
        /// <param name="toBeginY">目标数据块的起始高度索引</param>
        /// <exception cref="ArgumentException">指定的参数超出范围</exception>
        public void MoveToRect(int beginX, int countX, int beginY, int countY, int toBeginX, int toBeginY)
        {
            int endX = beginX + countX - 1;
            int endY = beginY + countY - 1;
            if (f_tryMoveArg(beginX, endX, beginY, endY, toBeginX, toBeginY)) ListExtend.f_moveToRect(p_arr, beginX, endX, beginY, endY, toBeginX, toBeginY);
            else throw new ArgumentException();

        }

        /// <summary>
        /// 使用新容量调整二维数组大小
        /// </summary>
        /// <remarks>
        /// 此方法会实例化新的二维数组，再将数据拷贝过去
        /// </remarks>
        /// <param name="width">新的数组长度</param>
        /// <param name="height">新的数组高度</param>
        public void Capacity(int width, int height)
        {
            if (width == Width && height == Height) return;

            T[,] narr = new T[width, height];

            p_arr.CopyTo(0, System.Math.Min(width, Width), 0, System.Math.Min(height, Height), narr, 0, 0);

            p_arr = narr;
        }

        /// <summary>
        /// 将数组返回到新的二维数组中
        /// </summary>
        /// <returns></returns>
        public T[,] ToArray()
        {
            return (T[,])p_arr.Clone();
        }

        #endregion

        #endregion

        #endregion

    }

}
