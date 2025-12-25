using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProjectTaskAssignment_TaskId_MemberId_Unique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE UNIQUE INDEX
                IF NOT EXISTS 
                project_task_assignments_task_id_member_id 
                ON project_task_assigmnets (task_id, member_id)
                """
                );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DROP INDEX IF EXISTS project_task_assignments_task_id_member_id"
                );
        }
    }
}
