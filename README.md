# MentorLink

Mentorship platform connecting students with verified professional mentors — structured goals, milestone tracking, and real-time chat.

**Stack:** ASP.NET Core Web API · Blazor WebAssembly · SignalR · Entity Framework Core · PostgreSQL (Supabase)

## Project layout

```
MentorLink.sln
src/
  MentorLink.Shared/    Entities + DTOs shared by client and server
  MentorLink.Api/       ASP.NET Core Web API + SignalR hub + EF Core (hosts the client)
  MentorLink.Client/    Blazor WebAssembly front end (all 14 screens)
```

## Run it

```bash
dotnet run --project src/MentorLink.Api
```

Then open http://localhost:5180. The Blazor client is served by the API, so that one command runs everything.

**Demo accounts** (password `demo1234`):

| Role    | Email                  | Lands on              |
|---------|------------------------|-----------------------|
| Student | `amara@student.dev`    | Student Dashboard     |
| Mentor  | `nnamdi@mentor.dev`    | Mentor Dashboard      |
| Admin   | `admin@mentorlink.dev` | Mentor Verification   |

Tip: open two browser windows (one normal, one private), sign in as Amara in one and Dr. Okafor in the other, and chat — messages arrive live over SignalR.

## Database — Supabase (PostgreSQL)

Out of the box the app uses an **in-memory database** with seed data, so it runs with zero setup (data resets on restart).

To switch to Supabase:

1. In your Supabase project go to **Settings → Database → Connection string** and copy the direct connection values.
2. Put them in `src/MentorLink.Api/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=db.YOUR-PROJECT-REF.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=YOUR-DB-PASSWORD;Ssl Mode=Require;Trust Server Certificate=true"
}
```

3. Restart the app. On first run EF Core creates the schema (`EnsureCreated`) and seeds the demo data.

> If your network blocks direct connections (IPv6), use Supabase's **session pooler** connection string instead — same format, host like `aws-0-REGION.pooler.supabase.com`, port `5432`, username `postgres.YOUR-PROJECT-REF`.

## The 14 screens

| # | Screen | Route |
|---|--------|-------|
| 1 | Landing Page | `/` |
| 2 | Sign Up | `/signup` |
| 3 | Login | `/login` |
| 4 | Student Dashboard | `/dashboard` |
| 5 | Discover Mentors | `/discover` |
| 6 | Mentor Profile | `/mentor/{id}` |
| 7 | Mentorship Request | `/request/{mentorId}` |
| 8 | Messages (SignalR) | `/messages` |
| 9 | Goal Tracker | `/goals` |
| 10 | Mentor Dashboard | `/mentor-dashboard` |
| 11 | Notifications | `/notifications` |
| 12 | Settings & Profile | `/settings` |
| 13 | My Mentorships | `/mentorships` |
| 14 | Admin — Mentor Verification | `/admin/verifications` |

## Notes / known simplifications (fine for an MVP, fix before production)

- Passwords are stored in plain text and there's no token auth — swap in ASP.NET Identity or Supabase Auth.
- Schema is created with `EnsureCreated()` — move to EF Core migrations (`dotnet ef migrations add Init`) once the model stabilizes.
- File attachments in chat and photo upload are placeholders.
