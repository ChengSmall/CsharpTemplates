using System;
using System.Collections.Generic;
using System.IO;

namespace Cheng.GameTemplates.PushingBoxes.XSB
{

    /// <summary>
    /// xsb关卡转化器
    /// </summary>
    public sealed class XSBlvlReader
    {

        #region 构造

        public XSBlvlReader()
        {
            p_lineStrBuf = new List<string>();
            p_emptyScene = new PushBoxScene();
        }

        #endregion

        #region 参数

        private PushBoxScene p_emptyScene;

        private List<string> p_lineStrBuf;

        #endregion

        #region 功能

        /// <summary>
        /// 判断字符属于xsb内的标准替代符
        /// </summary>
        /// <param name="value">字符</param>
        /// <returns></returns>
        public static bool IsXSBChar(char value)
        {
            switch (value)
            {
                case XSBReader.XSB_box:
                case XSBReader.XSB_boxOnTarget:
                case XSBReader.XSB_ground_1:
                case XSBReader.XSB_ground_2:
                case XSBReader.XSB_ground_3:
                case XSBReader.XSB_player:
                case XSBReader.XSB_playerOnTarget:
                case XSBReader.XSB_target:
                case XSBReader.XSB_wall:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 判断整个字符串的所有字符是否有不属于xsb场景格式的字符
        /// </summary>
        /// <param name="value"></param>
        /// <returns>是否有不属于xsb场景格式的字符；字符串为null或空字符串也返回false</returns>
        public static bool IsNotXSBString(string value)
        {
            if (string.IsNullOrEmpty(value)) return true;
            int length = value.Length;
            for (int i = 0; i < length; i++)
            {
                if (!IsXSBChar(value[i]))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 读取xsb关卡并返回关卡合集
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public PushBoxLevel[] ReaderXSBToPushBoxLevel(TextReader reader)
        {
            var enumator = f_readerXSBToPushBoxLevelEnumator(reader);
            enumator = System.Linq.Enumerable.Where(enumator, f_whereFunc);

            return System.Linq.Enumerable.ToArray(enumator);
        }

        private static bool f_whereFunc(PushBoxLevel t_lvl)
        {
            return (object)t_lvl.scene != null;
        }

        /// <summary>
        /// 读取xsb关卡合集返回关卡的枚举器
        /// </summary>
        /// <param name="reader">xsb关卡读取器</param>
        /// <returns>
        /// <para>每次枚举会返回一个关卡实例</para>
        /// <para>如果一次返回的关卡实例参数为null，则此次不属于完整关卡，只有<see cref="PushBoxLevel.scene"/>不为null时才算一个完整的关卡</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">读取器是null</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="ObjectDisposedException">读取器已释放</exception>
        /// <exception cref="Exception">其它读取器错误</exception>
        public IEnumerable<PushBoxLevel> ReaderXSBToPushBoxLevelEnumator(TextReader reader)
        {
            if (reader is null) throw new ArgumentNullException();

            return f_readerXSBToPushBoxLevelEnumator(reader);
        }

        /// <summary>
        /// 读取xsb关卡合集返回关卡的枚举器
        /// </summary>
        /// <param name="reader">xsb关卡读取器</param>
        /// <returns>
        /// <para>每次枚举会返回一个完整的关卡实例</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">读取器是null</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="ObjectDisposedException">读取器已释放</exception>
        /// <exception cref="Exception">其它读取器错误</exception>
        public IEnumerable<PushBoxLevel> ReaderXSBToPushBoxCompleteLevelEnumator(TextReader reader)
        {
            if (reader is null) throw new ArgumentNullException();
            var enumator = f_readerXSBToPushBoxLevelEnumator(reader);
            enumator = System.Linq.Enumerable.Where(enumator, f_whereFunc);
            return enumator;
        }

        private IEnumerable<PushBoxLevel> f_readerXSBToPushBoxLevelEnumator(TextReader reader)
        {
            //List<PushBoxLevel> p_readList = p_listBuf;

            string title = null, author = null;
            PushBoxScene scene;
            var lineStrBuf = p_lineStrBuf;

            bool? titleIsUp = true;
            string line;
            while (true)
            {
                //WhileHead:
                line = reader.ReadLine();

                if (line is null)
                {
                    break; //读取到尽头
                }

                //CreateHead:

                //此行有数据
                
                //CreateTitle:

                //按line创建title
                if (titleIsUp == true)
                {
                    bool isTitleC = false, isAuC = false;
                    if (line.Length == 0)
                    {
                        yield return default;
                        continue; //空行跳过
                    }

                    if (line.Length < 6)
                    {
                        //全都不是
                        title = string.Empty;
                        author = string.Empty;
                        titleIsUp = false;
                        goto CreateScene;
                    }

                    if (char.ToLower(line[0]).Equals('t') && char.ToLower(line[1]).Equals('i') && char.ToLower(line[2]).Equals('t') && char.ToLower(line[3]).Equals('l') && char.ToLower(line[4]).Equals('e') && line[5].Equals(':'))
                    {
                        //标题
                        title = line.Substring(6).Trim();
                    }
                    else
                    {
                        //不属于标题
                        //跳转到作者判断
                        isTitleC = true;
                        goto CreateAuthor;
                    }


                    //读取下一行
                    line = reader.ReadLine();
                    if (line is null)
                    {
                        break; //读取到尽头
                    }

                    CreateAuthor:
                    //按line创建作者
                    if (line.Length == 0)
                    {
                        yield return default;
                        continue; //空行跳过
                    }

                    if (line.Length < 7)
                    {
                        //不是作者
                        author = string.Empty;
                        goto CreateScene;
                    }

                    if (char.ToLower(line[0]).Equals('a') && char.ToLower(line[1]).Equals('u') && char.ToLower(line[2]).Equals('t') && char.ToLower(line[3]).Equals('h') && char.ToLower(line[4]).Equals('o') && char.ToLower(line[5]).Equals('r') && line[6].Equals(':'))
                    {
                        //作者
                        author = line.Substring(7).Trim();
                    }
                    else
                    {
                        //既不是作者也不是标题
                        //标题不是头部
                        isAuC = true;
                        if(isTitleC && isAuC)
                        {
                            titleIsUp = false;
                        }
                        
                        goto CreateScene;
                    }

                    //读取下一行
                    line = reader.ReadLine();
                    if (line is null)
                    {
                        break; //读取到尽头
                    }

                }

                CreateScene:

                //按line创建关卡
                if (line.Length == 0)
                {
                    //空关卡跳过
                    yield return default;
                    continue;
                }

                //非空行
                int width, height;

                //将有效数据读取完毕
                while (true)
                {
                    lineStrBuf.Add(line);
                    line = reader.ReadLine();
                    if (IsNotXSBString(line))
                    {
                        //空行
                        break;
                    }
                }
                //纵列
                height = lineStrBuf.Count;
                //长度
                width = System.Linq.Enumerable.Max(lineStrBuf, strLineLengthToInt);

                int strLineLengthToInt(string tf_value)
                {
                    return tf_value.Length;
                }

                //实例化场景
                scene = new PushBoxScene(width, height);
                scene.ResetEmpty();
                SceneGrid sceneGrid;
                string sceneLine;
                for (int t_y = 0; t_y < height; t_y++)
                {
                    //读取一行
                    sceneLine = lineStrBuf[t_y];

                    for (int t_x = 0; t_x < width && t_x < sceneLine.Length; t_x++)
                    {
                        if (XSBReader.XSBToGrid(sceneLine[t_x], out sceneGrid))
                        {
                            scene[t_x, (height - 1) - t_y] = sceneGrid;
                        }
                        else
                        {
                            //读取到不属于xsb的格式
                            //跳过
                            scene = null;
                            title = null;
                            author = null;
                            lineStrBuf.Clear();
                            goto NextTitleBegin;
                        }

                    }
                }

                lineStrBuf.Clear();
                NextTitleBegin:
                //到底底层

                if (titleIsUp == false)
                {
                    bool isTitleC = false, isAuC = false;
                    if (string.IsNullOrEmpty(line))
                    {
                        line = reader.ReadLine();
                    }
                    
                    if(line is null)
                    {
                        goto AllOver;
                    }

                    if (line.Length == 0)
                    {
                        goto AllOver;
                    }

                    if (line.Length < 6)
                    {
                        //全都不是
                        goto AllOver;
                    }

                    if (char.ToLower(line[0]).Equals('t') && char.ToLower(line[1]).Equals('i') && char.ToLower(line[2]).Equals('t') && char.ToLower(line[3]).Equals('l') && char.ToLower(line[4]).Equals('e') && line[5].Equals(':'))
                    {
                        //标题
                        title = line.Substring(6).Trim();
                    }
                    else
                    {
                        //不属于标题
                        //跳转到作者判断
                        isTitleC = true;
                        goto CreateAuthor;
                    }


                    //读取下一行
                    line = reader.ReadLine();
                    if (line is null)
                    {
                        goto AllOver; //读取到尽头
                    }

                    CreateAuthor:
                    //按line创建作者
                    if (line.Length == 0)
                    {
                        goto AllOver;
                    }

                    if (line.Length < 7)
                    {
                        //不是作者
                        author = string.Empty;
                        goto AllOver;
                    }

                    if (char.ToLower(line[0]).Equals('a') && char.ToLower(line[1]).Equals('u') && char.ToLower(line[2]).Equals('t') && char.ToLower(line[3]).Equals('h') && char.ToLower(line[4]).Equals('o') && char.ToLower(line[5]).Equals('r') && line[6].Equals(':'))
                    {
                        //作者
                        author = line.Substring(7).Trim();
                    }
                    else
                    {
                        //既不是作者也不是标题
                        //标题不是头部
                        isAuC = true;
                        if(isTitleC && isAuC)
                        {
                            titleIsUp = null;
                        }
                        
                        goto AllOver;
                    }

                }

                AllOver:
                yield return new PushBoxLevel(title, author, scene);
                //p_readList.Add(new PushBoxLevel(title, author, scene));
            }

            lineStrBuf.Clear();
            
        }

        #endregion

    }

}
