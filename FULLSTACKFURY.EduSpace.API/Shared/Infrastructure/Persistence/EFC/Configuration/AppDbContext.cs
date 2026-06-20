using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.ValueObjects;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IoTMonitoring.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Entities;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Configuration;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<ActivationToken> ActivationTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.AddCreatedUpdatedInterceptor();
        base.OnConfiguring(builder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ── IAM ──────────────────────────────────────────────────────────────────

        builder.Entity<Account>().HasKey(a => a.Id);
        builder.Entity<Account>().Property(a => a.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Account>().Property(a => a.Username).IsRequired();
        builder.Entity<Account>().Property(a => a.PasswordHash).IsRequired();
        builder.Entity<Account>().Property(a => a.Role).IsRequired();
        builder.Entity<Account>().Property(a => a.IsActive).IsRequired().HasDefaultValue(false);

        builder.Entity<RefreshToken>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.AccountId).IsRequired();
            e.Property(x => x.TokenHash).IsRequired().HasMaxLength(128);
            e.HasIndex(x => x.TokenHash).IsUnique();
            e.HasIndex(x => x.ExpiresAt);
            e.HasIndex(x => x.AccountId);
            e.HasOne<Account>()
                .WithMany()
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ActivationToken>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.AccountId).IsRequired();
            e.Property(x => x.TokenHash).IsRequired().HasMaxLength(64);
            e.HasIndex(x => x.TokenHash).IsUnique();
            e.Property(x => x.ExpiresAt).IsRequired();
            e.Property(x => x.UsedAt).IsRequired(false);
            e.HasOne<Account>()
                .WithMany()
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Profiles ─────────────────────────────────────────────────────────────

        builder.Entity<TeacherProfile>().HasKey(tp => tp.Id);
        builder.Entity<TeacherProfile>().Property(tp => tp.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<TeacherProfile>().Property(tp => tp.AdministratorId).IsRequired();
        builder.Entity<TeacherProfile>().OwnsOne(tp => tp.ProfileName,
            pn =>
            {
                pn.WithOwner().HasForeignKey("Id");
                pn.Property(tp => tp.FirstName).HasColumnName("FirstName");
                pn.Property(tp => tp.LastName).HasColumnName("LastName");
            });

        builder.Entity<TeacherProfile>().OwnsOne(tp => tp.ProfilePrivateInformation,
            pi =>
            {
                pi.WithOwner().HasForeignKey("Id");
                pi.Property(tp => tp.Email).HasColumnName("Email");
                pi.Property(tp => tp.Dni).HasColumnName("Dni");
                pi.Property(tp => tp.Address).HasColumnName("Address");
                pi.Property(tp => tp.Phone).HasColumnName("Phone");
            });

        builder.Entity<AdminProfile>().HasKey(ap => ap.Id);
        builder.Entity<AdminProfile>().Property(ap => ap.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<AdminProfile>().OwnsOne(ap => ap.ProfileName,
            pn =>
            {
                pn.WithOwner().HasForeignKey("Id");
                pn.Property(ap => ap.FirstName).HasColumnName("FirstName");
                pn.Property(ap => ap.LastName).HasColumnName("LastName");
            });
        builder.Entity<AdminProfile>().OwnsOne(ap => ap.ProfilePrivateInformation,
            pi =>
            {
                pi.WithOwner().HasForeignKey("Id");
                pi.Property(tp => tp.Email).HasColumnName("Email");
                pi.Property(tp => tp.Dni).HasColumnName("Dni");
                pi.Property(tp => tp.Address).HasColumnName("Address");
                pi.Property(tp => tp.Phone).HasColumnName("Phone");
            });

        // ── Spaces & Resource Management ─────────────────────────────────────────

        builder.Entity<Classroom>().HasKey(c => c.Id);
        builder.Entity<Classroom>().Property(c => c.Name).IsRequired();
        builder.Entity<Classroom>().Property(c => c.Description).IsRequired();
        builder.Entity<Classroom>().OwnsOne(r => r.TeacherId,
            ti =>
            {
                ti.WithOwner().HasForeignKey("Id");
                ti.Property(r => r.TeacherIdentifier).HasColumnName("TeacherId");
            });
        builder.Entity<Classroom>().Property(c => c.ZoneId).IsRequired(false).HasMaxLength(64);
        // Unique classroom name to prevent duplicates
        builder.Entity<Classroom>().HasIndex(c => c.Name).IsUnique();

        builder.Entity<Resource>().HasKey(r => r.Id);
        builder.Entity<Resource>().Property(r => r.Name).IsRequired();
        builder.Entity<Resource>().Property(r => r.KindOfResource).IsRequired();
        builder.Entity<Resource>()
            .HasOne(r => r.Classroom)
            .WithMany(c => c.Resources)
            .HasForeignKey(r => r.ClassroomId)
            .OnDelete(DeleteBehavior.Cascade);
        // Index for name scoped to classroom (resource names must be unique per classroom)
        builder.Entity<Resource>().HasIndex(r => new { r.ClassroomId, r.Name }).IsUnique();

        builder.Entity<SharedArea>().HasKey(sa => sa.Id);
        builder.Entity<SharedArea>().Property(sa => sa.Name).IsRequired();
        builder.Entity<SharedArea>().Property(sa => sa.Capacity).IsRequired();
        builder.Entity<SharedArea>().Property(sa => sa.Description).IsRequired();
        builder.Entity<SharedArea>().Property(sa => sa.ZoneId).IsRequired(false).HasMaxLength(64);

        // Reserve a shared space
        
        builder.Entity<SharedAreaReservation>().HasKey(sr => sr.Id);
        builder.Entity<SharedAreaReservation>().Property(sr => sr.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<SharedAreaReservation>().Property(sr => sr.SharedAreaId).IsRequired();
        builder.Entity<SharedAreaReservation>().Property(sr => sr.TeacherId).IsRequired();
        builder.Entity<SharedAreaReservation>().Property(sr => sr.ReservationDate).IsRequired();
        builder.Entity<SharedAreaReservation>().Property(sr => sr.ReservationDate)
            .HasConversion(v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v));

        builder.Entity<SharedAreaReservation>().Property(sr => sr.StartTime)
            .HasConversion(v => v.ToTimeSpan(), v => TimeOnly.FromTimeSpan(v));
        
        builder.Entity<SharedAreaReservation>().Property(sr => sr.EndTime)
            .HasConversion(v => v.ToTimeSpan(), v => TimeOnly.FromTimeSpan(v));
        
        builder.Entity<SharedAreaReservation>().Property(sr => sr.StartTime).IsRequired();
        builder.Entity<SharedAreaReservation>().Property(sr => sr.EndTime).IsRequired();        
        builder.Entity<SharedAreaReservation>().Property(sr => sr.Reason).IsRequired();     
        builder.Entity<SharedAreaReservation>().Property(sr => sr.CreatedAt).IsRequired();      
        builder.Entity<SharedAreaReservation>()
            .HasOne(sr => sr.SharedArea)
            .WithMany()
            .HasForeignKey(sr => sr.SharedAreaId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Entity<SharedAreaReservation>()
            .HasIndex(sr => new { sr.SharedAreaId, sr.ReservationDate, sr.StartTime })
            .HasDatabaseName("ix_shared_area_res_sa_id_date_start");



        // ── Reservation Scheduling ───────────────────────────────────────────────

        builder.Entity<Meeting>().HasKey(m => m.Id);
        builder.Entity<Meeting>().Property(m => m.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Meeting>().Property(m => m.Title).IsRequired();
        builder.Entity<Meeting>().Property(m => m.Description).IsRequired();
        builder.Entity<Meeting>().Property(m => m.Date).IsRequired();
        builder.Entity<Meeting>().Property(m => m.Date)
            .HasConversion(v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v));

        builder.Entity<Meeting>()
            .Property(m => m.EndTime)
            .HasConversion(
                v => v.ToTimeSpan(),
                v => TimeOnly.FromTimeSpan(v));

        builder.Entity<Meeting>()
            .Property(m => m.StartTime)
            .HasConversion(
                v => v.ToTimeSpan(),
                v => TimeOnly.FromTimeSpan(v));

        builder.Entity<Meeting>().Property(m => m.StartTime).IsRequired();
        builder.Entity<Meeting>().Property(m => m.EndTime).IsRequired();

        builder.Entity<Meeting>().OwnsOne(m => m.AdministratorId,
            ai =>
            {
                ai.WithOwner().HasForeignKey("Id");
                ai.Property(r => r.AdministratorIdentifier).HasColumnName("AdministratorId");
            });

        builder.Entity<Meeting>().OwnsOne(m => m.ClassroomId,
            ci =>
            {
                ci.WithOwner().HasForeignKey("Id");
                ci.Property(r => r.ClassroomIdentifier).HasColumnName("ClassroomId");
            });

        builder.Entity<MeetingSession>()
            .HasKey(ms => new { ms.MeetingId, ms.TeacherId });

        builder.Entity<MeetingSession>()
            .HasOne(ms => ms.Meeting)
            .WithMany(m => m.MeetingParticipants)
            .HasForeignKey(ms => ms.MeetingId)
            .OnDelete(DeleteBehavior.Cascade);

        // Teacher navigation removed (ACL boundary fix) — TeacherId is a plain scalar FK.
        // Unique index prevents double-booking the same teacher in overlapping sessions.
        builder.Entity<MeetingSession>()
            .HasIndex(ms => new { ms.TeacherId, ms.MeetingId });

        // ── IoT Monitoring ───────────────────────────────────────────────────────

        builder.Entity<SensorReading>().HasKey(sr => sr.Id);
        builder.Entity<SensorReading>().Property(sr => sr.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<SensorReading>().Property(sr => sr.EdgeReadingId).IsRequired();
        builder.Entity<SensorReading>().Property(sr => sr.DeviceId).IsRequired().HasMaxLength(64);
        builder.Entity<SensorReading>().Property(sr => sr.ZoneId).IsRequired(false).HasMaxLength(64);
        builder.Entity<SensorReading>().HasIndex(sr => sr.ZoneId);
        // Idempotency: one row per (device, edge reading id). reading_id is only unique per edge node,
        // so the device is part of the key. Also serves queries filtering by device_id (leftmost prefix).
        builder.Entity<SensorReading>()
            .HasIndex(sr => new { sr.DeviceId, sr.EdgeReadingId })
            .IsUnique()
            .HasDatabaseName("ix_sensor_readings_device_edge_reading");
        builder.Entity<SensorReading>().Property(sr => sr.Temperature).IsRequired();
        builder.Entity<SensorReading>().Property(sr => sr.Humidity).IsRequired();
        builder.Entity<SensorReading>().Property(sr => sr.OccupancyPresent).IsRequired();
        builder.Entity<SensorReading>().Property(sr => sr.AlertLedState).IsRequired();
        builder.Entity<SensorReading>().Property(sr => sr.RecordedAt).IsRequired();
        builder.Entity<SensorReading>().Property(sr => sr.ReceivedAt).IsRequired();

        // ── Breakdown Management ─────────────────────────────────────────────────

        builder.Entity<Report>().HasKey(r => r.Id);
        builder.Entity<Report>().Property(r => r.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Report>().Property(r => r.KindOfReport).IsRequired();
        builder.Entity<Report>().Property(r => r.Description).IsRequired();
        builder.Entity<Report>().Property(r => r.CreatedAt).IsRequired();

        builder.Entity<Report>().Property(r => r.Status)
            .HasConversion(
                status => status.Value,
                value => ReportStatus.FromString(value)
            ).IsRequired();
        // Index for filtering reports by status
        builder.Entity<Report>().HasIndex(r => r.Status);

        // ResourceId is a VO stored as a plain int column via HasConversion.
        // Store as "resource_id" column (snake_case applied by UseSnakeCaseNamingConvention).
        builder.Entity<Report>().Property(r => r.ResourceId)
            .HasConversion(
                resourceId => resourceId.Id,
                id => new ResourceId(id)
            )
            .HasColumnName("ResourceId");

        // FK constraint: Report.resource_id → resources.id
        // Defined via shadow property so EF can bind a typed int FK while the domain
        // property remains a VO. The shadow property intentionally shares the column name
        // with the converted VO property — we suppress EF's duplicate check by calling
        // HasAnnotation to mark it as the FK backing field.
        // Note: EF Core 8 does not support HasConversion + HasForeignKey on the same
        // property when the conversion target type differs from the property type.
        // We instead omit the DB-level FK and rely on application-level validation
        // (ExternalResourceService) for referential integrity.
        // The HasIndex on status below is kept for query performance.

        builder.UseSnakeCaseNamingConvention();
    }
}
