using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class project_task_assignment_key : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_project_task_assignment",
                table: "project_task_assigmnets");

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                table: "project_task_assigmnets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "pk_project_task_assignments",
                table: "project_task_assigmnets",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_project_task_assigmnets_task_id",
                table: "project_task_assigmnets",
                column: "task_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_project_task_assignments",
                table: "project_task_assigmnets");

            migrationBuilder.DropIndex(
                name: "IX_project_task_assigmnets_task_id",
                table: "project_task_assigmnets");

            migrationBuilder.DropColumn(
                name: "id",
                table: "project_task_assigmnets");

            migrationBuilder.AddPrimaryKey(
                name: "pk_project_task_assignment",
                table: "project_task_assigmnets",
                columns: new[] { "task_id", "member_id" });
        }
    }
}
