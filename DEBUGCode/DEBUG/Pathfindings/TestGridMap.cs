using System;
using System.Text;
using System.Collections.Generic;

using Cheng.DataStructure.Collections;
using Cheng.Algorithm.Pathfindings;
using Cheng.DataStructure.Cherrsdinates;
using Cheng.Algorithm.Pathfindings.GridPathfindings;

namespace Cheng.DEBUG
{

    /// <summary>
    /// 测试用网格地图
    /// </summary>
    public class TestGridMap : IGridMap, ITwoDimensionalArray<int>
    {

        #region ctor

        public TestGridMap(int[,] array)
        {
            this.array = array;
            width = array.GetLength(0);
            height = array.GetLength(1);
            p_canDiagonally = true;
        }

        public TestGridMap(int width, int height)
        {
            p_canDiagonally = true;
            array = new int[width, height];
            this.width = width;
            this.height = height;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    array[x, y] = 0;
                }
            }
        }

        #endregion

        #region par

        private readonly int[,] array;
        private readonly int width;
        private readonly int height;
        private bool p_canDiagonally;

        #endregion

        #region 派生

        public bool CanDiagonally
        {
            get => p_canDiagonally;
            set => p_canDiagonally = value;
        }

        /// <summary>
        /// 设置指定索引下的地图格子是可用路径还是不可用路径
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>true表示可用路径，false表示不可用路径</returns>
        public int this[int x, int y]
        {
            get => array[x, y];
            set => array[x, y] = value;
        }

        public int Count => array.Length;

        public int Width => width;

        public int Height => height;

        public int MaxPrice => 0;

        #endregion

        #region debug

        public const char On = '□';
        public const char Off = '■';
        public const char Path = '★';

        /// <summary>
        /// 返回当前地图数据
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetMapString(On, Off);
        }

        /// <summary>
        /// 返回当前地图数据
        /// </summary>
        /// <param name="On">可用路径点字符</param>
        /// <param name="Off">不可行走路径点字符</param>
        /// <returns></returns>
        public string GetMapString(char On, char Off)
        {

            StringBuilder sb = new StringBuilder(array.Length + (height * 2));

            sb.Clear();

            int x, y;

            for (y = 0; y < height; y++)
            {
                for (x = 0; x < width; x++)
                {
                    char c = (array[x, y] < 0) ? Off : On;
                    sb.Append(c);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// 返回当前地图数据
        /// </summary>
        /// <param name="mapPaths">行进路径</param>
        /// <param name="sb">要添加到的缓冲区</param>
        /// <param name="On">可用路径点字符</param>
        /// <param name="Off">不可行走路径点字符</param>
        /// <param name="pathChar">行进路径字符</param>
        public void GetMapStringByPath(IList<PointInt2> mapPaths, StringBuilder sb, char On, char Off, char pathChar)
        {
            //sb.Clear();

            int x, y;

            for (y = 0; y < height; y++)
            {
                for (x = 0; x < width; x++)
                {
                    char c;
                    var b = mapPaths.Contains(new PointInt2(x, y));
                    //int ix = mapPaths.IndexOf(new PointInt2(x, y));
                    if(b)
                    {
                        c = pathChar;
                    }
                    else
                    {
                        c = (array[x, y] < 0) ? Off : On;
                    }

                    sb.Append(c);
                }
                sb.AppendLine();
            }

            //return sb.ToString();
        }

        public void GetMapStringByPath(IList<PointInt2> mapPaths, StringBuilder append)
        {
            GetMapStringByPath(mapPaths, append, On, Off, Path);
        }

        public string GetMapStringByPath(IList<PointInt2> mapPaths)
        {
            StringBuilder sb = new StringBuilder(32);
            GetMapStringByPath(mapPaths, sb, On, Off, Path);
            return sb.ToString();
        }

        public int GetGridPrice(int x, int y)
        {
            if (x < 0 || y < 0 || x >= width || y >= height) return -1;
            return array[x, y];
        }

        #endregion

    }
}
