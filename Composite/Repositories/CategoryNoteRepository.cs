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
        public async Task<bool> Create(CategoryNote categoryNote)
        {
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var checkQuery = "Select Count(1) From Categories Where NameCategory = @NameCategory";
                var exists = await connection.QuerySingleAsync<int>(checkQuery, new { NameCategory = categoryNote.NameCategory });
                if (exists > 0) return false;

                var queryAddCategory = "Insert Into Categories(NameCategory) Values(@NameCategory)";
                var resultAddCategory = await connection.ExecuteAsync(queryAddCategory, categoryNote);
                if (resultAddCategory > 0) return true;
            }
            return false;
        }
        public async Task<bool> Delete(string nameCategory)
        {
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var queryDeleteCategory = "Delete From Categories Where NameCategory = @NameCategory";
                var resultDeleteCategory = await connection.ExecuteAsync(queryDeleteCategory, new { NameCategory = nameCategory });

                if (resultDeleteCategory > 0) return true;
            }
            return false;
        }
        public async Task<bool> SetCategory(string nameCategory)
        {
            bool result = false;

            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var querySetCategoryNote = "Update Notes Set Category = @DefaultCategory Where Category = @NameCategory";
                var resultquerySetCategoryNote = await connection.ExecuteAsync(querySetCategoryNote, new { DefaultCategory = "Без категории", NameCategory = nameCategory });
                if (resultquerySetCategoryNote > 0) result = true;

                var querySetCategoryHardNote = "Update HardNotes Set Category = @DefaultCategory Where Category = @NameCategory";
                var resultquerySetCategoryHardNote = await connection.ExecuteAsync(querySetCategoryHardNote, new { DefaultCategory = "Без категории", NameCategory = nameCategory });
                if (resultquerySetCategoryHardNote > 0) result = true;

                return result;
            }
        }
    }
}