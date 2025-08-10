using Composite.Common.Factories;
using Dapper;

namespace Composite.Repositories
{
    public class SettingMediaPlayerRepository(IDbConnectionFactory dbConnectionFactory) : ISettingMediaPlayerRepository
    {
        public async Task<bool> Create(string path)
        {
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var query = "Delete From PathFolder; Insert Into PathFolder(Path) Values(@path)";
                var resultQuery = await connection.ExecuteAsync(query, new { path });

                if(resultQuery > 0) return true;
            }

            return false;
        }
        public string Read()
        {
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var query = "Select * From PathFolder";
                var resultQuery = connection.QueryFirstOrDefault<string>(query);

                return resultQuery;
            }
        }
    }
}