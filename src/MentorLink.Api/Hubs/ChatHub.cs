using MentorLink.Api.Data;
using MentorLink.Shared.Dtos;
using MentorLink.Shared.Models;
using Microsoft.AspNetCore.SignalR;

namespace MentorLink.Api.Hubs;

public class ChatHub : Hub
{
    private readonly AppDbContext _db;

    public ChatHub(AppDbContext db)
    {
        _db = db;
    }

    public async Task Register(int userId)
    {
        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            GetUserGroup(userId)
        );
    }

    public async Task SendMessage(SendMessageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return;

        var sender = await _db.Users.FindAsync(request.SenderId);

        if (sender == null)
            return;

        var message = CreateMessage(request);

        _db.Messages.Add(message);

        AddNotification(request.RecipientId, sender.FullName, message);

        await _db.SaveChangesAsync();

        var messageDto = CreateMessageDto(message);

        await SendMessageToUser(request.RecipientId, messageDto);
        await SendMessageToUser(request.SenderId, messageDto);
    }

    private static ChatMessage CreateMessage(SendMessageRequest request)
    {
        return new ChatMessage
        {
            SenderId = request.SenderId,
            RecipientId = request.RecipientId,
            Content = request.Content.Trim(),
            SentAt = DateTime.UtcNow,
            IsRead = false
        };
    }

    private void AddNotification(
        int recipientId,
        string senderName,
        ChatMessage message)
    {
        _db.Notifications.Add(new Notification
        {
            UserId = recipientId,
            Kind = NotificationKind.NewMessage,
            Title = $"New message from {senderName}",
            Body = $"\"{Truncate(message.Content, 80)}\"",
            CreatedAt = message.SentAt,
            IsRead = false
        });
    }

    private static ChatMessageDto CreateMessageDto(ChatMessage message)
    {
        return new ChatMessageDto(
            message.Id,
            message.SenderId,
            message.RecipientId,
            message.Content,
            message.SentAt
        );
    }

    private async Task SendMessageToUser(
        int userId,
        ChatMessageDto message)
    {
        await Clients
            .Group(GetUserGroup(userId))
            .SendAsync("ReceiveMessage", message);
    }

    private static string GetUserGroup(int userId)
    {
        return $"user-{userId}";
    }

    private static string Truncate(string text, int maxLength)
    {
        return text.Length <= maxLength
            ? text
            : text[..maxLength] + "…";
    }
}