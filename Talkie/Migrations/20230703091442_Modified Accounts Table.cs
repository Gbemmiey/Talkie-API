using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talkie.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedAccountsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthPin",
                table: "Accounts");

            migrationBuilder.AddColumn<byte[]>(
                name: "TransactPin",
                table: "Accounts",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactPin",
                table: "Accounts");

            migrationBuilder.AddColumn<int>(
                name: "AuthPin",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
