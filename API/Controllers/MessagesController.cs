using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Extentions;
using API.Helpers;
using API.Interfaces;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public MessagesController(IMessageRepository messageRepository,
            IUserRepository userRepository, IMapper mapper)
        {
            this._mapper = mapper;
            this._userRepository = userRepository;
            this._messageRepository = messageRepository;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();
            if(username == createMessageDto.RecipientUsername) 
                return BadRequest("Không thể tự gữi tin nhắn cho bản thân");
            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);
            if (recipient == null) return NotFound();
            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                Content = createMessageDto.Content,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,

            };

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllChanges()) return (_mapper.Map<MessageDto>(message));

            return BadRequest("Tin nhắn chưa được gữi");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageForUser([FromQuery]MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();
            var message = await _messageRepository.GetMessageForUser(messageParams);
            Response.AddPaginationHeader(message.CurrentPage, 
                message.PageSize, message.TotalCount, message.TotalPages);
            
            return message;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUsername = User.GetUsername();
            var message = await _messageRepository.GetMessageThread(currentUsername, username);
            return Ok(message);
        }
    }
}