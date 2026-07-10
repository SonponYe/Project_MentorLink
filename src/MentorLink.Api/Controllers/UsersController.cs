using MentorLink.Api.Data;
using MentorLink.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace MentorLink.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    public UsersController(AppDbContext db) => _db = db;

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> Get(int id)
    {
        var user = await _db.Users.FindAsync(id);
        return user is null ? NotFound() : AuthController.ToDto(user);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDto>> Update(int id, UpdateProfileRequest request)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();

        user.FullName = request.FullName.Trim();
        user.Email = request.Email.ToLower().Trim();
        user.Field = request.Field.Trim();
        user.Bio = request.Bio;
        user.LinkedInUrl = request.LinkedInUrl;
        user.TwitterUrl = request.TwitterUrl;
        await _db.SaveChangesAsync();

        return AuthController.ToDto(user);
    }
}
