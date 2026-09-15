using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymFit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BugFixMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberBranchAccess_Branches_BranchId",
                table: "MemberBranchAccess");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberBranchAccess_Members_MemberId",
                table: "MemberBranchAccess");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Branches_BranchId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_TrainerBranchAssignment_Branches_BranchId",
                table: "TrainerBranchAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_TrainerBranchAssignment_Trainers_TrainerId",
                table: "TrainerBranchAssignment");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_MemberId",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Payments_MemberId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_MemberId",
                table: "Attendances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrainerBranchAssignment",
                table: "TrainerBranchAssignment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemberBranchAccess",
                table: "MemberBranchAccess");

            migrationBuilder.RenameTable(
                name: "TrainerBranchAssignment",
                newName: "TrainerBranchAssignments");

            migrationBuilder.RenameTable(
                name: "MemberBranchAccess",
                newName: "MemberBranchAccesses");

            migrationBuilder.RenameIndex(
                name: "IX_TrainerBranchAssignment_TrainerId",
                table: "TrainerBranchAssignments",
                newName: "IX_TrainerBranchAssignments_TrainerId");

            migrationBuilder.RenameIndex(
                name: "IX_TrainerBranchAssignment_BranchId",
                table: "TrainerBranchAssignments",
                newName: "IX_TrainerBranchAssignments_BranchId");

            migrationBuilder.RenameIndex(
                name: "IX_MemberBranchAccess_MemberId",
                table: "MemberBranchAccesses",
                newName: "IX_MemberBranchAccesses_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_MemberBranchAccess_BranchId",
                table: "MemberBranchAccesses",
                newName: "IX_MemberBranchAccesses_BranchId");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                table: "Payments",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BranchId1",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrainerBranchAssignments",
                table: "TrainerBranchAssignments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemberBranchAccesses",
                table: "MemberBranchAccesses",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "BodyMeasurements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    MeasurementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChestCm = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    WaistCm = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    HipsCm = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    LeftArmCm = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    RightArmCm = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    LeftThighCm = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    RightThighCm = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyMeasurements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BodyMeasurements_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DietPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    TrainerId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Goal = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DailyCalories = table.Column<int>(type: "int", nullable: false),
                    ProteinGrams = table.Column<int>(type: "int", nullable: false),
                    CarbsGrams = table.Column<int>(type: "int", nullable: false),
                    FatGrams = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DietPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DietPlans_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DietPlans_Trainers_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "Trainers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProgressRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    RecordDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WeightKg = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    BodyFatPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    MuscleMassKg = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    StrengthScore = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgressRecords_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNotifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DietMeals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DietPlanId = table.Column<int>(type: "int", nullable: false),
                    MealOrder = table.Column<int>(type: "int", nullable: false),
                    MealType = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    FoodItems = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Calories = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DietMeals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DietMeals_DietPlans_DietPlanId",
                        column: x => x.DietPlanId,
                        principalTable: "DietPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_MemberId",
                table: "Subscriptions",
                column: "MemberId",
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BranchId1",
                table: "Payments",
                column: "BranchId1");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_MemberId_PaymentDate",
                table: "Payments",
                columns: new[] { "MemberId", "PaymentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Status_PaymentDate",
                table: "Payments",
                columns: new[] { "Status", "PaymentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TransactionId",
                table: "Payments",
                column: "TransactionId",
                unique: true,
                filter: "[TransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_MemberId",
                table: "Attendances",
                column: "MemberId",
                unique: true,
                filter: "[CheckOutTime] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BodyMeasurements_MemberId_MeasurementDate",
                table: "BodyMeasurements",
                columns: new[] { "MemberId", "MeasurementDate" });

            migrationBuilder.CreateIndex(
                name: "IX_DietMeals_DietPlanId_MealOrder",
                table: "DietMeals",
                columns: new[] { "DietPlanId", "MealOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_DietPlans_MemberId_IsActive",
                table: "DietPlans",
                columns: new[] { "MemberId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_DietPlans_TrainerId",
                table: "DietPlans",
                column: "TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressRecords_MemberId_RecordDate",
                table: "ProgressRecords",
                columns: new[] { "MemberId", "RecordDate" });

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_UserId_IsRead_CreatedAt",
                table: "UserNotifications",
                columns: new[] { "UserId", "IsRead", "CreatedAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_MemberBranchAccesses_Branches_BranchId",
                table: "MemberBranchAccesses",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberBranchAccesses_Members_MemberId",
                table: "MemberBranchAccesses",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Branches_BranchId",
                table: "Payments",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Branches_BranchId1",
                table: "Payments",
                column: "BranchId1",
                principalTable: "Branches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainerBranchAssignments_Branches_BranchId",
                table: "TrainerBranchAssignments",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrainerBranchAssignments_Trainers_TrainerId",
                table: "TrainerBranchAssignments",
                column: "TrainerId",
                principalTable: "Trainers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberBranchAccesses_Branches_BranchId",
                table: "MemberBranchAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberBranchAccesses_Members_MemberId",
                table: "MemberBranchAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Branches_BranchId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Branches_BranchId1",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_TrainerBranchAssignments_Branches_BranchId",
                table: "TrainerBranchAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TrainerBranchAssignments_Trainers_TrainerId",
                table: "TrainerBranchAssignments");

            migrationBuilder.DropTable(
                name: "BodyMeasurements");

            migrationBuilder.DropTable(
                name: "DietMeals");

            migrationBuilder.DropTable(
                name: "ProgressRecords");

            migrationBuilder.DropTable(
                name: "UserNotifications");

            migrationBuilder.DropTable(
                name: "DietPlans");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_MemberId",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Payments_BranchId1",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_MemberId_PaymentDate",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_Status_PaymentDate",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_TransactionId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_MemberId",
                table: "Attendances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrainerBranchAssignments",
                table: "TrainerBranchAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemberBranchAccesses",
                table: "MemberBranchAccesses");

            migrationBuilder.DropColumn(
                name: "BranchId1",
                table: "Payments");

            migrationBuilder.RenameTable(
                name: "TrainerBranchAssignments",
                newName: "TrainerBranchAssignment");

            migrationBuilder.RenameTable(
                name: "MemberBranchAccesses",
                newName: "MemberBranchAccess");

            migrationBuilder.RenameIndex(
                name: "IX_TrainerBranchAssignments_TrainerId",
                table: "TrainerBranchAssignment",
                newName: "IX_TrainerBranchAssignment_TrainerId");

            migrationBuilder.RenameIndex(
                name: "IX_TrainerBranchAssignments_BranchId",
                table: "TrainerBranchAssignment",
                newName: "IX_TrainerBranchAssignment_BranchId");

            migrationBuilder.RenameIndex(
                name: "IX_MemberBranchAccesses_MemberId",
                table: "MemberBranchAccess",
                newName: "IX_MemberBranchAccess_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_MemberBranchAccesses_BranchId",
                table: "MemberBranchAccess",
                newName: "IX_MemberBranchAccess_BranchId");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrainerBranchAssignment",
                table: "TrainerBranchAssignment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemberBranchAccess",
                table: "MemberBranchAccess",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_MemberId",
                table: "Subscriptions",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_MemberId",
                table: "Payments",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_MemberId",
                table: "Attendances",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberBranchAccess_Branches_BranchId",
                table: "MemberBranchAccess",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberBranchAccess_Members_MemberId",
                table: "MemberBranchAccess",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Branches_BranchId",
                table: "Payments",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrainerBranchAssignment_Branches_BranchId",
                table: "TrainerBranchAssignment",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrainerBranchAssignment_Trainers_TrainerId",
                table: "TrainerBranchAssignment",
                column: "TrainerId",
                principalTable: "Trainers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
