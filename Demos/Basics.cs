using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using Dapper;
using System.Dynamic;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Demos.Entities;

namespace Demos
{
    [TestClass]
    public class Basics
    {
        [TestMethod]        
        public void T01_ExecuteScalarSample()
        {
            // Return a scalar "object" value
            Helper.RunDemo(conn => {
                var firstName = conn.ExecuteScalar("SELECT [FirstName] FROM dbo.[Users] WHERE [Id] = 1");
                Console.WriteLine("{0} ({1}): {2}", nameof(firstName), firstName.GetType(), firstName);
            });

            // Return a scalar typed value
            Helper.RunDemo(conn => {
                var firstName = conn.ExecuteScalar<string>("SELECT [FirstName] FROM dbo.[Users] WHERE [Id] = 1");
                Console.WriteLine("{0} ({1}): {2}", nameof(firstName), firstName.GetType(), firstName);
            });
        }

        [TestMethod]
        public void T02_ExecuteSample()
        {
            // Return affected number rows
            Helper.RunDemo(conn =>
            {
                int affectedRows = conn.Execute("UPDATE dbo.[Users] SET [FirstName] = 'John' WHERE [Id] = 1");
                Console.WriteLine("'UPDATE' Affected Rows: {0}", affectedRows);
            });

            Console.WriteLine();

            // Any returned value is just discarded (uses an ExecuteNonQuery behind the scenes)
            Helper.RunDemo(conn => {
                int affectedRows = conn.Execute("SELECT [FirstName] FROM dbo.[Users] WHERE [Id] = 1");
                Console.WriteLine("'SELECT' Affected Rows: {0}", affectedRows);
            });
        }

        [TestMethod]
        public void T03_QuerySample()
        {
            // Return an IEnumerable of dynamic (DapperRow) object
            Helper.RunDemo(conn =>
            {
                var queryResult = conn.Query("SELECT [Id], [FirstName], [LastName] FROM dbo.[Users]");
                Console.WriteLine("{0} ({1})", nameof(queryResult), queryResult.GetType());

                queryResult.ToList().ForEach(u => Console.WriteLine($"{u}"));
            });

            Console.WriteLine();

            // Return an IEnumerable of User object
            Helper.RunDemo(conn =>
            {
                var queryResult = conn.Query<User>("SELECT [Id], [FirstName], [LastName] FROM dbo.[Users]");
                Console.WriteLine("{0} ({1})", nameof(queryResult), queryResult.GetType());

                queryResult.ToList().ForEach(u => Console.WriteLine($"{u}"));
            });
        }

        [TestMethod]
        public void T04_QueryWithAnonymousObjectParameters()
        {
            Helper.RunDemo(conn =>
            {
                var queryResult = conn.QueryFirst<User>("SELECT [Id], [FirstName], [LastName] FROM dbo.[Users] WHERE Id = @Id", new { Id = 1 });
                Console.WriteLine("{0} ({1}): {2}", nameof(queryResult), queryResult.GetType(), queryResult);
            });

            Console.WriteLine();

            Helper.RunDemo(conn =>
            {
                var queryResult = conn.QueryFirst<User>("SELECT [Id], [EmailAddress] FROM dbo.[Users] WHERE FirstName = @FirstName AND LastName = @LastName", new { FirstName = "Davide", LastName = "Mauri" });
                Console.WriteLine("{0} ({1}): {2}", nameof(queryResult), queryResult.GetType(), queryResult);
            });
        }

        [TestMethod]
        public void T05_QueryWithDynamicParameters()
        {
            Helper.RunDemo(conn =>
            {
                DynamicParameters dp = new DynamicParameters();
                dp.Add("Id", 1);

                var queryResult = conn.QueryFirst<User>("SELECT [Id], [FirstName], [LastName] FROM dbo.[Users] WHERE Id = @Id", dp);
                Console.WriteLine("{0} ({1}): {2}", nameof(queryResult), queryResult.GetType(), queryResult);
            });

            Console.WriteLine();

            Helper.RunDemo(conn =>
            {
                DynamicParameters dp = new DynamicParameters();
                dp.Add("FirstName", "Davide");
                dp.Add("LastName", "Mauri");

                var queryResult = conn.QueryFirst<User>("SELECT [Id], [EmailAddress] FROM dbo.[Users] WHERE FirstName = @FirstName AND LastName = @LastName", dp);
                Console.WriteLine("{0} ({1}): {2}", nameof(queryResult), queryResult.GetType(), queryResult);
            });
        }

        [TestMethod]
        public void T06_ExecuteProcedure()
        {
            Helper.RunDemo(conn =>
            {
                var queryResult = conn.QuerySingle("dbo.ProcedureBasic", new { @email = "info@davidemauri.it" }, commandType: CommandType.StoredProcedure);

                Console.WriteLine("{0} ({1}): {2}", nameof(queryResult), queryResult.GetType(), queryResult);
            });

            Console.WriteLine();

            Helper.RunDemo(conn =>
            {
                var queryResult = conn.QuerySingle<User>("dbo.ProcedureBasic", new { @email = "info@davidemauri.it" }, commandType: CommandType.StoredProcedure);

                Console.WriteLine("{0} ({1}): {2}", nameof(queryResult), queryResult.GetType(), queryResult);
            });
        }

        [TestMethod]
        public void T07_ExecuteProcedureWithOutputAndReturnValue()
        {
            Helper.RunDemo(conn =>
            {
                DynamicParameters dp = new DynamicParameters();
                dp.Add("email", "info@davidemauri.it");
                dp.Add("firstName", null, DbType.String, ParameterDirection.Output, 100); // Manually specify parameter details
                dp.Add("lastName", "", direction: ParameterDirection.Output); // Infer type and size from given value
                dp.Add("result", null, DbType.Int32, ParameterDirection.ReturnValue);

                conn.Execute("dbo.ProcedureWithOutputAndReturnValue", dp, commandType: CommandType.StoredProcedure);

                Console.WriteLine("User: {0}, {1}, {2}", dp.Get<int>("result"), dp.Get<string>("firstName"), dp.Get<string>("lastName"));
            });
        }
    }
}
