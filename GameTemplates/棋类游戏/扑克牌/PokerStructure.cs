using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Cheng.GameTemplates.Pokers
{

    #region 扑克结构

    #region 类型

    /// <summary>
    /// 扑克牌牌面数值
    /// </summary>
    public enum PokerNum : byte
    {
        /// <summary>
        /// 空值
        /// </summary>
        None = 0,
        A = 1,
        _2,
        _3,
        _4,
        _5,
        _6,
        _7,
        _8,
        _9,
        _10,
        J,
        Q,
        K,
        /// <summary>
        /// 小王（灰色Joker）
        /// </summary>
        LittleJoker,
        /// <summary>
        /// 大王（彩色Joker）
        /// </summary>
        Joker,
    }

    /// <summary>
    /// 扑克牌花色类型，若牌面值是鬼王则花色无效
    /// </summary>
    public enum PokerFlower : byte
    {
        /// <summary>
        /// 无花色
        /// </summary>
        None = 0,
        /// <summary>
        /// 红心♥
        /// </summary>
        Hearts = 1,
        /// <summary>
        /// 黑桃♠
        /// </summary>
        Spades = 2,
        /// <summary>
        /// 方片♦
        /// </summary>
        Square_Slices = 3,
        /// <summary>
        /// 梅花♣
        /// </summary>
        Plum_Blossoms = 4,
    }

    #endregion

    /// <summary>
    /// 一个扑克牌结构
    /// </summary>
    public struct Poker : IEquatable<Poker>, IComparable<Poker>
    {

        #region 构造

        /// <summary>
        /// 使用扑克牌类型id初始化扑克牌结构
        /// </summary>
        /// <param name="id"></param>
        public Poker(byte id)
        {
            this.id = id;
        }

        /// <summary>
        /// 初始化扑克牌结构
        /// </summary>
        /// <param name="num">扑克牌面值</param>
        /// <param name="flower">扑克牌花色</param>
        public Poker(PokerNum num, PokerFlower flower)
        {
            id = (byte)(((byte)((((byte)num) & 0xF) << 3)) | (((byte)(((byte)flower) & 0x7))));
        }

        #endregion

        #region 参数

        /// <summary>
        /// 扑克牌类型id
        /// </summary>
        /// <remarks>
        /// <para>一个扑克牌类型组合值；右侧3bit表示花色，中间4bit表示牌面值，最后1bit保留</para>
        /// </remarks>
        public readonly byte id;

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 访问扑克牌面值
        /// </summary>
        public PokerNum Num
        {
            //中4bit => [1,15]
            get
            {
                return (PokerNum)((id >> 3) & 0xF);
            }
        }

        /// <summary>
        /// 访问扑克牌花色
        /// </summary>
        public PokerFlower Flower
        {
            // 右侧3bit => [1,4]
            get
            {
                return (PokerFlower)((id) & 0b111);
            }
        }

        /// <summary>
        /// 获取扑克牌牌面值
        /// </summary>
        /// <param name="num">扑克牌牌面值</param>
        /// <param name="flower">扑克牌花色</param>
        public void GetValue(out PokerNum num, out PokerFlower flower)
        {
            flower = (PokerFlower)(id & 0x7);
            num = (PokerNum)((id >> 3) & 0xF);
        }

        /// <summary>
        /// 空扑克结构
        /// </summary>
        /// <remarks>当id为0时可表示一个空的扑克结构</remarks>
        public static Poker EmptyPoker
        {
            get => default;
        }

        /// <summary>
        /// 当前扑克是否为一个空实例（id == 0）
        /// </summary>
        public bool IsEmpty
        {
            get => id == 0;
        }

        /// <summary>
        /// 获取一张表示小王（灰色Joker）的扑克
        /// </summary>
        public static Poker LittleJoker
        {
            get => new Poker(PokerNum.LittleJoker, 0);
        }

        /// <summary>
        /// 获取一张表示大王（彩色Joker）的扑克
        /// </summary>
        public static Poker Joker
        {
            get => new Poker(PokerNum.Joker, 0);
        }

        /// <summary>
        /// 返回重设扑克牌面值的新扑克对象
        /// </summary>
        /// <param name="newNum">新的牌面值</param>
        /// <returns>新设置的扑克牌</returns>
        public Poker SetNum(PokerNum newNum)
        {
            return new Poker((byte)((id & 0b1_0000_111) | (((byte)newNum) << 3)));
        }

        /// <summary>
        /// 返回重设扑克牌花色的新扑克对象
        /// </summary>
        /// <param name="flower">新的牌花色</param>
        /// <returns>新设置的扑克牌</returns>
        public Poker SetFlower(PokerFlower flower)
        {
            return new Poker((byte)((id & 0b1_1111_000) | ((byte)flower)));
        }

        #endregion

        #region 比较

        /// <summary>
        /// 比较相等
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator ==(Poker p1, Poker p2)
        {
            return p1.id == p2.id;
        }

        /// <summary>
        /// 比较不相等
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator !=(Poker p1, Poker p2)
        {
            return p1.id != p2.id;
        }

        public static bool operator <(Poker p1, Poker p2)
        {
            return p1.id < p2.id;
        }
        public static bool operator >(Poker p1, Poker p2)
        {
            return p1.id > p2.id;
        }
        public static bool operator <=(Poker p1, Poker p2)
        {
            return p1.id <= p2.id;
        }
        public static bool operator >=(Poker p1, Poker p2)
        {
            return p1.id >= p2.id;
        }

        #endregion

        #region 派生

        public override bool Equals(object obj)
        {
            if(obj is Poker p)
            {
                return id == p.id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return id;
        }

        /// <summary>
        /// 以字符串的形式返回当前扑克牌面
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string flower, num;

            GetValue(out var n, out var f);

            if (n >= PokerNum._2 && n <= PokerNum._10)
            {
                num = ((byte)n).ToString();
            }
            else
            {
                switch (n)
                {
                    case PokerNum.A:
                        num = "A";
                        break;
                    case PokerNum.J:
                        num = "J";
                        break;
                    case PokerNum.Q:
                        num = "Q";
                        break;
                    case PokerNum.K:
                        num = "K";
                        break;
                    case PokerNum.LittleJoker:
                        return "小王";
                    case PokerNum.Joker:
                        return "大王";
                    default:
                        return id.ToString();
                }
            }

            switch (f)
            {
                case PokerFlower.Hearts:
                    flower = "♥";
                    break;
                case PokerFlower.Spades:
                    flower = "♠";
                    break;
                case PokerFlower.Square_Slices:
                    flower = "♦";
                    break;
                case PokerFlower.Plum_Blossoms:
                    flower = "♣";
                    break;
                case PokerFlower.None:
                    flower = string.Empty;
                    break;
                default:
                    return id.ToString();
            }

            return flower + num;
        }

        /// <summary>
        /// 判断该值是否等于参数
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Poker other)
        {
            return id == other.id;
        }

        /// <summary>
        /// 与另一个实例比较大小，默认使用id作为比较源
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Poker other)
        {
            return id - other.id;
        }

        #endregion

        #endregion

    }

    #endregion

}
