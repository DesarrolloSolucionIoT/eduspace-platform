using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FULLSTACKFURY.EduSpace.API.Migrations
{
    /// <inheritdoc />
    public partial class AlignSensorReadingWithEdgePayload : Migration
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

            migrationBuilder.CreateIndex(
                name: "ix_sensor_readings_device_edge_reading",
                table: "sensor_readings",
                columns: new[] { "device_id", "edge_reading_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_sensor_readings_device_edge_reading",
                table: "sensor_readings");

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
