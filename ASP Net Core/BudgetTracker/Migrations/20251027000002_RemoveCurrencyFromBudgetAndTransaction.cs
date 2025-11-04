using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTracker.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCurrencyFromBudgetAndTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove Currency column from Budgets table
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Budgets");

            // Remove Currency column from Transactions table
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Transactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore Currency column to Budgets table
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Budgets",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "BGN");

            // Restore Currency column to Transactions table
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Transactions",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "BGN");
        }
    }
}

