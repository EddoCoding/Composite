using Composite.Common.Factories;
using Composite.Models.Notes.Note;
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

                var queryAddNote = "Insert Into Notes(Id, Title, Content, Category, DateCreate, FontFamily, FontSize, Password) " +
                                   "Values (@Id, @Title, @Content, @Category, @DateCreate, @FontFamily, @FontSize, @Password)";
                var resultAddNote = await connection.ExecuteAsync(queryAddNote, note);

                if(resultAddNote > 0) return true;
            }

            return false;
        }
        public IEnumerable<Note> Read()
        {
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var queryGetNotes = "Select * From Notes";
                var resultGetNotes = connection.Query<Note>(queryGetNotes);

                return resultGetNotes;
            }
        }
        public async Task<bool> Update(Note note)
        {
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var queryUpdateNote = "Update Notes Set Title = @Title, Content = @Content, Category = @Category, DateCreate = @DateCreate, " +
                                      "FontFamily = @FontFamily, FontSize = @FontSize, Password = @Password Where Id = @Id";
                var resultUpdateNote = await connection.ExecuteAsync(queryUpdateNote, note);

                if (resultUpdateNote > 0) return true;
            }
            return false;
        }
        public async Task<bool> Delete(string id)
        {
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var queryDeleteNote = "Delete From Notes Where Id = @id";
                var resultDeleteNote = await connection.ExecuteAsync(queryDeleteNote, new { id });

                if(resultDeleteNote > 0) return true;

                return false;
            }
        }
    }
}