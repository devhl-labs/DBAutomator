﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Dapper;
using Npgsql;

using static devhl.DBAutomator.PostgresMethods;
using devhl.DBAutomator.Interfaces;

namespace devhl.DBAutomator
{
    internal class PostgresSelectQuery <C> : ISelectQuery<C>
    {
        private readonly DBAutomator _dBAutomator;
        private readonly QueryOptions _queryOptions;
        private readonly ILogger? _logger;

        public PostgresSelectQuery(DBAutomator dBAutomator, QueryOptions queryOptions, ILogger? logger = null)
        {
            _dBAutomator = dBAutomator;
            _queryOptions = queryOptions;
            _logger = logger;
        }

        public async Task<IEnumerable<C>> GetAsync(Expression<Func<C, object>>? where = null, OrderByClause<C>? orderBy = null)
        {
            RegisteredClass<C> registeredClass = (RegisteredClass<C>) _dBAutomator.RegisteredClasses.First(r => r is RegisteredClass<C>);

            List<ExpressionModel<C>> expressions = new List<ExpressionModel<C>>();

            BinaryExpression? binaryExpression = GetBinaryExpression(where);

            GetExpressions(binaryExpression, expressions, registeredClass);

            string sql = $"SELECT";

            foreach(var property in registeredClass.RegisteredProperties)
            {
                sql = $"{sql} \"{property.ColumnName}\",";
            }

            sql = sql[0..^1];

            sql = $"{sql} FROM \"{registeredClass.TableName}\"";

            if (where != null)
            {
                sql = $"{sql} WHERE {where.GetWhereClause(expressions)}";
            }

            if (orderBy != null)
            {
                sql = $"{sql} {orderBy.GetOrderByClause()}";
            }

            sql = $"{sql};";

            _logger.LogTrace(sql);

            DynamicParameters p = GetDynamicParametersFromExpression(expressions);

            using NpgsqlConnection connection = new NpgsqlConnection(_queryOptions.ConnectionString);

            await connection.OpenAsync().ConfigureAwait(false);

            Stopwatch stopwatch = StopWatchStart();

            var result = await connection.QueryAsync<C>(sql, p, _queryOptions.DbTransaction, _queryOptions.CommandTimeOut).ConfigureAwait(false);

            StopWatchEnd(stopwatch, "GetAsync()");

            foreach(var item in result)
            {
                if (item is IDBObject dbObjectLoaded)
                {
                    dbObjectLoaded.IsNew = false;

                    dbObjectLoaded.IsDirty = false;
                }

                if (item is IDBEvent dbEventLoaded)
                {
                    await dbEventLoaded.OnLoadedAsync(_dBAutomator).ConfigureAwait(false);
                }
            }

            return result;
        }

        private Stopwatch StopWatchStart()
        {
            Stopwatch result = new Stopwatch();
            result.Start();
            return result;
        }

        private void StopWatchEnd(Stopwatch stopwatch, string methodName)
        {
            stopwatch.Stop();
            if (stopwatch.Elapsed > _queryOptions.SlowQueryWarning)
            {
                _dBAutomator.SlowQueryDetected(methodName, stopwatch.Elapsed);
            }
        }
    }
}
