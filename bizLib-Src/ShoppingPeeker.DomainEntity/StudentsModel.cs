using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ShoppingPeeker.DbManage;
using ShoppingPeeker.DbManage.Utilities;

namespace ShoppingPeeker.DomainEntity
{
    [TableAttribute(Name = "students")]
    [PrimaryKey(Name = "Id")]
    [Serializable]
    public class StudentsModel : BaseEntity
    {

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
        private Nullable<int> _Age;
        [DataMember]
        public Nullable<int> Age
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
        private Nullable<bool> _Sex;
        [DataMember]
        public Nullable<bool> Sex
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
        private Nullable<DateTime> _AddTime;
        [DataMember]
        public Nullable<DateTime> AddTime
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
        private Nullable<decimal> _Score;
        [DataMember]
        public Nullable<decimal> Score
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
        private Nullable<double> _Longitude;
        [DataMember]
        public Nullable<double> Longitude
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
        private Nullable<double> _Latitude;
        [DataMember]
        public Nullable<double> Latitude
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
        private Nullable<decimal> _HasPay;
        [DataMember]
        public Nullable<decimal> HasPay
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
        private Nullable<int> _HomeNumber;
        [DataMember]
        public Nullable<int> HomeNumber
        {
            get { return _HomeNumber; }
            set
            {
                _HomeNumber = value;
                NotifyPropertyChange("HomeNumber", value);
            }
        }

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
/*------ 代码生成时出现错误: ------
D:\LearningManage\tools--代码生成\Codematic2\Template\TemplateFile\LearningManage.cmt\DomainEntity.cmt(0,0) : warning CS1668: Compiling transformation: “LIB 环境变量”中指定的搜索路径“%LIB%”无效 --“系统找不到指定的路径。 ”
*/
