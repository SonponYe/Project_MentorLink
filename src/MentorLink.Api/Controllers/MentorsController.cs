using MentorLink.Api.Data;
using MentorLink.Shared.Dtos;
using MentorLink.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MentorLink.Api.Controllers;

[ApiController]
[Route("api/mentors")]
[Authorize]
public class MentorsController : ControllerBase
{
    private readonly AppDbContext _db;
    public MentorsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<List<MentorCardDto>> Discover([FromQuery] string? search = null)
    {
        var query =
            from p in _db.MentorProfiles
            join u in _db.Users on p.UserId equals u.Id
            where p.Verification == VerificationStatus.Approved
            select new { p, u };

        var rows = await query.ToListAsync();

        var cards = rows.Select(x => new MentorCardDto(
            x.u.Id, x.u.FullName, x.p.Title, x.p.Company, x.u.Field,
            x.p.Rating, x.p.ReviewCount, x.p.Available, FirstSentence(x.u.Bio)));

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            cards = cards.Where(c =>
                c.Name.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                c.Field.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                c.Company.Contains(s, StringComparison.OrdinalIgnoreCase));
        }

        return cards.OrderByDescending(c => c.Rating).ToList();
    }

    [HttpGet("{userId:int}")]
    public async Task<ActionResult<MentorProfileDto>> Profile(int userId)
    {
        var user = await _db.Users.FindAsync(userId);
        var profile = await _db.MentorProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (user is null || profile is null) return NotFound();

        var reviews = await _db.Reviews
            .Where(r => r.MentorUserId == userId)
            .Select(r => new ReviewDto(r.StudentName, r.Rating, r.Text))
            .ToListAsync();

        var card = new MentorCardDto(user.Id, user.FullName, profile.Title, profile.Company, user.Field,
            profile.Rating, profile.ReviewCount, profile.Available, FirstSentence(user.Bio));

        var skills = profile.SkillsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return new MentorProfileDto(card, user.Bio, user.LinkedInUrl, user.TwitterUrl, skills, reviews);
    }

    [HttpPost("{userId:int}/reviews")]
    public async Task<ActionResult<MentorProfileDto>> AddReview(int userId, CreateReviewRequest request)
    {
        var profile = await _db.MentorProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile is null) return NotFound();

        var rating = Math.Clamp(request.Rating, 1, 5);
        _db.Reviews.Add(new Review
        {
            MentorUserId = userId,
            StudentName = request.StudentName.Trim(),
            Rating = rating,
            Text = request.Text.Trim()
        });

        profile.Rating = (profile.Rating * profile.ReviewCount + rating) / (profile.ReviewCount + 1);
        profile.ReviewCount++;
        await _db.SaveChangesAsync();

        return await Profile(userId);
    }

    internal static string FirstSentence(string bio)
    {
        if (string.IsNullOrWhiteSpace(bio)) return "";
        var idx = bio.IndexOf('.');
        return idx > 0 && idx < bio.Length - 1 ? bio[..(idx + 1)] : bio;
    }
}
