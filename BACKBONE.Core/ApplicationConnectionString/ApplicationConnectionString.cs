using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Core.ApplicationConnectionString
{
    public class ApplicationConnectionString
    {
        public static string GetConnectionString(int dbId)
        {
            var connectionStrings = new Dictionary<int, string>
            {
                //Test DB - MSSQL
                { 1, @"Server=DESKTOP-5RDL6ET\SAQSQL2019;Database=EpyDispatch;User Id=sa;Password=123;TrustServerCertificate=True;" },
                //Muscat - Oracle
                { 2, "User Id=###;Password=###;Data Source=####:####/####;" }
            };

            if (connectionStrings.ContainsKey(dbId))
            {
                return connectionStrings[dbId];
            }
            else
            {
                return "";
            }
        }

        public static string GetConnectionString(int countryId, DatabaseType dbType)
        {
            var connectionStrings = new Dictionary<(int, DatabaseType), string>
            {
                // Test DB - Oracle
                {(1, DatabaseType.Oracle), "User Id=#######;Password=#######;Data Source=#######;"},
                // Muscat - Oracle
                {(2, DatabaseType.Oracle), "User Id=#######;Password=#######;Data Source=#######;"},
                // KSA - Oracle
                {(3, DatabaseType.Oracle), "User Id=#######;Password=#######;Data Source=#######;"},
                // Test DB - MySQL
                {(1, DatabaseType.MySQL), "Server=#######;Port=#######;Database=#######;User=#######;Password=#######;"},
                // Muscat - MySQL
                {(2, DatabaseType.MySQL), "Server=#########; Port=#######; Database=#######; Uid=#######; Pwd=########;Pooling=false;"},
                // KSA - MySQL
                {(3, DatabaseType.MySQL), "Server=#######; Port=#######; Database=#######; Uid=#######; Pwd=#######;Pooling=false;"}

            };

            if (connectionStrings.ContainsKey((countryId, dbType)))
            {
                return connectionStrings[(countryId, dbType)];
            }
            else
            {
                return "";
            }
        }

        public enum DatabaseType
        {
            Oracle,
            MySQL,
            MsSQL
        }
    }
}
