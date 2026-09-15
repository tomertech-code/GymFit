using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymFit.Infrastructure.Migrations;

public partial class AddGymFitIntegrityIndexes : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            ;WITH Ranked AS (
                SELECT Id, ROW_NUMBER() OVER (PARTITION BY MemberId ORDER BY CreatedAt DESC, Id DESC) AS rn
                FROM Subscriptions
                WHERE IsActive = 1
            )
            UPDATE s
            SET IsActive = 0, Status = 2
            FROM Subscriptions s
            INNER JOIN Ranked r ON r.Id = s.Id
            WHERE r.rn > 1;
        ");

        migrationBuilder.Sql(@"
            ;WITH Ranked AS (
                SELECT Id, ROW_NUMBER() OVER (PARTITION BY MemberId ORDER BY CheckInTime DESC, Id DESC) AS rn
                FROM Attendances
                WHERE CheckOutTime IS NULL
            )
            UPDATE a
            SET CheckOutTime = CheckInTime
            FROM Attendances a
            INNER JOIN Ranked r ON r.Id = a.Id
            WHERE r.rn > 1;
        ");

        migrationBuilder.CreateIndex(
            name: "IX_Subscriptions_Active_MemberId",
            table: "Subscriptions",
            column: "MemberId",
            unique: true,
            filter: "[IsActive] = 1");

        migrationBuilder.CreateIndex(
            name: "IX_Attendances_Open_MemberId",
            table: "Attendances",
            column: "MemberId",
            unique: true,
            filter: "[CheckOutTime] IS NULL");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "IX_Subscriptions_Active_MemberId", table: "Subscriptions");
        migrationBuilder.DropIndex(name: "IX_Attendances_Open_MemberId", table: "Attendances");
    }
}
