using Composite.Common.Factories;
using Composite.Models.Notes;
using Dapper;

namespace Composite.Repositories
{
    public class CategoryNoteRepository(IDbConnectionFactory dbConnectionFactory) : ICategoryNoteRepository
    {
        public IEnumerable<CategoryNote> Read()
        {
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var queryGetCategories = "Select * From Categories";
                var resultGetCategories = connection.Query<CategoryNote>(queryGetCategories).ToList();
                
                return resultGetCategories;
            }
        }
    }
}