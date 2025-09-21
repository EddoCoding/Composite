using Composite.Common.Mappers;
using Composite.Repositories;
using Composite.ViewModels.Notes;

namespace Composite.Services
{
    public class CategoryNoteService(ICategoryNoteRepository categoryNoteRepository, ICategoryNoteMap categoryNoteMap) : ICategoryNoteService
    {
        public IEnumerable<CategoryNoteVM> GetCategories()
        {
            List<CategoryNoteVM> categoriesVM;

            try
            {
                var categories = categoryNoteRepository.Read();
                if (categories != null && categories.Any())
                {
                    categoriesVM = new();
                    foreach (var category in categories)
                    {
                        var categoryVM = categoryNoteMap.MapToViewModel(category);
                        categoriesVM.Add(categoryVM);
                    }
                    return categoriesVM;
                }
                return Enumerable.Empty<CategoryNoteVM>();
            }
            catch (Exception)
            {
                return Enumerable.Empty<CategoryNoteVM>();
            }
        }
        public async Task<bool> AddCategory(CategoryNoteVM categoryNoteVM)
        {
            var categoryNote = categoryNoteMap.MapToModel(categoryNoteVM);

            try
            {    
                if (await categoryNoteRepository.Create(categoryNote)) return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> DeleteCategory(string nameCategory)
        {
            if (nameCategory == "Без категории") return false;

            try
            {
                if (await categoryNoteRepository.Delete(nameCategory)) return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> SetCategory(string nameCategory)
        {
            try
            {
                if (await categoryNoteRepository.SetCategory(nameCategory)) return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}