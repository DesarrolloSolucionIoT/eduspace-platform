using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace FULLSTACKFURY.EduSpace.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountActivation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "accounts",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            // Backfill: auto-activate all pre-existing accounts so no one is locked out.
            // This MUST run before any application code checks IsActive (REQ-019).
            migrationBuilder.Sql("UPDATE accounts SET is_active = TRUE;");

            migrationBuilder.CreateTable(
                name: "activation_tokens",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    account_id = table.Column<int>(type: "int", nullable: false),
                    token_hash = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    used_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_activation_tokens", x => x.id);
                    table.ForeignKey(
                        name: "f_k_activation_tokens_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_activation_tokens_account_id",
                table: "activation_tokens",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "i_x_activation_tokens_token_hash",
                table: "activation_tokens",
                column: "token_hash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "activation_tokens");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "accounts");
        }
    }
}
