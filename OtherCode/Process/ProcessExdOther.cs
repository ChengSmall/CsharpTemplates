using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.Windows.Processes
{

    public static class ProcessExdOther
    {

        /// <summary>
        /// 打开指定进程并重定向输出流异步读取输出
        /// </summary>
        /// <param name="startInfo">进程启动参数</param>
        /// <param name="OutputDataCallback">进程启动后的异步标准输出回调</param>
        /// <param name="errorOutputCallback">进程启动后的异步标准错误输出回调</param>
        /// <returns>启动的新的进程资源</returns>
        /// <exception cref="Exception">错误</exception>
        public static Process StartProcessReadOutput(ProcessStartInfo startInfo, DataReceivedEventHandler OutputDataCallback, DataReceivedEventHandler errorOutputCallback)
        {
            if (startInfo is null) throw new ArgumentNullException();

            startInfo.RedirectStandardOutput = true; // 重定向标准输出，以便读取命令的输出
            startInfo.RedirectStandardError = true;  // 重定向标准错误，以便读取错误输出
            startInfo.UseShellExecute = false;     // 设置为 false 以允许重定向
            startInfo.CreateNoWindow = true;         // 不创建窗口

            Process process = null;
            try
            {
                process = new Process();
                process.StartInfo = startInfo;
                process.OutputDataReceived += OutputDataCallback;
                process.ErrorDataReceived += errorOutputCallback;

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }
            catch (Exception)
            {
                process?.Close();
                throw;
            }

            return process;

        }

        /// <summary>
        /// 打开指定进程并重定向输出流异步读取输出
        /// </summary>
        /// <param name="fileName">要启动的文件路径</param>
        /// <param name="args">命令参数</param>
        /// <param name="OutputDataCallback">进程启动后的异步标准输出回调，null表示没有</param>
        /// <param name="errorOutputCallback">进程启动后的异步标准错误输出回调，null表示没有</param>
        /// <returns>启动的新的进程资源</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="Exception">错误</exception>
        public static Process StartProcessReadOutput(string fileName, string args, DataReceivedEventHandler OutputDataCallback, DataReceivedEventHandler errorOutputCallback)
        {
            if (fileName is null) throw new ArgumentNullException();

            ProcessStartInfo startInfo = new ProcessStartInfo(fileName, args ?? string.Empty)
            {
                RedirectStandardOutput = true, // 重定向标准输出，以便读取命令的输出
                RedirectStandardError = true,  // 重定向标准错误，以便读取错误输出
                UseShellExecute = false,       // 设置为 false 以允许重定向
                CreateNoWindow = true,          // 不创建窗口
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            Process process = null;

            try
            {
                process = new Process();
                process.StartInfo = startInfo;
                process.OutputDataReceived += OutputDataCallback;
                process.ErrorDataReceived += errorOutputCallback;

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                return process;
            }
            catch (Exception)
            {
                process?.Close();
                throw;
            }
        }


    }
}
