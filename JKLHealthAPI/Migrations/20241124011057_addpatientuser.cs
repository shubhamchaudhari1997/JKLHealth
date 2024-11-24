using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JKLHealthAPI.Migrations
{
    /// <inheritdoc />
    public partial class addpatientuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Patient",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_UserId",
                table: "Patient",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patient_AspNetUsers_UserId",
                table: "Patient",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patient_AspNetUsers_UserId",
                table: "Patient");

            migrationBuilder.DropIndex(
                name: "IX_Patient_UserId",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Patient");
        }
    }
}
