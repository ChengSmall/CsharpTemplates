using System;
using System.Collections.Generic;
using System.IO;

namespace Cheng.Algorithm.Sorts.Comparers
{

    /// <summary>
    /// 将文件转化为字符串比较的文件比较器
    /// </summary>
    public sealed class FileInfoComparer : Comparer<FileInfo>
    {

        /// <summary>
        /// 文件字符串转化类型
        /// </summary>
        public enum ComparerType : byte
        {
            /// <summary>
            /// 文件名.后缀
            /// </summary>
            FileName,
            /// <summary>
            /// 文件完整路径
            /// </summary>
            FilePath,
            /// <summary>
            /// 文件名无后缀
            /// </summary>
            NotSuffix,
            /// <summary>
            /// 只有后缀
            /// </summary>
            OnlySuffix,
            /// <summary>
            /// 自定义转化
            /// </summary>
            Custom
        }

        #region 构造

        /// <summary>
        /// 实例化一个文件转字符串比较器，使用文件名.后缀做字符串比较
        /// </summary>
        /// <param name="strComparer">字符串比较器</param>
        /// <exception cref="ArgumentNullException">字符串比较器为null</exception>
        public FileInfoComparer(IComparer<string> strComparer) : this(strComparer, ComparerType.FileName, null)
        {
        }

        /// <summary>
        /// 实例化一个文件转字符串比较器，使用自定义字符串转化
        /// </summary>
        /// <param name="strComparer">字符串比较器</param>
        /// <param name="FileToStr">自定义文件转字符串方法</param>
        /// <exception cref="ArgumentNullException">字符串比较器为null</exception>
        public FileInfoComparer(IComparer<string> strComparer, Func<FileInfo, string> FileToStr) : this(strComparer, ComparerType.Custom, FileToStr ?? throw new ArgumentNullException())
        {
        }

        /// <summary>
        /// 实例化一个文件转字符串比较器，指定转化类型
        /// </summary>
        /// <param name="strComparer">字符串比较器</param>
        /// <param name="type">文件字符串转化类型</param>
        /// <param name="FileToStr">自定义文件转字符串方法，仅当<paramref name="type"/>为<see cref="ComparerType.Custom"/>时有效</param>
        /// <exception cref="ArgumentNullException">字符串比较器为null</exception>
        /// <exception cref="ArgumentException">转化类型枚举不是已有值</exception>
        public FileInfoComparer(IComparer<string> strComparer, ComparerType type, Func<FileInfo, string> FileToStr)
        {
            if (strComparer is null) throw new ArgumentNullException();
            if (type < ComparerType.FileName || type > ComparerType.Custom) throw new ArgumentException();

            comparer = strComparer;
            fileToStr = FileToStr;
            ctype = type;
        }

        /// <summary>
        /// 实例化一个文件转字符串比较器，指定转化类型
        /// </summary>
        /// <param name="strComparer">字符串比较器</param>
        /// <param name="type">文件字符串转化类型</param>
        /// <exception cref="ArgumentNullException">字符串比较器为null</exception>
        /// <exception cref="ArgumentException">转化类型枚举不是已有值</exception>
        public FileInfoComparer(IComparer<string> strComparer, ComparerType type) : this(strComparer, type, null)
        {
        }

        #endregion
        
        private IComparer<string> comparer;
        private Func<FileInfo, string> fileToStr;
        private ComparerType ctype;

        public sealed override int Compare(FileInfo x, FileInfo y)
        {
            
            switch (ctype)
            {
                case ComparerType.FileName:
                    return comparer.Compare(x?.Name, y?.Name);
                case ComparerType.FilePath:
                    return comparer.Compare(x?.FullName, y?.FullName);
                case ComparerType.NotSuffix:
                    return comparer.Compare(x?.Name, y.Name);
                case ComparerType.OnlySuffix:
                    return comparer.Compare(x?.Extension, y?.Extension);
                case ComparerType.Custom:
                    if (fileToStr is null) throw new ArgumentException();
                    return comparer.Compare(fileToStr.Invoke(x), fileToStr.Invoke(y));
                default:
                    throw new ArgumentException();
            }
        }
    }

}
