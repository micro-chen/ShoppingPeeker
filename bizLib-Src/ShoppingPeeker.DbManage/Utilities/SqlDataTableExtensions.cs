using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;
using ShoppingPeeker.Utilities.TypeFinder;

namespace ShoppingPeeker.DbManage.Utilities
{
    public static class SqlDataTableExtensions
    {

        /// <summary>    
        /// 将泛型集合类转换成DataTable    
        /// </summary>    
        /// <typeparam name="T">集合项类型</typeparam>    
        /// <param name="list">集合</param>    
        /// <param name="propertys">列名 属性集合</param>    
        /// <returns>数据集(表)</returns>    
        public static DataTable ConvertListToDataTable<T>(IEnumerable<T> list, ref PropertyInfo[] propertys)
        {

            DataTable result = new DataTable();
            var totalCount = list.Count();
            if (totalCount > 0)
            {
                //如果未传递属性 那么属性Property  反射出来
                if (null == propertys || propertys.Length <= 0)
                {
                    propertys = list.ElementAt(0).GetType().GetProperties();
                }


                foreach (PropertyInfo pi in propertys)
                {
                    result.Columns.Add(pi.Name, pi.PropertyType);

                }

                for (int i = 0; i < totalCount; i++)
                {
                    var model = list.ElementAt(i);
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {

                        object obj = ReflectionHelper.GetPropertyValue(model, pi);
                        tempList.Add(obj);

                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }


        /// <summary>
        /// 将DataTable中的行 转化成指定的实体
        /// </summary>
        /// <typeparam name="TElemet"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<TElement> ConvertDataTableToEntitys<TElement>(this DataTable dt)
            where TElement : BaseEntity, new()
        {


            if (dt.Rows.Count <= 0)
            {
                return null;
            }
            var lstElemets = new List<TElement>();





            foreach (DataRow row in dt.Rows)
            {
                var model = new TElement();
                //1 获取实体中所有的属性Property  
                var propertys = model.GetCurrentEntityProperties();

                //2  判断属性类型 转化成对应 的数据类型  赋值
                foreach (var p in propertys)
                {
                    if (row[p.Name] != null && row[p.Name].ToString() != "")
                    {
                        var filedValue = row[p.Name];
                        ReflectionHelper.SetPropertyValue( model, p, filedValue);
                        //p.SetValue(model, filedValue, null);
                    }
                    else
                    {
                        ReflectionHelper.SetPropertyValue(model, p, null);
                        // p.SetValue(model, null, null);
                    }


                }

                lstElemets.Add(model);
            }
            //3 返回赋值的集合
            return lstElemets;
        }
    }


}
