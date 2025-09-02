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
        /// 添加或覆盖一个待打包数据
        /// </summary>
        /// <param name="data">用于获取数据的对象</param>
        /// <param name="path">要添加或覆盖到的路径；<![CDATA[字符'/'或'\'都表示路径分隔符]]></param>
        /// <exception cref="ArgumentNullException">对象或路径是null</exception>
        /// <exception cref="ArgumentException">设置的路径拥有不可打印字符或其它路径错误</exception>
        void SetData(IGettingStream data, string path);

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
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="stream"/>已释放</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException"><paramref name="stream"/>提供的操作权限不足以给该方法用于打包数据</exception>
        /// <exception cref="Exception">其它错误</exception>
        void PackTo(Stream stream);

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
        /// 添加或覆盖一个待打包数据
        /// </summary>
        /// <param name="data">用于获取数据的对象</param>
        /// <param name="path">要添加或覆盖到的路径；<![CDATA[字符'/'或'\'都表示路径分隔符]]></param>
        /// <exception cref="ArgumentNullException">对象或路径是null</exception>
        /// <exception cref="ArgumentException">设置的路径拥有不可打印字符或其它路径错误</exception>
        public abstract void SetData(IGettingStream data, string path);

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
        /// 将当前队列的数据打包并写入指定流
        /// </summary>
        /// <param name="stream">接收包体数据的可写流</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="stream"/>已释放</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException"><paramref name="stream"/>提供的操作权限不足以给该方法用于打包数据</exception>
        /// <exception cref="Exception">其它错误</exception>
        public abstract void PackTo(Stream stream);

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

        #endregion

    }

}
