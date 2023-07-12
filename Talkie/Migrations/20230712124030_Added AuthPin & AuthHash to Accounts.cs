using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talkie.Migrations
{
    /// <inheritdoc />
    public partial class AddedAuthPinAuthHashtoAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AuthPin",
                table: "Accounts",
                newName: "PinSalt");

            migrationBuilder.AddColumn<byte[]>(
                name: "PinHash",
                table: "Accounts",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PinHash",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "PinSalt",
                table: "Accounts",
                newName: "AuthPin");
        }
    }
}
