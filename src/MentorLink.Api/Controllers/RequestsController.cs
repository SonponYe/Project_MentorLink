using MentorLink.Api.Data;
using MentorLink.Shared.Dtos;
using MentorLink.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MentorLink.Api.Controllers;

[ApiController]
[Route("api/requests")]
public class RequestsController : ControllerBase
{
    private readonly AppDbContext _db;
    public RequestsController(AppDbContext db) => _db = db;

    [HttpPost]
    public async Task<ActionResult<RequestDto>> Create(CreateMentorshipRequest request)
    {
        var student = await _db.Users.FindAsync(request.StudentId);
        var mentor = await _db.Users.FindAsync(request.MentorUserId);
        if (student is null || mentor is null) return NotFound();

        var entity = new MentorshipRequest
        {
            StudentId = request.StudentId,
            MentorUserId = request.MentorUserId,
            GoalType = request.GoalType,
            Message = request.Message.Trim(),
            Frequency = request.Frequency,
            Status = RequestStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        _db.MentorshipRequests.Add(entity);

        _db.Notifications.Add(new Notification
        {
            UserId = mentor.Id,
            Kind = NotificationKind.RequestReceived,
            Title = "New mentorship request received",
            Body = $"{student.FullName} requested {Label(request.GoalType)} mentorship.",
            CreatedAt = entity.CreatedAt
        });

        await _db.SaveChangesAsync();
        return ToDto(entity, student, mentor);
    }

    [HttpPost("{id:int}/accept")]
    public async Task<IActionResult> Accept(int id)
    {
        var request = await _db.MentorshipRequests.FindAsync(id);
        if (request is null) return NotFound();
        if (request.Status != RequestStatus.Pending) return Conflict("Request already handled.");

        request.Status = RequestStatus.Accepted;

        _db.Mentorships.Add(new Mentorship
        {
            StudentId = request.StudentId,
            MentorUserId = request.MentorUserId,
            Status = MentorshipStatus.Active,
            StartedAt = DateTime.UtcNow
        });

        var mentor = await _db.Users.FindAsync(request.MentorUserId);
        _db.Notifications.Add(new Notification
        {
            UserId = request.StudentId,
            Kind = NotificationKind.RequestAccepted,
            Title = $"{mentor?.FullName} accepted your mentorship request",
            Body = $"Your {Label(request.GoalType)} mentorship is now active — say hello.",
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id:int}/decline")]
    public async Task<IActionResult> Decline(int id)
    {
        var request = await _db.MentorshipRequests.FindAsync(id);
        if (request is null) return NotFound();
        request.Status = RequestStatus.Declined;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    internal static string Label(GoalType type) => type switch
    {
        GoalType.PersonalGrowth => "Personal Growth",
        _ => type.ToString()
    };

    internal static RequestDto ToDto(MentorshipRequest r, User student, User mentor)
        => new(r.Id, r.StudentId, student.FullName, student.Field, r.MentorUserId, mentor.FullName,
            r.GoalType, r.Message, r.Frequency, r.Status, r.CreatedAt);
}
