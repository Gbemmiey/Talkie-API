using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talkie.Migrations
{
    /// <inheritdoc />
    public partial class NewUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Accounts_UserNumber",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "UserNumber",
                table: "Messages",
                newName: "Number");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_UserNumber",
                table: "Messages",
                newName: "IX_Messages_Number");

            migrationBuilder.RenameColumn(
                name: "TransactPin",
                table: "Accounts",
                newName: "AuthPin");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Accounts_Number",
                table: "Messages",
                column: "Number",
                principalTable: "Accounts",
                principalColumn: "Number",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Accounts_Number",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "Number",
                table: "Messages",
                newName: "UserNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_Number",
                table: "Messages",
                newName: "IX_Messages_UserNumber");

            migrationBuilder.RenameColumn(
                name: "AuthPin",
                table: "Accounts",
                newName: "TransactPin");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Accounts_UserNumber",
                table: "Messages",
                column: "UserNumber",
                principalTable: "Accounts",
                principalColumn: "Number",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
