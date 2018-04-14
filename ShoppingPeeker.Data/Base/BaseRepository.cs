
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ShoppingPeeker.DbManage;
using System.Data.Common;
using System.Data;
using System.Linq.Expressions;

namespace ShoppingPeeker.Data
{
    public class BaseRepository<TElement> where TElement : BaseEntity, new()
    {

        protected IDbContext<TElement> dbContext;

        public BaseRepository()
        {
            this.dbContext = this.GetDbContext();
        }

        /// <summary>
        /// 获取当前数据库上下文
        /// 支持多数据库类型-工厂拆分
        /// </summary>
        /// <returns></returns>
        protected IDbContext<TElement> GetDbContext()
        {
            IDbContext<TElement> dbContext = null;

            switch (GlobalDBConnection.CurrentDbType)
            {
                case SupportDbType.Sqlserver:
                    dbContext = new SqlDbContext<TElement>();
                    break;
                case SupportDbType.Mysql:
                    dbContext = new MysqlDbContext<TElement>();
                    break;
                case SupportDbType.PostgreSQL:
                case SupportDbType.Oracle:
                default: throw new NotImplementedException();

            }

            return dbContext;
        }

        #region entity  orm 

       

        #region 查询实体


        /// <summary>
        /// id单个查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TElement GetElementById(long id)
        {
            return this.dbContext.GetElementById(id);
        }

        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>

        public List<TElement> GetElementsByCondition(Expression<Func<TElement, bool>> predicate)
        {
            return this.dbContext.GetElementsByCondition(predicate);
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
        public List<TElement> GetElementsByPagerAndCondition(int pageIndex, int pageSize, out int totalRecords, out int totalPages, Expression<Func<TElement, bool>> predicate, string sortField = null, OrderRule rule = OrderRule.ASC)
        {
            return this.dbContext.GetElementsByPagerAndCondition(pageIndex, pageSize, out totalRecords, out totalPages, predicate, sortField, rule);
        }


        #endregion

        #region 插入实体

        /// <summary>
        /// 单个插入
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public long Insert(TElement entity)
        {
            return this.dbContext.Insert(entity);
        }
        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public bool InsertMulitiEntities(IEnumerable<TElement> entities)
        {
            return this.dbContext.InsertMulitiEntities(entities);
        }
        #endregion

        #region 删除实体

        /// <summary>
        /// 单个删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Delete(TElement entity)
        {
            return this.dbContext.Delete(entity);
        }
        /// <summary>
        /// 条件删除
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int DeleteByCondition(Expression<Func<TElement, bool>> predicate)
        {
            return this.dbContext.DeleteByCondition(predicate);
        }

        #endregion

        #region 更新实体

        /// <summary>
        /// 单个更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>

        public int Update(TElement entity)
        {
            return this.dbContext.Update(entity);
        }


        /// <summary>
        /// 条件更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int UpdateByCondition(TElement entity, Expression<Func<TElement, bool>> predicate)
        {
            return this.dbContext.UpdateByCondition(entity, predicate);
        }

        #endregion

        #region 聚合函数

        /// <summary>
        /// 最大值-列
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="specialColumn"></param>
        /// <returns></returns>
        public int Max(Expression<Func<TElement, bool>> predicate, Fields<TElement> specialColumn)
        {
            return this.dbContext.Max(predicate, specialColumn);
        }

        /// <summary>
        /// 最小值-列
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="specialColumn"></param>
        /// <returns></returns>

        public int Min(Expression<Func<TElement, bool>> predicate, Fields<TElement> specialColumn)
        {
            return this.dbContext.Min(predicate, specialColumn);
        }

        /// <summary>
        /// 求和-列
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="specialColumn"></param>
        /// <returns></returns>

        public int Sum(Expression<Func<TElement, bool>> predicate, Fields<TElement> specialColumn)
        {
            return this.dbContext.Sum(predicate, specialColumn);
        }


        /// <summary>
        /// 统计计数-条件
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>

        public int Count(Expression<Func<TElement, bool>> predicate)
        {
            return this.dbContext.Count(predicate);
        }


        #endregion





        #endregion

        #region base ado.net 


        public DbDataReader ExecuteReader(string cmdText, CommandType cmdType = CommandType.Text, params DbParameter[] commandParameters)
        {
            return this.dbContext.ExecuteReader(cmdText, cmdType, commandParameters);
        }

        public  object ExecuteScalar(string cmdText, CommandType cmdType, params DbParameter[] commandParameters)
        {
            return this.dbContext.ExecuteScalar( cmdText, cmdType, commandParameters);
        }

        public  int ExecuteNonQuery(string cmdText, CommandType cmdType = CommandType.Text, params DbParameter[] commandParameters)
        {
            return this.dbContext.ExecuteNonQuery( cmdText,  cmdType,  commandParameters);
        }

        #endregion
    }
}
