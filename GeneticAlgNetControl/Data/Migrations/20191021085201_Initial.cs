using Microsoft.EntityFrameworkCore.Migrations;

namespace GeneticAlgNetControl.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlgorithmData",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Edges = table.Column<string>(nullable: true),
                    TargetNodes = table.Column<string>(nullable: true),
                    DrugTargetNodes = table.Column<string>(nullable: true),
                    AlgorithmRunId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlgorithmData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlgorithmParameters",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    RandomSeed = table.Column<int>(nullable: false),
                    MaximumIterations = table.Column<int>(nullable: false),
                    MaximumIterationsWithoutImprovement = table.Column<int>(nullable: false),
                    MaximumPathLength = table.Column<int>(nullable: false),
                    PopulationSize = table.Column<int>(nullable: false),
                    RandomGenesPerChromosome = table.Column<int>(nullable: false),
                    PercentageRandom = table.Column<double>(nullable: false),
                    PercentageElite = table.Column<double>(nullable: false),
                    ProbabilityMutation = table.Column<double>(nullable: false),
                    AlgorithmRunId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlgorithmParameters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlgorithmRuns",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    DateTimeList = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CurrentIteration = table.Column<int>(nullable: false),
                    CurrentIterationWithoutImprovement = table.Column<int>(nullable: false),
                    AlgorithmDataId = table.Column<string>(nullable: false),
                    AlgorithmParametersId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlgorithmRuns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlgorithmRuns_AlgorithmData_AlgorithmDataId",
                        column: x => x.AlgorithmDataId,
                        principalTable: "AlgorithmData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlgorithmRuns_AlgorithmParameters_AlgorithmParametersId",
                        column: x => x.AlgorithmParametersId,
                        principalTable: "AlgorithmParameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlgorithmRuns_AlgorithmDataId",
                table: "AlgorithmRuns",
                column: "AlgorithmDataId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlgorithmRuns_AlgorithmParametersId",
                table: "AlgorithmRuns",
                column: "AlgorithmParametersId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlgorithmRuns");

            migrationBuilder.DropTable(
                name: "AlgorithmData");

            migrationBuilder.DropTable(
                name: "AlgorithmParameters");
        }
    }
}
