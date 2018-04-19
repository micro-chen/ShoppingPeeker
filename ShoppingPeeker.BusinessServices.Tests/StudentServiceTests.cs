
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ShoppingPeeker.DbManage;
using ShoppingPeeker.DomainEntity;
using ShoppingPeeker.BusinessServices;
using ShoppingPeeker.Tests;
using ShoppingPeeker.BusinessServices.Services;

using System.Linq.Expressions;



namespace ShoppingPeeker.BusinessServices.Services.Tests
{
    [TestClass()]
    public class StudentServiceTests : TestBase
    {
        private StudentsService serviceOfStudents;


        public StudentServiceTests()
        {
            this.serviceOfStudents = new StudentsService();
        }

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



    }
}