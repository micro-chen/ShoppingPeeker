# ShoppingPeeker
这个项目是蜘蛛项目的可视化任务站点。

<img src="https://images2018.cnblogs.com/blog/371989/201805/371989-20180514184659338-1797885673.jpg" alt="" />
# 概述
ShoppingPeeker 是项目：<a href='https://github.com/micro-chen/ShoppingWebCrawler'>ShoppingWebCrawler</a>的可视化任务工具。   

项目使用.net core2.x进行构建。可以运行在Windwos/Linux/Mac平台。  

项目采用Socket通信模式，实现本地采集任务与蜘蛛服务器进行通信。稳定高效。  


# 如何部署？
1、克隆ShoppingWebCrawler项目到本地。如：d:\src\ShoppingWebCrawler；  

2、克隆ShoppingPeeker项目到本地。d:\src\ShoppingPeeker;  

（注意：两个项目必须在同一个目录下，因为使用了文件编译引用！！！！）
  
3、启动蜘蛛 ShoppingWebCrawler。
  
4、使用visual studio 2017 
   或者cmd/powershell 进入项目文件夹
   使用命令：dotnet build;dotnet run
  
5、恭喜，项目成功启动，示范站点的输入框，是一个根据输入的商品词，抓取对应电商平台的商品列表的功能示范！
  
# 项目构成
ShoppingPeeker.Web: .net core 2.x asp.net mvc 站点
插件模式：
不同的电商平台使用插件模式进行采集任务的解析。在站点启动的时候，扫描插件目录。对插件进行附加，并监视查询的变更！
# 数据持久化
基于Dapper的数据交互DataAccess.封装对Linq 方式，实现对原始表数据的 增删改查操作。

# TCP 进行蜘蛛通信示范
封装TCP通信，基于ADP.NET的连接方式，上手简单容易，可配置。  


基本通信：  

 var connStr = ConfigHelper.WebCrawlerSection.ConnectionStringCollection.First();  
 
                using (var conn = new SoapTcpConnection(connStr))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    ///发送ping

                    var str = conn.SendString(CommandConstants.CMD_Ping);
                    resut = string.Concat("time :", DateTime.Now.ToString(), "; tcp server response: ", str);
                }

  
  
采集请求：  

 var connStrConfig = webArgs.SystemAttachParas["SoapTcpConnectionString"] as WebCrawlerConnection;

                        //重写解析地址-首页的分片jsonp地址
                        string urlOfSlicedJsonp = this.ResolveSlicedSearchPageSilcedUrl(webArgs, next_start, show_items);
                        webArgs.ResolvedUrl = new ResolvedSearchUrlWithParas { Url = urlOfSlicedJsonp };
                        using (var conn = new SoapTcpConnection(connStrConfig))
                        {
                            if (conn.State == ConnectionState.Closed)
                            {
                                conn.Open();
                            }

                            //发送soap
                            var soapCmd = new SoapMessage() { Head = CommandConstants.CMD_FetchPage };
                            soapCmd.Body = JsonConvert.SerializeObject(webArgs);
                            var dataContainer = conn.SendSoapMessage(soapCmd);
                            if (null != dataContainer && dataContainer.Status == 1)
                            {
                                htmlItemsContent = dataContainer.Result;
                            }
                            else
                            {
                                StringBuilder errMsg = new StringBuilder("抓取网页请求失败！参数：");
                                errMsg.Append(soapCmd.Body);
                                if (null != dataContainer && !string.IsNullOrEmpty(dataContainer.ErrorMsg))
                                {
                                    errMsg.Append("；服务端错误消息：")
                                        .Append(dataContainer.ErrorMsg);
                                }
                                PluginContext.Logger.Error(errMsg.ToString());
                            }
                        }



   
   
   
# 数据交互示范
  /// <summary>
        /// 增加
        /// </summary>
        [TestMethod()]
        public void AddOneStudentsModelTest()
        {
         

            var model = new StudentsModel
            {

                Name = "你猜猜-" + DateTime.Now.ToString(),
                Age = DateTime.Now.Second,
                Sex = true,
                Score = 55.98m,
                Longitude = 555555.6666,
                AddTime = DateTime.Now
            };
           
            var result = serviceOfStudents.AddOneStudentsModel(model);


            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

             model = new StudentsModel
            {

                Name = "你猜222222222猜-" + DateTime.Now.ToString(),
                Age = DateTime.Now.Second,
                Sex = true,
                Score =6655.98m,
                Longitude = 99999999,
                AddTime = DateTime.Now
            };

             result = serviceOfStudents.AddOneStudentsModel(model);


            watch.Stop();

            Console.WriteLine(string.Format("real for insert one data use time  is :{0} ms.", watch.ElapsedMilliseconds));




            Assert.IsTrue(result > 0);
        }

        /// <summary>
        /// 批量增加
        /// </summary>

        [TestMethod()]
        public void AddMulitiStudentsModelsTest()
        {
            var lstData = new List<StudentsModel>();
            var rand = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i < 100; i++)
            {
                var model = new StudentsModel
                {
                    Name = "你猜猜-" + Guid.NewGuid().ToString(),
                    Age = rand.Next(1, 100),
                    Sex = false,
                    Score = 33355.98m,
                    Longitude = 59595959,
                    AddTime = DateTime.Now
                };
                lstData.Add(model);
            }
            var result = serviceOfStudents.AddMulitiStudentsModels(lstData);
            Assert.IsTrue(1 == 1);

        }

        /// <summary>
        /// 更新数据实体-by主键
        /// </summary>

        [TestMethod()]
        public void UpdateOneStudentsModelTest()
        {
            var model = new StudentsModel
            {
                Id = 1,
                Age = 100
            };

            var result = serviceOfStudents.UpdateOneStudentsModel(model);
            Assert.IsTrue(result);
        }




        /// <summary>
        /// 条件更新
        /// 多个条件 and 
        /// 
        /// </summary>

        [TestMethod()]
        public void UpdateStudentsModelsByConditionTest()
        {
            var model = new StudentsModel
            {
                Age = 333
            };
            var result = serviceOfStudents.UpdateStudentsModelsByCondition(
                model,
                x => x.Id > 0 && x.Name.Contains("你猜猜%"));

            Assert.IsTrue(result);

        }


        /// </summary>
        [TestMethod()]
        public void GetstudentsElementByIdTest()
        {

            var model = this.serviceOfStudents
                .GetstudentsElementById(1);

            Assert.IsTrue(null!= model);

          
        }

        /// <summary>
        /// 条件获取
        /// 或

        /// </summary>
        [TestMethod()]
        public void GetfStudentsElementsByConditionTest()
        {

            var lstData = this.serviceOfStudents
                .GetstudentsElementsByCondition(
                x => x.Id == 1 || x.Name.Contains("你猜猜%")
                );//(x => x.PubSubWsAddr.LenFuncInSql() > 0);

            Assert.IsTrue(lstData.Count > 0);

            lstData = this.serviceOfStudents
                .GetstudentsElementsByCondition(null);

            Assert.IsTrue(lstData.Count > 0);
        }
        /// <summary>
        /// 条件删除 
        /// </summary>
        [TestMethod()]
        public void DeleteMulitiservicesAddressByConditionTest()
        {
            //var result = this.serviceOfStudents
            //    .DeleteMulitiservicesAddressByCondition(x => x.PubSubWsAddr.LenFuncInSql() > 0);

            var result = this.serviceOfStudents
               .DeleteMulitistudentsByCondition(
                  x => x.Id == 1 || x.Name.Contains("你猜猜%")
            );
            Assert.IsTrue(result);
        }


        //多个查询条件构建 （使用Lambda表达式构建 进行条件body的合并）

        [TestMethod()]
        public void GetByMultipleConditionsTest()
        {
            //组合条件
            var predicate = PredicateBuilder.CreatNew<StudentsModel>();
           
            string id = "55";
            if (!string.IsNullOrEmpty(id) && id.ToInt() > 0)
            {
                predicate = predicate.And(s => s.Id <= id.ToInt());
            }

            //开始组合表达式body
            predicate = predicate.Or(s => s.Name.Contains("你猜猜-2%"));


            var model = this.serviceOfStudents.GetstudentsElementsByCondition(predicate);

            Assert.IsNotNull(model);

        }


        [TestMethod()]
        public void GetStudentsModelsElementsByPagerAndConditionTest()
        {
            var pageSize = 10;
            var pageIndex = 0;
            var totalRecords = -1;
            var totalPages = -1;

            var lstData = this.serviceOfStudents
               .GetstudentsElementsByPagerAndCondition(pageIndex,
               pageSize,
               out totalRecords,
               out totalPages, x => x.Id > 0,//PubSubWsAddr.LenFuncInSql() 
               "Id",
               OrderRule.DESC);
            Assert.IsTrue(lstData.Count > 0);


        }


    
# 联系作者
MyBlog:http://www.cnblogs.com/micro-chen/
<br/>
DotNET Core技术群: 59196458
# 赞助作者
一个好的项目离不开大家的支持，您的赞助，将给我更加充沛的动力。
<br/>
<br/>
<img src="https://images2018.cnblogs.com/blog/371989/201805/371989-20180514183954632-2054296110.jpg" alt="" />


