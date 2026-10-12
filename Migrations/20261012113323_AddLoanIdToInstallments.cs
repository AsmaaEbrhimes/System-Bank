using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Banking.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanIdToInstallments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoanId",
                table: "Installments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Installments_LoanId",
                table: "Installments",
                column: "LoanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Installments_Loans_LoanId",
                table: "Installments",
                column: "LoanId",
                principalTable: "Loans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Installments_Loans_LoanId",
                table: "Installments");

            migrationBuilder.DropIndex(
                name: "IX_Installments_LoanId",
                table: "Installments");

            migrationBuilder.DropColumn(
                name: "LoanId",
                table: "Installments");
        }
    }
}
