using MentorLink.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace MentorLink.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<MentorProfile> MentorProfiles => Set<MentorProfile>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<MentorshipRequest> MentorshipRequests => Set<MentorshipRequest>();
    public DbSet<Mentorship> Mentorships => Set<Mentorship>();
    public DbSet<Goal> Goals => Set<Goal>();
    public DbSet<Milestone> Milestones => Set<Milestone>();
    public DbSet<ChatMessage> Messages => Set<ChatMessage>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Goal>()
            .HasMany(g => g.Milestones)
            .WithOne()
            .HasForeignKey(m => m.GoalId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
    }
}
