using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Drawing;
using System.Runtime.Serialization;
using System.Runtime;
using System.Reflection;
using System.Resources;
using Cheng.Json;

namespace Cheng.Windows.Forms
{

    /// <summary>
    /// Json扩展
    /// </summary>
    public static class ParserToJsonExtend
    {

        #region DataGridView

        /// <summary>
        /// 将<see cref="DataGridView"/>网格转化为Json键值对列表
        /// </summary>
        /// <param name="dataGridView">待转化的表格数据</param>
        /// <returns>转化后的Json数据</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static JsonDictionary ParserToJsonByDictionary(this DataGridView dataGridView)
        {
            if (dataGridView is null) throw new ArgumentNullException();

            JsonDictionary jd;

            var cols = dataGridView.Columns;
            var rows = dataGridView.Rows;

            var columnCount = cols.Count;
            var rowCount = rows.Count;

            jd = new JsonDictionary(columnCount);

            for (int i = 0; i < columnCount; i++)
            {

                var column = cols[i];
                
                JsonList jlist = new JsonList();

                jd[column.HeaderText] = jlist;
                //jd.Add(column.HeaderText, jlist);

                for (int ri = 0; ri < rowCount; ri++)
                {
                    var data = dataGridView[i, ri];
                    var dataObj = data.Value;
                    jlist.Add(dataObj?.ToString());
                }

            }

            return jd;

        }

        /// <summary>
        /// 将<see cref="DataGridView"/>网格转化为Json集合
        /// </summary>
        /// <param name="dataGridView">待转化的表格数据</param>
        /// <returns>转化后的Json数据</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static JsonList ParserToJsonByList(this DataGridView dataGridView)
        {
            if (dataGridView is null) throw new ArgumentNullException();

            JsonList jlist;

            var cols = dataGridView.Columns;
            var rows = dataGridView.Rows;

            var columnCount = cols.Count;
            var rowCount = rows.Count;

            jlist = new JsonList(rowCount);

            for (int i = 0; i < rowCount; i++)
            {
                //var column = cols[i];

                JsonDictionary jd = new JsonDictionary(columnCount);

                for (int jdI = 0; jdI < columnCount; jdI++)
                {

                    string key = cols[jdI].HeaderText;

                    jd[key] = (JsonString)dataGridView[jdI, i].Value?.ToString();

                }

                jlist.Add(jd);

            }

            return jlist;
        }

        /// <summary>
        /// 将<see cref="DataGridView"/>网格转化为Json键值对表格数据
        /// </summary>
        /// <remarks>
        /// <para>将网格的第一个列数据用作json的根，采用键值对的方式从上到下转化每一行为json</para>
        /// </remarks>
        /// <param name="dataGridView">待转化的表格数据</param>
        /// <returns>转化后的Json数据</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="Exception">其它错误</exception>
        public static JsonDictionary ParserToJsonByFirstNameDict(this DataGridView dataGridView)
        {
            if (dataGridView is null) throw new ArgumentNullException();

            JsonDictionary json;

            var cols = dataGridView.Columns;
            var rows = dataGridView.Rows;

            //竖向的列数
            //var columnCount = cols.Count;

            //横向的行数
            var rowCount = rows.Count;

            json = new JsonDictionary(rowCount);

            //遍历每一行数据
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {

                var rowItem = rows[rowIndex];
                var rowCells = rowItem.Cells;

                var key = (rowCells[0].Value?.ToString()) ?? string.Empty;

                JsonDictionary jdt = new JsonDictionary(rowCells.Count);

                for (int colIndex = 1; colIndex < rowCells.Count; colIndex++)
                {
                    var item = rowCells[colIndex];
                    
                    //key
                    var itemKey = cols[item.ColumnIndex].HeaderText;
                    //value
                    var itemValue = item.Value;

                    if (itemValue is null)
                    {
                        jdt[itemKey] = null;
                    }
                    else if (itemValue is bool value_bv)
                    {
                        jdt[itemKey] = (JsonVariable)value_bv;
                    }
                    else
                    {
                        var itemValueStr = itemValue.ToString();
                        if (itemValueStr is null)
                        {
                            jdt[itemKey] = null;
                        }
                        else if (long.TryParse(itemValueStr, out var value_i))
                        {
                            jdt[itemKey] = (JsonVariable)value_i;
                        }
                        else if (double.TryParse(itemValueStr, out var value_r))
                        {
                            jdt[itemKey] = (JsonVariable)value_r;
                        }
                        else if (bool.TryParse(itemValueStr, out var value_b))
                        {
                            jdt[itemKey] = (JsonVariable)value_b;
                        }
                        else
                        {
                            jdt[itemKey] = new JsonString(itemValueStr);
                        }

                    }

                }

                json[key] = jdt;

            }


            return json;
        }

        #endregion

    }


}

