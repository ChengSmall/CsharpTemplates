
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using Cheng.Json;
using Cheng.Memorys;
using System.Globalization;

namespace Cheng.DEBUG
{
    /// <summary>
    /// DEBUG扩展方法
    /// </summary>
    public unsafe static class DEBUGTEST
    {
        const string nullstr = "[Null]";
        //const string emptystr = "[Empty]";

        #region print

        /// <summary>
        /// （DEBUG）打印输出到控制台
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static void print<T>(this T obj)
        {
            string str = obj?.ToString();
            if (str is null) Console.Write(nullstr);
            else Console.Write(str);
        }

        /// <summary>
        /// （DEBUG）后继换行打印输出到控制台
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static void printl<T>(this T obj)
        {
            string str = obj?.ToString();
            if (str is null) Console.WriteLine(nullstr);
            else Console.WriteLine(str);
        }

        /// <summary>
        /// （DEBUG）打印指针输出到控制台
        /// </summary>
        /// <param name="ptr"></param>
        public static void print(this IntPtr ptr)
        {
            ((ulong)ptr.ToPointer()).ToString("x").ToUpper().print();
        }

        /// <summary>
        /// （DEBUG）后继换行打印指针输出到控制台
        /// </summary>
        /// <param name="ptr"></param>
        public static void printl(this IntPtr ptr)
        {
            ((ulong)ptr.ToPointer()).ToString("x").ToUpper().printl();
        }

        /// <summary>
        /// （DEBUG）打印指针输出到控制台
        /// </summary>
        /// <param name="ptr"></param>
        public static void print(this UIntPtr ptr)
        {
            ((ulong)ptr.ToPointer()).ToString("x").ToUpper().print();
        }

        /// <summary>
        /// （DEBUG）后继换行打印指针输出到控制台
        /// </summary>
        /// <param name="ptr"></param>
        public static void printl(this UIntPtr ptr)
        {
            ((ulong)ptr.ToPointer()).ToString("x").ToUpper().printl();
        }

        /// <summary>
        /// （DEBUG）以指针模式打印输出到控制台
        /// </summary>
        /// <typeparam name="TPTR"></typeparam>
        /// <param name="ptr"></param>
        public static void ptrint<TPTR>(this TPTR ptr) where TPTR : unmanaged
        {
            var p = (*(IntPtr*)&ptr);
            p.print();
        }

        /// <summary>
        /// （DEBUG）以指针模式后继换行打印输出到控制台
        /// </summary>
        /// <typeparam name="TPTR"></typeparam>
        /// <param name="ptr"></param>
        public static void ptrintl<TPTR>(this TPTR ptr) where TPTR : unmanaged
        {
            var p = (*(IntPtr*)&ptr);
            p.printl();
        }

        static void f_toUp(char* originCharptr, char* toCharptr, int length)
        {
            //小写1大写0
            const byte cbit = 0b00100000;

            char c;
            for (int i = 0; i < length; i++)
            {
                c = originCharptr[i];

                if (c >= 'a' && c <= 'z')
                {
                    c |= (char)cbit;
                    toCharptr[i] = c;
                }
            }

        }

        static void f_toLop(char* originCharptr, char* toCharptr, int length)
        {
            //小写1大写0
            const ushort cbit = 0b11111111_11011111;

            char c;
            for (int i = 0; i < length; i++)
            {
                c = originCharptr[i];

                if (c >= 'A' && c <= 'Z')
                {
                    c &= (char)cbit;
                    toCharptr[i] = c;
                }
            }
        }

        public static string defToStr<T>(T obj)
        {
            string str = obj?.ToString();
            if (str is null) return nullstr;
            return str;
        }

        public static string defToStr(object obj)
        {
            if (obj is null) return nullstr;
            return obj.ToString();
        }

        #region
        //public static void ToBin(ulong value, char fen, char* buffer71)
        //{
        //    const char c0 = '0';
        //    const char c1 = '1';

        //    //char* cp = buffer;
        //    //fixed (char* cp = carr)
        //    //{

        //    int i = 0;
        //    for (int boi = 0; boi < sizeof(ulong); boi++, i++)
        //    {
        //        for (int bi = 0; bi < 8; bi++, i++)
        //        {
        //            int rimov = ((sizeof(ulong) * 8) - 1) - ((boi * 8) + bi);

        //            char sc = ((value >> rimov) & 0b1) == 0b1 ? c1 : c0;
        //            buffer71[i] = sc;
        //        }
        //        if (sizeof(ulong) - 1 != boi)
        //        {
        //            buffer71[i] = fen;
        //        }
        //    }
        //    //return new string(cp, 0, 8 * sizeof(ulong) + sizeof(ulong));
        //}

        //public static void ToBin(uint value, char fen, char* buffer35)
        //{
        //    const char c0 = '0';
        //    const char c1 = '1';

        //    //char* cp = buffer;

        //    int i = 0;
        //    for (int boi = 0; boi < sizeof(uint); boi++, i++)
        //    {
        //        for (int bi = 0; bi < 8; bi++, i++)
        //        {
        //            int rimov = ((sizeof(uint) * 8) - 1) - ((boi * 8) + bi);

        //            char sc = ((value >> rimov) & 0b1) == 0b1 ? c1 : c0;
        //            buffer35[i] = sc;
        //        }
        //        if (sizeof(uint) - 1 != boi)
        //        {
        //            buffer35[i] = fen;
        //        }
        //    }
        //}

        //public static void ToBin(ushort value, char fen, char* buffer17)
        //{
        //    const char c0 = '0';
        //    const char c1 = '1';

        //    //char* cp = buffer;
        //    int i = 0;
        //    for (int boi = 0; boi < sizeof(ushort); boi++, i++)
        //    {
        //        for (int bi = 0; bi < 8; bi++, i++)
        //        {
        //            int rimov = ((sizeof(ushort) * 8) - 1) - ((boi * 8) + bi);

        //            char sc = ((value >> rimov) & 0b1) == 0b1 ? c1 : c0;
        //            buffer17[i] = sc;
        //        }
        //        if (sizeof(ushort) - 1 != boi)
        //        {
        //            buffer17[i] = fen;
        //        }
        //    }
        //}

        //public static void ToBin(byte value, char* buffer8)
        //{
        //    const char c0 = '0';
        //    const char c1 = '1';
        //    for (int i = 0; i < 8; i++)
        //    {
        //        buffer8[i] = ((value >> (7 - i)) & 0b1) == 0b1 ? c1 : c0;
        //    }
        //}
        #endregion

        /// <summary>
        /// 返回16进制数字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToX16(object obj)
        {
            if (obj is byte b)
            {
                return b.ToString("X2").ToUpper();
            }
            if (obj is short s)
            {
                return s.ToString("X4").ToUpper();
            }
            if (obj is int i)
            {
                return i.ToString("X8").ToUpper();
            }
            if (obj is long L)
            {
                return L.ToString("X16").ToUpper();
            }
            if (obj is sbyte sb)
            {
                return sb.ToString("X2").ToUpper();
            }
            if (obj is ushort us)
            {
                return us.ToString("X4").ToUpper();
            }
            if (obj is uint ui)
            {
                return ui.ToString("X8").ToUpper();
            }
            if (obj is ulong uL)
            {
                return uL.ToString("X16").ToUpper();
            }
            if (obj is float f)
            {
                return (*(uint*)&f).ToString("X16").ToUpper();
            }
            if (obj is double fd)
            {
                return (*(ulong*)&fd).ToString("X16").ToUpper();
            }
            return (obj as IFormattable)?.ToString("X", null);
        }

        /// <summary>
        /// 返回16进制数字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">可转化为16进制数的对象</param>
        /// <returns></returns>
        public static string ToX16<T>(T obj) where T : unmanaged
        {
            int size = sizeof(T);

            switch (size)
            {
                case 1:
                    return (*(byte*)&obj).ToString("X2").ToUpper();
                case 2:
                    return (*(ushort*)&obj).ToString("X4").ToUpper();
                case 4:
                    return (*(uint*)&obj).ToString("X8").ToUpper();
                case 8:
                    return (*(ulong*)&obj).ToString("X16").ToUpper();
            }

            return (obj as IFormattable)?.ToString("X", null);
        }

        /// <summary>
        /// （DEBUG）遍历集合
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="lineCount">一行显示数</param>
        /// <param name="fen">每个元素分隔符</param>
        /// <param name="toStr">字符串转化方法</param>
        /// <returns></returns>
        public static string Foreach(this IEnumerable arr, int lineCount = 10, string fen = " ", Func<object, string> toStr = null, Predicate<object> isPrint = null)
        {
            if (arr is null) throw new ArgumentNullException();
            var e = arr.GetEnumerator();
            try
            {
                return foreachEnumator(e, lineCount, fen, toStr ?? defToStr, isPrint);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (e is IDisposable dis) dis.Dispose();
            }
        }

        /// <summary>
        /// （DEBUG）遍历集合
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="lineCount">一行显示数</param>
        /// <param name="fen">每个元素分隔符</param>
        /// <param name="toStr">字符串转化方法</param>
        /// <returns></returns>
        public static string Foreach<T>(this IEnumerable<T> arr, int lineCount = 10, string fen = " ", Func<T, string> toStr = null, Predicate<T> isPrint = null)
        {
            if (arr is null) throw new ArgumentNullException();
            using (var e = arr.GetEnumerator())
            {
                return foreachEnumator(e, lineCount, fen, toStr ?? defToStr, isPrint);
            }
        }

        static string foreachEnumator<T>(IEnumerator<T> enumator, int lineCount, string fen, Func<T, string> toStr, Predicate<T> isPrint = null)
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            T temp;

            while (enumator.MoveNext())
            {
                temp = enumator.Current;

                var print = isPrint?.Invoke(temp);

                if (print.GetValueOrDefault(true))
                {

                    if ((count % lineCount == 0) && count != 0)
                    {
                        sb.AppendLine();
                    }

                    sb.Append(toStr.Invoke(temp) + fen);

                }

                count++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// （DEBUG）遍历集合
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index">起始索引</param>
        /// <param name="count">遍历数量</param>
        /// <param name="lineCount">一行打印数</param>
        /// <param name="fen">元素分隔符</param>
        /// <param name="toStr">元素转化为字符串的方法</param>
        /// <param name="isPrint">该元素是否需要打印</param>
        /// <returns></returns>
        public static string ForeachList(this IList list, int index, int count, int lineCount = 10, string fen = " ", Func<object, string> toStr = null, Predicate<object> isPrint = null)
        {
            if (toStr is null) toStr = defToStr;
            StringBuilder sb = new StringBuilder(list.Count * 2);

            int i;
            int length = index + count;
            object obj;
            int end = length - 1;
            int ct;
            for (i = index, ct = 0; i < length; i++)
            {
                obj = list[i];

                bool isP = (isPrint?.Invoke(obj)).GetValueOrDefault(true);

                if (isP)
                {
                    sb.Append(toStr.Invoke(obj));
                    if (i + 1 < length) sb.Append(fen);
                    ct++;
                }

                if (i != end)
                {
                    if (((ct == lineCount)))
                    {
                        sb.AppendLine();
                        ct = 0;
                    }
                }

            }

            return sb.ToString();
        }

        /// <summary>
        /// （DEBUG）遍历集合
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index">起始索引</param>
        /// <param name="count">遍历数量</param>
        /// <param name="lineCount">一行打印数</param>
        /// <param name="fen">元素分隔符</param>
        /// <param name="toStr">元素转化为字符串的方法</param>
        /// <param name="isPrint">该元素是否需要打印</param>
        /// <returns></returns>
        public static string ForeachList<T>(this IList<T> list, int index, int count, int lineCount = 10, string fen = " ", Func<T, string> toStr = null, Predicate<T> isPrint = null)
        {
            if (toStr is null) toStr = defToStr;
            StringBuilder sb = new StringBuilder(list.Count * 2);

            int i;
            int length = index + count;
            T obj;
            int end = length - 1;
            int ct;
            for (i = index, ct = 0; i < length; i++)
            {

                obj = list[i];

                bool isP = (isPrint?.Invoke(obj)).GetValueOrDefault(true);

                if (isP)
                {
                    sb.Append(toStr.Invoke(obj));
                    if (i + 1 < length) sb.Append(fen);
                    ct++;
                }

                if (i != end)
                {
                    if (((ct == lineCount)))
                    {
                        sb.AppendLine();
                        ct = 0;
                    }
                }

            }

            return sb.ToString();
        }

        /// <summary>
        /// （DEBUG）遍历集合
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ForeachList(this IList list)
        {
            return ForeachList(list, 0, list.Count);
        }

        /// <summary>
        /// （DEBUG）遍历集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ForeachList<T>(this IList<T> list)
        {
            return ForeachList(list, 0, list.Count);
        }

        static string foreachEnumator(IEnumerator enumator, int lineCount, string fen, Func<object, string> toStr, Predicate<object> isPrint = null)
        {
            StringBuilder sb = new StringBuilder();

            int count = 0;
            object temp;

            while (enumator.MoveNext())
            {
                temp = enumator.Current;

                var print = isPrint?.Invoke(temp);

                if (print.GetValueOrDefault(true))
                {
                    if ((count % lineCount == 0) && count != 0)
                    {
                        sb.AppendLine();
                    }

                    sb.Append(toStr.Invoke(temp) + fen);
                }

                count++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// （DEBUG）换行
        /// </summary>
        public static void printl()
        {
            Console.WriteLine();
        }

        public static int Sizeof<T>(this T value) where T : unmanaged
        {
            return sizeof(T);
        }

        public static int Sizeof<T>() where T : unmanaged
        {
            return sizeof(T);
        }

        /// <summary>
        /// （DEBUG）将值当作字节大小动态转化为字符串形式
        /// </summary>
        /// <param name="value"></param>
        /// <param name="highSize">最高等级，0表示最大到字节，1表示KB，2表示MB，其它表示GB</param>
        /// <returns></returns>
        public static string ToFileSizeStr(this long value, int highSize)
        {
            const int kb = 1024;

            const int mb = 1024 * 1024;

            const long gb = mb * 1024L;

            if (value < kb)
            {
                return value.ToString() + "Byte";
            }
            if (highSize == 0) return value.ToString() + "Byte";

            if (value < mb)
            {
                return ((double)value / kb).ToString("0.######") + "KB";
            }
            if (highSize == 1) return ((double)value / kb).ToString("0.#######") + "KB";

            if (value < gb * 2)
            {
                return ((double)value / mb).ToString("0.######") + "MB";
            }
            if (highSize == 2) return ((double)value / mb).ToString("0.########") + "MB";

            return ((double)value / gb).ToString() + "GB";
        }

        /// <summary>
        /// （DEBUG）将值当作字节大小动态转化为字符串形式
        /// </summary>
        /// <param name="value"></param>
        /// <param name="highSize">最高等级，0表示最大到字节，1表示KB，2表示MB，其它表示GB</param>
        /// <returns></returns>
        public static string ToFileSizeStr(this ulong value, int highSize)
        {
            const uint kb = 1024;

            const uint mb = 1024 * 1024;

            const ulong gb = mb * 1024L;

            if (value < kb)
            {
                return value.ToString() + "Byte";
            }
            if (highSize == 0) return value.ToString() + "Byte";

            if (value < mb)
            {
                return ((double)value / kb).ToString("0.######") + "KB";
            }
            if (highSize == 1) return ((double)value / kb).ToString("0.#######") + "KB";

            if (value < gb * 2)
            {
                return ((double)value / mb).ToString("0.######") + "MB";
            }
            if (highSize == 2) return ((double)value / mb).ToString("0.########") + "MB";

            return ((double)value / gb).ToString() + "GB";
        }

        /// <summary>
        /// （DEBUG）将值当作字节大小动态转化为字符串形式
        /// </summary>
        /// <param name="value"></param>
        /// <param name="highSize">最高等级，0表示最大到字节，1表示KB，2表示MB，其它表示GB</param>
        /// <returns></returns>
        public static string ToFileSizeStr(this int value, int highSize)
        {
            return ToFileSizeStr((long)value, highSize);
        }

        /// <summary>
        /// （DEBUG）将值当作字节大小动态转化为字符串形式
        /// </summary>
        /// <param name="value"></param>
        /// <param name="highSize">最高等级，0表示最大到字节，1表示KB，2表示MB，其它表示GB</param>
        /// <returns></returns>
        public static string ToFileSizeStr(this uint value, int highSize)
        {
            return ToFileSizeStr((long)value, highSize);
        }

        /// <summary>
        /// 返回区域性语言类型文本
        /// </summary>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static string ToText(this CultureInfo cultureInfo)
        {
            if (cultureInfo is null) return "[Null]";
            StringBuilder sb = new StringBuilder(32);

            var two = cultureInfo.TwoLetterISOLanguageName;
            var thr = cultureInfo.ThreeLetterWindowsLanguageName;
            sb.Append(two);
            if (!string.IsNullOrEmpty(thr))
            {
                sb.Append('-');
                sb.Append(thr.ToLowerInvariant());
            }
            sb.Append(" => ");
            sb.Append(cultureInfo.DisplayName);
            return sb.ToString();
        }

        #endregion

        #region 反射

        /// <summary>
        /// （DEBUG）以反射查看某个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">要查看的对象</param>
        /// <returns></returns>
        public static string Fanshe<T>(this T obj)
        {
            return Fanshe((obj?.GetType()) ?? typeof(T), obj);
        }

        /// <summary>
        /// （DEBUG）以反射查看某个类型
        /// </summary>
        /// <param name="type">要查看的类型</param>
        /// <param name="obj">该类型的对象，没有则为null</param>
        /// <returns></returns>
        public static string Fanshe(this Type type, object obj)
        {

            StringBuilder sb = new StringBuilder(256);
            var mems = type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            int length = mems.Length;
            MemberInfo mem;
            MemberTypes mt;
            sb.AppendLine("----------------------------------------------");
            sb.AppendLine("类型:" + type.FullName);
            sb.AppendLine();
            int end = length - 1;
            for (int i = 0; i < length; i++)
            {
                mem = mems[i];
                mt = mem.MemberType;
                sb.AppendLine("成员:" + mem.Name + " 成员类型:" + toMemberName(mt));
                
                sb.AppendLine(toStrMem(mem, mt, obj));
                if (i != end)
                {
                    sb.AppendLine("--------------------------------------");
                    sb.AppendLine();
                }
            }
            sb.AppendLine("----------------------------------------------");
            return sb.ToString();
        }


        static bool basefield(FieldInfo field)
        {
            Type t = field.FieldType;
            if (t.IsPrimitive) return true;
            if (t == typeof(decimal)) return true;
            if (t == typeof(string)) return true;
            if (t == typeof(object)) return true;
            if (t == typeof(DateTime)) return true;
            if (t == typeof(TimeSpan)) return true;
            if (t == typeof(Guid)) return true;
            if (t == typeof(IntPtr)) return true;
            if (t == typeof(UIntPtr)) return true;
            return false;
        }

        static string toMemberName(MemberTypes type)
        {
            StringBuilder sb = new StringBuilder(8);


            if ((type & MemberTypes.Custom) == MemberTypes.Custom)
            {
                sb.Append("自定义");
            }

            if ((type & MemberTypes.NestedType) == MemberTypes.NestedType)
            {
                sb.Append("嵌套");
            }
            if ((type & MemberTypes.Constructor) == MemberTypes.Constructor)
            {
                sb.Append("构造");
            }


            if ((type & MemberTypes.Field) == MemberTypes.Field)
            {
                sb.Append("字段");
            }
            if ((type & MemberTypes.Property) == MemberTypes.Property)
            {
                sb.Append("属性");
            }
          

            if ((type & MemberTypes.Event) == MemberTypes.Event)
            {
                sb.Append("事件");
            }

            if ((type & MemberTypes.Method) == MemberTypes.Method)
            {
                sb.Append("方法");
            }

            if ((type & MemberTypes.TypeInfo) == MemberTypes.TypeInfo)
            {
                sb.Append("类型");
            }

            return sb.ToString();

        }

        static string objToStr(object obj)
        {
            if (obj is null) return "[Null]";

            if(obj is IntPtr)
            {
                return (((IntPtr)obj)).ToString("x").ToUpper();
            }
            if(obj is UIntPtr)
            {
                return ((ulong)((UIntPtr)obj).ToPointer()).ToString("x").ToUpper();
            }

            if (obj is System.Drawing.Color color)
            {
                return color.Name + $" (R:{color.R},G:{color.G},B:{color.B},A:{color.A})";
            }

            return obj.ToString();
        }

        static string toStrMem(MemberInfo info, MemberTypes type, object obj)
        {
            const string space = " ";
            object fie;
            int i;
            if((type & MemberTypes.Field) == MemberTypes.Field)
            {
                if (obj is null) return "";
                
                FieldInfo fi = (FieldInfo)info;

                fie = fi.GetValue(obj);

                //if (basefield(fi))
                //{
                //    return (fi.FieldType.FullName + ": " + objToStr(fie));
                //}
                

                return (fi.FieldType.FullName + ": " + objToStr(fie));

                //return Fanshe(fie.GetType(), fie);
            }
            StringBuilder sb;

            if ((type & MemberTypes.Constructor) == MemberTypes.Constructor)
            {
                ConstructorInfo method = (ConstructorInfo)info;
                sb = new StringBuilder();

                sb.Append(method.DeclaringType.Name + '(');

                var pars = method.GetParameters();
                for (i = 0; i < pars.Length; i++)
                {
                    sb.Append((pars[i].ParameterType.Name + pars[i].Name));
                    if (i != pars.Length - 1) sb.Append(',');
                }
                sb.Append(");");
                return sb.ToString();
            }

            if ((type & MemberTypes.Method) == MemberTypes.Method)
            {
                MethodInfo method = (MethodInfo)info;
                sb = new StringBuilder();

                sb.Append(method.ReturnType + space);
                sb.Append(method.Name + '(');

                var pars = method.GetParameters();
                for (i = 0; i < pars.Length; i++)
                {
                    sb.Append((pars[i].ParameterType.Name + pars[i].Name));
                    if (i != pars.Length - 1) sb.Append(',');
                }
                sb.Append(");");
                return sb.ToString();

            }
           
            return info.ToString();
        }

        #endregion

    }

}
#if XMLDOC

#endif