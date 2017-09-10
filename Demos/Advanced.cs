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
using Microsoft.SqlServer.Types;

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

        [TestMethod]
        public void SpecialDataTypesSpatial()
        {            
            // Test as a scalar result
            Helper.RunDemo(conn =>
            {
                var result = conn.ExecuteScalar<SqlGeometry>("SELECT geometry::Parse('POINT(0 0)') AS TestPoint");

                Console.WriteLine(result.STAsText().Value);
            });

            // Test as a parameter
            Helper.RunDemo(conn =>
            {
                var p = SqlGeography.Point(47.6062, 122.3321, 4326);

                var result = conn.ExecuteScalar<SqlGeography>("SELECT @p AS TestPoint", new { @p = p });

                Console.WriteLine(result.STAsText().Value);
            });

            // Test as a property in a POCO
            Helper.RunDemo(conn =>
            {
                var result = conn.QuerySingle<Address>("SELECT geography::STPointFromText('POINT(122.3321 47.6062)', 4326) AS Location, 'Seattle' AS City, 'WA' AS State, 'US' AS Country");

                Console.WriteLine("{0}: {1}", result.ToString(), result.Location.ToString());
            });
        }

        [TestMethod]
        public void SpecialDataTypesHiearchyID()
        {
            Helper.RunDemo(conn =>
            {
                var n1 = SqlHierarchyId.Parse("/1/1.1/2/");

                var n2 = conn.ExecuteScalar<SqlHierarchyId>("SELECT @h.GetAncestor(2) AS HID", new { @h = n1 });

                Console.WriteLine("Is {0} a descendant of {1}? {2}", n1, n2, n1.IsDescendantOf(n2));
            });
        }

        [TestMethod]
        public void Buffered()
        {
            Helper.RunDemo(conn =>
            {
                conn.StateChange += ConnectionStateChange;

                var rows = conn.Query("SELECT TOP (5000) ROW_NUMBER() OVER (ORDER BY a.[object_id]) AS n FROM sys.all_columns a, sys.all_columns b;", buffered: true);
                foreach (var row in rows)
                {
                    System.Threading.Thread.Sleep(2);
                }
            });        
        }

        [TestMethod]
        public void Unbuffered()
        {
            Helper.RunDemo(conn =>
            {
                conn.StateChange += ConnectionStateChange;

                var rows = conn.Query("SELECT TOP (5000) ROW_NUMBER() OVER (ORDER BY a.[object_id]) AS n FROM sys.all_columns a, sys.all_columns b;", buffered: false);
                foreach (var row in rows)
                {
                    System.Threading.Thread.Sleep(2);
                }
            });
        }

        [TestMethod]
        public void JsonAsPlainText()
        {
            Helper.RunDemo(conn =>
            {
                User u = new User()
                {
                    Id = 5,
                    FirstName = "Davide",
                    LastName = "Mauri",
                    EmailAddress = "info@davidemauri.it",
                    Tags = new JArray(),
                    Roles = new Roles()
                    {
                        new Role() { RoleName = "User" },
                        new Role() { RoleName = "Developer" },
                        new Role() { RoleName = "Administrator"}
                    },
                    Preferences = new Preferences()
                    {
                        Resolution = "1920x1080",
                        Style = "Black",
                        Theme = "Modern"
                    }
                };

                var result = conn.Execute("[dbo].[SetUserViaJson]", new { @userData = JsonConvert.SerializeObject(u) }, commandType: CommandType.StoredProcedure);
            });
        }


        /* 
         * This will fail since a string cannot be
         * natively converted to a JArray 
         */
        [TestMethod]       
        public void JsonAutomaticMapping()
        {
            Helper.RunDemo(conn =>
            {       
                var result = conn.QuerySingle<User>("SELECT Id, FirstName, LastName, EmailAddress, Tags = '[''one'', ''two'']' FROM dbo.[Users] WHERE Id=1");
            });
        }

        /* 
         * Workaround (for now, the real solution is to use Custom Type Handlers)
         */
        [TestMethod]
        public void JsonAutomaticMappingWork()
        {
            Helper.RunDemo(conn =>
            {
                var r = conn.QuerySingle("SELECT Id, FirstName, LastName, EmailAddress, Tags = '[''one'', ''two'']' FROM dbo.[Users] WHERE Id=1");
                var u = new User
                {
                    Id = r.Id,
                    FirstName = r.FirstName,
                    LastName = r.LastName,
                    EmailAddress = r.EmailAddress,
                    Tags = JArray.Parse(r.Tags)
                };

                Console.WriteLine(u);
                Console.WriteLine(u.Tags);
            });
        }

        private void ConnectionStateChange(object sender, StateChangeEventArgs e)
        {
            Console.WriteLine($"{DateTime.Now}: {e.CurrentState}");
        }
    }
}
