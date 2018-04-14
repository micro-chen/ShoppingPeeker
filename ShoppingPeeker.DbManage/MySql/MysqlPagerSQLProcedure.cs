using System;
using MySql.Data.MySqlClient;
using ShoppingPeeker.DbManage.Utilities;
using ShoppingPeeker.Utilities;

namespace ShoppingPeeker.DbManage
{

    /// <summary>
    /// 检测是否存在需要的分页存储过程
    /// </summary>
    public static class MySqlPagerSQLProcedure
    {

        //检查存储过程存在--命令
        private static readonly string Cmd_Check_Target_Procedure = @"select count(1) from mysql.proc
        where db = '{0}'
        and `type` = 'PROCEDURE'
        and `name`='{1}'";


        internal static void CheckAndCreatePagerSQLProcedure(DbConnConfig dbConfig)
        {
            var connStr = dbConfig.ConnString;

            //1检查数据库是否存在存储过程   //2创建
            //分页调用入口
            var isHasExist = CheckIsHasExistProcedure(connStr, Contanst.PageSql_Call_Name);
            if (isHasExist == true)
            {
                return;
            }
            CreateSQLProcedure(connStr, PageSql_Call_MySqlCommand);

        }

        /// <summary>
        /// 检查指定的存储过程是否存在
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="procName"></param>
        /// <returns></returns>
        public static bool CheckIsHasExistProcedure(string connStr, string procName)
        {
            var result = false;

            //这里不执行异常的输出！
            try
            {

                using (var conn = new MySqlConnection(connStr))
                {
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        conn.Open();
                    }

                    var dbName = conn.Database;
                    if (string.IsNullOrEmpty(dbName))
                    {
                        throw new Exception("数据库实例不能为空！");
                    }

                    var sqlCmd = string.Format(Cmd_Check_Target_Procedure, dbName, procName);

                    var cmd = new MySqlCommand(sqlCmd, conn);
                    cmd.CommandType = System.Data.CommandType.Text;
                    result = Convert.ToInt32(cmd.ExecuteScalar()) > 0 ? true : false;

                }

            }
            catch
            {
            }

            return result;
        }



        private static void CreateSQLProcedure(string connStr, string MySqlCommand)
        {
            if (string.IsNullOrEmpty(connStr))
            {
                throw new Exception("数据库连接异常，请检查连接字符串的配置！");
            }



            using (var conn = new MySqlConnection(connStr))
            {
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.Open();
                }
                MySqlScript cmdScript = new MySqlScript(conn);
                cmdScript.Query = MySqlCommand;
                cmdScript.Delimiter = "??";//设定结束符

                int result = cmdScript.Execute();
            }
        }

        #region mysql  分页存储过程

        //调用入口sql存储过程
        private static string PageSql_Call_MySqlCommand = @"DROP PROCEDURE  IF EXISTS `DbManage_GetRecordsByPage`??

                            CREATE PROCEDURE DbManage_GetRecordsByPage(
                                PageIndex INT,  #查询页索引
                                PageSize INT,   #每页记录数 
                                TableName varchar(4000),  #要查询的表  
                                SelectFields  VARCHAR(1000), #要查询的字段，用逗号(,)分隔   
                                PrimaryKey  varchar(200),#主键
                                ConditionWhere VARCHAR(2000),   #查询条件  
                                SortField VARCHAR(200),  #排序规则
                                IsDesc INT,#0升序 1降序  
  
                                #输出参数  
                                OUT TotalRecords INT,  #总记录数  
                                OUT TotalPageCount INT    #总页数  
                                )
                            BEGIN
                             #140529-xxj-分页存储过程  
                                #计算起始行号  
                                SET @startRow = PageSize * PageIndex;  
                                SET @pageSize = PageSize;  
                                SET @rowindex = 0; #行号  
  
                                #合并字符串  
                                SET @strsql = CONCAT(  
                                    #'select sql_calc_found_rows  @rowindex:=@rowindex+1 as rownumber,' #记录行号  
                                    'select sql_calc_found_rows '  
                                    ,SelectFields   
                                    ,' from '  
                                    ,TableName  
                                    ,CASE IFNULL(ConditionWhere, '') WHEN '' THEN '' ELSE CONCAT(' where ', ConditionWhere) END  
                                    ,CASE IFNULL(SortField, '') WHEN '' THEN CONCAT(' order by ', PrimaryKey) ELSE CONCAT(' order by ', SortField) END  
                                    ,case IsDesc when 0 then concat(' ASC') when 1 then concat(' DESC') else('') end
                                  ,' limit '   
                                    ,@startRow  
                                    ,','   
                                    ,@pageSize  
                                );  
  
                                PREPARE strsql FROM @strsql;#定义预处理语句   
                                EXECUTE strsql;                         #执行预处理语句   
                                DEALLOCATE PREPARE strsql;  #删除定义   
                                #通过 sql_calc_found_rows 记录没有使用 limit 语句的记录，使用 found_rows() 获取行数  
                                SET TotalRecords = FOUND_ROWS();  
  
                                #计算总页数  
                                IF (TotalRecords <= PageSize) THEN  
                                    SET TotalPageCount = 1;  
                                ELSE IF (TotalRecords % PageSize > 0) THEN  
                                    SET TotalPageCount = TotalRecords div PageSize+1;  
                                ELSE  
                                    SET TotalPageCount = TotalRecords div PageSize;  
                                END IF;  
                                END IF;  
  
                                END??
";

        #endregion

    }
}
