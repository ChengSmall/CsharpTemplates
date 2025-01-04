
#region

//using Cheng.DataStructure.Collections;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;

//using Cheng.DataStructure.Colors;
//using Cheng.DataStructure.Cherrsdinates;

//namespace Cheng.Consoles.ConsoleUI
//{

//    [Obsolete("未完成", false)]
//    internal class ConsoleMenuUI2D
//    {

//        #region 构造

//        public ConsoleMenuUI2D(int width, int height)
//        {
//            p_buttons = new RectArray<ConsoleTextUIButton>(width, height);
//            Separator = '\u3000';
//            SeparatorWidth = 10;
//            SeparatorHeight = 1;
//            LeftWhiteSpaceCount = 10;
//        }

//        #endregion

//        #region 参数

//        /// <summary>
//        /// 选择索引
//        /// </summary>
//        private PointInt2 p_selectIndex;

//        /// <summary>
//        /// 二维按钮集合
//        /// </summary>
//        private RectArray<ConsoleTextUIButton> p_buttons;

//        /// <summary>
//        /// 左侧前置空白数
//        /// </summary>
//        private int p_leftWhiteSpace;

//        /// <summary>
//        /// 选择箭头
//        /// </summary>
//        private string p_selectValue;

//        private Colour p_selectValueColor;

//        /// <summary>
//        /// 左右空行对齐字符数
//        /// </summary>
//        private int p_separatorWidth;

//        /// <summary>
//        /// 上下隔行数
//        /// </summary>
//        private int p_separatorHeight;

//        /// <summary>
//        /// 空行符号
//        /// </summary>
//        private char p_sep;

//        private bool p_beginSelectValueColor;

//        #endregion

//        #region 功能

//        #region 参数访问

//        /// <summary>
//        /// 按钮集合
//        /// </summary>
//        public RectArray<ConsoleTextUIButton> Buttons
//        {
//            get => p_buttons;
//        }

//        /// <summary>
//        /// x轴的空行字符，该值默认为全角空格 \u3000
//        /// </summary>
//        public char Separator
//        {
//            get => p_sep;
//            set => p_sep = value;
//        }

//        /// <summary>
//        /// 每个文本间的对齐数量
//        /// </summary>
//        /// <value>
//        /// <para>
//        /// 该参数决定了x轴上文本与文本间的最大空行数量，使用对齐的方式输出文本
//        /// </para>
//        /// <para>
//        /// text1 与 text2 间的空行数最终结果为:
//        /// <code>
//        /// 该参数 - [text1]字符数
//        /// </code>
//        /// 若text2为当前选中项，则空行数会减去选中项要打印的字符数<br/>
//        /// 若结果小于0则 text2 的输出会覆盖 text1
//        /// </para>
//        /// <para>该参数默认为10</para>
//        /// </value>
//        /// <exception cref="ArgumentOutOfRangeException">参数小于0</exception>
//        public int SeparatorWidth
//        {
//            get => p_separatorWidth;
//            set
//            {
//                if (value < 0) throw new ArgumentOutOfRangeException();
//                p_separatorWidth = value;
//            }
//        }

//        /// <summary>
//        /// 文本纵列的空行数
//        /// </summary>
//        /// <value>
//        /// 该值表示每一列文本之间的空行数，该参数默认为1
//        /// </value>
//        /// <exception cref="ArgumentOutOfRangeException">参数小于0</exception>
//        public int SeparatorHeight
//        {
//            get => p_separatorHeight;
//            set
//            {
//                if (value < 0) throw new ArgumentOutOfRangeException();
//                p_separatorHeight = value;
//            }
//        }

//        /// <summary>
//        /// 在打印按钮矩阵前，每行要输出的空行
//        /// </summary>
//        public int LeftWhiteSpaceCount
//        {
//            get => p_leftWhiteSpace;
//            set
//            {
//                if (value < 0) throw new ArgumentOutOfRangeException();
//                p_leftWhiteSpace = value;
//            }
//        }

//        public const string DefaultLeftWhiteSpace = "      ";

//        /// <summary>
//        /// 选择器文本颜色
//        /// </summary>
//        /// <value>
//        /// <para>使用<see cref="ConsoleTextStyle"/>打印带颜色的文本，该参数忽略透明通道</para>
//        /// <para>该参数仅在<see cref="BeginSelectValueColor"/>参数为true生效</para>
//        /// </value>
//        public Colour SelectValueColor
//        {
//            get => p_selectValueColor;
//            set
//            {
//                p_selectValueColor = value;
//            }
//        }

//        /// <summary>
//        /// 是否开启选择器颜色
//        /// </summary>
//        public bool BeginSelectValueColor
//        {
//            get => p_beginSelectValueColor;
//            set
//            {
//                p_beginSelectValueColor = value;
//            }
//        }

//        /// <summary>
//        /// 要绘制的选择指示器
//        /// </summary>
//        /// <value>
//        /// <para>该参数为选择器绘制文本，打印在选择的文本左侧</para>
//        /// <para>该值默认为<see cref="DefaultSelectValue"/></para>
//        /// </value>
//        public string SelectValue
//        {
//            get => p_selectValue;
//            set
//            {
//                p_selectValue = value ?? string.Empty;
//            }
//        }

//        /// <summary>
//        /// 默认选择器文本
//        /// </summary>
//        public const string DefaultSelectValue = "=>";

//        /// <summary>
//        /// 当前选择的按钮索引
//        /// </summary>
//        /// <value>
//        /// <para>用于绘制左侧选择器，如果范围不在数组内，则不绘制选择器</para>
//        /// </value>
//        public PointInt2 SelectIndex
//        {
//            get => p_selectIndex;
//            set
//            {
//                p_selectIndex = value;
//            }
//        }

//        #endregion

//        #region 选择移动

//        /// <summary>
//        /// 移动选择器
//        /// </summary>
//        /// <param name="moveX">在x轴移动的坐标</param>
//        /// <param name="moveY">在y轴移动的坐标</param>
//        public void MoveSelect(int moveX, int moveY)
//        {
//            int x = p_selectIndex.x + moveX;
//            int y = p_selectIndex.y + moveY;
//            p_selectIndex = new PointInt2(x, y);
//        }

//        #endregion

//        #endregion

//    }

//}

#endregion
