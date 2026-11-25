using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LuckyApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pick4Draws",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DrawDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DrawType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    WinningNumber = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    Source = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pick4Draws", x => x.Id);
                    table.CheckConstraint("CK_Pick4Draw_DrawType", "\"DrawType\" IN ('Midday', 'Night')");
                    table.CheckConstraint("CK_Pick4Draw_WinningNumber", "\"WinningNumber\" ~ '^[0-9]{4}$'");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pick4Draws_DrawDate_DrawType",
                table: "Pick4Draws",
                columns: new[] { "DrawDate", "DrawType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pick4Draws");
        }
    }
}
