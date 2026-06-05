using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.shared.persistence.migrations
{
    /// <inheritdoc />
    public partial class MoveApplicationUserToPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "People",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "People",
                type: "integer",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "BirthDate", table: "People");

            migrationBuilder.DropColumn(name: "Gender", table: "People");
        }
    }
}
