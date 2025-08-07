using Composite.Common.Factories;
using Composite.Models;
using Composite.ViewModels.Notes;
using Dapper;

namespace Composite.Repositories
{
    public class CategoryNoteRepository(IDbConnectionFactory dbConnectionFactory) : ICategoryNoteRepository
    {
        public async Task<bool> Create(CategoryNote categoryNote)
        {
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var queryAddCategory = "Insert Into Categories(NameCategory) Values(@NameCategory)";
                var resultAddCategory = await connection.ExecuteAsync(queryAddCategory, categoryNote);

                if (resultAddCategory > 0) return true;
            }

            return false;
        }
        public IEnumerable<CategoryNote> Read()
        {
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var queryGetCategories = "Select * From Categories";
                var resultGetCategories = connection.Query<CategoryNote>(queryGetCategories);

                return resultGetCategories;
            }
        }
        public async Task<bool> Delete(string NameCategory)
        {
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var queryDeleteCategory = "Delete From Categories Where NameCategory = @NameCategory";
                var resultDeleteCategory = await connection.ExecuteAsync(queryDeleteCategory, new { NameCategory });

                if (resultDeleteCategory > 0)
                {
                    var queryChangeNameCategory = "Update Notes Set Category = 'Без категории' Where Category = @NameCategory";
                    var resultChangeNameCategory = await connection.ExecuteAsync(queryChangeNameCategory, new { NameCategory });

                    return true;
                }

                return false;
            }
        }
    }
}