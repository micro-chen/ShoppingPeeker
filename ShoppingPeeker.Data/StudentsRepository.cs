using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using Dapper;
using ShoppingPeeker.DbManage;
using ShoppingPeeker.DomainEntity;
using ShoppingPeeker.Utilities.Interface;

namespace ShoppingPeeker.Data.Repository
{
    public class StudentsRepository : BaseRepository<StudentsModel>, IDbContext<StudentsModel>, IRepository
    {
        public StudentsRepository()//:base("Db_SqlServer")
        {
            /*示范代码：*/
            //1 切换数据库上下文
            //this.dbContext = GetDbContext("Db_SqlServer");

            //2 dapper orm
            //var model = new StudentsModel();
            //this.Insert(model, transaction);

            // 3 ado.net
            // this.ExecuteNonQuery()

            //4 dapper 
            //using (var conn = this.GetDbConnection())
            //{
            //    conn.ExecuteScalar<int>(sql, paras);
            //} 
            
        }

       
    }
}