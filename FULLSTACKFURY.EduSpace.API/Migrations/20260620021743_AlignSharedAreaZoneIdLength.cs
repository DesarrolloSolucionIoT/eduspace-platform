using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FULLSTACKFURY.EduSpace.API.Migrations
{
    /// <inheritdoc />
    public partial class AlignSharedAreaZoneIdLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "zone_id",
                table: "shared_areas",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "zone_id",
                table: "shared_areas",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldNullable: true);
        }
    }
}
