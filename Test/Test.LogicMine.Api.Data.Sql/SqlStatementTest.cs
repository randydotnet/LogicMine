﻿using System.Data;
using System.Data.SqlClient;
using LogicMine.Api.Data.Sql;
using Xunit;

namespace Test.LogicMine.Api.Data.Sql
{
  public class SqlStatementTest
  {
    [Fact]
    public void ConstructBasic()
    {
      var statement = new SqlStatement("select * from frog");
      Assert.Equal("select * from frog", statement.Text);
      Assert.Empty(statement.Parameters);
      Assert.Equal(CommandType.Text, statement.Type);
    }

    [Fact]
    public void ConstructType()
    {
      var statement = new SqlStatement("exec proc", CommandType.StoredProcedure);
      Assert.Equal("exec proc", statement.Text);
      Assert.Empty(statement.Parameters);
      Assert.Equal(CommandType.StoredProcedure, statement.Type);
    }

    [Fact]
    public void ConstructParameters()
    {
      var statement = new SqlStatement(
        "select * from frog where species like @species and colour like @colour",
        new SqlParameter("@species", "tree"), new SqlParameter("@colour", "green"));

      Assert.Equal("select * from frog where species like @species and colour like @colour", statement.Text);
      Assert.Equal(CommandType.Text, statement.Type);
      Assert.Equal(2, statement.Parameters.Length);
      Assert.Contains(statement.Parameters, p => p.ParameterName == "@species" && (string) p.Value == "tree");
      Assert.Contains(statement.Parameters, p => p.ParameterName == "@colour" && (string) p.Value == "green");
    }

    [Fact]
    public void ConstructTypeParameters()
    {
      var statement = new SqlStatement(
        "exec getfrogs @species, @colour", CommandType.StoredProcedure,
        new SqlParameter("@species", "tree"), new SqlParameter("@colour", "green"));

      Assert.Equal("exec getfrogs @species, @colour", statement.Text);
      Assert.Equal(CommandType.StoredProcedure, statement.Type);
      Assert.Equal(2, statement.Parameters.Length);
      Assert.Contains(statement.Parameters, p => p.ParameterName == "@species" && (string) p.Value == "tree");
      Assert.Contains(statement.Parameters, p => p.ParameterName == "@colour" && (string) p.Value == "green");
    }
  }
}
