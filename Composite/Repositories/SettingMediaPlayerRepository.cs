using Composite.Common.Factories;
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

                int totalInserted = 0;

                var query = "Insert Into Songs(Id, Title, Data) Values(@Id, @Title, @Data)";
                foreach (var song in songs)
                {
                    var result = await connection.ExecuteAsync(query, new
                    {
                        Id = song.Id,
                        Title = song.Title,
                        Data = song.Data
                    });
                    totalInserted += result;
                }
                return totalInserted > 0;
            }
        }
        public IEnumerable<Song> ReadSong()
        {
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var query = "Select * From Songs";
                var resultQuery = connection.Query<Song>(query);

                if (resultQuery != Enumerable.Empty<Song>())
                {
                    List<Song> songs = new List<Song>();
                    foreach (var song in resultQuery) songs.Add(song);

                    return songs;
                }
            }
            return Enumerable.Empty<Song>();
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
    }
}