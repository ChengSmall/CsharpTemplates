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
        /// 初始化参数
        /// </summary>
        /// <param name="args">命令行参数</param>
        public InitArgs(string[] args)
        {
            commandArgs = args;

            currentDomain = AppDomain.CurrentDomain;
            
            streamParser = new StreamParserDefault();

            applicationName = currentDomain.FriendlyName;

            isX64 = sizeof(void*) == 8;

            appConfigName = applicationName + ".config";

            rootDirectory = currentDomain.BaseDirectory;

            debugLogPrint = null;

            setUpFilePath = Path.Combine(rootDirectory, "state.sav");

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

        #endregion

        #region 参数

        /// <summary>
        /// 命令行参数
        /// </summary>
        public readonly string[] commandArgs;

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
        /// 流存储器
        /// </summary>
        public StreamParserDefault streamParser;

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

        /// <summary>
        /// 将对象保存到流
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="stream">保存到的流</param>
        /// <param name="exception">异常错误</param>
        /// <returns>是否成功</returns>
        public bool SaveToStream(object obj, Stream stream, out Exception exception)
        {
            try
            {
                streamParser.ConverToStream(obj, stream);
                exception = null;
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
          
        }

        /// <summary>
        /// 读取流数据到对象
        /// </summary>
        /// <param name="stream">读取的流</param>
        /// <param name="obj">从中读取的对象</param>
        /// <param name="exception">错误异常</param>
        /// <returns>是否成功</returns>
        public bool LoadToStream(Stream stream, out object obj, out Exception exception)
        {
            try
            {
                obj = streamParser.ConverToObject(stream);
                exception = null;
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                obj = null;
                return false;
            }        

        }

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
