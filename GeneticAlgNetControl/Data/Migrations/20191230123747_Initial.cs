using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GeneticAlgNetControl.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Analyses",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    DateTimeStarted = table.Column<DateTime>(nullable: true),
                    DateTimeEnded = table.Column<DateTime>(nullable: true),
                    DateTimeIntervals = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    NumberOfEdges = table.Column<int>(nullable: false),
                    Edges = table.Column<string>(nullable: true),
                    NumberOfNodes = table.Column<int>(nullable: false),
                    Nodes = table.Column<string>(nullable: true),
                    NumberOfTargetNodes = table.Column<int>(nullable: false),
                    TargetNodes = table.Column<string>(nullable: true),
                    NumberOfPreferredNodes = table.Column<int>(nullable: false),
                    PreferredNodes = table.Column<string>(nullable: true),
                    CurrentIteration = table.Column<int>(nullable: false),
                    CurrentIterationWithoutImprovement = table.Column<int>(nullable: false),
                    Parameters = table.Column<string>(nullable: true),
                    Population = table.Column<string>(nullable: true),
                    HistoricBestFitness = table.Column<string>(nullable: true),
                    HistoricAverageFitness = table.Column<string>(nullable: true),
                    Solutions = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Analyses", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Analyses");
        }
    }
}
