using CrilieTechReviewsBlog.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrilieTechReviewsBlog.DataManagement.Repositories
{
    public interface ICategoryRepository
    {
        Category GetCategory(int Id);
        List<Category> GetAllCategories();
        Task<int> AddCategory(Category category);
        Task<bool> UpdateCategory(Category category);
        Task RemoveCategory(int Id);
    }
}
