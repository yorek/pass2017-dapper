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

namespace Demos
{
    [TestClass]
    public class Advanced
    {
        [TestMethod]
        public void MultipleExecution()
        {
            // Return a scalar "object" value
            Helper.RunDemo(conn =>
            {
                var paramList = new List<DynamicParameters>();

                var l1 = new DynamicParameters();
                l1.Add("UserId", 1);
                l1.Add("AttemptTime", DateTimeOffset.Now.AddSeconds(-10));
                l1.Add("Result", 'F');
                paramList.Add(l1);

                var l2 = new DynamicParameters();
                l2.Add("UserId", 1);
                l2.Add("AttemptTime", DateTimeOffset.Now.AddSeconds(-5));
                l2.Add("Result", 'F');
                paramList.Add(l2);

                var l3 = new DynamicParameters();
                l3.Add("UserId", 1);
                l3.Add("AttemptTime", DateTimeOffset.Now);
                l3.Add("Result", 'S');
                paramList.Add(l3);

                var affectedRows = conn.Execute("INSERT INTO [log].[Logins] ([UserId], [AttemptTime], [Result]) VALUES (@UserId, @AttemptTime, @Result)", paramList);
                Console.WriteLine("Inserted rows: {0}", affectedRows);
            });
        }

        [TestMethod]
        public void MultipleMapping()
        {
            Helper.RunDemo(conn =>
            {
                var queryResult = conn.Query<User, Company, User>(
                    "SELECT * FROM [dbo].[UsersAndCompany]",
                    (u, c) =>
                    {
                        u.Company = c;
                        return u;
                    },
                    splitOn: "CompanyId").First();

                Console.WriteLine(queryResult);
                Console.WriteLine(queryResult.Company);
            });

            Console.WriteLine();

            Helper.RunDemo(conn =>
            {
                var queryResult = conn.Query<User, Company, Address, User>(
                    "SELECT * FROM [dbo].[UsersAndCompany]",
                    (u, c, a) =>
                    {
                        u.Company = c;
                        u.Company.Address = a;
                        return u;
                    },
                    splitOn: "CompanyId,Street").First();

                Console.WriteLine(queryResult);
                Console.WriteLine(queryResult.Company);
                Console.WriteLine(queryResult.Company.Address);
            });
        }

        [TestMethod]
        public void MultipleResults()
        {
            Helper.RunDemo(conn =>
            {
                using (var multiQueryResults = conn.QueryMultiple("SELECT FirstName, LastName, EMailAddress FROM dbo.[Users]; SELECT CompanyName, Street, City, State, Country FROM dbo.[Company];"))
                {
                    var users = multiQueryResults.Read<User>();
                    var company = multiQueryResults.ReadFirst<Company>();

                    users.ToList().ForEach(u => Console.WriteLine($"{u}"));
                    Console.WriteLine();
                    Console.WriteLine(company);
                }
            });
        }

        [TestMethod]
        public void TableValuedParameters()
        {
            Helper.RunDemo(conn =>
            {
                var ut = new DataTable();
                ut.Columns.Add("UserId", typeof(string));
                ut.Columns.Add("Tag", typeof(string));

                ut.Rows.Add(5, "Developer");
                ut.Rows.Add(5, "Data Guy");
                ut.Rows.Add(1, "SysAdmin");

                conn.Execute(
                    "INSERT INTO dbo.[UserTags] SELECT * FROM @ut",
                    new
                    {
                        @ut = ut.AsTableValuedParameter("UserTagsType")
                    });
            });
        }
    }
}
