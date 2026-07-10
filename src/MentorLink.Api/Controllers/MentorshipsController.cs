using MentorLink.Api.Data;
using MentorLink.Shared.Dtos;
using MentorLink.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MentorLink.Api.Controllers;

[ApiController]
[Route("api/mentorships")]
public class MentorshipsController : ControllerBase
{
    private readonly AppDbContext _db;
    public MentorshipsController(AppDbContext db) => _db = db;

    /// Active + completed mentorships, plus pending requests, for the student's
    /// My Mentorships screen (Active / Pending / Past tabs).
    [HttpGet("student/{studentId:int}")]
    public async Task<List<MentorshipDto>> ForStudent(int studentId)
    {
        var mentorships = await _db.Mentorships.Where(m => m.StudentId == studentId).ToListAsync();
        var pending = await _db.MentorshipRequests
            .Where(r => r.StudentId == studentId && r.Status == RequestStatus.Pending)
            .ToListAsync();
        var goals = await _db.Goals.Include(g => g.Milestones)
            .Where(g => g.StudentId == studentId)
            .ToListAsync();
        var users = await _db.Users.ToDictionaryAsync(u => u.Id);
        var profiles = await _db.MentorProfiles.ToDictionaryAsync(p => p.UserId);

        string RoleLine(int mentorUserId) =>
            profiles.TryGetValue(mentorUserId, out var p)
                ? $"{users[mentorUserId].Field} · {p.Company}"
                : users[mentorUserId].Field;

        var result = mentorships.Select(m =>
        {
            var goal = goals.FirstOrDefault(g => g.MentorUserId == m.MentorUserId);
            var progress = goal is null || goal.Milestones.Count == 0 ? 0
                : (int)Math.Round(100.0 * goal.Milestones.Count(x => x.IsDone) / goal.Milestones.Count);
            return new MentorshipDto(m.Id, m.MentorUserId, users[m.MentorUserId].FullName, RoleLine(m.MentorUserId),
                goal?.Title ?? "No shared goal yet", progress, m.LastSessionAt, m.Status, false);
        }).ToList();

        result.AddRange(pending.Select(r =>
            new MentorshipDto(r.Id, r.MentorUserId, users[r.MentorUserId].FullName, RoleLine(r.MentorUserId),
                $"{RequestsController.Label(r.GoalType)} mentorship requested", 0, null, MentorshipStatus.Active, true)));

        return result;
    }
}
