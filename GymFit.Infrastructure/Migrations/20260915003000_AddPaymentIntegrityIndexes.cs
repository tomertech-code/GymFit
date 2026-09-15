using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymFit.Infrastructure.Migrations;

public partial class AddPaymentIntegrityIndexes : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "TransactionId",
            table: "Payments",
            type: "nvarchar(64)",
            maxLength: 64,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Payments_TransactionId_Unique",
            table: "Payments",
            column: "TransactionId",
            unique: true,
            filter: "[TransactionId] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_Payments_MemberId_PaymentDate",
            table: "Payments",
            columns: new[] { "MemberId", "PaymentDate" });

        migrationBuilder.CreateIndex(
            name: "IX_Payments_Status_PaymentDate",
            table: "Payments",
            columns: new[] { "Status", "PaymentDate" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "IX_Payments_TransactionId_Unique", table: "Payments");
        migrationBuilder.DropIndex(name: "IX_Payments_MemberId_PaymentDate", table: "Payments");
        migrationBuilder.DropIndex(name: "IX_Payments_Status_PaymentDate", table: "Payments");

        migrationBuilder.AlterColumn<string>(
            name: "TransactionId",
            table: "Payments",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(64)",
            oldMaxLength: 64,
            oldNullable: true);
    }
}
