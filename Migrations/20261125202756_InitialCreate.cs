using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameCatalogApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Studios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "TEXT", nullable: false),
                    FoundedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Headquarters = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Studios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VideoGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Genre = table.Column<string>(type: "TEXT", nullable: false),
                    ReleaseYear = table.Column<int>(type: "INTEGER", nullable: false),
                    Rating = table.Column<decimal>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    StudioId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoGames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoGames_Studios_StudioId",
                        column: x => x.StudioId,
                        principalTable: "Studios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_VideoGames_StudioId",
                table: "VideoGames",
                column: "StudioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoGames");

            migrationBuilder.DropTable(
                name: "Studios");
        }
    }
}
