using Cheng.Algorithm.Collections;
using Cheng.Algorithm.Randoms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.DEBUG
{

    public static class ListExt
    {

        /// <summary>
        /// 验证集合是否合格
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ListIsOK<T>(this IList<T> list)
        {
            if(list is null)
            {
                return "[Null]";
            }
            StringBuilder sb = new StringBuilder();

            try
            {

                if (list.IsReadOnly)
                {
                    var fors = list.ForeachList(0, list.Count);
                    return "属于只读集合，遍历:\r\n" + fors;
                }

                LICRandom random = new LICRandom();

                T[] buf = new T[1024];
                sb.AppendLine("遍历:");
                sb.AppendLine(list.ForeachList(0, list.Count));
                sb.AppendLine("---------------------------");

                sb.AppendLine("验证增删查改:");

                int i;

                for (i = 0; i < 1024; i++)
                {
                    list.Add(default(T));
                }
                sb.AppendLine("添加1024个默认值");

                for (i = 0; i < 1024; i++)
                {
                    list.Insert(0, default(T));
                }
                sb.AppendLine("插入1024个默认值");

                for (i = 0; i < 1024; i++)
                {
                    list.RemoveAt(0);
                }
                sb.AppendLine("删除1024个前端值");

                list.Clear();
                sb.AppendLine("清空全部值");

                list.Add(default);
                list.Add(default);
                list.Add(default);
                list.Add(default);

                T[] arr = new T[5];
                list.CopyTo(arr, 1);
                sb.AppendLine("验证完毕！");

            }
            catch (Exception ex)
            {
                sb.AppendLine("----------------------------");
                sb.AppendLine("异常:");
                sb.AppendLine(ex.Message);
                sb.Append("类型:");
                sb.AppendLine(ex.GetType().FullName);
                sb.AppendLine(ex.StackTrace);
            }

            return sb.ToString();

        }

    }

}
