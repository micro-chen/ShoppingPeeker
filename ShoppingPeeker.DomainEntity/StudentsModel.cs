using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Dapper.Contrib.Extensions;
using ShoppingPeeker.DbManage;
using ShoppingPeeker.DbManage.Utilities;

namespace ShoppingPeeker.DomainEntity
{
    [Table("students")]
    [PrimaryKey(Name = "Id")]
    [Serializable]
    public class StudentsModel : BaseEntity
    {
        #region 表字段属性

        ///// <summary>
        /// auto_increment
        /// </summary>	
        private long _Id;
        [DataMember]
      
        public long Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
                NotifyPropertyChange("Id", value);
            }
        }
        ///// <summary>
        /// Age
        /// </summary>	
        private int? _Age;
        [DataMember]
        public int? Age
        {
            get { return _Age; }
            set
            {
                _Age = value;
                NotifyPropertyChange("Age", value);
            }
        }
        ///// <summary>
        /// Name
        /// </summary>	
        private string _Name;
        [DataMember]
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                NotifyPropertyChange("Name", value);
            }
        }
        ///// <summary>
        /// Sex
        /// </summary>	
        private bool? _Sex;
        [DataMember]
        public bool? Sex
        {
            get { return _Sex; }
            set
            {
                _Sex = value;
                NotifyPropertyChange("Sex", value);
            }
        }
        ///// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>	
        private DateTime? _AddTime;
        [DataMember]
        public DateTime? AddTime
        {
            get { return _AddTime; }
            set
            {
                _AddTime = value;
                NotifyPropertyChange("AddTime", value);
            }
        }
        ///// <summary>
        /// Score
        /// </summary>	
        private decimal? _Score;
        [DataMember]
        public decimal? Score
        {
            get { return _Score; }
            set
            {
                _Score = value;
                NotifyPropertyChange("Score", value);
            }
        }
        ///// <summary>
        /// Longitude
        /// </summary>	
        private double? _Longitude;
        [DataMember]
        public double? Longitude
        {
            get { return _Longitude; }
            set
            {
                _Longitude = value;
                NotifyPropertyChange("Longitude", value);
            }
        }
        ///// <summary>
        /// Latitude
        /// </summary>	
        private double? _Latitude;
        [DataMember]
        public double? Latitude
        {
            get { return _Latitude; }
            set
            {
                _Latitude = value;
                NotifyPropertyChange("Latitude", value);
            }
        }
        ///// <summary>
        /// HasPay
        /// </summary>	
        private decimal? _HasPay;
        [DataMember]
        public decimal? HasPay
        {
            get { return _HasPay; }
            set
            {
                _HasPay = value;
                NotifyPropertyChange("HasPay", value);
            }
        }
        ///// <summary>
        /// HomeNumber
        /// </summary>	
        private int? _HomeNumber;
        [DataMember]
        public int? HomeNumber
        {
            get { return _HomeNumber; }
            set
            {
                _HomeNumber = value;
                NotifyPropertyChange("HomeNumber", value);
            }
        }


        #endregion

        #region 导航属性+字段

        [IgnoreDbField]
        [Write(false)]
        public int Other_Id { get; set; }


        #endregion

        #region  当前实体的属性/映射到表中的字段 CLR属性集合
        private static System.Reflection.PropertyInfo[] _CurrentModelPropertys;


        [IgnoreDataMember]
        public static System.Reflection.PropertyInfo[] CurrentModelPropertys
        {
            get
            {
                if (null == _CurrentModelPropertys)
                {
                    var propLst = typeof(StudentsModel).GetProperties();//this.GetType().GetProperties();
                    var specilProperty = propLst.Where(x => !DbTypeAndCLRType.EntityPropertyClrTypeIsCollentionType(x));
                    _CurrentModelPropertys = specilProperty.ToArray();
                }

                return _CurrentModelPropertys;
            }
        }
        /// <summary>
        /// 获取当前实体类的属性集合-暴漏给外部调用的方法
        /// </summary>
        /// <returns></returns>
        public override System.Reflection.PropertyInfo[] GetCurrentEntityProperties()
        {
            return CurrentModelPropertys;
        }
        #endregion
    }
}
