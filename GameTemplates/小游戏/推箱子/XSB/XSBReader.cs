using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;

namespace Cheng.GameTemplates.PushingBoxes.XSB
{

    /// <summary>
    /// xsb关卡转化器
    /// </summary>
    public sealed class XSBReader
    {

        #region 构造

        public XSBReader()
        {
            p_readBuffer = new List<string>();
        }

        #endregion

        #region 参数

        #region 常量

        /// <summary>
        /// xsb字符字典 - 玩家
        /// </summary>
        public const char XSB_player = '@';

        /// <summary>
        /// xsb字符字典 - 箱子
        /// </summary>
        public const char XSB_box = '$';

        /// <summary>
        /// xsb字符字典 - 墙
        /// </summary>
        public const char XSB_wall = '#';

        /// <summary>
        /// xsb字符字典 - 地面 1
        /// </summary>
        public const char XSB_ground_1 = '-';

        /// <summary>
        /// xsb字符字典 - 地面 2
        /// </summary>
        public const char XSB_ground_2 = '_';

        /// <summary>
        /// xsb字符字典 - 地面 3
        /// </summary>
        public const char XSB_ground_3 = ' ';

        /// <summary>
        /// xsb字符字典 - 目标点位
        /// </summary>
        public const char XSB_target = '.';

        /// <summary>
        /// xsb字符字典 - 玩家处于目标点位
        /// </summary>
        public const char XSB_playerOnTarget = '+';

        /// <summary>
        /// xsb字符字典 - 箱子处于目标点位
        /// </summary>
        public const char XSB_boxOnTarget = '*';

        #endregion

        List<string> p_readBuffer;

        #endregion

        #region 功能

        /// <summary>
        /// 将一个XSB字符转化为一个场景格式
        /// </summary>
        /// <param name="xsb">xsb字符</param>
        /// <param name="grid">转化后的场景格式</param>
        /// <returns><paramref name="xsb"/>是否成功转化</returns>
        public static bool XSBToGrid(char xsb, out SceneGrid grid)
        {
            switch (xsb)
            {
                case XSB_box:
                    grid = SceneGrid.BoxGrid;
                    break;
                case XSB_boxOnTarget:
                    grid = SceneGrid.TragetBoxGrid;
                    break;
                case XSB_ground_1:
                case XSB_ground_2:
                case XSB_ground_3:
                    grid = SceneGrid.GroundObject;
                    break;
                case XSB_player:
                    grid = SceneGrid.PlayerGrid;
                    break;
                case XSB_playerOnTarget:
                    grid = SceneGrid.TragetPlayerGrid;
                    break;
                case XSB_target:
                    grid = SceneGrid.TragetGrid;
                    break;
                case XSB_wall:
                    grid = SceneGrid.WallGrid;
                    break;
                default:
                    grid = default;
                    return false;
            }

            return true;

        }

        /// <summary>
        /// 通过读取器读取xsb文本并转化到场景实例
        /// </summary>
        /// <param name="reader">要读取的文本</param>
        /// <returns>转化后的场景实例</returns>
        /// <exception cref="ArgumentNullException">读取器为null</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="ArgumentOutOfRangeException">一行文本超出int32位的字符数</exception>
        /// <exception cref="FormatException">读取的文本不是标准xsb格式</exception>
        public PushBoxScene ReaderXSBToScene(TextReader reader)
        {
            if (reader is null) throw new ArgumentNullException(nameof(reader), "文本读取器是null");
            string line;
            while (true)
            {
                line = reader.ReadLine();
                if (string.IsNullOrEmpty(line)) break;
                p_readBuffer.Add(line);
            }
            //获取列数
            int height = p_readBuffer.Count;
            int width;
            if(height == 0)
            {
                throw new FormatException("无有效场景数据");
            }

            //获取其中最大行数
            width = System.Linq.Enumerable.Max<string>(p_readBuffer, lineMaxFunc);

            int lineMaxFunc(string value)
            {
                if (string.IsNullOrEmpty(value)) return 0;
                return value.Length;
            }
            //创建场景
            PushBoxScene pushBoxScene = new PushBoxScene(width, height);
            pushBoxScene.ResetEmpty();

            int x, y;

            for(y = 0; y < height; y++)
            {
                //获取一行字符
                line = p_readBuffer[y];

                for(x = 0; x < width || x < line.Length; x++)
                {
                    char xsbChar = line[x];

                    if(!XSBToGrid(line[x], out var sceneGrid))
                    {
                        throw new FormatException("xsb包含非标准字符");
                    }

                    pushBoxScene[x, y] = sceneGrid;
                }
            }

            p_readBuffer.Clear();

            return pushBoxScene;
        }

        #endregion

    }


}
