using Cheng.DataStructure.Collections;
using Cheng.GameTemplates.ChineseChess.DataStructrue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.GameTemplates.ChineseChess
{
    /// <summary>
    /// 表示象棋棋盘
    /// </summary>
    /// <remarks>
    /// <para>使用二维数组<see cref="ChiessPiece"/>[,]表示一个象棋棋盘，以<see cref="ChiessPiece"/>作为象棋棋子；象棋棋盘的基础索引使用平面直角坐标，从左下角开始是[0,0]，x轴从左往右递进，y轴从下往上递进</para>
    /// <para>棋盘大小固定在9 * 10，索引范围是[0~8,0~9]</para>
    /// </remarks>
    public class ChiessBoard : ITwoDimensionalArray<ChiessPiece>
    {

        #region 构造
        /// <summary>
        /// 实例化一个象棋棋盘，使用已有棋子二维数组
        /// </summary>
        /// <param name="pieces">表示棋子的二维数组，长宽必须为[9,10]</param>
        /// <exception cref="ArgumentException">参数为null，或数组的长宽不是[9,10]</exception>
        public ChiessBoard(ChiessPiece[,] pieces)
        {
            if (pieces is null) throw new ArgumentNullException();
            if (pieces.GetLength(0) != BoardWidth || pieces.GetLength(1) != BoardHeight) throw new ArgumentOutOfRangeException();
        }
        /// <summary>
        /// 实例化一个象棋棋盘
        /// </summary>
        public ChiessBoard()
        {
            p_board = new ChiessPiece[BoardWidth, BoardHeight];
        }

        #endregion

        #region 参数

        #region 常量
        /// <summary>
        /// 象棋棋盘宽度(x轴)
        /// </summary>
        public const int BoardWidth = 9;
        /// <summary>
        /// 象棋棋盘高度(y轴)
        /// </summary>
        public const int BoardHeight = 10;
        #endregion

        private ChiessPiece[,] p_board;

        #endregion

        #region 参数访问
        /// <summary>
        /// 获取象棋棋盘的基础数组实例
        /// </summary>
        public ChiessPiece[,] BaseBoardArray
        {
            get => p_board;
        }
        /// <summary>
        /// 象棋棋盘的总格子数量
        /// </summary>
        public int Count => BoardHeight * BoardWidth;

        /// <summary>
        /// 访问或设置象棋棋盘指定位置上的棋子
        /// </summary>
        /// <param name="x">横坐标索引</param>
        /// <param name="y">纵坐标索引</param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException">坐标超出范围</exception>
        public ChiessPiece this[byte x, byte y]
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
        /// 访问或设置象棋棋盘指定位置上的棋子
        /// </summary>
        /// <param name="position">要访问的坐标</param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException">坐标超出范围</exception>
        public ChiessPiece this[Chessrdinate position]
        {
            get
            {
                position.GetPosition(out byte x, out byte y);
                return p_board[x, y];
            }
            set
            {
                position.GetPosition(out byte x, out byte y);
                p_board[x, y] = value;
            }
        }

        /// <summary>
        /// 获取象棋棋盘指定位置上的棋子
        /// </summary>
        /// <param name="x">横坐标索引</param>
        /// <param name="y">纵坐标索引</param>
        /// <param name="piece">要获取的引用</param>
        /// <returns>是否成功获取；若指定的位置上存在棋子，则返回true；若不存在棋子或索引超出范围，返回false</returns>
        public bool TryGetPiece(byte x, byte y, out ChiessPiece piece)
        {
            piece = default;
            if (x < 0 || x > BoardWidth || y < 0 || y > BoardHeight) return false;
            piece = this[x, y];
            return !piece.IsEmpty;
        }
        /// <summary>
        /// 获取象棋棋盘指定位置上的棋子
        /// </summary>
        /// <param name="position">要获取的坐标</param>
        /// <param name="piece">要获取的引用</param>
        /// <returns>是否成功获取；若指定坐标没有超出范围则返回true；若索引超出范围，返回false</returns>
        public bool TryGetPiece(Chessrdinate position, out ChiessPiece piece)
        {
            piece = default;
            position.GetPosition(out byte x, out byte y);
            if (x < 0 || x > BoardWidth || y < 0 || y > BoardHeight) return false;
            piece = this[x, y];
            return true;
        }

        /// <summary>
        /// 检查超出范围
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>超出范围true，不超出false</returns>
        private bool f_checkPos(Chessrdinate pos)
        {
            pos.GetPosition(out byte x, out byte y);
            return (x < 0 || x > BoardWidth || y < 0 || y > BoardHeight);
        }

        #endregion

        #region 功能
        /// <summary>
        /// 清除棋盘上的所有棋子
        /// </summary>
        public void Clear()
        {
            Array.Clear(p_board, 0, BoardWidth * BoardHeight);
        }
        /// <summary>
        /// 将指定位置上的棋子移动到另一个位置
        /// </summary>
        /// <param name="orgin">要移动的棋子位置</param>
        /// <param name="to">将要移动到的目标位置</param>
        /// <param name="isCoverPiece">目标位置是否已存在棋子，已存在棋子则为true，不存在则为false</param>
        /// <returns>
        /// 是否成功移动；
        /// <para>
        /// 当指定位置上有棋子且移动成功时，返回true；<br/>
        /// 当指定位置上没有棋子，或移动的位置超出范围则返回false
        /// </para>
        /// </returns>
        public bool MoveTo(Chessrdinate orgin, Chessrdinate to, out bool isCoverPiece)
        {
            bool flag;
            ChiessPiece p, t;

            isCoverPiece = false;
            //获取原位置
            flag = TryGetPiece(orgin, out p);
            if ((!flag) || p.IsEmpty) return false;
            //获取移动位置
            flag = TryGetPiece(to, out t);
            if (!flag) return false;
            //表示覆盖
            if (!t.IsEmpty) isCoverPiece = true;

            this[to] = p;
            this[orgin] = default;
            return true;
        }
        /// <summary>
        /// 将指定位置上的棋子移动到另一个位置
        /// </summary>
        /// <param name="orgin">要移动的棋子位置</param>
        /// <param name="to">将要移动到的目标位置</param>
        /// <returns>
        /// 是否成功移动；
        /// <para>
        /// 当指定位置上有棋子且移动成功时，返回true；<br/>
        /// 当指定位置上没有棋子，或移动的位置超出范围则返回false
        /// </para>
        /// </returns>
        public bool MoveTo(Chessrdinate orgin, Chessrdinate to)
        {
            bool flag;
            ChiessPiece p;

            //获取原位置
            flag = TryGetPiece(orgin, out p);
            if ((!flag) || p.IsEmpty) return false;
            //获取移动位置
            flag = f_checkPos(to);
            if (flag) return false;

            this[to] = p;
            this[orgin] = default;
            return true;
        }

        /// <summary>
        /// 将指定位置上的棋子移动到另一个位置
        /// </summary>
        /// <param name="x">要移动的棋子的横坐标</param>
        /// <param name="y">要移动的棋子的纵坐标</param>
        /// <param name="tox">移动到的目标点横坐标</param>
        /// <param name="toy">移动到的目标点纵坐标</param>
        /// <param name="isCoverPiece">目标位置是否已存在棋子，已存在棋子则为true，不存在则为false</param>
        /// <returns>
        /// 是否成功移动；
        /// <para>
        /// 当指定位置上有棋子且移动成功时，返回true；<br/>
        /// 当指定位置上没有棋子，或移动的位置超出范围则返回false
        /// </para>
        /// </returns>
        public bool MoveTo(byte x, byte y, byte tox, byte toy, out bool isCoverPiece)
        {
            return MoveTo(new Chessrdinate(x, y), new Chessrdinate(tox, toy), out isCoverPiece);
        }
        /// <summary>
        /// 将指定位置上的棋子移动到另一个位置
        /// </summary>
        /// <param name="orgin">要移动的棋子位置</param>
        /// <param name="to">将要移动到的目标位置</param>
        /// <returns>
        /// 是否成功移动；
        /// <para>
        /// 当指定位置上有棋子且移动成功时，返回true；<br/>
        /// 当指定位置上没有棋子，或移动的位置超出范围则返回false
        /// </para>
        /// </returns>
        public bool MoveTo(byte x, byte y, byte tox, byte toy)
        {
            return MoveTo(new Chessrdinate(x, y), new Chessrdinate(tox, toy));
        }

        /// <summary>
        /// 重置棋局，将棋盘设为初始布局
        /// </summary>
        /// <param name="downTeam">指定棋盘下方的阵营，布局会根据参数选择红黑方位置</param>
        public void Reset(ChiessTeam downTeam)
        {
            downTeam = (ChiessTeam)(((byte)downTeam) & 0b11000);

            ChiessTeam upTeam = downTeam == ChiessTeam.Black ? ChiessTeam.Red : ChiessTeam.Black;

            #region 下
            //将
            this[4, 0] = new ChiessPiece(ChiessPieceType.Generals, downTeam);

            //边
            this[0, 0] = new ChiessPiece(ChiessPieceType.Car, downTeam);
            this[1, 0] = new ChiessPiece(ChiessPieceType.Horses, downTeam);
            this[2, 0] = new ChiessPiece(ChiessPieceType.Minister, downTeam);
            this[3, 0] = new ChiessPiece(ChiessPieceType.Scholars, downTeam);

            this[8, 0] = new ChiessPiece(ChiessPieceType.Car, downTeam);
            this[7, 0] = new ChiessPiece(ChiessPieceType.Horses, downTeam);
            this[6, 0] = new ChiessPiece(ChiessPieceType.Minister, downTeam);
            this[5, 0] = new ChiessPiece(ChiessPieceType.Scholars, downTeam);

            //炮
            this[1, 2] = new ChiessPiece(ChiessPieceType.Artillery, downTeam);
            this[7, 2] = new ChiessPiece(ChiessPieceType.Artillery, downTeam);

            //兵
            this[0, 3] = new ChiessPiece(ChiessPieceType.Soldiers, downTeam);
            this[2, 3] = new ChiessPiece(ChiessPieceType.Soldiers, downTeam);
            this[4, 3] = new ChiessPiece(ChiessPieceType.Soldiers, downTeam);
            this[6, 3] = new ChiessPiece(ChiessPieceType.Soldiers, downTeam);
            this[8, 3] = new ChiessPiece(ChiessPieceType.Soldiers, downTeam);
            #endregion

            #region 上
            this[4, 9] = new ChiessPiece(ChiessPieceType.Generals, upTeam);

            //边
            this[0, 9] = new ChiessPiece(ChiessPieceType.Car, upTeam);
            this[1, 9] = new ChiessPiece(ChiessPieceType.Horses, upTeam);
            this[2, 9] = new ChiessPiece(ChiessPieceType.Minister, upTeam);
            this[3, 9] = new ChiessPiece(ChiessPieceType.Scholars, upTeam);

            this[8, 9] = new ChiessPiece(ChiessPieceType.Car, upTeam);
            this[7, 9] = new ChiessPiece(ChiessPieceType.Horses, upTeam);
            this[6, 9] = new ChiessPiece(ChiessPieceType.Minister, upTeam);
            this[5, 9] = new ChiessPiece(ChiessPieceType.Scholars, upTeam);

            //炮
            this[1, 7] = new ChiessPiece(ChiessPieceType.Artillery, upTeam);
            this[7, 7] = new ChiessPiece(ChiessPieceType.Artillery, upTeam);

            //兵
            this[0, 6] = new ChiessPiece(ChiessPieceType.Soldiers, upTeam);
            this[2, 6] = new ChiessPiece(ChiessPieceType.Soldiers, upTeam);
            this[4, 6] = new ChiessPiece(ChiessPieceType.Soldiers, upTeam);
            this[6, 6] = new ChiessPiece(ChiessPieceType.Soldiers, upTeam);
            this[8, 6] = new ChiessPiece(ChiessPieceType.Soldiers, upTeam);
            #endregion

        }
        /// <summary>
        /// 重置棋局，将棋盘设为以下方棋盘为红方的初始布局
        /// </summary>
        public void Reset()
        {
            Reset(ChiessTeam.Red);
        }
        /// <summary>
        /// 删除指定坐标的棋子
        /// </summary>
        /// <param name="position">坐标</param>
        /// <returns></returns>
        public bool RemoveAt(Chessrdinate position)
        {
            if (f_checkPos(position)) return false;
            this[position] = default;
            return true;
        }

        #endregion

        #region 派生

        int ITwoDimensionalArray<ChiessPiece>.Width => BoardWidth;
        int ITwoDimensionalArray<ChiessPiece>.Height => BoardHeight;
        ChiessPiece ITwoDimensionalArray<ChiessPiece>.this[int x, int y]
        {
            get => this[(byte)x, (byte)y];
            set
            {
                this[(byte)x, (byte)y] = value;
            }
        }

        #endregion

    }
}
