using System.Net.Http.Json;
using MentorLink.Shared.Dtos;

namespace MentorLink.Client.Services;

/// <summary>Typed wrapper over the MentorLink Web API.</summary>
public class Api
{
    private readonly HttpClient _http;
    public Api(HttpClient http) => _http = http;

    // ---- Auth ----
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", request);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<AuthResponse>() : null;
    }

    public async Task<(AuthResponse? Auth, string? Error)> SignupAsync(SignupRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/auth/signup", request);
        if (response.IsSuccessStatusCode)
            return (await response.Content.ReadFromJsonAsync<AuthResponse>(), null);
        return (null, await response.Content.ReadAsStringAsync());
    }

    public async Task<UserDto?> UpdateProfileAsync(int userId, UpdateProfileRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/users/{userId}", request);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<UserDto>() : null;
    }

    public async Task<UserDto?> UpdatePreferencesAsync(int userId, UpdatePreferencesRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/users/{userId}/preferences", request);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<UserDto>() : null;
    }

    public async Task<string?> ChangePasswordAsync(int userId, ChangePasswordRequest request)
    {
        var response = await _http.PostAsJsonAsync($"api/users/{userId}/change-password", request);
        return response.IsSuccessStatusCode ? null : (await response.Content.ReadAsStringAsync()).Trim('"');
    }

    // ---- Mentors ----
    public Task<List<MentorCardDto>?> DiscoverMentorsAsync(string? search = null)
        => _http.GetFromJsonAsync<List<MentorCardDto>>(
            string.IsNullOrWhiteSpace(search) ? "api/mentors" : $"api/mentors?search={Uri.EscapeDataString(search)}");

    public Task<MentorProfileDto?> GetMentorProfileAsync(int userId)
        => _http.GetFromJsonAsync<MentorProfileDto>($"api/mentors/{userId}");

    public async Task<MentorProfileDto?> AddReviewAsync(int mentorUserId, CreateReviewRequest request)
    {
        var response = await _http.PostAsJsonAsync($"api/mentors/{mentorUserId}/reviews", request);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<MentorProfileDto>() : null;
    }

    // ---- Goals ----
    public Task<List<GoalDto>?> GetGoalsAsync(int studentId)
        => _http.GetFromJsonAsync<List<GoalDto>>($"api/goals/student/{studentId}");

    public async Task<GoalDto?> ToggleMilestoneAsync(int milestoneId)
    {
        var response = await _http.PostAsync($"api/goals/milestones/{milestoneId}/toggle", null);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<GoalDto>() : null;
    }

    public async Task<GoalDto?> CreateGoalAsync(CreateGoalRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/goals", request);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<GoalDto>() : null;
    }

    // ---- Messaging ----
    public Task<List<ConversationDto>?> GetConversationsAsync(int userId)
        => _http.GetFromJsonAsync<List<ConversationDto>>($"api/messages/conversations/{userId}");

    public Task<List<ChatMessageDto>?> GetThreadAsync(int userId, int partnerId)
        => _http.GetFromJsonAsync<List<ChatMessageDto>>($"api/messages/thread/{userId}/{partnerId}");

    // ---- Requests / mentorships ----
    public async Task<bool> CreateRequestAsync(CreateMentorshipRequest request)
        => (await _http.PostAsJsonAsync("api/requests", request)).IsSuccessStatusCode;

    public async Task AcceptRequestAsync(int id) => await _http.PostAsync($"api/requests/{id}/accept", null);
    public async Task DeclineRequestAsync(int id) => await _http.PostAsync($"api/requests/{id}/decline", null);

    public Task<List<MentorshipDto>?> GetMentorshipsAsync(int studentId)
        => _http.GetFromJsonAsync<List<MentorshipDto>>($"api/mentorships/student/{studentId}");

    // ---- Notifications ----
    public Task<List<NotificationDto>?> GetNotificationsAsync(int userId)
        => _http.GetFromJsonAsync<List<NotificationDto>>($"api/notifications/{userId}");

    public async Task MarkAllReadAsync(int userId)
        => await _http.PostAsync($"api/notifications/{userId}/read-all", null);

    // ---- Dashboards ----
    public Task<StudentDashboardDto?> GetStudentDashboardAsync(int studentId)
        => _http.GetFromJsonAsync<StudentDashboardDto>($"api/dashboard/student/{studentId}");

    public Task<MentorDashboardDto?> GetMentorDashboardAsync(int mentorUserId)
        => _http.GetFromJsonAsync<MentorDashboardDto>($"api/dashboard/mentor/{mentorUserId}");

    // ---- Admin ----
    public Task<List<VerificationDto>?> GetVerificationsAsync()
        => _http.GetFromJsonAsync<List<VerificationDto>>("api/admin/verifications");

    public async Task ApproveVerificationAsync(int profileId)
        => await _http.PostAsync($"api/admin/verifications/{profileId}/approve", null);

    public async Task RejectVerificationAsync(int profileId)
        => await _http.PostAsync($"api/admin/verifications/{profileId}/reject", null);
}
