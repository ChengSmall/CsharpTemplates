using System;
using System.Collections.Generic;
using System.Collections;
using Cheng.Algorithm.HashCodes;

namespace Cheng.GameTemplates.ChineseChess.DataStructrue
{

    #region 枚举

    /// <summary>
    /// 象棋棋子的类型
    /// </summary>
    public enum ChiessPieceType : byte
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        /// <summary>
        /// 将
        /// </summary>
        Generals =  0b0001,
        /// <summary>
        /// 士
        /// </summary>
        Scholars =  0b0010,
        /// <summary>
        /// 象
        /// </summary>
        Minister =  0b0011,
        /// <summary>
        /// 车
        /// </summary>
        Car =       0b0100,
        /// <summary>
        /// 马
        /// </summary>
        Horses =    0b0101,
        /// <summary>
        /// 炮
        /// </summary>
        Artillery = 0b0110,
        /// <summary>
        /// 卒
        /// </summary>
        Soldiers =  0b0111,
    }

    /// <summary>
    /// 象棋的阵营
    /// </summary>
    public enum ChiessTeam : byte
    {
        /// <summary>
        /// 红
        /// </summary>
        Red = 0,

        /// <summary>
        /// 黑
        /// </summary>
        Black = 0x1,
    }

    #endregion

    /// <summary>
    /// 表示一个象棋棋子
    /// </summary>
    [Serializable]
    public unsafe struct ChiessPiece : IEquatable<ChiessPiece>
    {

        #region 构造

        /// <summary>
        /// 使用象棋结构值初始化象棋棋子
        /// </summary>
        /// <param name="value">值</param>
        public ChiessPiece(byte value)
        {
            this.value = value;
        }

        /// <summary>
        /// 初始化象棋结构
        /// </summary>
        /// <param name="type">象棋类型</param>
        /// <param name="team">象棋阵营</param>
        public ChiessPiece(ChiessPieceType type, ChiessTeam team)
        {
            value = (byte)(((byte)type & 0b111) | ((((byte)team) << 3) & 0b1000));
        }

        #endregion

        #region 参数

        #region 常量

        #region 象棋字符

        const string cp_generals = "将";

        const string cp_scholars = "士";

        const string cp_minister = "象";

        const string cp_car = "车";

        const string cp_horses = "马";

        const string cp_artillery = "砲";

        const string cp_soldier = "卒";

        const string cp_redGenerals = "帅";

        const string cp_redScholars = "仕";

        const string cp_redMinister = "相";

        const string cp_redCar = "車";

        const string cp_redHorses = "馬";

        const string cp_redArtillery = "炮";

        const string cp_redSoldier = "兵";

        #endregion

        #endregion

        #region 字段

        /// <summary>
        /// 象棋结构的值
        /// </summary>
        public readonly byte value;

        #endregion

        #endregion

        #region 属性参数

        /// <summary>
        /// 访问或修改棋子类型
        /// </summary>
        public ChiessPieceType PieceType
        {
            get
            {
                return (ChiessPieceType)(value & 0b111);
            }
        }

        /// <summary>
        /// 访问棋子阵营
        /// </summary>
        public ChiessTeam PieceTeam
        {
            get => (ChiessTeam)((value >> 3) & 0x1);
        }

        /// <summary>
        /// 实例是否为空实例
        /// </summary>
        public bool IsEmpty
        {
            get => value == 0;
        }

        #endregion

        #region 功能

        #region 值

        /// <summary>
        /// 获取象棋结构的值
        /// </summary>
        /// <param name="type">象棋类型</param>
        /// <param name="team">象棋阵营</param>
        public void GetPiece(out ChiessPieceType type, out ChiessTeam team)
        {
            type = (ChiessPieceType)(value & 0b111);
            team = (ChiessTeam)((value >> 3) & 0x1);
        }

        #endregion

        #region 运算符

        /// <summary>
        /// 比较棋子是否相等
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static bool operator ==(ChiessPiece c1, ChiessPiece c2)
        {
            return c1.value == c2.value;
        }

        /// <summary>
        /// 比较棋子是否不相等
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static bool operator !=(ChiessPiece c1, ChiessPiece c2)
        {
            return c1.value != c2.value;
        }

        #endregion

        #region 派生

        /// <summary>
        /// 返回棋子默认名称
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if(PieceTeam == ChiessTeam.Black)
            {
                switch (PieceType)
                {
                    case ChiessPieceType.Generals:
                        return cp_generals;
                    case ChiessPieceType.Scholars:
                        return cp_scholars;
                    case ChiessPieceType.Minister:
                        return cp_minister;
                    case ChiessPieceType.Car:
                        return cp_car;
                    case ChiessPieceType.Horses:
                        return cp_horses;
                    case ChiessPieceType.Artillery:
                        return cp_artillery;
                    case ChiessPieceType.Soldiers:
                        return cp_soldier;
                }
            }
            else if(PieceTeam == ChiessTeam.Red)
            {
                switch (PieceType)
                {
                    case ChiessPieceType.Generals:
                        return cp_redGenerals;
                    case ChiessPieceType.Scholars:
                        return cp_redScholars;
                    case ChiessPieceType.Minister:
                        return cp_redMinister;
                    case ChiessPieceType.Car:
                        return cp_redCar;
                    case ChiessPieceType.Horses:
                        return cp_redHorses;
                    case ChiessPieceType.Artillery:
                        return cp_redArtillery;
                    case ChiessPieceType.Soldiers:
                        return cp_redSoldier;
                }
            }
            
            return string.Empty;
        }

        public bool Equals(ChiessPiece other)
        {
            return value == other.value;
        }

        public override bool Equals(object obj)
        {
            if(obj is ChiessPiece c)
            {
                return value == c.value;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return value;
        }

        #endregion

        #endregion

    }

    /// <summary>
    /// 象棋棋盘的坐标
    /// </summary>
    [Serializable]
    public struct Chessrdinate : IEquatable<Chessrdinate>, IComparable<Chessrdinate>
    {

        #region 构造

        /// <summary>
        /// 实例化一个象棋棋盘坐标结构
        /// </summary>
        /// <param name="x">横坐标</param>
        /// <param name="y">纵坐标</param>
        public Chessrdinate(int x, int y)
        {
            position = (byte)((((byte)x) & 0b1111) | ((((byte)y) & 0b1111) << 4));
        }

        /// <summary>
        /// 实例化一个象棋棋盘坐标结构
        /// </summary>
        /// <param name="x">横坐标</param>
        /// <param name="y">纵坐标</param>
        public Chessrdinate(byte x, byte y)
        {
            position = (byte)((x & 0b1111) | ((y & 0b1111) << 4));
        }

        /// <summary>
        /// 实例化象棋棋盘坐标结构
        /// </summary>
        /// <param name="position">坐标结构位置信息参数</param>
        public Chessrdinate(byte position)
        {
            this.position = position;
        }
        
        #endregion

        #region 参数

        /// <summary>
        /// 位置信息参数
        /// </summary>
        public readonly byte position;

        #endregion

        #region 参数访问

        /// <summary>
        /// 访问横坐标
        /// </summary>
        /// <remarks>参数的有效范围在[0,15]</remarks>
        public byte X
        {
            get
            {
                return (byte)(position & 0b1111);
            }
        }

        /// <summary>
        /// 访问纵坐标
        /// </summary>
        /// <remarks>参数的有效范围在[0,15]</remarks>
        public byte Y
        {
            get
            {
                return (byte)(position >> 4);
            }
        }

        /// <summary>
        /// 获取坐标位置信息
        /// </summary>
        /// <param name="x">要获取的坐标</param>
        /// <param name="y">要获取的坐标</param>
        public void GetPosition(out int x, out int y)
        {
            x = (position & 0b1111);
            y = (position >> 4);
        }

        #endregion

        #region 功能

        #region 功能属性

        /// <summary>
        /// 返回将此坐标以棋盘中心对称反转的实例
        /// </summary>
        public Chessrdinate Inversion
        {
            get
            {
                return new Chessrdinate((byte)(8 - (position & 0b1111)), (byte)(9 - (position >> 4)));
            }
        }

        /// <summary>
        /// 判断该坐标是否超出棋盘范围
        /// </summary>
        /// <returns>true表示超出棋盘范围，false表示没有超出范围</returns>
        public bool OutRangeBoard
        {
            get
            {
                int x = (position & 0b1111), y = (position >> 4);
                return (x < 0 || y < 0 || x >= ChiessBoard.BoardWidth || y >= ChiessBoard.BoardHeight);
            }
        }

        /// <summary>
        /// 返回以楚河汉界为轴上下反转位置的坐标
        /// </summary>
        public Chessrdinate FoldingTransition
        {
            get
            {
                return new Chessrdinate((byte)(position & 0b1111), (byte)(9 - (position >> 4)));
            }
        }

        /// <summary>
        /// 返回以“将”所在的y坐标为轴，反转的坐标位置
        /// </summary>
        public Chessrdinate BilateralSymmetry
        {
            get
            {
                return new Chessrdinate((byte)(8 - (position & 0b1111)), (byte)((position >> 4)));
            }
        }

        #endregion

        #region 运算符

        /// <summary>
        /// 比较坐标是否相等
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator ==(Chessrdinate p1, Chessrdinate p2)
        {
            return p1.position == p2.position;
        }

        /// <summary>
        /// 比较坐标是否不相等
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator !=(Chessrdinate p1, Chessrdinate p2)
        {
            return p1.position != p2.position;
        }

        #endregion

        #region 派生

        public bool Equals(Chessrdinate other)
        {
            return position == other.position;
        }

        public override bool Equals(object obj)
        {
            if(obj is Chessrdinate c)
            {
                return position == c.position;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return position;
        }

        /// <summary>
        /// 以字符串的格式返回坐标
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            GetPosition(out int x, out int y);
            return "(" + x.ToString() + "," + y.ToString() + ")";
        }

        /// <summary>
        /// 以指定字符串的格式返回坐标
        /// </summary>
        /// <param name="format">指定格式将<see cref="byte"/>整数对象的值转换为它的等效字符串表示形式</param>
        /// <returns></returns>
        public string ToString(string format)
        {
            GetPosition(out int x, out int y);
            return "(" + x.ToString(format) + "," + y.ToString(format) + ")";
        }

        public int CompareTo(Chessrdinate other)
        {
            return position - other.position;
        }

        #endregion

        #endregion

    }

}
