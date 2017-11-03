using System;
using System.Data.SqlClient;
using System.Dynamic;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.SqlServer.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dapper;
using Dapper.FluentMap;
using Demos.Entities;
using Demos.Mappers;
using Demos.Handlers;

namespace Demos
{
    [TestClass]
    public class Advanced
    {
        [TestMethod]
        public void T01_MultipleExecution()
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
                Assert.AreEqual(3, affectedRows);
            });
        }

        [TestMethod]
        public void T02_MultipleResults()
        {
            Helper.RunDemo(conn =>
            {
                using (var multiQueryResults = conn.QueryMultiple("SELECT Id, FirstName, LastName, EMailAddress FROM dbo.[Users] WHERE Id <= 5; SELECT CompanyName, Street, City, State, Country FROM dbo.[Company] WHERE Id <= 1;"))
                {
                    var users = multiQueryResults.Read<User>();
                    var company = multiQueryResults.ReadFirst<Company>();

                    users.ToList().ForEach(u => Console.WriteLine($"{u}"));
                    Console.WriteLine();
                    Console.WriteLine(company);

                    Assert.AreEqual(5, users.Count());
                    Assert.AreEqual("Acme LLC", company.CompanyName);
                }                
            });
        }

        [TestMethod]
        public void T03_MultipleMapping()
        {
            Helper.RunDemo(conn =>
            {
                var queryResult = conn.Query<User, Company, User>(
                    "SELECT * FROM [dbo].[UsersAndCompany] WHERE UserId = 5",
                    (u, c) =>
                    {
                        u.Company = c;
                        return u;
                    },
                    splitOn: "CompanyId").First();

                Console.WriteLine(queryResult);
                Console.WriteLine(queryResult.Company);

                Assert.AreEqual("Davide", queryResult.FirstName);
                Assert.AreEqual("Acme LLC", queryResult.Company.CompanyName);
            });

            Console.WriteLine();

            Helper.RunDemo(conn =>
            {
                var queryResult = conn.Query<User, Company, Address, User>(
                    "SELECT * FROM [dbo].[UsersAndCompanyAndAddress]",
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

                Assert.AreEqual("Davide", queryResult.FirstName);
                Assert.AreEqual("Acme LLC", queryResult.Company.CompanyName);
                Assert.AreEqual("10123 Ave NE", queryResult.Company.Address.Street);
            });
        }

        [TestMethod]
        public void T04_TableValuedParameters()
        {
            Helper.RunDemo(conn =>
            {
                var ut = new DataTable();
                ut.Columns.Add("UserId", typeof(int));
                ut.Columns.Add("Tag", typeof(string));

                ut.Rows.Add(5, "Developer");
                ut.Rows.Add(5, "Data Guy");
                ut.Rows.Add(1, "SysAdmin");

                int affectedRows = conn.Execute(
                    "INSERT INTO dbo.[UserTags] SELECT * FROM @ut",
                    new
                    {
                        @ut = ut.AsTableValuedParameter("UserTagsType")
                    });

                Console.WriteLine(affectedRows);
                Assert.AreEqual(3, affectedRows);
            });
        }

        [TestMethod]
        public void T05_SpecialDataTypesSpatial()
        {            
            // Test as a scalar result
            Helper.RunDemo(conn =>
            {
                var result = conn.ExecuteScalar<SqlGeometry>("SELECT geometry::Parse('POINT(0 0)') AS TestPoint");

                Console.WriteLine(result.STAsText().Value);
                Assert.AreEqual(0, result.STX);
                Assert.AreEqual(0, result.STY);
            });

            // Test as a parameter
            Helper.RunDemo(conn =>
            {
                var p = SqlGeography.Point(47.6062, -122.3321, 4326);

                var result = conn.ExecuteScalar<SqlGeography>("SELECT @p AS TestPoint", new { @p = p });

                Console.WriteLine(result.STAsText().Value);
                Assert.AreEqual(-122.3321, result.Long);
                Assert.AreEqual(47.6062, result.Lat);
            });

            // Test as a property in a POCO
            Helper.RunDemo(conn =>
            {
                var result = conn.QuerySingle<Address>("SELECT geography::STPointFromText('POINT(-122.3321 47.6062)', 4326) AS Location, 'Seattle' AS City, 'WA' AS State, 'US' AS Country");

                Console.WriteLine("{0}: {1}", result.ToString(), result.Location.ToString());
                Assert.AreEqual("Seattle,WA,US", result.ToString());
            });
        }

        [TestMethod]
        public void T06_SpecialDataTypesHiearchyID()
        {
            Helper.RunDemo(conn =>
            {
                var n1 = SqlHierarchyId.Parse("/1/1.1/2/");

                var n2 = conn.ExecuteScalar<SqlHierarchyId>("SELECT @h.GetAncestor(2) AS HID", new { @h = n1 });
               
                Console.WriteLine("Is {0} a descendant of {1}? {2}", n1, n2, n1.IsDescendantOf(n2));
                Assert.AreEqual(true, n1.IsDescendantOf(n2));
            });
        }

        [TestMethod]
        public void T07_JsonAsPlainText()
        {
            Helper.RunDemo(conn =>
            {
                User u = new User()
                {
                    Id = 5,
                    FirstName = "Davide",
                    LastName = "Mauri",
                    EmailAddress = "info@davidemauri.it",
                    Tags = new JArray() { "one", "two" },
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
                    },
                    Company = new Company()
                    {
                        Id = 1,
                        CompanyName = "Acme LLC",
                        Address = new Address()
                        {
                            Street = "10123 Ave NE",
                            City = "Redmond",
                            State = "WA",
                            Country = "United States"
                        }
                    }
                };

                var affectedRows = conn.Execute("[dbo].[SetUserViaJson]", new { @userData = JsonConvert.SerializeObject(u) }, commandType: CommandType.StoredProcedure);
                Console.WriteLine(affectedRows);
                //Assert.AreEqual(9, affectedRows);
            });
        }

        /* 
         * This will raise an exception since a string cannot be
         * natively converted to a JArray 
         */
        [TestMethod]
        //[ExpectedException(typeof(DataException))]
        public void T08_DefaultHandlingWithCustomType()
        {
            Helper.RunDemo(conn =>
            {
                var result = conn.QuerySingle<User>("SELECT Id, FirstName, LastName, EmailAddress, Tags FROM dbo.[UsersTagsView] WHERE Id=5");
            });
        }

        /* 
         * Workaround (for now, the real solution is to use Custom Type Handlers)
         */
        [TestMethod]
        public void T09_DefaultHandlingWithCustomTypeWorkaround()
        {
            Helper.RunDemo(conn =>
            {
                var r = conn.QuerySingle("SELECT Id, FirstName, LastName, EmailAddress, Tags FROM dbo.[UsersTagsView] WHERE Id=5");
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
                Assert.AreEqual("Davide", u.FirstName);
                Assert.AreEqual("one", u.Tags[0]);
            });
        }

        [TestMethod]
        public void T10_CustomMapping()
        {
            Helper.RunDemo(conn => {

                // For sake of simplicity use only one dictionary
                // since all column names are unique
                Dictionary<string, string> columnMaps = new Dictionary<string, string>
                {
                    // Column, Property
                    { "UserId", "Id" },
                    { "CompanyId", "Id" }
                };

                // Create mapping function
                var mapper = new Func<Type, string, PropertyInfo>((type, columnName) =>
                {
                    if (columnMaps.ContainsKey(columnName))
                        return type.GetProperty(columnMaps[columnName]);
                    else
                        return type.GetProperty(columnName);
                });

                // Create customer mapper for User object
                var userMap = new CustomPropertyTypeMap(
                    typeof(User), 
                    (type, columnName) => mapper(type, columnName)
                    );

                // Create customer mapper for Company object
                var companyMap = new CustomPropertyTypeMap(
                    typeof(Company),
                    (type, columnName) => mapper(type, columnName)
                    );

                // Notify Dapper to use the mappers
                SqlMapper.SetTypeMap(typeof(User), userMap);
                SqlMapper.SetTypeMap(typeof(Company), companyMap);

                // Same query as before (just the split column is changed)
                var queryResult = conn.Query<User, Company, Address, User>(
                                    "SELECT * FROM [dbo].[UsersAndCompanyAndAddress] WHERE UserId = 5",
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

                Assert.AreEqual(5, queryResult.Id);
                Assert.AreEqual(1, queryResult.Company.Id);
            });
        }

        [TestMethod]
        public void T11_CustomFluentMapping()
        {
            Helper.RunDemo(conn => {

                FluentMapper.Initialize(config =>
                {
                    config.AddMap(new UserMap());
                    config.AddMap(new CompanyMap());
                });

                // Same query as before (just the split column is changed)
                var queryResult = conn.Query<User, Company, Address, User>(
                                    "SELECT * FROM [dbo].[UsersAndCompanyAndAddress] WHERE UserId = 5",
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

                Assert.AreEqual(5, queryResult.Id);
                Assert.AreEqual(1, queryResult.Company.Id);
            });
        }

        [TestMethod]
        public void T12_CustomHandlingJArray()
        {
            Helper.RunDemo(conn =>
            {
                SqlMapper.ResetTypeHandlers();
                SqlMapper.AddTypeHandler(new JArrayTypeHandler());

                /* Add tags */
                JArray tags = new JArray() { "one", "two", "three" };

                conn.Execute("dbo.SetUserTagsViaJson", new { @userId = 1, @tags = tags }, commandType: CommandType.StoredProcedure);

                /* Query roles */
                var queryResult = conn.QueryFirstOrDefault<User>("SELECT Id, FirstName, LastName, EmailAddress, Tags FROM dbo.[UsersTagsView] WHERE Id = 1");

                Console.WriteLine("Tags: {0}", queryResult?.Tags);
                Assert.AreEqual(3, queryResult.Tags.Count());
            });
        }

        [TestMethod]
        public void T13_CustomHandlingRole()
        {
            Helper.RunDemo(conn =>
            {
                SqlMapper.ResetTypeHandlers();
                SqlMapper.AddTypeHandler(new RolesTypeHandler());

                Roles roles = new Roles
                {
                    new Role("one"),
                    new Role("two"),
                    new Role("three")
                };

                conn.Execute("dbo.SetUserRoles", new { @userId = 1, @roles = roles }, commandType: CommandType.StoredProcedure);

                var queryResult = conn.QueryFirstOrDefault<User>("SELECT Id, FirstName, LastName, EmailAddress, Roles FROM dbo.[UsersRolesView] WHERE Id = 1");

                Console.WriteLine("Roles: {0}", queryResult?.Roles);
                Assert.AreEqual(3, queryResult.Roles.Count());
            });
        }

        [TestMethod]
        public void T14_ComplexCustomHandling()
        {
            Helper.RunDemo(conn =>
            {
                SqlMapper.ResetTypeHandlers();
                SqlMapper.AddTypeHandler(new UserTypeHandler());

                User u = new User()
                {
                    FirstName = "Davide",
                    LastName = "Mauri",
                    EmailAddress = "davide.mauri@gmail.com",
                    Tags = new JArray() { "alpha", "beta" },
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
                    },
                    Company = new Company()
                    {
                        CompanyName = "Sensoria Inc.",
                        Address = new Address()
                        {
                             City = "Redmond",
                             State = "WA",
                             Country = "United States",
                             Street = "16225 NE 87th Street"
                        }
                    }
                };

                var executeResult = conn.ExecuteScalar<User>("dbo.SetUserViaJson", new { @userData = u }, commandType: CommandType.StoredProcedure);

                Console.WriteLine("User saved: {0}", executeResult);
                Assert.AreEqual("Davide", executeResult.FirstName);
            });
        }        

        [TestMethod]
        public void T15_Buffered()
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
        public void T16_Unbuffered()
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

        private void ConnectionStateChange(object sender, StateChangeEventArgs e)
        {
            Console.WriteLine($"{DateTime.Now}: {e.CurrentState}");
        }
    }    
}
