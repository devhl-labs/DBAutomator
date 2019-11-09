﻿using System;
using System.Data;
using static DBAutomatorStandard.Enums;

namespace DBAutomatorStandard
{
    public class QueryOptions
    {
        public string ConnectionString { get; set; } = string.Empty;

        public TimeSpan SlowQueryWarning { get; set; } = TimeSpan.FromSeconds(5);

        public IDbTransaction? DbTransaction { get; set; }

        public int? CommandTimeOut { get; set; }

        public DataStore DataStore { get; set; } = DataStore.PostgreSQL;
    }
}
