using Cheng.Streams.Parsers;
using Cheng.Streams.Parsers.Default;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Cheng
{

    /// <summary>
    /// C#程序基本参数
    /// </summary>
    public unsafe class InitArgs
    {

        #region

        /// <summary>
        /// 参数
        /// </summary>
        public static InitArgs Args
        {
            get => p_arg;
        }

        static InitArgs p_arg;

        public static void Init()
        {
            p_arg = new InitArgs(Array.Empty<string>());
        }

        public static void Init(string[] args)
        {
            p_arg = new InitArgs(args);
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="args">命令行参数</param>
        private InitArgs(string[] args)
        {
            commandArgs = args;

            currentDomain = AppDomain.CurrentDomain;

            applicationName = currentDomain.FriendlyName;

            isX64 = sizeof(void*) == 8;

            appConfigName = applicationName + ".config";

            rootDirectory = currentDomain.BaseDirectory;

            debugLogPrint = null;

            setUpFilePath = Path.Combine(rootDirectory, "state.sav");

            commandline = f_getLine();

            f_init();
        }

        private void f_init()
        {
            appConfigXml = new XmlDocument();

            try
            {
                using (StreamReader sr = new StreamReader(Path.Combine(rootDirectory, appConfigName), Encoding.UTF8, true, 1024 * 2))
                {
                    appConfigXml.Load(sr);
                }
            }
            catch (Exception)
            {
            }

        }

        private static string f_getLine()
        {
            var line = System.Environment.CommandLine;
            var index = f_FindChar(line, 0, char.IsWhiteSpace);
            if (index == -1) return string.Empty; //没有空格表示无额外命令参数

            index = f_FindChar(line, index, isNotWhite);
            //获取空白后第一个非空白索引
            if (index == -1) return string.Empty;

            return line.Substring(index);

            bool isNotWhite(char tc)
            {
                return !char.IsWhiteSpace(tc);
            }
        }

        static int f_FindChar(string value, int index, Predicate<char> predicate)
        {
            int length = value.Length;
            for (int i = index; i < length; i++)
            {
                if (predicate.Invoke(value[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        #endregion

        #region 参数

        public readonly string[] commandArgs;

        /// <summary>
        /// 原始命令行参数
        /// </summary>
        public readonly string commandline;

        /// <summary>
        /// 当前应用程序域
        /// </summary>
        public AppDomain currentDomain;

        /// <summary>
        /// 程序根目录
        /// </summary>
        public readonly string rootDirectory;

        /// <summary>
        /// 该程序文件名称
        /// </summary>
        public readonly string applicationName;

        /// <summary>
        /// 该程序公共配置文件名称
        /// </summary>
        public readonly string appConfigName;

        /// <summary>
        /// 该程序的配置文件xml文档
        /// </summary>
        public XmlDocument appConfigXml;

        /// <summary>
        /// DEBUG日志打印时调用对象
        /// </summary>
        public TextWriter debugLogPrint;

        /// <summary>
        /// 程序状态快照保存文件完整路径
        /// </summary>
        public readonly string setUpFilePath;

        /// <summary>
        /// 是否为64位环境
        /// </summary>
        public readonly bool isX64;

        #endregion

        #region 功能

        #region

        #endregion

        #region

        /// <summary>
        /// 打印一行日志
        /// </summary>
        /// <param name="message"></param>
        public void DebugPrintLine(string message)
        {
            debugLogPrint?.WriteLine(message);
        }

        /// <summary>
        /// 打印一段日志信息
        /// </summary>
        /// <param name="message"></param>
        public void DebugPrint(string message)
        {
            debugLogPrint?.Write(message);
        }

        #endregion

        #endregion

    }

}
