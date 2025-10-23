using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cheng.DataStructure;

namespace Cheng.Algorithm.Compressions
{

    /// <summary>
    /// 用于包体打包操作的统一接口
    /// </summary>
    public interface IPackagingOperation
    {

        #region

        /// <summary>
        /// 当前待打包数据队列的数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 允许在打包时设置加密密码
        /// </summary>
        bool CanPassword { get; }

        /// <summary>
        /// 允许在设置打包项时分配独立的加密密码
        /// </summary>
        /// <returns>如果该参数是true，则<see cref="CanPassword"/>参数无效</returns>
        bool CanAlonePassword { get; }

        /// <summary>
        /// 添加或覆盖一个待打包数据
        /// </summary>
        /// <param name="data">用于获取数据的对象</param>
        /// <param name="path">要添加或覆盖到的路径；<![CDATA[字符'/'或'\'都表示路径分隔符]]></param>
        /// <param name="password">此路径的加密密码，null或空元素表示不设置密码；（如果参数<see cref="CanAlonePassword"/>是false，该参数无效）</param>
        /// <exception cref="ArgumentNullException">对象或路径是null</exception>
        /// <exception cref="ArgumentException">设置的路径拥有不可打印字符或其它路径错误</exception>
        void SetData(IGettingStream data, string path, byte[] password);

        /// <summary>
        /// 是否允许对象从数据队列中删除已添加的路径
        /// </summary>
        bool CanRemoveData { get; }

        /// <summary>
        /// 删除某个路径上的待打包数据
        /// </summary>
        /// <param name="path">要删除的路径；<![CDATA[字符'/'或'\'都表示路径分隔符]]></param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">不存在该路径</exception>
        /// <exception cref="NotSupportedException">没有删除权限</exception>
        void RemoveData(string path);

        /// <summary>
        /// 是否允许对象从数据队列中确认某个路径是否存在数据
        /// </summary>
        bool CanContainsData { get; }

        /// <summary>
        /// 查找某个路径是否存在当前待打包队列中
        /// </summary>
        /// <param name="path">要查找的路径；<![CDATA[字符'/'或'\'都表示路径分隔符]]></param>
        /// <returns>true表示存在，false表示不存在；在判断时应当忽略非必要的路径字符，例如<![CDATA['/'和'\']]>这两个路径分隔符</returns>
        /// <exception cref="ArgumentNullException">对象或路径是null</exception>
        /// <exception cref="NotSupportedException">没有查找权限</exception>
        bool ContainsData(string path);

        /// <summary>
        /// 将当前队列的数据打包并写入指定流
        /// </summary>
        /// <param name="stream">接收包体数据的可写流</param>
        /// <param name="password">包体数据的加密密码，，null或空元素表示不设置密码；（如果参数<see cref="CanPassword"/>是false，该参数无效）</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="stream"/>已释放</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException"><paramref name="stream"/>提供的操作权限不足以给该方法用于打包数据</exception>
        /// <exception cref="Exception">其它错误</exception>
        void PackTo(Stream stream, byte[] password);

        #endregion

    }

    /// <summary>
    /// 用于包体打包操作的基类
    /// </summary>
    public abstract class PackagingOperation : IPackagingOperation
    {

        #region 派生

        /// <summary>
        /// 当前待打包数据队列的数量
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// 是否允许对象从数据队列中删除已添加的路径
        /// </summary>
        public abstract bool CanRemoveData { get; }

        /// <summary>
        /// 是否允许对象从数据队列中确认某个路径是否存在数据
        /// </summary>
        public abstract bool CanContainsData { get; }

        /// <summary>
        /// 允许在打包时设置加密密码
        /// </summary>
        public virtual bool CanPassword => false;

        /// <summary>
        /// 允许在设置打包项时分配独立的加密密码
        /// </summary>
        /// <returns>如果该参数是true，则优先采用独立密码项；没有设置密码的项在打包时采用<see cref="PackTo(Stream, byte[])"/>的参数作为加密密码</returns>
        public virtual bool CanAlonePassword => false;

        /// <summary>
        /// 添加或覆盖一个待打包数据
        /// </summary>
        /// <param name="data">用于获取数据的对象</param>
        /// <param name="path">要添加或覆盖到的路径；<![CDATA[字符'/'或'\'都表示路径分隔符]]></param>
        /// <exception cref="ArgumentNullException">对象或路径是null</exception>
        /// <exception cref="ArgumentException">设置的路径拥有不可打印字符或其它路径错误</exception>
        public virtual void SetData(IGettingStream data, string path)
        {
            SetData(data, path, null);
        }

        /// <summary>
        /// 删除某个路径上的待打包数据
        /// </summary>
        /// <param name="path">要删除的路径；<![CDATA[字符'/'或'\'都表示路径分隔符]]></param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">不存在该路径</exception>
        /// <exception cref="NotSupportedException">没有删除权限</exception>
        public virtual void RemoveData(string path) => throw new NotSupportedException();

        /// <summary>
        /// 查找某个路径是否存在当前待打包队列中
        /// </summary>
        /// <param name="path">要查找的路径；<![CDATA[字符'/'或'\'都表示路径分隔符]]></param>
        /// <returns>true表示存在，false表示不存在；在判断时应当忽略非必要的路径字符，例如<![CDATA['/'和'\']]>这两个路径分隔符</returns>
        /// <exception cref="ArgumentNullException">对象或路径是null</exception>
        /// <exception cref="NotSupportedException">没有查找权限</exception>
        public virtual bool ContainsData(string path) => throw new NotSupportedException();

        /// <summary>
        /// 删除某个路径上的待打包数据
        /// </summary>
        /// <param name="path">要删除的路径；<![CDATA[字符'/'或'\'都表示路径分隔符]]></param>
        /// <returns>返回true表示成功删除，false表示当前待打包队列内没有此路径数据</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">没有删除权限</exception>
        public virtual bool TryRemoveData(string path)
        {
            if (!ContainsData(path)) return false;
            RemoveData(path);
            return true;
        }

        /// <summary>
        /// 将当前队列的数据打包并写入指定流
        /// </summary>
        /// <param name="stream">接收包体数据的可写流</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="stream"/>已释放</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException"><paramref name="stream"/>提供的操作权限不足以给该方法用于打包数据</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual void PackTo(Stream stream)
        {
            PackTo(stream, null);
        }

        /// <summary>
        /// 将当前队列的数据打包并写入指定文件
        /// </summary>
        /// <param name="filePath">接收包体数据的文件所在路径</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ObjectDisposedException">已释放</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="DirectoryNotFoundException">指定的路径无效，例如位于未映射的驱动器上</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual void PackToFile(string filePath)
        {
            using (FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                PackTo(file);
            }
        }

        /// <summary>
        /// 添加或覆盖一个待打包数据
        /// </summary>
        /// <param name="data">用于获取数据的对象</param>
        /// <param name="path">要添加或覆盖到的路径；<![CDATA[字符'/'或'\'都表示路径分隔符]]></param>
        /// <param name="password">此路径的加密密码，null或空元素表示不设置密码；（如果参数<see cref="CanAlonePassword"/>是false，该参数无效）</param>
        /// <exception cref="ArgumentNullException">对象或路径是null</exception>
        /// <exception cref="ArgumentException">设置的路径拥有不可打印字符或其它路径错误</exception>
        public abstract void SetData(IGettingStream data, string path, byte[] password);

        /// <summary>
        /// 将当前队列的数据打包并写入指定流
        /// </summary>
        /// <param name="stream">接收包体数据的可写流</param>
        /// <param name="password">包体数据的加密密码，null或空元素表示不设置密码；（如果参数<see cref="CanPassword"/>是false，该参数无效）</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="stream"/>已释放</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException"><paramref name="stream"/>提供的操作权限不足以给该方法用于打包数据</exception>
        /// <exception cref="Exception">其它错误</exception>
        public abstract void PackTo(Stream stream, byte[] password);

        /// <summary>
        /// 添加或覆盖一个待打包数据
        /// </summary>
        /// <param name="data">用于获取数据的对象</param>
        /// <param name="path">要添加或覆盖到的路径；<![CDATA[字符'/'或'\'都表示路径分隔符]]></param>
        /// <param name="password">加密此路径数据的密码文本，null或空字符串表示不设置密码；（如果参数<see cref="CanAlonePassword"/>是false，该参数无效）</param>
        /// <param name="encoding">匹配加密密码文本的字符编码，null表示不使用密码加密</param>
        /// <exception cref="ArgumentNullException">对象或路径是null</exception>
        /// <exception cref="ArgumentException">设置的路径拥有不可打印字符或其它路径错误</exception>
        public virtual void SetData(IGettingStream data, string path, string password, Encoding encoding)
        {
            SetData(data, path, string.IsNullOrEmpty(password) ? null : encoding?.GetBytes(password));
        }

        /// <summary>
        /// 将当前队列的数据打包并写入指定流
        /// </summary>
        /// <param name="stream">接收包体数据的可写流</param>
        /// <param name="password">加密包体数据的密码文本，null或空字符串表示不设置密码；（如果参数<see cref="CanPassword"/>是false，该参数无效）</param>
        /// <param name="encoding">匹配加密密码文本的字符编码，null表示不使用密码加密</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="stream"/>已释放</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException"><paramref name="stream"/>提供的操作权限不足以给该方法用于打包数据</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual void PackTo(Stream stream, string password, Encoding encoding)
        {
            PackTo(stream, string.IsNullOrEmpty(password) ? null : encoding?.GetBytes(password));
        }

        #endregion

    }

}
