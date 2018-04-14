using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingPeeker.DomainEntity;
using ShoppingPeeker.Utilities;
using ShoppingPeeker.DbManage;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ShoppingPeeker.Tests
{
    public class TestBase
    {

        public TestBase()
        {
            this.Init();
        }
        public void Init()
        {
			//设置配置加载
			var builder = new ConfigurationBuilder()
			 .SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json");
			ConfigHelper.AppSettingsConfiguration =  builder.Build();

            //设置数据库连接
            InitDatabase.Init();

			//var connStringSection = ConfigHelper.GetConnectionStringSection();
			//var providerName = connStringSection.ProviderName;
			//InitDatabase.SetDatabaseConnection(connStringSection.ConnectionString, providerName);


		}

	}
}
