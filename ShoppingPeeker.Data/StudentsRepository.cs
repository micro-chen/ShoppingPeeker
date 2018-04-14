using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;

using ShoppingPeeker.DbManage;
using ShoppingPeeker.DomainEntity;
using ShoppingPeeker.Utilities.Interface;

namespace ShoppingPeeker.Data.Repository
{
    public class StudentsRepository : BaseRepository<StudentsModel>, IDbContext<StudentsModel>, IRepository
    {
        public StudentsRepository()
        { }

       
    }
}