using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace ShoppingPeeker.DbManage.Utilities
{
    public static class DbTypeAndCLRType
    {




        // <summary>
        // Converts the given CLR type into a DbType
        // </summary>
        // <param name="clrType"> The CLR type to convert </param>
        public static DbType ConvertClrTypeToDbType(Type clrType)
        {
            switch (Type.GetTypeCode(clrType))
            {
                case TypeCode.Empty:
                    throw new ArgumentException("clrType参数为空错误！");

                case TypeCode.Object:
                    if (clrType == typeof(Byte[]))
                    {
                        return DbType.Binary;
                    }
                    if (clrType == typeof(Char[]))
                    {
                        // Always treat char and char[] as string
                        return DbType.String;
                    }
                    else if (clrType == typeof(Guid))
                    {
                        return DbType.Guid;
                    }
                    else if (clrType == typeof(TimeSpan))
                    {
                        return DbType.Time;
                    }
                    else if (clrType == typeof(DateTimeOffset))
                    {
                        return DbType.DateTimeOffset;
                    }
                    else if (clrType == typeof(Nullable<Int16>))
                        return DbType.Int16;
                    else if (clrType == typeof(Nullable<UInt16>))
                        return DbType.UInt16;
                    else if (clrType == typeof(Nullable<Int32>))
                        return DbType.Int32;
                    else if (clrType == typeof(Nullable<UInt32>))
                        return DbType.UInt32;
                    else if (clrType == typeof(Nullable<Int64>))
                        return DbType.Int64;
                    else if (clrType == typeof(Nullable<UInt64>))
                        return DbType.UInt64;
                    else if (clrType == typeof(Nullable<Single>))
                        return DbType.Single;
                    else if (clrType == typeof(Nullable<Double>))
                        return DbType.Double;
                    else if (clrType == typeof(Nullable<Decimal>))
                        return DbType.Decimal;
                    else if (clrType == typeof(Nullable<DateTime>))
                        return DbType.DateTime;

                    return DbType.Object;

                case TypeCode.DBNull:
                    return DbType.Object;
                case TypeCode.Boolean:
                    return DbType.Boolean;
                case TypeCode.SByte:
                    return DbType.SByte;
                case TypeCode.Byte:
                    return DbType.Byte;
                case TypeCode.Char:
                    // Always treat char and char[] as string
                    return DbType.String;
                case TypeCode.Int16:
                    return DbType.Int16;
                case TypeCode.UInt16:
                    return DbType.UInt16;
                case TypeCode.Int32:
                    return DbType.Int32;
                case TypeCode.UInt32:
                    return DbType.UInt32;
                case TypeCode.Int64:
                    return DbType.Int64;
                case TypeCode.UInt64:
                    return DbType.UInt64;
                case TypeCode.Single:
                    return DbType.Single;
                case TypeCode.Double:
                    return DbType.Double;
                case TypeCode.Decimal:
                    return DbType.Decimal;
                case TypeCode.DateTime:
                    return DbType.DateTime;
                case TypeCode.String:
                    return DbType.String;
                default:
                    throw new ArgumentException("clrType 为未知类型错误！");
            }
        }




        /// <summary>
        /// 判断是否是数据库支持的基本数据类型
        /// </summary>
        /// <param name="clrType"></param>
        /// <returns></returns>
        public static bool EntityPropertyClrTypeIsBasiscDbType(Type clrType)
        {
            switch (Type.GetTypeCode(clrType))
            {
                //case TypeCode.Empty:
                //    throw new ArgumentException("clrType参数为空错误！");

                case TypeCode.Object:
                    if (clrType == typeof(Byte[]))
                    {
                        //return DbType.Binary;
                        return true;//二进制数据类型
                    }
                    if (clrType == typeof(Char[]))
                    {
                        // Always treat char and char[] as string
                        //return DbType.String;
                        return true;//字符类型数组
                    }
                    else if (clrType == typeof(Guid))
                    {
                        //return DbType.Guid;
                        return true;//Guid
                    }
                    else if (clrType == typeof(TimeSpan))
                    {
                        //return DbType.Time;
                        return true;//Time
                    }
                    else if (clrType == typeof(DateTimeOffset))
                    {
                        /// return DbType.DateTimeOffset;
                        return true;//DateTime
                    }
                    else
                    {
                        return false;
                    }

                case TypeCode.DBNull:
                //return DbType.Object;
                case TypeCode.Boolean:
                //return DbType.Boolean;
                case TypeCode.SByte:
                // return DbType.SByte;
                case TypeCode.Byte:
                //return DbType.Byte;
                case TypeCode.Char:
                // Always treat char and char[] as string
                //return DbType.String;
                case TypeCode.Int16:
                //return DbType.Int16;
                case TypeCode.UInt16:
                // return DbType.UInt16;
                case TypeCode.Int32:
                // return DbType.Int32;
                case TypeCode.UInt32:
                //return DbType.UInt32;
                case TypeCode.Int64:
                //return DbType.Int64;
                case TypeCode.UInt64:
                // return DbType.UInt64;
                case TypeCode.Single:
                // return DbType.Single;
                case TypeCode.Double:
                //return DbType.Double;
                case TypeCode.Decimal:
                //return DbType.Decimal;
                case TypeCode.DateTime:
                //return DbType.DateTime;
                case TypeCode.String:
                    //return DbType.String;
                    return true;
                default://不在以上范围的均不是实体的有效类型到数据库表类型
                    return false;
                //throw new ArgumentException("clrType 为未知类型错误！");
            }
        }



        /// <summary>
        /// 是集合对象而不是基本对象类型
        /// </summary>
        /// <param name="pInfo"></param>
        /// <returns></returns>
        public static bool EntityPropertyClrTypeIsCollentionType(PropertyInfo pInfo)
        {
            var result = false;


            //数组类型 char[]  string[]  ....
            //if (pInfo.PropertyType.IsArray)
            //{
            //    return true;
            //}

            switch (pInfo.PropertyType.Name)
            {

                case "List`1":
                case "Array":
                case "ArrayList":
                case "Dictionary`2":
                case "KeyValuePair`2":
                case "IEnumerable`1":
                case "PropertyInfo[]":
                    result = true;
                    break;
            }

            return result;
        }

    }
}
