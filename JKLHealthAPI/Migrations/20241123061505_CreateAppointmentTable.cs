using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JKLHealthAPI.Migrations
{
    /// <inheritdoc />
    public partial class CreateAppointmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaregiverNote_Caregiver_CaregiverId",
                table: "CaregiverNote");

            migrationBuilder.DropForeignKey(
                name: "FK_CaregiverNote_Patient_PatientId",
                table: "CaregiverNote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CaregiverNote",
                table: "CaregiverNote");

            migrationBuilder.RenameTable(
                name: "CaregiverNote",
                newName: "CaregiverNotes");

            migrationBuilder.RenameIndex(
                name: "IX_CaregiverNote_PatientId",
                table: "CaregiverNotes",
                newName: "IX_CaregiverNotes_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_CaregiverNote_CaregiverId",
                table: "CaregiverNotes",
                newName: "IX_CaregiverNotes_CaregiverId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CaregiverNotes",
                table: "CaregiverNotes",
                column: "NoteId");

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    AppointmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AppointmentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    CaregiverId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.AppointmentId);
                    table.ForeignKey(
                        name: "FK_Appointments_Caregiver_CaregiverId",
                        column: x => x.CaregiverId,
                        principalTable: "Caregiver",
                        principalColumn: "CaregiverId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointments_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_CaregiverId",
                table: "Appointments",
                column: "CaregiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_CaregiverNotes_Caregiver_CaregiverId",
                table: "CaregiverNotes",
                column: "CaregiverId",
                principalTable: "Caregiver",
                principalColumn: "CaregiverId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CaregiverNotes_Patient_PatientId",
                table: "CaregiverNotes",
                column: "PatientId",
                principalTable: "Patient",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaregiverNotes_Caregiver_CaregiverId",
                table: "CaregiverNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_CaregiverNotes_Patient_PatientId",
                table: "CaregiverNotes");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CaregiverNotes",
                table: "CaregiverNotes");

            migrationBuilder.RenameTable(
                name: "CaregiverNotes",
                newName: "CaregiverNote");

            migrationBuilder.RenameIndex(
                name: "IX_CaregiverNotes_PatientId",
                table: "CaregiverNote",
                newName: "IX_CaregiverNote_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_CaregiverNotes_CaregiverId",
                table: "CaregiverNote",
                newName: "IX_CaregiverNote_CaregiverId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CaregiverNote",
                table: "CaregiverNote",
                column: "NoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_CaregiverNote_Caregiver_CaregiverId",
                table: "CaregiverNote",
                column: "CaregiverId",
                principalTable: "Caregiver",
                principalColumn: "CaregiverId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CaregiverNote_Patient_PatientId",
                table: "CaregiverNote",
                column: "PatientId",
                principalTable: "Patient",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
