using MentorLink.Shared.Models;

namespace MentorLink.Shared.Dtos;

// ---- Auth ----
public record LoginRequest(string Email, string Password);
public record SignupRequest(string FullName, string Email, string Field, string Password, UserRole Role);
public record UserDto(int Id, string FullName, string Email, UserRole Role, string Field, string Bio, string LinkedInUrl, string TwitterUrl,
    string? PhotoUrl, bool EmailNotifs, bool InAppNotifs, bool RequestAlerts, bool GoalAlerts, bool IsPublic);
public record AuthResponse(UserDto User, string Token);
public record UpdateProfileRequest(string FullName, string Email, string Field, string Bio, string LinkedInUrl, string TwitterUrl, string? PhotoUrl);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
public record UpdatePreferencesRequest(bool EmailNotifs, bool InAppNotifs, bool RequestAlerts, bool GoalAlerts, bool IsPublic);

// ---- Mentors / discovery ----
public record MentorCardDto(int UserId, string Name, string Title, string Company, string Field, double Rating, int ReviewCount, bool Available, string Bio);
public record ReviewDto(string StudentName, int Rating, string Text);
public record MentorProfileDto(MentorCardDto Card, string FullBio, string LinkedInUrl, string TwitterUrl, string[] Skills, List<ReviewDto> Reviews);
public record CreateReviewRequest(string StudentName, int Rating, string Text);

// ---- Goals ----
public record MilestoneDto(int Id, string Title, bool IsDone);
public record GoalDto(int Id, string Title, GoalType Type, GoalStatus Status, string MentorName, List<MilestoneDto> Milestones)
{
    public int Progress => Milestones.Count == 0 ? 0
        : (int)Math.Round(100.0 * Milestones.Count(m => m.IsDone) / Milestones.Count);
}
public record CreateGoalRequest(int StudentId, string Title, GoalType Type, int? MentorUserId, List<string> Milestones);

// ---- Messaging ----
public record ConversationDto(int PartnerId, string PartnerName, string PartnerRoleLine, string LastMessage, DateTime LastAt, int Unread);
public record ChatMessageDto(int Id, int SenderId, int RecipientId, string Content, DateTime SentAt);
public record SendMessageRequest(int SenderId, int RecipientId, string Content);

// ---- Mentorship requests / mentorships ----
public record CreateMentorshipRequest(int StudentId, int MentorUserId, GoalType GoalType, string Message, string Frequency);
public record RequestDto(int Id, int StudentId, string StudentName, string StudentField, int MentorUserId, string MentorName,
    GoalType GoalType, string Message, string Frequency, RequestStatus Status, DateTime CreatedAt);
public record MentorshipDto(int Id, int PartnerId, string PartnerName, string PartnerRoleLine, string GoalTitle,
    int Progress, DateTime? LastSessionAt, MentorshipStatus Status, bool IsPendingRequest);

// ---- Notifications ----
public record NotificationDto(int Id, NotificationKind Kind, string Title, string Body, DateTime CreatedAt, bool IsRead);

// ---- Dashboards ----
public record StudentDashboardDto(int ActiveMentors, int GoalsInProgress, int UnreadMessages,
    List<MentorCardDto> Mentors, List<GoalDto> Goals);
public record MenteeRowDto(int StudentId, string Name, string Field, string GoalTitle, int Progress, DateTime? LastSessionAt);
public record MentorDashboardDto(int ActiveMentees, int PendingRequests, int SessionsThisMonth,
    List<RequestDto> Requests, List<MenteeRowDto> Mentees);

// ---- Admin ----
public record VerificationDto(int ProfileId, int UserId, string Name, string Field, string Company, string Title,
    DateTime AppliedOn, VerificationStatus Status, string LinkedInUrl);
