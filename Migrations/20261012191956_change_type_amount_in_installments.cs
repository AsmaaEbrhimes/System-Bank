using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Banking.Migrations
{
    /// <inheritdoc />
    public partial class change_type_amount_in_installments : Migration
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
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Installments",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Installments_Loans_LoanId",
                table: "Installments",
                column: "LoanId",
                principalTable: "Loans",
                principalColumn: "Id");
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
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Amount",
                table: "Installments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddForeignKey(
                name: "FK_Installments_Loans_LoanId",
                table: "Installments",
                column: "LoanId",
                principalTable: "Loans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
