using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;

namespace Demos
{
    public class Helper
    {
        private static string _connectionString;

        static Helper()
        {
            var data = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.GetDirectories("Data").Single();
            AppDomain.CurrentDomain.SetData("DataDirectory", data.FullName);

            var builder = new SqlConnectionStringBuilder()
            {
                DataSource = @"(LocalDB)\MSSQLLocalDB",
                AttachDBFilename = @"|DataDirectory|\DapperSample.mdf",
                IntegratedSecurity = true,
                ConnectTimeout = 30,
                ApplicationName = "DapperDemo"

            };

            _connectionString = builder.ConnectionString;
        }

        public static void RunDemo(Action<SqlConnection> demoAction)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                demoAction(conn);
            }
        }
    }
}
