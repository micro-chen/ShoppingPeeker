
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

        private const string Default_ConnName = "Default";
        /// <summary>
        /// 构造数据仓储对象
        /// </summary>
        /// <param name="connName">连接字符串Name，默认为：Default；为空将返回默认第一个数据库连接</param>
        public BaseRepository(string connName= Default_ConnName)
        {
            this.dbContext = GetDbContext(connName);
        }


        /// <summary>
        /// /// <summary>
        /// 获取当前数据库上下文
        /// 根据写库的类型进行数据库类型的判断
        /// 支持多数据库类型-工厂拆分
        /// </summary>
        /// </summary>
        /// <param name="connName"></param>
        /// <returns></returns>
        protected static IDbContext<TElement> GetDbContext(string connName = Default_ConnName)
        {
            IDbContext<TElement> dbContext = null;

            DbConnConfig dbConfig = null;
            if (string.IsNullOrEmpty(connName))
            {
                //必须有连接配置，如果没有 那么抛出异常
                dbConfig = GlobalDBConnection.AllDbConnConfigs.FirstOrDefault().Value;
            }else
            {
                //检测是否有name
                if (!GlobalDBConnection.AllDbConnConfigs.ContainsKey(connName))
                {
                    throw new Exception("指定的数据库连接名称不存在配置中！Name："+connName);
                }
                dbConfig = GlobalDBConnection.AllDbConnConfigs[connName];

            }

            switch (dbConfig.DbType)
            {
                case SupportDbType.SQLSERVER:
                    dbContext = new SqlDbContext<TElement>(dbConfig);
                    break;
                case SupportDbType.MYSQL:
                    dbContext = new MySqlDbContext<TElement>(dbConfig);
                    break;
                case SupportDbType.POSTGRESQL:
                case SupportDbType.ORACLE:
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
        public int Insert(TElement entity)
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

        /// <summary>
        /// 批量执行SQL命令
        /// </summary>
        /// <param name="SqlCmdList"></param>
        /// <returns></returns>
        public bool SqlBatchExcute(Dictionary<string, DbParameter[]> SqlCmdList)
        {
            return this.dbContext.SqlBatchExcute(SqlCmdList);
        }



        #endregion

        #region base ado.net 

        /// <summary>
        /// 【读】执行查询 返回 DataSet
        /// </summary>
        /// <returns></returns>
        public DataSet ExecuteDataSet(string cmdText, DbParameter[] commandParameters = null, CommandType cmdType = CommandType.Text)
        {
            return this.dbContext.ExecuteDataSet(cmdText, commandParameters, cmdType);
        }

        public DbDataReader ExecuteReader(string cmdText, DbParameter[] commandParameters = null, CommandType cmdType = CommandType.Text)
        {
            return this.dbContext.ExecuteReader(cmdText, commandParameters, cmdType);
        }

        public object ExecuteScalar(string cmdText, DbParameter[] commandParameters = null, CommandType cmdType = CommandType.Text)
        {
            return this.dbContext.ExecuteScalar(cmdText, commandParameters, cmdType);
        }

        public int ExecuteNonQuery(string cmdText, DbParameter[] commandParameters = null, CommandType cmdType = CommandType.Text)
        {
            return this.dbContext.ExecuteNonQuery(cmdText, commandParameters, cmdType);
        }

        public List<TElement> SqlQuery(string commandText, CommandType commandType, params DbParameter[] parameters)
        {
            return this.dbContext.SqlQuery(commandText, commandType, parameters);

        }

        #endregion
    }
}
