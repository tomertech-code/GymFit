using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymFit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixBranchFKs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_AspNetUsers_UserId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_Trainers_AssignedTrainerId",
                table: "Members");

            migrationBuilder.AddColumn<int>(
                name: "PrimaryBranchId",
                table: "Trainers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "WorksAtMultipleBranches",
                table: "Trainers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "AllowsMultiBranchAccess",
                table: "MembershipPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxBranchesAllowed",
                table: "MembershipPlans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasMultiBranchAccess",
                table: "Members",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PrimaryBranchId",
                table: "Members",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Attendances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PinCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ManagerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpeningTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ClosingTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BranchEquipment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    EquipmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaintenanceSchedule = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsWorking = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchEquipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchEquipment_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberBranchAccess",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    HasAccess = table.Column<bool>(type: "bit", nullable: false),
                    GrantedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberBranchAccess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberBranchAccess_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberBranchAccess_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainerBranchAssignment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrainerId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    WorkingDays = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainerBranchAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainerBranchAssignment_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainerBranchAssignment_Trainers_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "Trainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_PrimaryBranchId",
                table: "Trainers",
                column: "PrimaryBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BranchId",
                table: "Payments",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_PrimaryBranchId",
                table: "Members",
                column: "PrimaryBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_BranchId",
                table: "Attendances",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchEquipment_BranchId",
                table: "BranchEquipment",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberBranchAccess_BranchId",
                table: "MemberBranchAccess",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberBranchAccess_MemberId",
                table: "MemberBranchAccess",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainerBranchAssignment_BranchId",
                table: "TrainerBranchAssignment",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainerBranchAssignment_TrainerId",
                table: "TrainerBranchAssignment",
                column: "TrainerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Branches_BranchId",
                table: "Attendances",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Members_AspNetUsers_UserId",
                table: "Members",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Branches_PrimaryBranchId",
                table: "Members",
                column: "PrimaryBranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Trainers_AssignedTrainerId",
                table: "Members",
                column: "AssignedTrainerId",
                principalTable: "Trainers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Branches_BranchId",
                table: "Payments",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trainers_Branches_PrimaryBranchId",
                table: "Trainers",
                column: "PrimaryBranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Branches_BranchId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_AspNetUsers_UserId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_Branches_PrimaryBranchId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_Trainers_AssignedTrainerId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Branches_BranchId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Trainers_Branches_PrimaryBranchId",
                table: "Trainers");

            migrationBuilder.DropTable(
                name: "BranchEquipment");

            migrationBuilder.DropTable(
                name: "MemberBranchAccess");

            migrationBuilder.DropTable(
                name: "TrainerBranchAssignment");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Trainers_PrimaryBranchId",
                table: "Trainers");

            migrationBuilder.DropIndex(
                name: "IX_Payments_BranchId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Members_PrimaryBranchId",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_BranchId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "PrimaryBranchId",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "WorksAtMultipleBranches",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "AllowsMultiBranchAccess",
                table: "MembershipPlans");

            migrationBuilder.DropColumn(
                name: "MaxBranchesAllowed",
                table: "MembershipPlans");

            migrationBuilder.DropColumn(
                name: "HasMultiBranchAccess",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "PrimaryBranchId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Attendances");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_AspNetUsers_UserId",
                table: "Members",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Trainers_AssignedTrainerId",
                table: "Members",
                column: "AssignedTrainerId",
                principalTable: "Trainers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
