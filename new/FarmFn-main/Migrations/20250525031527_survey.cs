using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsoleDI.Example.Migrations
{
    /// <inheritdoc />
    public partial class survey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SurveyForms");

            migrationBuilder.CreateTable(
                name: "Surveys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CageName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CageCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CageArea = table.Column<double>(type: "float", nullable: false),
                    ChickenCount = table.Column<int>(type: "int", nullable: false),
                    ChickenBreed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BreedOther = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurposeOther = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MainFeed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WaterSource = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WaterSourceOther = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ventilation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lighting = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hygiene = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Surveys", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Surveys");

            migrationBuilder.CreateTable(
                name: "SurveyForms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CageId = table.Column<int>(type: "int", nullable: false),
                    ChickenCount = table.Column<int>(type: "int", nullable: false),
                    SurveyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurveyForms_Cage_CageId",
                        column: x => x.CageId,
                        principalTable: "Cage",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SurveyForms_CageId",
                table: "SurveyForms",
                column: "CageId");
        }
    }
}
