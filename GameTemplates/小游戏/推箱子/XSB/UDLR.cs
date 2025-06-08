using Cheng.Memorys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.GameTemplates.PushingBoxes.XSB
{

    /// <summary>
    /// 推箱子游戏操作记录列表
    /// </summary>
    public sealed class PushBoxUDLR
    {

        #region 构造

        public PushBoxUDLR()
        {
            p_moveList = new List<MoveType>();
        }

        public PushBoxUDLR(int capacity)
        {
            p_moveList = new List<MoveType>(capacity);
        }

        public PushBoxUDLR(IEnumerable<MoveType> enumator)
        {
            p_moveList = new List<MoveType>(enumator);
        }

        #endregion

        #region 参数

        #region 常量

        public const char LeftChar = 'l';

        public const char RightChar = 'r';

        public const char UpChar = 'u';

        public const char DownChar = 'd';

        #endregion

        private List<MoveType> p_moveList;

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 用于存储移动轨迹的列表
        /// </summary>
        public List<MoveType> UDLRList
        {
            get => p_moveList;
        }

        #endregion

        #region 集合

        /// <summary>
        /// 清空UDLR列表
        /// </summary>
        public void Clear()
        {
            p_moveList.Clear();
        }

        /// <summary>
        /// 添加一个移动方位
        /// </summary>
        /// <param name="moveType"></param>
        public void Add(MoveType moveType)
        {
            p_moveList.Add(moveType);
        }

        public MoveType this[int index]
        {
            get => p_moveList[index];
            set => p_moveList[index] = value;
        }

        public int Count
        {
            get => p_moveList.Count;
        }

        #endregion

        #region 转化

        /// <summary>
        /// 使用读取器读取标准UDLR操作列表到集合
        /// </summary>
        /// <param name="reader">读取UDLR文本的读取器</param>
        /// <exception cref="ArgumentNullException">读取器是null</exception>
        public void AddMoveListByUDLR(TextReader reader)
        {
            if (reader is null) throw new ArgumentNullException();

            while (true)
            {
                var re = reader.Read();

                if (re < 0) return;

                var c = ((char)re).ToLower();
                switch (c)
                {
                    case LeftChar:
                        p_moveList.Add(MoveType.Left);
                        break;
                    case RightChar:
                        p_moveList.Add(MoveType.Right);
                        break;
                    case UpChar:
                        p_moveList.Add(MoveType.Up);
                        break;
                    case DownChar:
                        p_moveList.Add(MoveType.Down);
                        break;
                    default:
                        break;
                }

            }

        }

        /// <summary>
        /// 将当前对象内的操作记录按照UDLR规则写入到文本
        /// </summary>
        /// <param name="writer">文本写入器</param>
        /// <param name="upper">让写入的文本都是大写</param>
        public void WriterToUDLR(TextWriter writer, bool upper)
        {
            if (writer is null) throw new ArgumentNullException();

            int length = p_moveList.Count;

            for (int i = 0; i < length; i++)
            {
                var moveType = p_moveList[i];

                switch (moveType)
                {
                    case MoveType.Left:
                        writer.Write(upper ? LeftChar.ToUpper() : LeftChar);
                        break;
                    case MoveType.Right:
                        writer.Write(upper ? RightChar.ToUpper() : RightChar);
                        break;
                    case MoveType.Up:
                        writer.Write(upper ? UpChar.ToUpper() : UpChar);
                        break;
                    case MoveType.Down:
                        writer.Write(upper ? DownChar.ToUpper() : DownChar);
                        break;
                }

            }

        }

        /// <summary>
        /// 将当前对象内的操作记录按照UDLR规则添加到字符串缓冲区
        /// </summary>
        /// <param name="append">字符串缓冲区</param>
        /// <param name="upper">让写入的文本都是大写</param>
        public void WriterToUDLR(StringBuilder append, bool upper)
        {
            if (append is null) throw new ArgumentNullException();

            int length = p_moveList.Count;

            for (int i = 0; i < length; i++)
            {
                var moveType = p_moveList[i];

                switch (moveType)
                {
                    case MoveType.Left:
                        append.Append(upper ? LeftChar.ToUpper() : LeftChar);
                        break;
                    case MoveType.Right:
                        append.Append(upper ? RightChar.ToUpper() : RightChar);
                        break;
                    case MoveType.Up:
                        append.Append(upper ? UpChar.ToUpper() : UpChar);
                        break;
                    case MoveType.Down:
                        append.Append(upper ? DownChar.ToUpper() : DownChar);
                        break;
                }

            }
        }

        #endregion

        #region 操作

        /// <summary>
        /// 将当前的操作记录提交到指定游戏控制台的枚举器
        /// </summary>
        /// <param name="game">要提交到的游戏控制台</param>
        public void ListOperationGame(PushBoxGame game)
        {
            if (game?.scene is null) throw new ArgumentNullException(nameof(game), "游戏控制台是null");
            int length = p_moveList.Count;
            for (int i = 0; i < length; i++)
            {
                game.MoveTo(p_moveList[i]);
            }
        }

        /// <summary>
        /// 返回一个将当前的操作记录提交到指定游戏控制台的枚举器
        /// </summary>
        /// <param name="game">要提交到的游戏控制台</param>
        /// <returns>每次推进都是一次动作提交</returns>
        public IEnumerable<MoveType> ListOperationGameEnumator(PushBoxGame game)
        {
            if (game?.scene is null) throw new ArgumentNullException();

            return f_listOperationGame(game, p_moveList.GetEnumerator());
        }

        private IEnumerable<MoveType> f_listOperationGame(PushBoxGame game, List<MoveType>.Enumerator enumerator)
        {
            
            while (enumerator.MoveNext())
            {
                var move = enumerator.Current;
                game.MoveTo(move);
                yield return move;
            }

        }

        #endregion

        #endregion

    }

}
