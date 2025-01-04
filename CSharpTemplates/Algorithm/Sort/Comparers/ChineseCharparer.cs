using Cheng.Json;
using System;
using System.Collections.Generic;

namespace Cheng.Algorithm.Sorts.Comparers
{

    /// <summary>
    /// 按照拼音首字母比较汉字的比较器模板
    /// </summary>
    /// <remarks>
    /// <para>按照汉字拼音首字母对字符比较的比较器，使用字典表示汉字和拼音的映射关系；在比较时若出现不存在字典上的字符，则使用默认或指定的字符比较器</para>
    /// </remarks>
    public sealed class ChineseCharparer : Comparer<char>
    {

        #region 构造

        /// <summary>
        /// 实例化一个首字母汉字比较器
        /// </summary>
        /// <param name="chineseDict">该参数表示一个汉字字符映射字典，key表示要查的汉字，value表示该汉字拼音</param>
        /// <param name="comparer">该参数用于当检测到非汉字字符（也就是字典内不存在的值）时使用的字符比较器，null表示默认比较器</param>
        /// <exception cref="ArgumentNullException">字典为null</exception>
        public ChineseCharparer(IDictionary<char, string> chineseDict, IComparer<char> comparer)
        {
            f_init(chineseDict, comparer, null, null);
        }

        /// <summary>
        /// 实例化一个首字母汉字比较器
        /// </summary>
        /// <param name="chineseDict">该参数表示一个汉字字符映射字典，key表示要查的汉字，value表示该汉字拼音</param>
        /// <exception cref="ArgumentNullException">字典为null</exception>
        public ChineseCharparer(IDictionary<char, string> chineseDict) : this(chineseDict, null)
        {
        }

        /// <summary>
        /// 实例化一个首字母汉字比较器，使用json键值对映射
        /// </summary>
        /// <param name="jsonDict">
        /// 表示一个汉字字符映射字典，key的第一位字符表示要查的汉字，value表示该汉字拼音
        /// </param>
        /// <param name="comparer">该参数用于当检测到非汉字字符（也就是字典内不存在的值）时使用的字符比较器，null表示默认比较器</param>
        /// <exception cref="ArgumentNullException">json对象为null</exception>
        public ChineseCharparer(JsonDictionary jsonDict, IComparer<char> comparer) : this(jsonDict, comparer, null, null)
        {
        }

        /// <summary>
        /// 实例化一个首字母汉字比较器，使用json键值对映射
        /// </summary>
        /// <param name="jsonDict">
        /// 表示一个汉字字符映射字典，key的第一位字符表示要查的汉字，value表示该汉字拼音
        /// </param>
        /// <param name="comparer">该参数用于当检测到非汉字字符（也就是字典内不存在的值）时使用的字符比较器，null表示默认比较器</param>
        /// <param name="pinComparer">该参数用于在比较汉字拼音字符串时使用的比较器，null表示默认比较器</param>
        /// <exception cref="ArgumentNullException">json字典为null</exception>
        public ChineseCharparer(JsonDictionary jsonDict, IComparer<char> comparer, IComparer<string> pinComparer) : this(jsonDict, comparer, pinComparer, null)
        {
        }

        /// <summary>
        /// 实例化一个首字母汉字比较器，使用json键值对映射
        /// </summary>
        /// <param name="jsonDict">
        /// 表示一个汉字字符映射字典，key的第一位字符表示要查的汉字，value表示该汉字拼音
        /// </param>
        /// <param name="comparer">该参数用于当检测到非汉字字符（也就是字典内不存在的值）时使用的字符比较器，null表示默认比较器</param>
        /// <param name="pinComparer">该参数用于在比较汉字拼音字符串时使用的比较器，null表示默认比较器</param>
        /// <param name="pinEqualComparer">当拼音比较完毕后，如果两个字符相等，则使用该比较器比较；null表示默认实现的字符比较器</param>
        /// <exception cref="ArgumentNullException">json字典为null</exception>
        public ChineseCharparer(JsonDictionary jsonDict, IComparer<char> comparer, IComparer<string> pinComparer, IComparer<char> pinEqualComparer)
        {
            //if (jsonDict is null) throw new ArgumentNullException();
            f_init(JsonToDictionary(jsonDict), comparer, pinComparer, pinEqualComparer);
        }

        /// <summary>
        /// 实例化一个首字母汉字比较器，使用json键值对映射
        /// </summary>
        /// <param name="jsonDict">
        /// 表示一个汉字字符映射字典，key的第一位字符表示要查的汉字，value表示该汉字拼音
        /// </param>
        /// <exception cref="ArgumentNullException">json对象为null</exception>
        public ChineseCharparer(JsonDictionary jsonDict) : this(jsonDict, null, null, null)
        {
        }

        /// <summary>
        /// 实例化一个首字母汉字比较器
        /// </summary>
        /// <param name="chineseDict">该参数表示一个汉字字符映射字典，key表示要查的汉字，value表示该汉字拼音</param>
        /// <param name="comparer">该参数用于当检测到非汉字字符（也就是字典内不存在的值）时使用的字符比较器，null表示默认比较器</param>
        /// <param name="pinComparer">该参数用于在比较汉字拼音字符串时使用的比较器，null表示默认比较器</param>
        /// <exception cref="ArgumentNullException">字典为null</exception>
        public ChineseCharparer(IDictionary<char, string> chineseDict, IComparer<char> comparer, IComparer<string> pinComparer)
        {
            f_init(chineseDict, comparer, pinComparer, null);
        }

        /// <summary>
        /// 实例化一个首字母汉字比较器
        /// </summary>
        /// <param name="chineseDict">该参数表示一个汉字字符映射字典，key表示要查的汉字，value表示该汉字拼音</param>
        /// <param name="comparer">该参数用于当检测到非汉字字符（也就是字典内不存在的值）时使用的字符比较器，null表示默认比较器</param>
        /// <param name="pinComparer">该参数用于在比较汉字拼音字符串时使用的比较器，null表示默认比较器</param>
        /// <param name="pinEqualComparer">当拼音比较完毕后，如果两个字符相等，则使用该比较器比较；null表示默认实现的字符比较器</param>
        /// <exception cref="ArgumentNullException">字典为null</exception>
        public ChineseCharparer(IDictionary<char, string> chineseDict, IComparer<char> comparer, IComparer<string> pinComparer, IComparer<char> pinEqualComparer)
        {
            f_init(chineseDict, comparer, pinComparer, pinEqualComparer);
        }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="chineseDict">汉字拼音查询表</param>
        /// <param name="comparer">无法查询时的字符比较器</param>
        /// <param name="pin">拼音比较器</param>
        /// <param name="pinEqualInvoke">拼音相同时的比较器</param>
        private void f_init(IDictionary<char, string> chineseDict, IComparer<char> comparer, IComparer<string> pin, IComparer<char> pinEqualInvoke)
        {
            if (chineseDict is null) throw new ArgumentNullException();
            p_hashChars = chineseDict;
            p_defaultChar = comparer ?? Comparer<char>.Default;            
            p_pin = pin ?? Comparer<string>.Default;
            p_pinEqualInvoke = pinEqualInvoke ?? Comparer<char>.Default;
        }

        #endregion

        #region 参数

        private IDictionary<char, string> p_hashChars;
        private IComparer<char> p_pinEqualInvoke;
        private IComparer<char> p_defaultChar;
        private IComparer<string> p_pin;

        #endregion

        #region 功能

        /// <summary>
        /// 创建一个<see cref="Dictionary{TKey, TValue}"/>字典，并将json对象表写入
        /// </summary>
        /// <param name="jsonDict">一个表示字符串映射表的json对象</param>
        /// <returns>
        /// 将<paramref name="jsonDict"/>的key的第一位字符，和value的字符串依次写入字典并返回；如果其中有无法写入的键值对则跳过
        /// </returns>
        public static Dictionary<char, string> JsonToDictionary(JsonDictionary jsonDict)
        {
            if (jsonDict is null) throw new ArgumentNullException();
            Dictionary<char, string> dict = new Dictionary<char, string>(jsonDict.Count);
            foreach (var item in jsonDict)
            {
                if (item.Key.Length < 1 || item.Value.DataType != JsonType.String) continue;
                dict[item.Key[0]] = item.Value.String;
            }
            return dict;
        }

        #endregion

        #region

        public sealed override int Compare(char x, char y)
        {
            string left, right;
            bool flag;

            if (x == y) return 0;

            flag = p_hashChars.TryGetValue(x, out left);
            if (!flag) return p_defaultChar.Compare(x, y);
            flag = p_hashChars.TryGetValue(y, out right);
            if (!flag) return p_defaultChar.Compare(x, y);

            int r = p_pin.Compare(left, right);
            return (r == 0) ? p_pinEqualInvoke.Compare(x, y) : r;
        }

        #endregion

    }

}
