using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data.SqlClient;
using Dapper;
using System.Dynamic;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Test1
{
    class Program
    {
        private static string _connectionString;

        static Program()
        {
            var builder = new SqlConnectionStringBuilder()
            {
                DataSource = @"(LocalDB)\MSSQLLocalDB",
                AttachDBFilename = @"d:\Work\Personal\Conferences\PASS2017\Dapper\Demo\Data\DapperSample.mdf",
                IntegratedSecurity = true,
                ConnectTimeout = 30,
                ApplicationName = "DapperDemo"
            };

            _connectionString = builder.ConnectionString;
        }

        static void RunDemo(Action<SqlConnection> demoAction)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                demoAction(conn);
            }
        }

        static void Main(string[] args)
        {
            // Execute Scalar
            // Returns an object of the correct type, 
            //RunDemo(conn => {            
            //    var firstName = conn.ExecuteScalar("SELECT [FirstName] FROM dbo.[Users] WHERE [Id] = 1");
            //    Console.WriteLine("{0} ({1}): {2}", nameof(firstName), firstName.GetType(), firstName);
            //});
            /*
             * 
            // Execute Scalar
            // Returns typed object
            using (SqlConnection conn = new SqlConnection(b.ConnectionString))
            {
                var firstName = conn.ExecuteScalar<string>("SELECT [FirstName] FROM dbo.[Users] WHERE [Id] = 1");
                Console.WriteLine("{0} ({1}): {2}", nameof(firstName), firstName.GetType(), firstName);
            }

            // Execute SQL code that doesn't return a value
            // If value is returned it will just be ignored
            using (SqlConnection conn = new SqlConnection(b.ConnectionString))
            {
                int affectedRows = conn.Execute("SELECT [FirstName] FROM dbo.[Users] WHERE [Id] = 1");
                Console.WriteLine("Affected Rows: {0}", affectedRows);
            }

            using (SqlConnection conn = new SqlConnection(b.ConnectionString))
            {
                int affectedRows = conn.Execute("UPDATE dbo.[Users] SET [FirstName] = 'John' WHERE [Id] = 1");
                Console.WriteLine("Affected Rows: {0}", affectedRows);
            }

            // Return an IEnumerable of dynamic (DapperRow) object
            using (SqlConnection conn = new SqlConnection(b.ConnectionString))
            {
                var queryResult = conn.Query("SELECT [Id], [FirstName], [LastName] FROM dbo.[Users]");
                Console.WriteLine("{0} ({1})", nameof(queryResult), queryResult.GetType());

                foreach(var r in queryResult)
                {                    
                    Console.WriteLine($"{r}");
                }
            }

            // Return an IEnumerable of User object
            using (SqlConnection conn = new SqlConnection(b.ConnectionString))
            {
                var queryResult = conn.Query<User>("SELECT [Id], [FirstName], [LastName] FROM dbo.[Users]");
                Console.WriteLine("{0} ({1})", nameof(queryResult), queryResult.GetType());

                foreach (var r in queryResult)
                {
                    Console.WriteLine($"{r}");
                }
            }
            */

            // Specify Parameters
            // Use an anonymous object
            RunDemo(conn =>
            {
                var queryResult = conn.Query<User>("SELECT [Id], [FirstName], [LastName] FROM dbo.[Users] WHERE Id = @Id", new { Id = 1 });
                Console.WriteLine("{0} ({1})", nameof(queryResult), queryResult.GetType());
            });

            RunDemo(conn =>
            {
                var queryResult = conn.Query<User>("SELECT [Id], [EmailAddress] FROM dbo.[Users] WHERE FirstName = @FirstName, LastName = @LastName", new { FirstName = "Davide", LastName = "Mauri" });
                Console.WriteLine("{0} ({1})", nameof(queryResult), queryResult.GetType());
            });

            // Specify Parameters
            // Use DynamicParameters
            RunDemo(conn =>
            {
                var queryResult = conn.Query<User>("SELECT [Id], [FirstName], [LastName] FROM dbo.[Users] WHERE Id = @Id", new { Id = 1 });
                Console.WriteLine("{0} ({1})", nameof(queryResult), queryResult.GetType());
            });

            RunDemo(conn =>
            {
                SqlMapper.ResetTypeHandlers();
                SqlMapper.AddTypeHandler(new JArrayTypeHandler());

                var queryResult = conn.QueryFirstOrDefault<User>("SELECT Id, FirstName, LastName, EmailAddress, Tags = '[''one'', ''two'']' FROM dbo.[Users] WHERE Id=1");

                Console.WriteLine("Tags: {0}", queryResult?.Tags);
            });

            RunDemo(conn =>
            {
                SqlMapper.ResetTypeHandlers();
                SqlMapper.AddTypeHandler(new RolesTypeHandler());

                var queryResult = conn.QueryFirstOrDefault<User>("SELECT Id, FirstName, LastName, EmailAddress, Roles = 'one|two' FROM dbo.[Users] WHERE Id=1");

                Console.WriteLine("Roles: {0}", queryResult?.Roles);
            });

            RunDemo(conn =>
            {
                SqlMapper.ResetTypeHandlers();
                SqlMapper.AddTypeHandler(new UserTypeHandler());

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

                u.Tags.Add("one");
                u.Tags.Add("two");

                var executeResult = conn.Execute("dbo.SetUserViaJson", new { @userData = u }, commandType: CommandType.StoredProcedure);

                Console.WriteLine("User saved: {0}", executeResult);

                var queryResult = conn.Execute("dbo.GetUserViaJson", new { @id = 5 }, commandType: CommandType.StoredProcedure);

                Console.WriteLine("User saved: {0}", queryResult);

            });

            Console.WriteLine();
            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }

    public class JArrayTypeHandler : SqlMapper.TypeHandler<JArray>
    {
        public override JArray Parse(object value)
        {
            string json = value.ToString();
            json.Replace("\"", "'");
            return JArray.Parse(value.ToString());
        }

        public override void SetValue(IDbDataParameter parameter, JArray value)
        {
            parameter.Value = value.ToString();
        }
    }

    public class RolesTypeHandler : SqlMapper.TypeHandler<Roles>
    {
        public override Roles Parse(object value)
        {
            Roles result = new Roles();

            string[] roles = value.ToString().Split('|'
                );
            foreach(var r in roles)
            {
                result.Add(new Role() { RoleName = r });
            }

            return result;
        }

        public override void SetValue(IDbDataParameter parameter, Roles value)
        {
            throw new NotImplementedException();
        }
    }

    public class UserTypeHandler : SqlMapper.TypeHandler<User>
    {
        public override User Parse(object value)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(IDbDataParameter parameter, User value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }
    }
}
