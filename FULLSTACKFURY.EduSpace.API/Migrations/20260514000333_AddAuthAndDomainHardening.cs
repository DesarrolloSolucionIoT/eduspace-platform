using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace FULLSTACKFURY.EduSpace.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthAndDomainHardening : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_meeting_sessions_teacher_profiles_teacher_id",
                table: "meeting_sessions");

            migrationBuilder.DropIndex(
                name: "i_x_resources_classroom_id",
                table: "resources");

            migrationBuilder.DropIndex(
                name: "i_x_meeting_sessions_teacher_id",
                table: "meeting_sessions");

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "verification_codes",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "resources",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "reports",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "classrooms",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    account_id = table.Column<int>(type: "int", nullable: false),
                    token_hash = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    revoked_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    replaced_by_token_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "f_k_refresh_tokens_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_resources_classroom_id_name",
                table: "resources",
                columns: new[] { "classroom_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_reports_status",
                table: "reports",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "i_x_meeting_sessions_teacher_id_meeting_id",
                table: "meeting_sessions",
                columns: new[] { "teacher_id", "meeting_id" });

            migrationBuilder.CreateIndex(
                name: "i_x_classrooms_name",
                table: "classrooms",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_refresh_tokens_account_id",
                table: "refresh_tokens",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "i_x_refresh_tokens_expires_at",
                table: "refresh_tokens",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "i_x_refresh_tokens_token_hash",
                table: "refresh_tokens",
                column: "token_hash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropIndex(
                name: "i_x_resources_classroom_id_name",
                table: "resources");

            migrationBuilder.DropIndex(
                name: "i_x_reports_status",
                table: "reports");

            migrationBuilder.DropIndex(
                name: "i_x_meeting_sessions_teacher_id_meeting_id",
                table: "meeting_sessions");

            migrationBuilder.DropIndex(
                name: "i_x_classrooms_name",
                table: "classrooms");

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "verification_codes",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "resources",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "reports",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "classrooms",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.CreateIndex(
                name: "i_x_resources_classroom_id",
                table: "resources",
                column: "classroom_id");

            migrationBuilder.CreateIndex(
                name: "i_x_meeting_sessions_teacher_id",
                table: "meeting_sessions",
                column: "teacher_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_meeting_sessions_teacher_profiles_teacher_id",
                table: "meeting_sessions",
                column: "teacher_id",
                principalTable: "teacher_profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
