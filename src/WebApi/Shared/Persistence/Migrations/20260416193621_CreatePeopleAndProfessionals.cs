using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApi.shared.persistence.migrations
{
    /// <inheritdoc />
    public partial class CreatePeopleAndProfessionals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_RefreshToken_TokenHash", table: "RefreshToken");

            migrationBuilder.AlterColumn<string>(
                name: "TokenHash",
                table: "RefreshToken",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200
            );

            migrationBuilder.AlterColumn<bool>(
                name: "RegistrationCompleted",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false
            );

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true
            );

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    FirstName = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    LastName = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    Dni = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Professionals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ConsultationCost = table.Column<decimal>(
                        type: "numeric(10,2)",
                        precision: 10,
                        scale: 2,
                        nullable: false
                    ),
                    AppointmentType = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    Address = table.Column<string>(type: "text", nullable: true),
                    NationalLicense = table.Column<string>(type: "text", nullable: true),
                    ProvincialLicense = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professionals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Professionals_People_Id",
                        column: x => x.Id,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_People_Email",
                table: "People",
                column: "Email",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Professionals");

            migrationBuilder.DropTable(name: "People");

            migrationBuilder.AlterColumn<string>(
                name: "TokenHash",
                table: "RefreshToken",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.AlterColumn<bool>(
                name: "RegistrationCompleted",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean"
            );

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_TokenHash",
                table: "RefreshToken",
                column: "TokenHash",
                unique: true
            );
        }
    }
}
