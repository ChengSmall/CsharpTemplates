using System;
using Cheng.DataStructure.Collections;
using Cheng.Algorithm.Pathfindings;
using System.Text;
using System.Collections.Generic;
using Cheng.DataStructure.Cherrsdinates;

namespace Cheng.DEBUG
{

    /// <summary>
    /// 测试地图数据
    /// </summary>
    public class TestGridObjects : IGridObjects, ITwoDimensionalArray<bool>
    {

        #region ctor

        public TestGridObjects(bool[,] array)
        {
            this.array = array;
            width = array.GetLength(0);
            height = array.GetLength(1);
        }

        public TestGridObjects(int width, int height)
        {
            array = new bool[width, height];
            this.width = width;
            this.height = height;
        }

        #endregion

        #region par

        private readonly bool[,] array;
        private readonly int width;
        private readonly int height;

        #endregion

        #region 派生

        public bool this[int x, int y]
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

        const char On = '□';
        const char Off = '■';

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
                    char c = array[x, y] ? On : Off;
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
        /// <param name="sb"></param>
        /// <param name="On">可用路径点字符</param>
        /// <param name="Off">不可行走路径点字符</param>
        /// <param name="pathChar">行进路径字符</param>
        /// <returns></returns>
        public string GetMapString(List<PointInt2> mapPaths, StringBuilder sb, char On, char Off, char pathChar)
        {

            //StringBuilder sb = new StringBuilder(array.Length + (height * 2));
            sb.Clear();

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
                        c = array[x, y] ? On : Off;
                    }

                    sb.Append(c);
                }
                sb.AppendLine();
            }

            return sb.ToString();

        }

        public int GetGridPrice(int x, int y)
        {
            if (x < 0 || y < 0 || x >= width || y >= height) return int.MinValue;
            return array[x, y] ? 0 : -1;
        }

        #endregion

    }
}
