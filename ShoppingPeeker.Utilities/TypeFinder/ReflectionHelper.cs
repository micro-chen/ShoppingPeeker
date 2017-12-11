using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ShoppingPeeker.Utilities.TypeFinder
{

    /// <summary>
    /// 实体类反射帮助类
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// 根据输入类型获取对应的成员属性集合
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        /// <summary>
        /// 根据输入类型获取对应的字段属性集合
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FieldInfo[] GetFields(Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        }




        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetPropertyValue<T>(T entity, string propertyName)
        {
            PropertyInfo property = entity.GetType().GetProperty(propertyName);
            if (property != null)
            {
                return property.FastGetValue(entity);
            }

            return null;
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static object GetPropertyValue<T>(T entity, PropertyInfo property)
        {
            if (property != null)
            {
                return property.FastGetValue(entity);
            }

            return null;
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static void SetPropertyValue<T>(T entity, PropertyInfo property,object value)
        {
            if (property != null)
            {
                 property.FastSetValue(entity, value);
            }

        }

        /// <summary>
        /// 获取属性类型
        /// </summary>
        /// <param name="classType"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static Type GetPropertyType(Type classType, string propertyName)
        {
            PropertyInfo property = classType.GetProperty(propertyName);
            if (property != null)
            {
                return property.PropertyType;
            }

            return null;
        }







        public static object FastInvoke(this MethodInfo methodInfo, object instance, params object[] parameters)
        {
            return FastReflectionCaches.MethodInvokerCache.Get(methodInfo).Invoke(instance, parameters);
        }

        public static void FastSetValue(this PropertyInfo propertyInfo, object instance, object value)
        {
            FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo).SetValue(instance, value);
        }

        public static object FastGetValue(this PropertyInfo propertyInfo, object instance)
        {
            return FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo).GetValue(instance);
        }

        public static object FastGetValue(this FieldInfo fieldInfo, object instance)
        {
            return FastReflectionCaches.FieldAccessorCache.Get(fieldInfo).GetValue(instance);
        }

        public static object FastInvoke(this ConstructorInfo constructorInfo, params object[] parameters)
        {
            return FastReflectionCaches.ConstructorInvokerCache.Get(constructorInfo).Invoke(parameters);
        }


        /// <summary>
        /// 实现实体的深度克隆（使用二进制序列化进行对象的序列化到流，然后再进行反序列化操作
        /// 对象必须是声明：Serializable
        /// ）
        /// </summary>
        /// <returns></returns>
        public static object CloneData<T>(T data)
        {
            IFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            bf.Serialize(ms, data);

            ms.Seek(0, SeekOrigin.Begin);

            var obj = bf.Deserialize(ms);

            ms.Flush();
            ms.Close();
            ms.Dispose();

            return obj;
        }

    }

}


