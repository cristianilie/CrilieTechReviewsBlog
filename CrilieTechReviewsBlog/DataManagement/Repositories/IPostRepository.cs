using CrilieTechReviewsBlog.Models;
using CrilieTechReviewsBlog.ViewModels;
using System.Threading.Tasks;

namespace CrilieTechReviewsBlog.DataManagement.Repositories
{
    public interface IPostRepository
    {
        Post GetPost(int Id);
        IndexViewModel GetAllPosts(int pageNumber, string search);
        Task<int> AddPost(Post post);
        Task<bool> UpdatePost(Post post);
        Task RemovePost(int Id);
        Task AddSubComment(SubComment subComment);
        Task UpdateComment(int Id, int PostId, string Message);
        Task DeleteComment(int Id);
    }
}
