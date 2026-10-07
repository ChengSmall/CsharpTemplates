using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Diagnostics;

using Cheng.DataStructure;
using Cheng.Algorithm;
using Cheng.Memorys;
using Cheng.DataStructure.Windows;

using EnvDict = System.Collections.Generic.IReadOnlyDictionary<string, string>;
using EnvDictCrFunc = Cheng.DataStructure.CreateDictionaryByPairs<string, string>;
using ProWinAPI = Cheng.Windows.Processes.ProcessAPI;

namespace Cheng.Windows.EnvironmentVariables
{


    public static unsafe class EnvVariableAPI
    {

        #region winapi

#if DEBUG
        /// <summary>
        /// 检索当前进程的环境变量
        /// </summary>
        /// <remarks>
        /// <para>函数返回指向内存块的指针，该内存块包含调用进程的环境变量 (系统和用户环境变量)</para>
        /// <para>
        /// 每个环境块包含以下格式的环境变量<br/>
        /// <code>
        /// name1=value1\0
        /// name2=value2\0
        /// name3=value3\0
        /// ...
        /// nameN=valueN\0\0
        /// </code>
        /// 每个name=value字符串都使用一个\0作为结尾，而在整个环境快字符串末尾，还有一个\0；<br/>
        /// 环境变量的名称不能包含等号
        /// </para>
        /// </remarks>
        /// <returns>
        /// <para>如果函数成功，则返回值是指向当前进程的环境块的指针；失败则为null</para>
        /// <para>不再使用后，需要调用<see cref="fc_FreeEnvironmentStrings(char*)"/>释放内存</para>
        /// </returns>
#endif
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetEnvironmentStrings")]
        internal unsafe static extern char* api_GetEnvironmentStrings();

#if DEBUG
        /// <summary>
        /// 释放环境字符串块
        /// </summary>
        /// <param name="pStrings">指向环境字符串块的指针</param>
        /// <returns>如果函数成功，则返回值为非零，失败返回0</returns>
#endif
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "FreeEnvironmentStrings")]
        internal unsafe static extern uint api_FreeEnvironmentStrings(char* pStrings);

        #endregion

        #region 封装

        static KeyValuePair<string, string>? f_createEnvPair(CPtr<char> str, int index, int count)
        {
            char* cp = str;

            int eqi;
            int end = index + count;
            for (eqi = index; eqi < end; eqi++)
            {
                if (cp[eqi] == '=')
                {
                    goto checkEQ;
                }
            }
            //未检测到等号
            return null;

            checkEQ:

            string key = new string(cp, index, (eqi - index));

            string val = new string(cp, eqi + 1, ((end - 1) - (eqi)));

            return new KeyValuePair<string, string>(key, val);
        }

        internal static IEnumerable<KeyValuePair<string, string>> f_getEnvs(CPtr<char> envstrptr)
        {
            int startIdnex = 0;
            int strIndex = 0;

            int envLen;

            Loop:
            //计数
            envLen = 0;
            while (envstrptr[strIndex] != '\0')
            {
                strIndex++;
                envLen++;
            }

            //查找到\0
            //获取环境变量块并转换
            var pair = f_createEnvPair(envstrptr, startIdnex, envLen);
            if (pair.HasValue)
            {
                if (!string.IsNullOrEmpty(pair.Value.Key)) yield return pair.Value;
            }
            else
            {
                throw new NotImplementedException();
            }

            //推进索引
            strIndex++;

            if (envstrptr[strIndex] == '\0')
            {
                //结尾
                yield break;
            }

            //未达到结尾
            //推进前索引
            startIdnex = strIndex;
            goto Loop;

        }

        #endregion

    }

}
