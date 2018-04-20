using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;

using ShoppingPeeker.DomainEntity;
using ShoppingPeeker.DbManage;
using ShoppingPeeker.Data.Repository;
using ShoppingPeeker.Utilities.Interface;

namespace ShoppingPeeker.BusinessServices
{
    public class StudentsService : BaseService, IBusinessBaseService
    {
        #region 属性集合

        #endregion

        #region   字段集合
        private StudentsRepository dal_students;
        #endregion

        #region  构造函数

        public StudentsService()
        {
            this.dal_students = Single<StudentsRepository>();
        }

        #endregion

        #region   业务方法


        #region  Insert操作

        /// <summary>
        /// 添加单个StudentsModel对象方法(可返回对应数据表中 的此实体ID)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public long AddOneStudentsModel(StudentsModel entity)
        {
            long result;

            try
            {

                //事务代码-附加模式
                using (var tran = new TransactionScope())//TransactionScopeOption.
                {
                    var entityID = dal_students.Insert(entity);
                    result = entityID;

                    tran.Complete();
                }

                //------------db  事务----
                //using (var conn=DatabaseFactory.GetDbConnection())
                //{
                //    if (conn.State!= System.Data.ConnectionState.Open)
                //    {
                //        conn.Open();

                //    }

                //    var tran = conn.BeginTransaction();

                //    var entityID = dal_students.Insert(entity,tran);
                //    result = entityID;

                //    tran.Commit();
                //}

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// 批量插入StudentsModel对象方法(不能返回对应数据表中 的ID)
        /// </summary>
        /// <param name="entities"></param>
        /// <returns>返回操作结果</returns>
        public bool AddMulitiStudentsModels(IEnumerable<StudentsModel> entities)
        {
            var result = false;
            try
            {
                result = dal_students.InsertMulitiEntities(entities);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }


        #endregion


        #region Update 更新操作
        /// <summary>
        /// 更新单个StudentsModel实体模型
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool UpdateOneStudentsModel(StudentsModel entity)
        {
            var result = false;

            try
            {
                result = dal_students.Update(entity) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 更新StudentsModel元素 通过  符合条件的
        /// </summary>
        /// <param name="entity">携带值的载体</param>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public bool UpdateStudentsModelsByCondition(StudentsModel entity, Expression<Func<StudentsModel, bool>> predicate)
        {
            var result = false;

            try
            {
                result = dal_students.UpdateByCondition(entity, predicate) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        #endregion


        #region Select   查询操作
        /// <summary>
        /// 通过主键获取单个StudentsModel元素
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public StudentsModel GetstudentsElementById(long id)
        {
            StudentsModel result = null;

            try
            {
                result = dal_students.GetElementById(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }



        /// <summary>
        /// 通过特定的条件查询出StudentsModel元素集合
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<StudentsModel> GetstudentsElementsByCondition(Expression<Func<StudentsModel, bool>> predicate)
        {
            List<StudentsModel> result = null;

            try
            {
                result = dal_students.GetElementsByCondition(predicate);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }


        /// <summary>
        /// 分页获取元素集合
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="totalRecords">总记录数</param>
        /// <param name="totalPages">总页数</param>
        /// <param name="predicate">条件</param>
        /// <param name="sortField">排序字段（如果不指定排序字段 那么默认按照id 排序）</param>
        /// <param name="rule">排序规则</param>
        /// <returns></returns>
        /// <returns></returns>
        public List<StudentsModel> GetstudentsElementsByPagerAndCondition(int pageIndex, int pageSize, out int totalRecords, out int totalPages, Expression<Func<StudentsModel, bool>> predicate, string sortField = null, OrderRule rule = OrderRule.ASC)
        {
            List<StudentsModel> result = null;

            try
            {
                result = dal_students.GetElementsByPagerAndCondition(pageIndex, pageSize, out totalRecords, out totalPages, predicate, sortField, rule);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }


        #endregion


        #region Delete 删除操作
        /// <summary>
        /// 删除一个StudentsModel实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool DeleteOneStudentsModel(StudentsModel entity)
        {
            var result = false;

            try
            {
                result = dal_students.Delete(entity) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }


        // <summary>
        /// 删除符合条件的StudentsModel实体
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool DeleteMulitistudentsByCondition(Expression<Func<StudentsModel, bool>> predicate)
        {
            var result = false;

            try
            {
                result = dal_students.DeleteByCondition(predicate) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        #endregion

        #endregion

    }
}