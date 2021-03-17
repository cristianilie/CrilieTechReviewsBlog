using CrilieTechReviewsBlog.Models;
using CrilieTechReviewsBlog.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrilieTechReviewsBlog.DataManagement.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly AppDbContext _context;

        public PostRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddPost(Post post)
        {
            _context.Add(post);
            await _context.SaveChangesAsync();
            _context.Entry(post).GetDatabaseValues();

            return post.Id;
        }

        public async Task AddSubComment(SubComment subComment)
        {
            _context.SubComments.Add(subComment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateComment(int Id, int PostId, string Message)
        {
            Comment comm;
            comm = _context.MainComments.FirstOrDefault(c => c.Id == Id);

            if (comm == null)
                comm = _context.SubComments.FirstOrDefault(c => c.Id == Id);

            comm.Message = Message;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteComment(int Id)
        {
            MainComment comm = _context.MainComments.FirstOrDefault(c => c.Id == Id);
            if (comm != null)
                _context.MainComments.Remove(comm);

            SubComment subComm = _context.SubComments.FirstOrDefault(c => c.Id == Id);
            if (subComm != null)
                _context.SubComments.Remove(subComm);

            await _context.SaveChangesAsync();
        }


        public IndexViewModel GetAllPosts(int pageNumber, string search)
        {
            var query = _context.Posts.AsNoTracking().AsQueryable();

            int postsCount = query == null ? 0 : query.Count();
            int pageSize = 5;
            int pageCount = (int)Math.Ceiling(postsCount / (decimal)pageSize);

            if (pageNumber > pageCount)
                pageNumber = pageCount;

            int skipAmount =  pageSize * (pageNumber - 1) < 0 ? 0 : pageSize * (pageNumber - 1);

            if (!string.IsNullOrEmpty(search) && query != null)
                query = query.Where(c => EF.Functions.Like(c.Title, $"%{search}%") ||
                                         EF.Functions.Like(c.Summary, $"%{search}%") ||
                                         EF.Functions.Like(c.Body, $"%{search}%"));

            return new IndexViewModel
            {
                PageNumber = pageNumber,
                PageCount = pageCount,
                Pages = Enumerable.Range(1, pageCount).ToList(),
                Search = search,
                Posts = query == null ? new List<Post>() : query.OrderByDescending(p => p.Id).Skip(skipAmount).Take(pageSize).ToList()
            };
        }

        public Post GetPost(int Id)
        {
            return _context.Posts.Include(p => p.MainComments)
                                 .ThenInclude(mc => mc.SubComments)
                                 .FirstOrDefault(p => p.Id == Id);
        }

        public async Task RemovePost(int Id)
        {
            _context.Posts.Remove(GetPost(Id));
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdatePost(Post post)
        {
            _context.Posts.Update(post);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
