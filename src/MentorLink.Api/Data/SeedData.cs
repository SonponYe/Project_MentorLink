using MentorLink.Shared.Models;

namespace MentorLink.Api.Data;

public static class SeedData
{
    public static void Ensure(AppDbContext db)
    {
        if (db.Users.Any()) return;

        var now = DateTime.UtcNow;

        // ---- Users ----
        var amara  = new User { FullName = "Amara Osei",        Email = "amara@student.dev",  Password = "demo1234", Role = UserRole.Student, Field = "Computer Science",     Bio = "Third-year CS student chasing a data engineering internship. Currently deep in SQL, slowly falling in love with schemas.", LinkedInUrl = "https://linkedin.com/in/amaraosei" };
        var david  = new User { FullName = "David Kim",         Email = "david@student.dev",  Password = "demo1234", Role = UserRole.Student, Field = "Visual Communication" };
        var priya  = new User { FullName = "Priya Sharma",      Email = "priya@student.dev",  Password = "demo1234", Role = UserRole.Student, Field = "Statistics" };
        var tunde  = new User { FullName = "Tunde Bakare",      Email = "tunde@student.dev",  Password = "demo1234", Role = UserRole.Student, Field = "Computer Science" };
        var fatima = new User { FullName = "Fatima Al-Hassan",  Email = "fatima@student.dev", Password = "demo1234", Role = UserRole.Student, Field = "Statistics" };

        var nnamdi = new User { FullName = "Dr. Nnamdi Okafor", Email = "nnamdi@mentor.dev",  Password = "demo1234", Role = UserRole.Mentor, Field = "Data Science",
            Bio = "I've spent twelve years building data teams at Microsoft and, before that, two fintech startups in Lagos and London. I mentor because someone did it for me: a manager who spent thirty minutes a week asking the questions nobody else would. I work best with students who have a concrete goal — an internship to land, a thesis to finish, a portfolio project that keeps stalling — and I'll hold you to the milestones we set together. Expect direct feedback, real-world datasets, and the occasional homework assignment.",
            LinkedInUrl = "https://linkedin.com/in/nnamdiokafor", TwitterUrl = "https://twitter.com/nnamdiokafor" };
        var sarah  = new User { FullName = "Sarah Chen",        Email = "sarah@mentor.dev",   Password = "demo1234", Role = UserRole.Mentor, Field = "Product Design",
            Bio = "Design Director at Figma. I coach early-career designers on portfolios, storytelling, and surviving their first design critique.", LinkedInUrl = "https://linkedin.com/in/sarahchen" };
        var james  = new User { FullName = "James Whitfield",   Email = "james@mentor.dev",   Password = "demo1234", Role = UserRole.Mentor, Field = "Finance",
            Bio = "VP at Goldman Sachs. I help students break into investment banking without the family network." };
        var leila  = new User { FullName = "Dr. Leila Haddad",  Email = "leila@mentor.dev",   Password = "demo1234", Role = UserRole.Mentor, Field = "Biomedical",
            Bio = "Research Lead at Genentech. From lab bench to first-author publication.", LinkedInUrl = "https://linkedin.com/in/leilahaddad" };
        var marcus = new User { FullName = "Marcus Reid",       Email = "marcus@mentor.dev",  Password = "demo1234", Role = UserRole.Mentor, Field = "Entrepreneurship",
            Bio = "Built and sold two startups; now a founder-turned-investor helping the next wave get their first hundred customers." };
        var elena  = new User { FullName = "Elena Petrova",     Email = "elena@mentor.dev",   Password = "demo1234", Role = UserRole.Mentor, Field = "Software Engineering",
            Bio = "Staff Engineer at Spotify. System design interviews, demystified.", LinkedInUrl = "https://linkedin.com/in/elenapetrova" };
        var robert = new User { FullName = "Robert Osei",       Email = "robert@mentor.dev",  Password = "demo1234", Role = UserRole.Mentor, Field = "Finance & Audit",
            Bio = "Senior Manager at KPMG Ghana.", LinkedInUrl = "https://linkedin.com/in/robertosei" };

        var admin  = new User { FullName = "Rita Adeyemi",      Email = "admin@mentorlink.dev", Password = "demo1234", Role = UserRole.Admin, Field = "Operations" };

        db.Users.AddRange(amara, david, priya, tunde, fatima, nnamdi, sarah, james, leila, marcus, elena, robert, admin);
        db.SaveChanges();

        // ---- Mentor profiles ----
        db.MentorProfiles.AddRange(
            new MentorProfile { UserId = nnamdi.Id, Title = "Principal Data Scientist", Company = "Microsoft",          Rating = 4.9, ReviewCount = 31, Available = true,  Verification = VerificationStatus.Approved, AppliedOn = now.AddMonths(-14), SkillsCsv = "Machine Learning,SQL & Data Modeling,Python,A/B Testing,Data Visualization,Career Strategy,Interview Prep,Research Methods,Team Leadership" },
            new MentorProfile { UserId = sarah.Id,  Title = "Design Director",          Company = "Figma",              Rating = 4.8, ReviewCount = 24, Available = true,  Verification = VerificationStatus.Approved, AppliedOn = now.AddMonths(-10), SkillsCsv = "Portfolio Coaching,UX Research,Design Systems,Storytelling,Critique,Career Strategy" },
            new MentorProfile { UserId = james.Id,  Title = "Vice President",           Company = "Goldman Sachs",      Rating = 4.7, ReviewCount = 18, Available = false, Verification = VerificationStatus.Approved, AppliedOn = now.AddMonths(-9),  SkillsCsv = "Investment Banking,Financial Modeling,Networking,Interview Prep" },
            new MentorProfile { UserId = marcus.Id, Title = "Founder & GP",             Company = "Northloop Ventures", Rating = 4.6, ReviewCount = 12, Available = true,  Verification = VerificationStatus.Approved, AppliedOn = now.AddMonths(-8),  SkillsCsv = "Fundraising,Go-To-Market,Public Speaking,Product Strategy" },
            new MentorProfile { UserId = elena.Id,  Title = "Staff Engineer",           Company = "Spotify",            Rating = 4.9, ReviewCount = 27, Available = true,  Verification = VerificationStatus.Approved, AppliedOn = now.AddMonths(-6),  SkillsCsv = "System Design,Distributed Systems,Code Review,Interview Prep,Mentoring Engineers" },
            new MentorProfile { UserId = leila.Id,  Title = "Research Lead",            Company = "Genentech",          Rating = 5.0, ReviewCount = 9,  Available = true,  Verification = VerificationStatus.Pending,  AppliedOn = now.AddDays(-2),    SkillsCsv = "Research Methods,Scientific Writing,Lab Skills,PhD Applications" },
            new MentorProfile { UserId = robert.Id, Title = "Senior Manager",           Company = "KPMG Ghana",         Rating = 0,   ReviewCount = 0,  Available = true,  Verification = VerificationStatus.Pending,  AppliedOn = now.AddDays(-3),    SkillsCsv = "Audit,Accounting,Career Strategy" }
        );

        // ---- Reviews for Dr. Okafor ----
        db.Reviews.AddRange(
            new Review { MentorUserId = nnamdi.Id, StudentName = "Amara Osei",   Rating = 5, Text = "Dr. Okafor doesn't let you coast. Every session ends with a concrete next step, and he actually checks whether you did it. My SQL went from shaky to interview-ready in two months." },
            new Review { MentorUserId = nnamdi.Id, StudentName = "David Kim",    Rating = 5, Text = "He reviewed my internship application line by line and told me exactly what a hiring manager would think. Harsh in the best possible way." },
            new Review { MentorUserId = nnamdi.Id, StudentName = "Priya Sharma", Rating = 4, Text = "Generous with his time and his network. He introduced me to two people on his team when I was researching my thesis topic." },
            new Review { MentorUserId = sarah.Id,  StudentName = "Amara Osei",   Rating = 5, Text = "Sarah rebuilt how I present my work. My portfolio finally tells a story instead of listing artifacts." },
            new Review { MentorUserId = elena.Id,  StudentName = "Tunde Bakare", Rating = 5, Text = "Elena's mock system design interviews are brutal and brilliant. Passed my real one first try." }
        );

        // ---- Goals + milestones ----
        var goalSql = new Goal { StudentId = amara.Id, MentorUserId = nnamdi.Id, Title = "Master SQL & Database Design", Type = GoalType.Academic, Status = GoalStatus.InProgress,
            Milestones = { new Milestone { Title = "Complete the normalization module", IsDone = true },
                           new Milestone { Title = "Design a sample schema for MentorLink" },
                           new Milestone { Title = "Pass the mock database assessment" } } };
        var goalIntern = new Goal { StudentId = amara.Id, MentorUserId = sarah.Id, Title = "Land a Summer Internship", Type = GoalType.Career, Status = GoalStatus.InProgress,
            Milestones = { new Milestone { Title = "Rewrite CV with measurable outcomes", IsDone = true },
                           new Milestone { Title = "Publish portfolio case study", IsDone = true },
                           new Milestone { Title = "Complete three mock interviews" } } };
        var goalSpeak = new Goal { StudentId = amara.Id, MentorUserId = marcus.Id, Title = "Public Speaking Confidence", Type = GoalType.PersonalGrowth, Status = GoalStatus.Completed,
            Milestones = { new Milestone { Title = "Deliver a 5-minute lightning talk", IsDone = true },
                           new Milestone { Title = "Present at the student tech society", IsDone = true },
                           new Milestone { Title = "Host a workshop session", IsDone = true } } };
        var goalSys = new Goal { StudentId = amara.Id, MentorUserId = elena.Id, Title = "Ace the System Design Interview", Type = GoalType.Career, Status = GoalStatus.InProgress,
            Milestones = { new Milestone { Title = "Read Designing Data-Intensive Applications, ch. 1–4", IsDone = true },
                           new Milestone { Title = "Design a URL shortener end to end" },
                           new Milestone { Title = "Two mock interviews with Elena" },
                           new Milestone { Title = "Write up learnings" },
                           new Milestone { Title = "Final mock with a stranger" } } };
        var goalViz = new Goal { StudentId = david.Id, MentorUserId = nnamdi.Id, Title = "Break Into Data Visualization", Type = GoalType.Career, Status = GoalStatus.InProgress,
            Milestones = { new Milestone { Title = "Recreate three classic charts from scratch", IsDone = true },
                           new Milestone { Title = "Publish a small D3 explainer" },
                           new Milestone { Title = "Ship a portfolio dashboard piece" } } };
        var goalThesis = new Goal { StudentId = priya.Id, MentorUserId = nnamdi.Id, Title = "Publish Thesis Research", Type = GoalType.Academic, Status = GoalStatus.InProgress,
            Milestones = { new Milestone { Title = "Finalize methodology", IsDone = true },
                           new Milestone { Title = "Complete analysis", IsDone = true },
                           new Milestone { Title = "Draft paper", IsDone = true },
                           new Milestone { Title = "Submit to journal" },
                           new Milestone { Title = "Respond to reviewers" } } };
        db.Goals.AddRange(goalSql, goalIntern, goalSpeak, goalSys, goalViz, goalThesis);

        // ---- Mentorships ----
        db.Mentorships.AddRange(
            new Mentorship { StudentId = amara.Id, MentorUserId = nnamdi.Id, Status = MentorshipStatus.Active,    StartedAt = now.AddMonths(-2), LastSessionAt = now.AddDays(-7) },
            new Mentorship { StudentId = amara.Id, MentorUserId = sarah.Id,  Status = MentorshipStatus.Active,    StartedAt = now.AddMonths(-3), LastSessionAt = now.AddDays(-4) },
            new Mentorship { StudentId = amara.Id, MentorUserId = elena.Id,  Status = MentorshipStatus.Active,    StartedAt = now.AddDays(-20),  LastSessionAt = now.AddDays(-2) },
            new Mentorship { StudentId = amara.Id, MentorUserId = marcus.Id, Status = MentorshipStatus.Completed, StartedAt = now.AddMonths(-5), LastSessionAt = now.AddDays(-12), CompletedAt = now.AddDays(-10) },
            new Mentorship { StudentId = david.Id, MentorUserId = nnamdi.Id, Status = MentorshipStatus.Active,    StartedAt = now.AddMonths(-1), LastSessionAt = now.AddDays(-2) },
            new Mentorship { StudentId = priya.Id, MentorUserId = nnamdi.Id, Status = MentorshipStatus.Active,    StartedAt = now.AddMonths(-4), LastSessionAt = now.AddDays(-13) }
        );

        // ---- Mentorship requests ----
        db.MentorshipRequests.AddRange(
            new MentorshipRequest { StudentId = amara.Id,  MentorUserId = james.Id,  GoalType = GoalType.Career,   Frequency = "Weekly",  Status = RequestStatus.Pending, CreatedAt = now.AddDays(-1),
                Message = "I'm exploring finance as a second track alongside CS and would love structured guidance on breaking into IB internships." },
            new MentorshipRequest { StudentId = tunde.Id,  MentorUserId = nnamdi.Id, GoalType = GoalType.Career,   Frequency = "Weekly",  Status = RequestStatus.Pending, CreatedAt = now.AddDays(-3),
                Message = "I'm targeting data engineering internships next summer and my SQL is weak. Your mentee reviews mention exactly the kind of structure I need." },
            new MentorshipRequest { StudentId = fatima.Id, MentorUserId = nnamdi.Id, GoalType = GoalType.Academic, Frequency = "Monthly", Status = RequestStatus.Pending, CreatedAt = now.AddDays(-1),
                Message = "My dissertation uses ML methods I've only seen in lectures. I'm hoping for monthly check-ins to keep the analysis honest." }
        );

        // ---- Chat messages (Amara <-> mentors) ----
        db.Messages.AddRange(
            new ChatMessage { SenderId = nnamdi.Id, RecipientId = amara.Id, SentAt = now.AddHours(-3.5), IsRead = true,  Content = "Amara — how did the normalization module go? You were aiming to finish it this week." },
            new ChatMessage { SenderId = amara.Id,  RecipientId = nnamdi.Id, SentAt = now.AddHours(-3.2), IsRead = true, Content = "Finished it last night! Third normal form finally clicked once I redrew the tables by hand." },
            new ChatMessage { SenderId = amara.Id,  RecipientId = nnamdi.Id, SentAt = now.AddHours(-3.1), IsRead = true, Content = "I've started sketching the schema for the MentorLink sample project. Should I include the messaging tables too?" },
            new ChatMessage { SenderId = nnamdi.Id, RecipientId = amara.Id, SentAt = now.AddHours(-2.6), IsRead = true,  Content = "Yes — messages are the interesting part. Think about how you'd model read receipts." },
            new ChatMessage { SenderId = nnamdi.Id, RecipientId = amara.Id, SentAt = now.AddHours(-2.5), IsRead = true,  Content = "Great — send me the schema before Friday and we'll walk through it in our session." },

            new ChatMessage { SenderId = sarah.Id, RecipientId = amara.Id, SentAt = now.AddHours(-6),   IsRead = false, Content = "I left comments on your case study draft." },
            new ChatMessage { SenderId = sarah.Id, RecipientId = amara.Id, SentAt = now.AddHours(-5.9), IsRead = false, Content = "The intro is strong — the middle section needs the 'so what'. Let's talk Tuesday." },

            new ChatMessage { SenderId = james.Id, RecipientId = amara.Id, SentAt = now.AddDays(-1.2), IsRead = true, Content = "Let's push our intro chat to Thursday." },

            new ChatMessage { SenderId = elena.Id, RecipientId = amara.Id, SentAt = now.AddDays(-2.1), IsRead = false, Content = "That system design doc is a great start." },
            new ChatMessage { SenderId = elena.Id, RecipientId = amara.Id, SentAt = now.AddDays(-2.0), IsRead = false, Content = "Two things to think about: how does the cache invalidate, and what happens when a region goes down?" },
            new ChatMessage { SenderId = elena.Id, RecipientId = amara.Id, SentAt = now.AddDays(-2.0), IsRead = false, Content = "Bring answers to our next session and we'll whiteboard it." },

            new ChatMessage { SenderId = marcus.Id, RecipientId = amara.Id, SentAt = now.AddDays(-10), IsRead = true, Content = "Congrats on wrapping the mentorship — your workshop was genuinely good. Go be loud about your work." }
        );

        // ---- Notifications (for Amara) ----
        db.Notifications.AddRange(
            new Notification { UserId = amara.Id, Kind = NotificationKind.RequestAccepted,    CreatedAt = now.AddHours(-2), IsRead = false, Title = "Dr. Nnamdi Okafor accepted your mentorship request", Body = "Your Academic mentorship is now active — say hello." },
            new Notification { UserId = amara.Id, Kind = NotificationKind.NewMessage,         CreatedAt = now.AddHours(-6), IsRead = false, Title = "New message from Sarah Chen", Body = "\"I left comments on your case study draft.\"" },
            new Notification { UserId = amara.Id, Kind = NotificationKind.MilestoneCompleted, CreatedAt = now.AddHours(-9), IsRead = false, Title = "Milestone completed: Publish portfolio case study", Body = "Land a Summer Internship is now 67% complete." },
            new Notification { UserId = amara.Id, Kind = NotificationKind.NewMessage,         CreatedAt = now.AddDays(-2),  IsRead = true,  Title = "New message from Elena Petrova", Body = "\"That system design doc is a great start.\"" },
            new Notification { UserId = amara.Id, Kind = NotificationKind.MilestoneCompleted, CreatedAt = now.AddDays(-5),  IsRead = true,  Title = "Milestone completed: Rewrite CV with measurable outcomes", Body = "Nice work — two milestones to go." },
            new Notification { UserId = amara.Id, Kind = NotificationKind.System,             CreatedAt = now.AddDays(-8),  IsRead = true,  Title = "System update: session scheduling is here", Body = "Book recurring sessions directly from any conversation." },

            new Notification { UserId = nnamdi.Id, Kind = NotificationKind.RequestReceived, CreatedAt = now.AddDays(-3), IsRead = false, Title = "New mentorship request received", Body = "Tunde Bakare requested Career mentorship." },
            new Notification { UserId = nnamdi.Id, Kind = NotificationKind.RequestReceived, CreatedAt = now.AddDays(-1), IsRead = false, Title = "New mentorship request received", Body = "Fatima Al-Hassan requested Academic mentorship." }
        );

        db.SaveChanges();
    }
}
