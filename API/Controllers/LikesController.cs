using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Extentions;
using API.Helpers;
using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likeRepository;
        public LikesController(IUserRepository userRepository, ILikesRepository likeRepository)
        {
            this._likeRepository = likeRepository;
            this._userRepository = userRepository;
        }

        [HttpPost("{username}")]
        public async Task<IActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await _userRepository.GetUserByUsernameAsync(username); //Get User duoc Like
            var sourceUser = await _likeRepository.GetUserWithLikes(sourceUserId);  // Get minh ra tu bang like

            if(likedUser == null) return NotFound();
            if(sourceUser.UserName == username) return BadRequest("Không thể từ thích bản thân");

            var userLike = await _likeRepository.GetUserLike(sourceUserId, likedUser.Id);
            if(userLike != null) return BadRequest("Bạn đã thích người này");

            userLike = new UserLike 
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);
            
            if(await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Fail to like user");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery] LikeParams likeParams)
        {
            likeParams.UserId = User.GetUserId();
            var users = await _likeRepository.GetUserLikes(likeParams);
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }
    }
}