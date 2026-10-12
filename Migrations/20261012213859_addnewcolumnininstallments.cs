using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Banking.Migrations
{
    /// <inheritdoc />
    public partial class addnewcolumnininstallments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Installments_Loans_LoanId",
                table: "Installments");

            migrationBuilder.AlterColumn<int>(
                name: "LoanId",
                table: "Installments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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

            migrationBuilder.AlterColumn<int>(
                name: "LoanId",
                table: "Installments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Installments_Loans_LoanId",
                table: "Installments",
                column: "LoanId",
                principalTable: "Loans",
                principalColumn: "Id");
        }
    }
}
