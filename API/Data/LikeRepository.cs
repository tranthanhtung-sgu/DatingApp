using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Extentions;
using API.Helpers;
using API.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikeRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikeRepository(DataContext context)
        {
            this._context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likeUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, likeUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikeParams likeParams)
        {
            var users = _context.Users.OrderBy(x => x.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if (likeParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == likeParams.UserId);
                users = likes.Select(like => like.LikedUser);
            }

            if (likeParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == likeParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            var userLikes = users.Select(user => new LikeDto {
                Id = user.Id,
                Username = user.UserName,
                Age = user.DateOfBirth.CalculateAge(),
                City = user.City,
                KnownAs = user.KnownAs,
                PhotoUrl = user.Photos.FirstOrDefault(x=>x.IsMain).Url
            });
            return await PagedList<LikeDto>.CreateAsync(userLikes, likeParams.pageNumber, 
                likeParams.pageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
                .Include(l => l.LikedUsers)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}