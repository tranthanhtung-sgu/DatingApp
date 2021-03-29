using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Helpers;
using API.Models;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike (int sourceUserId, int likeUserId);
        Task<PagedList<LikeDto>> GetUserLikes (LikeParams likeParams);
        Task<AppUser> GetUserWithLikes (int userId);

    }
}