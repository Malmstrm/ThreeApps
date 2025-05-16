using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Calculations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstValue = table.Column<double>(type: "float", nullable: false),
                    SecondValue = table.Column<double>(type: "float", nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Result = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calculations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RPSGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerMove = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComputerMove = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Outcome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RPSGames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShapeCalculations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShapeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Area = table.Column<double>(type: "float", nullable: false),
                    Perimeter = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShapeCalculations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShapeParameters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShapeCalculationId = table.Column<int>(type: "int", nullable: false),
                    ParameterType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShapeParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShapeParameters_ShapeCalculations_ShapeCalculationId",
                        column: x => x.ShapeCalculationId,
                        principalTable: "ShapeCalculations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShapeParameters_ShapeCalculationId",
                table: "ShapeParameters",
                column: "ShapeCalculationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Calculations");

            migrationBuilder.DropTable(
                name: "RPSGames");

            migrationBuilder.DropTable(
                name: "ShapeParameters");

            migrationBuilder.DropTable(
                name: "ShapeCalculations");
        }
    }
}
