using System.ComponentModel.DataAnnotations.Schema;

namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates;

public class MeetingAudit
{
    [Column("MeetingAuditId")] public Guid MeetingAuditId { get; set; } = Guid.NewGuid();
    [Column("MeetingId")] public Guid MeetingId { get; set; }
    [Column("Action")] public string Action { get; set; } = string.Empty;
    [Column("ActionPerformedBy")] public string ActionPerformedBy { get; set; } = string.Empty;
    [Column("CreatedAt")] public DateTimeOffset? CreatedDate { get; set; }
    [Column("UpdatedAt")] public DateTimeOffset? UpdatedDate { get; set; }
    [Column("PreviousState")] public string? PreviousState { get; set; }
    [Column("NewState")] public string? NewState { get; set; }
}