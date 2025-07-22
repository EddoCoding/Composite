using Composite.Common.Factories;
using Composite.Models;
using Dapper;

namespace Composite.Repositories
{
    public class NoteRepository(IDbConnectionFactory dbConnectionFactory) : INoteRepository
    {
        public async Task<bool> Create(Note note)
        {
            using(var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var queryAddNote = "Insert Into Notes(Id, Title, Content, DateCreate, Password, Preview) Values (@Id, @Title, @Content, @DateCreate, @Password, @Preview)";
                var resultAddNote = await connection.ExecuteAsync(queryAddNote, note);

                if(resultAddNote > 0) return true;
            }

            return false;
        }
    }
}