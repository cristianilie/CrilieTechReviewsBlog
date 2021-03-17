using CrilieTechReviewsBlog.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrilieTechReviewsBlog.DataManagement.Repositories
{
    public interface IBlackListedUserRepository
    {
        BlacklistedUser GetBlacklistedUser(string userId);
        List<BlacklistedUser> GetBlacklistedUsers();
        Task AddUserToBlackList(BlacklistedUser user);
        Task RemoveUserFromBlackList(BlacklistedUser user);

    }
}
