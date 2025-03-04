
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using Cheng.Json;
using Cheng.Memorys;

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

        static string defToStr<T>(T obj)
        {
            string str = obj?.ToString();
            if (str is null) return nullstr;
            return str;
        }

        /// <summary>
        /// 二进制序列字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToBin(this byte value)
        {
            const char c0 = '0';
            const char c1 = '1';
            char* cs = stackalloc char[8];

            for (int i = 0; i < 8; i++)
            {
                cs[7 - i] = ((value >> i) & 0b1) == 1 ? c1 : c0;
            }

            return new string(cs, 0, 8);
        }

        /// <summary>
        /// 将单字节二进制序列写入到指定地址的字符串中
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cs"></param>
        public static void ToBinptr(this byte value, char* cs)
        {
            const char c0 = '0';
            const char c1 = '1';

            for (int i = 0; i < 8; i++)
            {
                cs[7 - i] = ((value >> i) & 0b1) == 1 ? c1 : c0;
            }

        }

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
        /// 将byte转为二进制文本
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ByteToX2(byte value)
        {
            char* cp = stackalloc char[8];

            for (int i = 0; i < 8; i++)
            {
                cp[7 - i] = ((value >> i) & 1) == 1 ? '1' : '0';
            }
            return new string(cp, 0, 8);
        }

        /// <summary>
        /// （DEBUG）遍历集合
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="lineCount">一行显示数</param>
        /// <param name="fen">每个元素分隔符</param>
        /// <param name="toStr">字符串转化方法</param>
        /// <returns></returns>
        public static string Foreach(this IEnumerable arr, int lineCount = 10, string fen = " ", Func<object, string> toStr = null)
        {
            if (arr is null) throw new ArgumentNullException();
            if (toStr is null) toStr = defToStr;

            return foreachEnumator(arr.GetEnumerator(), lineCount, fen, toStr);
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
        /// <returns></returns>
        public static string ForeachList(this IList list, int index, int count, int lineCount = 10, string fen = " ", Func<object, string> toStr = null)
        {
            if (toStr is null) toStr = defToStr;
            StringBuilder sb = new StringBuilder(list.Count);

            int i;
            int length = index + count;
            object obj;
            int end = length - 1;
            int ct;
            for (i = index, ct = 0; i < length; i++)
            {

                obj = list[i];

                sb.Append(toStr.Invoke(obj) + fen);
                ct++;

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
        /// <returns></returns>
        public static string ForeachList<T>(this IList<T> list, int index, int count, int lineCount = 10, string fen = " ", Func<T, string> toStr = null)
        {
            if (toStr is null) toStr = defToStr<T>;
            StringBuilder sb = new StringBuilder(list.Count);

            int i;
            int length = index + count;
            T obj;
            int end = length - 1;
            int ct;
            for (i = index, ct = 0; i < length; i++)
            {

                obj = list[i];

                sb.Append(toStr.Invoke(obj) + fen);
                ct++;

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

        static string foreachEnumator(IEnumerator enumator, int lineCount, string fen, Func<object, string> toStr)
        {
            StringBuilder sb = new StringBuilder();

            int count = 0;
            object temp;

            while (enumator.MoveNext())
            {
                temp = enumator.Current;


                if ((count % lineCount == 0) && count != 0)
                {
                    sb.AppendLine();
                }

                sb.Append(toStr.Invoke(temp) + fen);

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
        public static string ByteValueToStr(this long value, int highSize)
        {
            const int kb = 1024;

            const int mb = 1024 * 1024;

            const long gb = mb * 1024L;

            if(value < kb)
            {
                return value.ToString() + " Byte";
            }
            if(highSize == 0) return value.ToString() + " Byte";

            if (value < mb)
            {
                return ((double)value / kb).ToString("G6") + " KB";
            }
            if (highSize == 1) return ((double)value / kb).ToString("G6") + " KB";

            if (value < gb * 2)
            {
                return ((double)value / mb).ToString("G6") + " MB";
            }
            if (highSize == 2) return ((double)value / mb).ToString("G6") + " MB";

            return ((double)value / gb).ToString("G8") + " GB";
        }

        /// <summary>
        /// （DEBUG）将值当作字节大小动态转化为字符串形式
        /// </summary>
        /// <param name="value"></param>
        /// <param name="highSize">最高等级，0表示最大到字节，1表示KB，2表示MB，其它表示GB</param>
        /// <returns></returns>
        public static string ByteValueToStr(this ulong value, int highSize)
        {
            const uint kb = 1024;

            const uint mb = 1024 * 1024;

            const ulong gb = mb * 1024L;

            if (value < kb)
            {
                return value.ToString() + " Byte";
            }
            if (highSize == 0) return value.ToString() + " Byte";

            if (value < mb)
            {
                return ((double)value / kb).ToString("G6") + " KB";
            }
            if (highSize == 1) return ((double)value / kb).ToString("G6") + " KB";

            if (value < gb * 2)
            {
                return ((double)value / mb).ToString("G6") + " MB";
            }
            if (highSize == 2) return ((double)value / mb).ToString("G6") + " MB";

            return ((double)value / gb).ToString("G8") + " GB";
        }

        /// <summary>
        /// （DEBUG）将值当作字节大小动态转化为字符串形式
        /// </summary>
        /// <param name="value"></param>
        /// <param name="highSize">最高等级，0表示最大到字节，1表示KB，2表示MB，其它表示GB</param>
        /// <returns></returns>
        public static string ByteValueToStr(this int value, int highSize)
        {
            return ByteValueToStr((long)value, highSize);
        }

        /// <summary>
        /// （DEBUG）将值当作字节大小动态转化为字符串形式
        /// </summary>
        /// <param name="value"></param>
        /// <param name="highSize">最高等级，0表示最大到字节，1表示KB，2表示MB，其它表示GB</param>
        /// <returns></returns>
        public static string ByteValueToStr(this uint value, int highSize)
        {
            return ByteValueToStr((long)value, highSize);
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
