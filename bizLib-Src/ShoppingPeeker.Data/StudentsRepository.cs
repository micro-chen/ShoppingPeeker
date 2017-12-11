using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using ShoppingPeeker.Core.Interface;
using ShoppingPeeker.DbManage;
using ShoppingPeeker.DomainEntity;





namespace ShoppingPeeker.Data.Repository
{
    public class StudentsRepository : BaseRepository<StudentsModel>, IDbContext<StudentsModel>, IRepository
    {
        public StudentsRepository()
        { }

    }
}