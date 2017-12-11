
using System;
using System.Collections.Generic;
using ShoppingPeeker.Core;
using ShoppingPeeker.Core.Interface;
using ShoppingPeeker.Core.Ioc;

namespace ShoppingPeeker.BusinessServices
{
    public class BaseService
    {

        /// <summary>
        /// 返回数据仓储的单例模式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Single<T>() where T : IRepository, new()
        {
            if (null == Singleton<T>.Instance)
            {
                Singleton<T>.Instance = new T();
            }
            return Singleton<T>.Instance;

        }


    }
}
