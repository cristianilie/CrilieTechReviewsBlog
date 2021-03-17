using CrilieTechReviewsBlog.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrilieTechReviewsBlog.DataManagement.Repositories
{
    public class BlackListedUserRepository : IBlackListedUserRepository
    {
        private readonly AppDbContext _context;

        public BlackListedUserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddUserToBlackList(BlacklistedUser user)
        {
            _context.Add(user);
            await _context.SaveChangesAsync();
        }

        public List<BlacklistedUser> GetBlacklistedUsers()
        {
            return _context.BlacklistedUsers.ToList();
        }

        public BlacklistedUser GetBlacklistedUser(string userId)
        {
            return _context.BlacklistedUsers.SingleOrDefault(u => u.UserId == userId);
        }

        public async Task RemoveUserFromBlackList(BlacklistedUser user)
        {
            _context.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}