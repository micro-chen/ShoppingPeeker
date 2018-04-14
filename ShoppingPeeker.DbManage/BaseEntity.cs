using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using System.ComponentModel;


namespace ShoppingPeeker.DbManage
{

    /// <summary>
    /// 属性变更事件参数
    /// </summary>
    public class EntityPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        public EntityPropertyChangedEventArgs(string propertyName, object value) : base(propertyName)
        {
            this.Value = value;
        }

        /// <summary>
        /// 属性值
        /// </summary>
        public object Value { get; set; }
    }

    /// <summary>
    /// 标识列主键类
    /// </summary>
    public class EntityIdentity
    {
        /// <summary>
        /// 主键名
        /// </summary>
        public string IdentityKeyName { get; set; }

        /// <summary>
        /// 主键值
        /// </summary>
        public object IdentityValue { get; set; }
    }

    /// <summary>
    /// 主键属性
    /// </summary>
    public class PrimaryKeyAttribute : Attribute
    {
        public string Name { get; set; }
    }

    /// <summary>
    /// Base class for entities
    /// </summary>
    [Serializable]
    public abstract class BaseEntity : IEntityObject
    {
        ///// <summary>
        ///// 实体的主键名称
        ///// </summary>
        //private  string Entity_Identity_FiledName = "Id";
        /// <summary>
        /// 创建实体的时候，一个注册表，用来表示已经设置过值的属性 字典
        /// </summary>

        internal Dictionary<string, object> __HAS_SET_VALUE_PROPERTY_DIC;

        internal EntityIdentity __ENTITY_IDENTITY;
        public BaseEntity()
        {
            __HAS_SET_VALUE_PROPERTY_DIC = new Dictionary<string, object>();
            this.PropertyChanged -= OnBaseEntity_PropertyChangedHandler;
            this.PropertyChanged += OnBaseEntity_PropertyChangedHandler;
        }



        #region 数据更新事件通知


        private event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChange(string propertyName, object value)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new EntityPropertyChangedEventArgs(propertyName, value));
            }
        }
        #endregion

 



        /// <summary>
        /// 属性变更的时候 触发事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBaseEntity_PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            var agrs = e as EntityPropertyChangedEventArgs;
            if (null != agrs)
            {
                this.AddOrChangeSettedValuePropertyFromDic(agrs.PropertyName, agrs.Value);
            }

        }


        #region 实现主键类型的访问控制


        /// <summary>
        /// 实体的唯一标识  ，如果是实体表  那么此字段必须是-有默认值的主键
        /// </summary>
        public object GetIdentityValue()
        {
            var entityIdentity = this.GetIdentity();
            var identityName = entityIdentity.IdentityKeyName;
            if (__HAS_SET_VALUE_PROPERTY_DIC.ContainsKey(identityName))
            {
                return __HAS_SET_VALUE_PROPERTY_DIC[identityName];
            }
            return null;
        }


        /// <summary>
        /// 获取主键
        /// </summary>
        /// <returns></returns>
        public EntityIdentity GetIdentity()
        {
            if (null==this.__ENTITY_IDENTITY)
            {
                __ENTITY_IDENTITY = new EntityIdentity();

                var targetAttributes = this.GetType().GetCustomAttributes(typeof(PrimaryKeyAttribute), false);
                if (null == targetAttributes||targetAttributes.Length<=0)
                {
                    throw new Exception("the model class has not set PrimaryKeyAttribute!");
                }
                string name  = (targetAttributes[0] as PrimaryKeyAttribute).Name;
                __ENTITY_IDENTITY.IdentityKeyName = name;


            }
            return this.__ENTITY_IDENTITY;
        }

        #endregion


        #region 实现对设置过值的属性对象进行注册管理

        //1 添加注册
        //2 移除注册
        //3 获取注册表-属性值对

        /// <summary>
        /// 添加注册
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public void AddOrChangeSettedValuePropertyFromDic(string propertyName, object value)
        {
            if (__HAS_SET_VALUE_PROPERTY_DIC.ContainsKey(propertyName))
            {
                __HAS_SET_VALUE_PROPERTY_DIC[propertyName] = value;
            }
            else
            {
                __HAS_SET_VALUE_PROPERTY_DIC.Add(propertyName, value);
            }
        }

        /// <summary>
        /// 移除注册
        /// </summary>
        /// <param name="propertyName"></param>
        public void RemoveSettedValuePropertyFromDic(string propertyName)
        {
            if (__HAS_SET_VALUE_PROPERTY_DIC.ContainsKey(propertyName))
            {
                __HAS_SET_VALUE_PROPERTY_DIC.Remove(propertyName);
            }
        }

        /// <summary>
        /// 获取注册表-属性值对
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetSettedValuePropertyDic()
        {
            return this.__HAS_SET_VALUE_PROPERTY_DIC;
        }

        #endregion


        /// <summary>
        /// 虚属性--当前实体的属性数组(注：实现此抽象类的实体类，必须重写此属性)
        /// </summary>
        public abstract PropertyInfo[] GetCurrentEntityProperties();

        public override bool Equals(object obj)
        {
            return Equals(obj as BaseEntity);
        }

        private static bool IsTransient(BaseEntity obj)
        {
            return obj != null && Equals(obj.GetIdentityValue(), default(int));
        }

        private Type GetUnproxiedType()
        {
            return GetType();
        }

        public virtual bool Equals(BaseEntity other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (!IsTransient(this) &&
                !IsTransient(other) &&
                Equals(this.GetIdentityValue(), other.GetIdentityValue()))
            {
                var otherType = other.GetUnproxiedType();
                var thisType = GetUnproxiedType();
                return thisType.IsAssignableFrom(otherType) ||
                        otherType.IsAssignableFrom(thisType);
            }

            return false;
        }

        public override int GetHashCode()
        {
            if (Equals(this.GetIdentityValue(), default(int)))
                return base.GetHashCode();
            return this.GetIdentityValue().GetHashCode();
        }

        public static bool operator ==(BaseEntity x, BaseEntity y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(BaseEntity x, BaseEntity y)
        {
            return !(x == y);
        }



        /// <summary>
        /// 实现实体的深度克隆（使用二进制序列化进行对象的序列化到流，然后再进行反序列化操作
        /// 对象必须是声明：Serializable
        /// ）
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            IFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            bf.Serialize(ms, this);

            ms.Seek(0, SeekOrigin.Begin);

            var obj = bf.Deserialize(ms);

            ms.Flush();
            ms.Close();
            ms.Dispose();

            return obj;
        }
    }


}
