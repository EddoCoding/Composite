using Composite.Common.Mappers;
using Composite.Repositories;
using Composite.ViewModels.Notes;

namespace Composite.Services
{
    public class CategoryNoteService(ICategoryNoteRepository categoryNoteRepository, ICategoryNoteMap categoryNoteMap) : ICategoryNoteService
    {
        public IEnumerable<CategoryNoteVM> GetCategories()
        {
            var categories = categoryNoteRepository.Read();

            List<CategoryNoteVM> categoriesVM;
            if(categories != null && categories.Any())
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
    }
}