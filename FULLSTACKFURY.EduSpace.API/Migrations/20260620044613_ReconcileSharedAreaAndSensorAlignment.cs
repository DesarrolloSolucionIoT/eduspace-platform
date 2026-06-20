using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace FULLSTACKFURY.EduSpace.API.Migrations
{
    /// <inheritdoc />
    public partial class ReconcileSharedAreaAndSensorAlignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_sensor_readings_device_id",
                table: "sensor_readings");

            migrationBuilder.DropIndex(
                name: "i_x_sensor_readings_edge_reading_id",
                table: "sensor_readings");

            migrationBuilder.AddColumn<string>(
                name: "zone_id",
                table: "shared_areas",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "zone_id",
                table: "sensor_readings",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<int>(
                name: "edge_reading_id",
                table: "sensor_readings",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64);

            migrationBuilder.CreateTable(
                name: "shared_area_reservations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    shared_area_id = table.Column<int>(type: "int", nullable: false),
                    teacher_id = table.Column<int>(type: "int", nullable: false),
                    reservation_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    start_time = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    end_time = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    reason = table.Column<string>(type: "longtext", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_shared_area_reservations", x => x.id);
                    table.ForeignKey(
                        name: "f_k_shared_area_reservations_shared_areas_shared_area_id",
                        column: x => x.shared_area_id,
                        principalTable: "shared_areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_sensor_readings_device_edge_reading",
                table: "sensor_readings",
                columns: new[] { "device_id", "edge_reading_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shared_area_res_sa_id_date_start",
                table: "shared_area_reservations",
                columns: new[] { "shared_area_id", "reservation_date", "start_time" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "shared_area_reservations");

            migrationBuilder.DropIndex(
                name: "ix_sensor_readings_device_edge_reading",
                table: "sensor_readings");

            migrationBuilder.DropColumn(
                name: "zone_id",
                table: "shared_areas");

            migrationBuilder.AlterColumn<string>(
                name: "zone_id",
                table: "sensor_readings",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "edge_reading_id",
                table: "sensor_readings",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "i_x_sensor_readings_device_id",
                table: "sensor_readings",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "i_x_sensor_readings_edge_reading_id",
                table: "sensor_readings",
                column: "edge_reading_id",
                unique: true);
        }
    }
}
