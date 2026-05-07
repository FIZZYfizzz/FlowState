using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowStatePlanner.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "planner");

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "planner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "daily_plans",
                schema: "planner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanDate = table.Column<DateOnly>(type: "date", nullable: false),
                    GenerationSource = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_daily_plans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_daily_plans_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "planner",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoutineTemplates",
                schema: "planner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AppliesToDays = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutineTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutineTemplates_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "planner",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskItems",
                schema: "planner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TaskType = table.Column<int>(type: "integer", nullable: false),
                    RecurrenceRule = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskItems_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "planner",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoutineBlocks",
                schema: "planner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoutineTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    FlexibilityType = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutineBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutineBlocks_RoutineTemplates_RoutineTemplateId",
                        column: x => x.RoutineTemplateId,
                        principalSchema: "planner",
                        principalTable: "RoutineTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "daily_plan_items",
                schema: "planner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DailyPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceType = table.Column<int>(type: "integer", nullable: false),
                    RoutineBlockId = table.Column<Guid>(type: "uuid", nullable: true),
                    TaskItemId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    PlannedStartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    PlannedEndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_daily_plan_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_daily_plan_items_RoutineBlocks_RoutineBlockId",
                        column: x => x.RoutineBlockId,
                        principalSchema: "planner",
                        principalTable: "RoutineBlocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_daily_plan_items_TaskItems_TaskItemId",
                        column: x => x.TaskItemId,
                        principalSchema: "planner",
                        principalTable: "TaskItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_daily_plan_items_daily_plans_DailyPlanId",
                        column: x => x.DailyPlanId,
                        principalSchema: "planner",
                        principalTable: "daily_plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_daily_plan_items_DailyPlanId_TaskItemId_SourceType",
                schema: "planner",
                table: "daily_plan_items",
                columns: new[] { "DailyPlanId", "TaskItemId", "SourceType" });

            migrationBuilder.CreateIndex(
                name: "IX_daily_plan_items_RoutineBlockId",
                schema: "planner",
                table: "daily_plan_items",
                column: "RoutineBlockId");

            migrationBuilder.CreateIndex(
                name: "IX_daily_plan_items_TaskItemId",
                schema: "planner",
                table: "daily_plan_items",
                column: "TaskItemId");

            migrationBuilder.CreateIndex(
                name: "IX_daily_plans_UserId_PlanDate",
                schema: "planner",
                table: "daily_plans",
                columns: new[] { "UserId", "PlanDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoutineBlocks_RoutineTemplateId_IsDeleted_SortOrder",
                schema: "planner",
                table: "RoutineBlocks",
                columns: new[] { "RoutineTemplateId", "IsDeleted", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_RoutineTemplates_UserId_IsDeleted",
                schema: "planner",
                table: "RoutineTemplates",
                columns: new[] { "UserId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_UserId_IsDeleted",
                schema: "planner",
                table: "TaskItems",
                columns: new[] { "UserId", "IsDeleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "daily_plan_items",
                schema: "planner");

            migrationBuilder.DropTable(
                name: "RoutineBlocks",
                schema: "planner");

            migrationBuilder.DropTable(
                name: "TaskItems",
                schema: "planner");

            migrationBuilder.DropTable(
                name: "daily_plans",
                schema: "planner");

            migrationBuilder.DropTable(
                name: "RoutineTemplates",
                schema: "planner");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "planner");
        }
    }
}
