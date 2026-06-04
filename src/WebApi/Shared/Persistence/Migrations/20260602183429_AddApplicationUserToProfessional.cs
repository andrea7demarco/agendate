using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.shared.persistence.migrations
{
    /// <inheritdoc />
    public partial class AddApplicationUserToProfessional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "People",
                type: "text",
                nullable: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_People_ApplicationUserId",
                table: "People",
                column: "ApplicationUserId",
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "FK_People_AspNetUsers_ApplicationUserId",
                table: "People",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_People_AspNetUsers_ApplicationUserId",
                table: "People"
            );

            migrationBuilder.DropIndex(name: "IX_People_ApplicationUserId", table: "People");

            migrationBuilder.DropColumn(name: "ApplicationUserId", table: "People");
        }
    }
}
