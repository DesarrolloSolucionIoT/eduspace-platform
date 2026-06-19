using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace FULLSTACKFURY.EduSpace.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSensorReadings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "zone_id",
                table: "classrooms",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "sensor_readings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    edge_reading_id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    device_id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    zone_id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    temperature = table.Column<float>(type: "float", nullable: false),
                    humidity = table.Column<float>(type: "float", nullable: false),
                    occupancy_present = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    alert_led_state = table.Column<int>(type: "int", nullable: false),
                    recorded_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    received_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_sensor_readings", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_sensor_readings_device_id",
                table: "sensor_readings",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "i_x_sensor_readings_edge_reading_id",
                table: "sensor_readings",
                column: "edge_reading_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_sensor_readings_zone_id",
                table: "sensor_readings",
                column: "zone_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sensor_readings");

            migrationBuilder.DropColumn(
                name: "zone_id",
                table: "classrooms");
        }
    }
}
