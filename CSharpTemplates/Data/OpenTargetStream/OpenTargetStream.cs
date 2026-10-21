using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace Cheng.DataStructure
{

    #region 基类

    /// <summary>
    /// 提供一个从字符串路径打开流数据的基类
    /// </summary>
    public interface IOpenTargetStream
    {

        /// <summary>
        /// 打开指定路径下的数据
        /// </summary>
        /// <param name="path">要打开的路径，路径格式由派生类定义</param>
        /// <returns>要打开的对象访问器</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        IGettingStream OpenStreamGetter(string path);

    }

    #endregion

}