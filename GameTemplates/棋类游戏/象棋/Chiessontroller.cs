using Cheng.GameTemplates.ChineseChess.DataStructrue;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.GameTemplates.ChineseChess
{
    public class Chiessontroller
    {

        #region 构造
        /// <summary>
        /// 实例化一个玩家操控器，指定棋盘和阵营
        /// </summary>
        /// <param name="board">要操控的棋盘</param>
        /// <param name="team">所在的阵营</param>
        public Chiessontroller(ChiessBoard board, ChiessTeam team)
        {
            BaseBoard = board;
            PlayerTeam = team;
        }
        #endregion

        #region 参数
        private List<Chessrdinate> p_pickUpToPlaceable = new List<Chessrdinate>(4);
        private ChiessBoard p_board;

        private Chessrdinate? p_selectivePiece = null;
        private ChiessTeam p_team;
        #endregion

        #region 功能

        #region 参数访问
        /// <summary>
        /// 访问或设置要操控的棋盘
        /// </summary>
        /// <exception cref="ArgumentNullException">设置的对象为null</exception>
        public ChiessBoard BaseBoard
        {
            get => p_board;
            set
            {
                p_board = value ?? throw new ArgumentNullException();
            }
        }
        /// <summary>
        /// 访问或设置此操控器的阵营
        /// </summary>
        /// <exception cref="ArgumentException">值不是预设的阵营枚举</exception>
        public ChiessTeam PlayerTeam
        {
            get => p_team;
            set
            {
                if (value != ChiessTeam.Red && value != ChiessTeam.Black) throw new ArgumentException();
                p_team = value;
            }
        }
        /// <summary>
        /// 获取在棋盘上拿起的棋子，若没有拿起棋子则表示一个空结构
        /// </summary>
        public ChiessPiece Picked
        {
            get
            {
                if (!p_selectivePiece.HasValue) return default;
                return p_board[TranslatePosition(p_selectivePiece.Value)];
            }
        }
        /// <summary>
        /// 此控制器是否拿起了一个棋子
        /// </summary>
        public bool IsPicked
        {
            get => p_selectivePiece.HasValue;
        }
        #endregion

        #region 功能
        /// <summary>
        /// 转化棋盘坐标和相对坐标
        /// </summary>
        /// <param name="position">原始坐标</param>
        /// <returns>当参数为原始坐标时，返回此控制器的相对坐标；当参数为相对坐标时，返回棋盘坐标</returns>
        public Chessrdinate TranslatePosition(Chessrdinate position)
        {
            if (p_team == ChiessTeam.Black) return position.Inversion;
            return position;
        }

        /// <summary>
        /// 根据相对索引访问指定位置的棋子信息
        /// </summary>
        /// <remarks>
        /// 相对坐标表示为阵营方位的坐标，红色阵营的相对坐标和棋盘默认坐标一致，黑色阵营的相对坐标是默认棋盘坐标旋转180度后的结果，原点从左下角变成了右上角，x轴从右往左，y轴从上到下
        /// </remarks>
        /// <param name="x">相对横坐标</param>
        /// <param name="y">相对纵坐标</param>
        /// <param name="piece">获取的棋子</param>
        /// <param name="myPiece">棋子的阵营是否属于该操控器</param>
        /// <returns>指定的坐标是否棋盘范围内，在范围内返回true，否则返回false</returns>
        public bool TryGetByPiece(byte x, byte y, out ChiessPiece piece, out bool myPiece)
        {
            piece = default;
            myPiece = false;
            if (x < 0 || y < 0 || x >= ChiessBoard.BoardWidth || y >= ChiessBoard.BoardHeight) return false;

            var team = PlayerTeam;
            if (team == ChiessTeam.Red)
            {
                piece = p_board[x, y];
            }
            else
            {
                piece = p_board[(byte)(8 - x), (byte)(9 - y)];
            }
            myPiece = team == piece.PieceTeam;

            return true;
        }
        /// <summary>
        /// 根据相对索引访问指定位置的棋子信息
        /// </summary>
        /// <remarks>
        /// 相对坐标表示为阵营方位的坐标，红色阵营的相对坐标和棋盘默认坐标一致，黑色阵营的相对坐标是默认棋盘坐标旋转180度后的结果，原点从左下角变成了右上角，x轴从右往左，y轴从上到下
        /// </remarks>
        /// <param name="potion">相对坐标</param>
        /// <param name="piece">获取的棋子</param>
        /// <param name="myPiece">棋子的阵营是否属于该操控器</param>
        /// <returns>指定的坐标是否棋盘范围内，在范围内返回true，否则返回false</returns>
        public bool TryGetByPiece(Chessrdinate potion, out ChiessPiece piece, out bool myPiece)
        {
            piece = default;
            myPiece = false;
            byte x, y;
            potion.GetPosition(out x, out y);

            if (x < 0 || y < 0 || x >= ChiessBoard.BoardWidth || y >= ChiessBoard.BoardHeight) return false;

            var team = PlayerTeam;
            if (team == ChiessTeam.Red)
            {
                piece = p_board[x, y];
            }
            else
            {
                piece = p_board[(byte)(8 - x), (byte)(9 - y)];
            }
            myPiece = team == piece.PieceTeam;

            return true;
        }

        /// <summary>
        /// 选择指定坐标的棋子
        /// </summary>
        /// <param name="potion">相对坐标</param>
        /// <returns>
        /// 是否成功选择
        /// <para>坐标超出范围或棋子不是同一阵营，返回false；否则返回true</para>
        /// </returns>
        public bool SelectivePiece(Chessrdinate potion)
        {
            ChiessPiece cp;
            bool flag;
            bool ismy;
            flag = TryGetByPiece(potion, out cp, out ismy);

            if (flag || (!ismy)) return false;

            p_selectivePiece = potion;

            return true;
        }

        /// <summary>
        /// 选择指定坐标的棋子
        /// </summary>
        /// <param name="x">指定横坐标</param>
        /// <param name="y">指定纵坐标</param>
        /// <returns>
        /// 是否是否成功选择
        /// <para>坐标超出范围或棋子不是同一阵营，返回false；否则返回true</para>
        /// </returns>
        public bool SelectivePiece(byte x, byte y)
        {
            return SelectivePiece(new Chessrdinate(x, y));
        }

        public void PlaceOn()
        {

        }
        #endregion

        #endregion

    }
}
