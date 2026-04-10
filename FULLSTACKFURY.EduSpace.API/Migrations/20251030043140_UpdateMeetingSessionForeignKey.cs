using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FULLSTACKFURY.EduSpace.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMeetingSessionForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_meeting_sessions_teacher_profiles_teacher_id",
                table: "meeting_sessions");

            migrationBuilder.AddForeignKey(
                name: "f_k_meeting_sessions_teacher_profiles_teacher_id",
                table: "meeting_sessions",
                column: "teacher_id",
                principalTable: "teacher_profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_meeting_sessions_teacher_profiles_teacher_id",
                table: "meeting_sessions");

            migrationBuilder.AddForeignKey(
                name: "f_k_meeting_sessions_teacher_profiles_teacher_id",
                table: "meeting_sessions",
                column: "teacher_id",
                principalTable: "teacher_profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
