﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DBAutomatorLibrary
{
    internal interface IInsertQuery<I, C>
    {
        //Expression<Func<C, object>> Collection { get; }
        //List<ConditionModel> ConditionModels { get; }

        //Task<List<I>> GetListAsync();
        //Task<I> GetAsync();
        Task InsertAsync();
    }
}