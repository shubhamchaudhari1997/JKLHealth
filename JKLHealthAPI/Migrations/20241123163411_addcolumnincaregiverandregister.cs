using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JKLHealthAPI.Migrations
{
    /// <inheritdoc />
    public partial class addcolumnincaregiverandregister : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Caregiver",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Caregiver_UserId",
                table: "Caregiver",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Caregiver_AspNetUsers_UserId",
                table: "Caregiver",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Caregiver_AspNetUsers_UserId",
                table: "Caregiver");

            migrationBuilder.DropIndex(
                name: "IX_Caregiver_UserId",
                table: "Caregiver");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Caregiver");
        }
    }
}
