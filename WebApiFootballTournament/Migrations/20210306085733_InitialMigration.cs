using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApiFootballTournament.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PointsScored = table.Column<int>(type: "int", nullable: false),
                    Win = table.Column<int>(type: "int", nullable: false),
                    Draw = table.Column<int>(type: "int", nullable: false),
                    Lost = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("5c7e7eea-04fc-4935-8b18-d9c88da3b828"), "A" });

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("2107ce76-80ac-4041-a80a-3736cce78f1b"), "B" });

            migrationBuilder.InsertData(
                table: "Teams",
                columns: new[] { "Id", "Description", "Draw", "GroupId", "Lost", "Name", "PointsScored", "Win" },
                values: new object[] { new Guid("5b3621c0-7b12-4e80-9c8b-3398cba7ee05"), "Description2", 0, null, 0, "Team2", 0, 0 });

            migrationBuilder.InsertData(
                table: "Teams",
                columns: new[] { "Id", "Description", "Draw", "GroupId", "Lost", "Name", "PointsScored", "Win" },
                values: new object[] { new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b35"), "Description1", 0, new Guid("5c7e7eea-04fc-4935-8b18-d9c88da3b828"), 0, "Team1", 0, 0 });

            migrationBuilder.CreateIndex(
                name: "IX_Teams_GroupId",
                table: "Teams",
                column: "GroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}
