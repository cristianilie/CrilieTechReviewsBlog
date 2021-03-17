using CrilieTechReviewsBlog.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrilieTechReviewsBlog.DataManagement.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            _context.Entry(category).GetDatabaseValues();
            return category.Id;
        }

        public List<Category> GetAllCategories()
        {
            return _context.Categories.ToList();
        }

        public Category GetCategory(int Id)
        {
            return _context.Categories.FirstOrDefault(c => c.Id == Id);
        }

        public async Task RemoveCategory(int Id)
        {
            _context.Remove(GetCategory(Id));
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateCategory(Category category)
        {
            _context.Categories.Update(category);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
