using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Helpers;
using API.Interfaces;
using API.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._context = context;

        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                .OrderByDescending(m => m.DateSent)
                .AsQueryable();
            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUsername == messageParams.Username),
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username),
                _ => query.Where(u => u.RecipientUsername == messageParams.Username
                        && u.DateRead == null)
            };

            var message = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(message, 
                messageParams.pageNumber, messageParams.pageSize);  

        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await _context.Messages
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                .Where(u => u.Sender.UserName == currentUsername 
                        && u.Recipient.UserName == recipientUsername || 
                            u.Sender.UserName == recipientUsername
                                && u.Recipient.UserName == currentUsername)
                .OrderBy(m => m.DateSent)
                .ToListAsync();     // Lay ra luong tin nhan giua 2 nguoi

            var unReadMessage = await _context.Messages.Where(m => m.DateRead == null
                && m.Recipient.UserName == recipientUsername).ToListAsync();
            if (unReadMessage.Any())
            {
                foreach (var message in unReadMessage)
                {
                    message.DateRead = DateTime.Now;
                }
                await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);

        }

        public async Task<bool> SaveAllChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}