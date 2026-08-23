using MentorLink.Api.Data;
using MentorLink.Shared.Dtos;
using MentorLink.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MentorLink.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;
    public DashboardController(AppDbContext db) => _db = db;

    [HttpGet("student/{studentId:int}")]
    public async Task<StudentDashboardDto> Student(int studentId)
    {
        var mentorships = await _db.Mentorships
            .Where(m => m.StudentId == studentId && m.Status == MentorshipStatus.Active)
            .ToListAsync();
        var mentorIds = mentorships.Select(m => m.MentorUserId).ToList();

        var mentors = await (
            from p in _db.MentorProfiles
            join u in _db.Users on p.UserId equals u.Id
            where mentorIds.Contains(p.UserId)
            select new MentorCardDto(u.Id, u.FullName, p.Title, p.Company, u.Field,
                p.Rating, p.ReviewCount, p.Available, "")
        ).ToListAsync();

        var goals = await _db.Goals.Include(g => g.Milestones)
            .Where(g => g.StudentId == studentId && g.Status == GoalStatus.InProgress)
            .ToListAsync();
        var names = await _db.Users.ToDictionaryAsync(u => u.Id, u => u.FullName);

        var unread = await _db.Messages.CountAsync(m => m.RecipientId == studentId && !m.IsRead);

        return new StudentDashboardDto(
            mentorships.Count,
            goals.Count,
            unread,
            mentors,
            goals.Select(g => GoalsController.ToDto(g, names)).ToList());
    }

    [HttpGet("mentor/{mentorUserId:int}")]
    public async Task<MentorDashboardDto> Mentor(int mentorUserId)
    {
        var mentorships = await _db.Mentorships
            .Where(m => m.MentorUserId == mentorUserId && m.Status == MentorshipStatus.Active)
            .ToListAsync();

        var pending = await _db.MentorshipRequests
            .Where(r => r.MentorUserId == mentorUserId && r.Status == RequestStatus.Pending)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();

        var users = await _db.Users.ToDictionaryAsync(u => u.Id);
        var mentor = users[mentorUserId];

        var requests = pending
            .Select(r => RequestsController.ToDto(r, users[r.StudentId], mentor))
            .ToList();

        var studentIds = mentorships.Select(m => m.StudentId).ToList();
        var goals = await _db.Goals.Include(g => g.Milestones)
            .Where(g => g.MentorUserId == mentorUserId && studentIds.Contains(g.StudentId))
            .ToListAsync();

        var mentees = mentorships.Select(m =>
        {
            var student = users[m.StudentId];
            var goal = goals.FirstOrDefault(g => g.StudentId == m.StudentId);
            var progress = goal is null || goal.Milestones.Count == 0 ? 0
                : (int)Math.Round(100.0 * goal.Milestones.Count(x => x.IsDone) / goal.Milestones.Count);
            return new MenteeRowDto(student.Id, student.FullName, student.Field,
                goal?.Title ?? "No goal yet", progress, m.LastSessionAt);
        }).ToList();

        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var sessions = mentorships.Count(m => m.LastSessionAt >= monthStart);

        return new MentorDashboardDto(mentorships.Count, pending.Count, sessions, requests, mentees);
    }
}
