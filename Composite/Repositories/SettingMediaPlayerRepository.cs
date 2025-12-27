using Composite.DataBase;
using Composite.Models;
using Dapper;

namespace Composite.Repositories
{
    public class SettingMediaPlayerRepository(IDbConnectionFactory dbConnectionFactory) : ISettingMediaPlayerRepository
    {
        public async Task<bool> Create(IEnumerable<Song> songs)
        {
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();
                var query = "Insert Into Songs(Id, Title, Data) Values(@Id, @Title, @Data)";

                var result = await connection.ExecuteAsync(query, songs);
                return result > 0;
            }
        }
        public IEnumerable<Song> ReadMetaData()
        {
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var query = "Select Id, Title From Songs";
                var resultQuery = connection.Query<Song>(query);

                return resultQuery;
            }
        }
        public async Task<IEnumerable<Song>> ReadMetaDataAsync(List<string> idList)
        {
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var idListString = string.Join(",", idList.Select(id => $"'{id}'"));
                var query = $"SELECT Id, Title FROM Songs WHERE Id IN ({idListString})";
                var resultQuery = await connection.QueryAsync<Song>(query);

                return resultQuery;
            }
        }
        public async Task<byte[]> ReadArrayBytes(string id)
        {
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var queryGetArrayBytesById = "Select Data From Songs Where Id = @id";
                var resultGetArrayBytesById = await connection.QuerySingleOrDefaultAsync<byte[]>(queryGetArrayBytesById, new { id });

                return resultGetArrayBytesById;
            }
        }
        public async Task<bool> Delete(string id)
        {
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var queryDeleteSong = "Delete From Songs Where Id = @id";
                var resultDeleteSong = await connection.ExecuteAsync(queryDeleteSong, new { id });

                if (resultDeleteSong > 0) return true;
                return false;
            }
        }
        public async Task DeleteAll()
        {
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var queryDeleteSongs = "Delete From Songs";
                var resultDeleteSongs = await connection.ExecuteAsync(queryDeleteSongs);
            }
        }
    }
}