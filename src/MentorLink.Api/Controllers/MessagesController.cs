using MentorLink.Api.Data;
using MentorLink.Shared.Dtos;
using MentorLink.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MentorLink.Api.Controllers;

[ApiController]
[Route("api/messages")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly AppDbContext _db;
    public MessagesController(AppDbContext db) => _db = db;

    [HttpGet("conversations/{userId:int}")]
    public async Task<List<ConversationDto>> Conversations(int userId)
    {
        var messages = await _db.Messages
            .Where(m => m.SenderId == userId || m.RecipientId == userId)
            .ToListAsync();

        var partnerIds = messages
            .Select(m => m.SenderId == userId ? m.RecipientId : m.SenderId)
            .Distinct().ToList();

        var users = await _db.Users.Where(u => partnerIds.Contains(u.Id)).ToListAsync();
        var profiles = await _db.MentorProfiles.Where(p => partnerIds.Contains(p.UserId)).ToListAsync();

        return partnerIds.Select(pid =>
        {
            var thread = messages
                .Where(m => m.SenderId == pid || m.RecipientId == pid)
                .OrderByDescending(m => m.SentAt).ToList();
            var last = thread.First();
            var unread = thread.Count(m => m.RecipientId == userId && !m.IsRead);
            var partner = users.First(u => u.Id == pid);
            var profile = profiles.FirstOrDefault(p => p.UserId == pid);
            var roleLine = profile is null ? partner.Field : $"{profile.Title} · {profile.Company}";
            return new ConversationDto(pid, partner.FullName, roleLine, last.Content, last.SentAt, unread);
        })
        .OrderByDescending(c => c.LastAt)
        .ToList();
    }

    [HttpGet("thread/{userId:int}/{partnerId:int}")]
    public async Task<List<ChatMessageDto>> Thread(int userId, int partnerId)
    {
        var thread = await _db.Messages
            .Where(m => (m.SenderId == userId && m.RecipientId == partnerId) ||
                        (m.SenderId == partnerId && m.RecipientId == userId))
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        // Opening a thread marks it read.
        foreach (var m in thread.Where(m => m.RecipientId == userId && !m.IsRead))
            m.IsRead = true;
        await _db.SaveChangesAsync();

        return thread.Select(m => new ChatMessageDto(m.Id, m.SenderId, m.RecipientId, m.Content, m.SentAt)).ToList();
    }
}
