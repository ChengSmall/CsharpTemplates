using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Cheng.Parsers.CommandParsers
{

    /// <summary>
    /// 解析命令并执行的公共接口
    /// </summary>
    public interface ICommandParser
    {
        
        /// <summary>
        /// 执行一道命令
        /// </summary>
        /// <param name="command">命令字符串</param>
        /// <returns>是否执行成功</returns>
        bool Execute(string command);
    }


    /// <summary>
    /// 执行后续命令的解析器
    /// </summary>
    /// <param name="command">要执行的命令</param>
    /// <param name="startIndex">要执行的命令字符串的起始索引</param>
    /// <param name="count">要执行的命令字符串的字符数量</param>
    /// <returns>是否执行成功</returns>
    public delegate bool ExecuteCommandAction(string command, int startIndex, int count);

    /// <summary>
    /// 命令解析器错误类型
    /// </summary>
    public enum ExecuteCommandError : byte
    {
        /// <summary>
        /// 无错误
        /// </summary>
        NoneError,

        /// <summary>
        /// 没有指定的命令类型
        /// </summary>
        NotCommand,

        /// <summary>
        /// 命令方法返回了false
        /// </summary>
        RetCommandError,

        /// <summary>
        /// 错误的命令格式
        /// </summary>
        ErrorFormat,

        /// <summary>
        /// 参数为null
        /// </summary>
        NullArg

    }


    /// <summary>
    /// 可解析命令并执行的命令解析器
    /// </summary>
    public class CommandParser : ICommandParser
    {

        #region 构造

        /// <summary>
        /// 实例化一个命令解析器
        /// </summary>
        public CommandParser()
        {
            p_parsers = new Dictionary<string, ExecuteCommandAction>();
            p_separator = ' ';
            p_lastError = ExecuteCommandError.NoneError;
        }

        /// <summary>
        /// 实例化一个命令解析器
        /// </summary>
        /// <param name="separator">指定拆解命令的分隔符，默认为空格' '</param>
        public CommandParser(char separator)
        {
            p_parsers = new Dictionary<string, ExecuteCommandAction>();
            p_separator = separator;
            p_lastError = ExecuteCommandError.NoneError;
        }

        protected CommandParser(bool init)
        {
            if (init) p_parsers = new Dictionary<string, ExecuteCommandAction>();
        }

        #endregion

        #region 参数

        /// <summary>
        /// 装载命令执行单位的字典
        /// </summary>
        protected Dictionary<string, ExecuteCommandAction> p_parsers;        

        /// <summary>
        /// 命令字符串分隔符
        /// </summary>
        protected char p_separator;

        /// <summary>
        /// 上一个执行错误
        /// </summary>
        protected ExecuteCommandError p_lastError;

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 获取所有的命令集合
        /// </summary>
        public Dictionary<string, ExecuteCommandAction>.KeyCollection Commands
        {
            get => p_parsers.Keys;
        }

        /// <summary>
        /// 获取所有命令对应的执行委托集合
        /// </summary>
        public Dictionary<string, ExecuteCommandAction>.ValueCollection CommandActions
        {
            get => p_parsers.Values;
        }

        #endregion

        #region 基本命令删改

        /// <summary>
        /// 添加一行命令和匹配的可执行委托
        /// </summary>
        /// <param name="command">命令字符串</param>
        /// <param name="commandAction">执行委托</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public virtual void Add(string command, ExecuteCommandAction commandAction)
        {
            if (commandAction is null) throw new ArgumentNullException();
            p_parsers.Add(command, commandAction);
        }

        /// <summary>
        /// 删除一行命令和匹配的可执行委托
        /// </summary>
        /// <param name="command">命令字符串</param>
        /// <returns>是否成功删除</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public virtual bool Remove(string command)
        {
            return p_parsers.Remove(command);
        }

        /// <summary>
        /// 清除内部所有的命令和委托
        /// </summary>
        public virtual void Clear()
        {
            p_parsers.Clear();
        }

        /// <summary>
        /// 添加或重新设置一行命令和匹配的可执行委托
        /// </summary>
        /// <param name="command">命令字符串，若已有命令则会覆盖执行委托</param>
        /// <param name="commandAction">执行委托</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public virtual void SetCommand(string command, ExecuteCommandAction commandAction)
        {
            p_parsers[command] = commandAction ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// 获取指定命令与之匹配的可执行委托
        /// </summary>
        /// <param name="command">命令字符串</param>
        /// <param name="commandAction">获取的执行委托引用</param>
        /// <returns>是否成功获取</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public virtual bool TryGetCommand(string command, out ExecuteCommandAction commandAction)
        {
            return p_parsers.TryGetValue(command, out commandAction);
        }

        /// <summary>
        /// 查看是否有指定命令
        /// </summary>
        /// <param name="command">命令字符串</param>
        /// <returns>拥有返回true，没有返回false</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public virtual bool ContainsCommand(string command)
        {
            return p_parsers.ContainsKey(command);
        }

        /// <summary>
        /// 获取一个循环访问当前命令和对应委托的枚举器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, ExecuteCommandAction>> GetEnumerator()
        {
            return p_parsers.GetEnumerator();
        }

        #endregion

        #region 命令解析

        /// <summary>
        /// 重置错误代码
        /// </summary>
        public void ResetErrorArg()
        {
            p_lastError = ExecuteCommandError.NoneError;
        }

        /// <summary>
        /// 上一次执行命令后引发的错误代码
        /// </summary>
        public ExecuteCommandError LastError
        {
            get => p_lastError;
        }

        /// <summary>
        /// 执行一道命令
        /// </summary>
        /// <param name="command">命令字符串</param>
        /// <returns>是否执行成功</returns>
        public virtual bool Execute(string command)
        {
            
            if (command is null)
            {
                p_lastError = ExecuteCommandError.NullArg;
                return false;
            }

            int i = command.IndexOf(p_separator);

            string bc;

            bool isnotfen = i < 0;

            //无分隔符
            if (isnotfen)
            {
                bc = command;
            }
            else
            {
                //来到分隔符位置
                bc = command.Substring(0, i);
            }

            var b = p_parsers.TryGetValue(bc, out var action);

            if (b)
            {
                if (action is null)
                {
                    p_lastError = ExecuteCommandError.NullArg;
                    return false;
                }

                if (isnotfen)
                {
                    if(!action.Invoke(command, 0, command.Length))
                    {
                        p_lastError = ExecuteCommandError.RetCommandError;
                        return false;
                    }
                }
                else
                {
                    if(!action.Invoke(command, i + 1, command.Length - (i + 1)))
                    {
                        p_lastError = ExecuteCommandError.RetCommandError;
                        return false;
                    }
                }
            }
            else
            {
                p_lastError = ExecuteCommandError.NotCommand;
                return false;
            }

            p_lastError = ExecuteCommandError.NoneError;
            return true;
        }

        bool ICommandParser.Execute(string command)
        {
            return Execute(command);
        }

        #endregion

        #endregion

    }

}
