using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp.Migrations
{
    /// <inheritdoc />
    public partial class AddedRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plants_Users_UserId",
                table: "Plants");

            migrationBuilder.DropForeignKey(
                name: "FK_Variables_Plants_PlantId",
                table: "Variables");

            migrationBuilder.DropIndex(
                name: "IX_Variables_PlantId",
                table: "Variables");

            migrationBuilder.DropIndex(
                name: "IX_Plants_UserId",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "PlantId",
                table: "Variables");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Plants");

            migrationBuilder.CreateTable(
                name: "PlantUser",
                columns: table => new
                {
                    PlantsId = table.Column<int>(type: "integer", nullable: false),
                    UsersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantUser", x => new { x.PlantsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_PlantUser_Plants_PlantsId",
                        column: x => x.PlantsId,
                        principalTable: "Plants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlantUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlantVariable",
                columns: table => new
                {
                    PlantsId = table.Column<int>(type: "integer", nullable: false),
                    VariablesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantVariable", x => new { x.PlantsId, x.VariablesId });
                    table.ForeignKey(
                        name: "FK_PlantVariable_Plants_PlantsId",
                        column: x => x.PlantsId,
                        principalTable: "Plants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlantVariable_Variables_VariablesId",
                        column: x => x.VariablesId,
                        principalTable: "Variables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlantUser_UsersId",
                table: "PlantUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantVariable_VariablesId",
                table: "PlantVariable",
                column: "VariablesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlantUser");

            migrationBuilder.DropTable(
                name: "PlantVariable");

            migrationBuilder.AddColumn<int>(
                name: "PlantId",
                table: "Variables",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Plants",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Variables_PlantId",
                table: "Variables",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_Plants_UserId",
                table: "Plants",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Plants_Users_UserId",
                table: "Plants",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Variables_Plants_PlantId",
                table: "Variables",
                column: "PlantId",
                principalTable: "Plants",
                principalColumn: "Id");
        }
    }
}
