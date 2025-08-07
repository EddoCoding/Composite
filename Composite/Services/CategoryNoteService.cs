using Composite.Common.Mappers;
using Composite.Repositories;
using Composite.ViewModels.Notes;

namespace Composite.Services
{
    public class CategoryNoteService(ICategoryNoteRepository categoryNoteRepository, ICategoryNoteMap categoryNoteMap) : ICategoryNoteService
    {
        public async Task<bool> AddCategory(CategoryNoteVM categoryNoteVM)
        {
            var category = categoryNoteMap.MapToModel(categoryNoteVM);
            if (await categoryNoteRepository.Create(category)) return true;

            return false;
        }
        public IEnumerable<string> GetCategoryNotes()
        {
            var categories = categoryNoteRepository.Read();

            List<string> CategoriesVM = new();
            foreach(var category in categories)
            {
                var categoryVM = categoryNoteMap.MapToViewModel(category);
                CategoriesVM.Add(categoryVM.NameCategory);
            }

            return CategoriesVM;
        }
        public async Task<bool> DeleteCategory(string nameCategory)
        {
            if(nameCategory == "Без категории") return false;
            if (await categoryNoteRepository.Delete(nameCategory)) return true;
            return false;
        }
    }
}