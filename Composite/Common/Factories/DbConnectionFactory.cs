using Microsoft.Data.Sqlite;
using System.Data;

namespace Composite.Common.Factories
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        readonly string _connectionString = "Data Source=dbComposite.db";

        public DbConnectionFactory() => SQLitePCL.Batteries.Init();

        public IDbConnection CreateConnection() => new SqliteConnection(_connectionString);
    }
}