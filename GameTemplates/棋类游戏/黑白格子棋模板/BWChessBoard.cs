using Cheng.DataStructure.Collections;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.GameTemplates.BlackWhiteChiess
{
    /// <summary>
    /// 黑白棋类棋盘
    /// </summary>
    /// <remarks>实现包括但不限于围棋，五子棋，黑白棋的棋盘</remarks>
    public class BWChessBoard : ITwoDimensionalArray<BWPiece>
    {

        #region 构造

        public BWChessBoard(int width, int height)
        {
            if (width < 0 || height < 0) throw new ArgumentOutOfRangeException();
            p_width = width;
            p_height = height;
            p_board = new BWPiece[width, height];
            p_length = width * height;
        }

        #endregion

        #region 参数
        protected BWPiece[,] p_board;
        protected readonly int p_width;
        protected readonly int p_height;
        protected readonly int p_length;
        #endregion

        #region 功能

        #region 数组功能
        /// <summary>
        /// 获取棋盘的长度
        /// </summary>
        public int Width => p_width;
        /// <summary>
        /// 获取棋盘的高度
        /// </summary>
        public int Height => p_height;
        /// <summary>
        /// 获取棋盘的总格子数
        /// </summary>
        public int Count => p_length;
        /// <summary>
        /// 使用二维索引访问或修改棋盘棋子
        /// </summary>
        /// <param name="x">横坐标</param>
        /// <param name="y">纵坐标</param>
        /// <returns>棋子</returns>
        /// <exception cref="IndexOutOfRangeException">超出索引范围</exception>
        public BWPiece this[int x, int y]
        {
            get
            {
                return p_board[x, y];
            }
            set
            {
                p_board[x, y] = value;
            }
        }
        /// <summary>
        /// 获取指定位置的棋子
        /// </summary>
        /// <param name="x">横坐标</param>
        /// <param name="y">纵坐标</param>
        /// <param name="chess">棋子</param>
        /// <returns>是否成功获取，成功返回true，不成功返回false</returns>
        public bool TryGetValue(int x, int y, out BWPiece chess)
        {
            if (x < 0 || x >= p_width || y < 0 || y >= p_height)
            {
                chess = default;
                return false;
            }

            chess = p_board[x, y];
            return true;
        }
        /// <summary>
        /// 超出范围
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>超出范围true，没超false</returns>
        private bool f_isIndexOut(int x, int y)
        {
            return (x < 0 || x >= p_width || y < 0 || y >= p_height);
        }
        /// <summary>
        /// 获取棋盘基础数组实例
        /// </summary>
        public BWPiece[,] BasePieces => p_board;
        #endregion

        #region 棋盘功能
        /// <summary>
        /// 在指定位置上放置棋子
        /// </summary>
        /// <param name="x">横坐标</param>
        /// <param name="y">纵坐标</param>
        /// <param name="piece">要放置的棋子</param>
        /// <returns>是否成功放置，若放置处没有棋子则返回true，有棋子返回false且不会放置</returns>
        /// <exception cref="IndexOutOfRangeException">索引超出范围</exception>
        public bool Place(int x, int y, BWPiece piece)
        {
            if (p_board[x, y] != BWPiece.None) return false;
            p_board[x, y] = piece;
            return true;
        }
        /// <summary>
        /// 在指定位置上放置棋子
        /// </summary>
        /// <param name="x">横坐标</param>
        /// <param name="y">纵坐标</param>
        /// <param name="piece">要放置的棋子</param>
        /// <param name="success">是否成功放置，若放置处没有棋子则设为true，有棋子设为false且不会放置</param>
        /// <returns>没有超出棋盘范围，超出返回false，没有超出返回true</returns>
        public bool Place(int x, int y, BWPiece piece, out bool success)
        {
            success = false;

            if (f_isIndexOut(x, y)) return false;

            if (p_board[x, y] != BWPiece.None) success = true;
            p_board[x, y] = piece;
            return true;
        }
        /// <summary>
        /// 在棋盘上移除并提取一个棋子
        /// </summary>
        /// <param name="x">横坐标</param>
        /// <param name="y">纵坐标</param>
        /// <param name="piece">提取的棋子</param>
        /// <returns>没有超出棋盘范围，超出返回false，没有超出返回true</returns>
        public bool PickUp(int x, int y, out BWPiece piece)
        {
            piece = BWPiece.None;
            if (f_isIndexOut(x, y)) return false;
            piece = p_board[x, y];
            p_board[x, y] = BWPiece.None;
            return true;
        }
        /// <summary>
        /// 将棋盘上所有棋子移除
        /// </summary>
        public void Clear()
        {
            Array.Clear(p_board, 0, p_length);
        }
        #endregion

        #endregion

    }
}
