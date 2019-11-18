using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GeneticAlgNetControl.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Algorithms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    DateTimeStarted = table.Column<DateTime>(nullable: true),
                    DateTimeEnded = table.Column<DateTime>(nullable: true),
                    DateTimePeriods = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Edges = table.Column<string>(nullable: true),
                    Nodes = table.Column<string>(nullable: true),
                    TargetNodes = table.Column<string>(nullable: true),
                    PreferredNodes = table.Column<string>(nullable: true),
                    CurrentIteration = table.Column<int>(nullable: false),
                    CurrentIterationWithoutImprovement = table.Column<int>(nullable: false),
                    Parameters = table.Column<string>(nullable: true),
                    Population = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Algorithms", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Algorithms");
        }
    }
}
