﻿/*
MIT License

Copyright(c) 2018
Antonio Di Nucci

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using LogicMine.Api.Filter;
using LogicMine.Api.GetCollection;
using Microsoft.Data.Sqlite;

namespace LogicMine.Api.Data.Sqlite
{
  /// <summary>
  /// A terminal layer for dealing with T's which are mapped to a single SQLite table
  /// </summary>
  /// <typeparam name="TId">The identity type on T</typeparam>
  /// <typeparam name="T">The type which the terminals operate on</typeparam>
  public class SqliteMappedLayer<TId, T> : MappedDbLayer<TId, T, SqliteParameter>
    where T : new()
  {
    /// <summary>
    /// Construct a new SqliteMappedDbLayer
    /// </summary>
    /// <param name="connectionString">The db's connection string</param>
    /// <param name="descriptor">Metadata to enable mapping T's to database tables</param>
    /// <param name="mapper">An object-relational mapper</param>
    public SqliteMappedLayer(string connectionString, SqliteMappedObjectDescriptor<T> descriptor, IDbMapper<T> mapper)
      : base(new SqliteInterface(connectionString), descriptor, mapper)
    {
    }

    /// <summary>
    /// Get a new SqliteParameter based on the provided arguments
    /// </summary>
    /// <param name="name">The parameter name</param>
    /// <param name="value">The parameter value</param>
    /// <returns>A new SqliteParameter</returns>
    protected override SqliteParameter GetDbParameter(string name, object value)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

      return new SqliteParameter(name, value ?? DBNull.Value);
    }

    /// <inheritdoc />
    protected override IDbFilter<SqliteParameter> GetDbFilter(IFilter filter)
    {
      if (filter == null)
        throw new ArgumentNullException(nameof(filter));

      return new SqliteFilterGenerator(filter, (n) => MakeColumnNameSafe(Descriptor.GetMappedColumnName(n))).Generate();
    }

    /// <inheritdoc />
    protected override string GetSelectLastIdentityQuery()
    {
      return "SELECT last_insert_rowid();";
    }

    /// <inheritdoc />
    protected override IDbStatement<SqliteParameter> GetSelectDbStatement(IGetCollectionRequest<T> request)
    {
      var max = request.Max.GetValueOrDefault(0);
      if (max > 0)
      {
        if (request.Filter != null)
          return GetSelectDbStatement(request.Filter, max, request.Page.GetValueOrDefault(0));

        return GetSelectDbStatement(max, request.Page.GetValueOrDefault(0));
      }

      return base.GetSelectDbStatement(request);
    }

    private IDbStatement<SqliteParameter> GetSelectDbStatement(int max, int page)
    {
      var query =
        $"SELECT {GetSelectableColumns()} FROM {Descriptor.Table} WHERE {Descriptor.PrimaryKey} NOT IN (" +
        $"SELECT {Descriptor.PrimaryKey} FROM {Descriptor.Table} ORDER BY {Descriptor.PrimaryKey} LIMIT {max * page}) " +
        $"ORDER BY {Descriptor.PrimaryKey} LIMIT {max}";

      return new DbStatement<SqliteParameter>(query);
    }

    private IDbStatement<SqliteParameter> GetSelectDbStatement(IFilter<T> filter, int max, int page)
    {
      var sqlFilter = GetDbFilter(filter);

      var query =
        $"SELECT {GetSelectableColumns()} FROM {Descriptor.Table} {sqlFilter.WhereClause} AND {Descriptor.PrimaryKey} NOT IN (" +
        $"SELECT {Descriptor.PrimaryKey} FROM {Descriptor.Table} {sqlFilter.WhereClause} ORDER BY {Descriptor.PrimaryKey} LIMIT {max * page}) " +
        $"ORDER BY {Descriptor.PrimaryKey} LIMIT {max}";

      return new DbStatement<SqliteParameter>(query, sqlFilter.Parameters);
    }
  }
}
