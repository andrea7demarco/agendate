using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApi.shared.persistence.migrations
{
    /// <inheritdoc />
    public partial class AddProfessionalProfileDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DegreeTitle",
                table: "People",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "GraduationYear",
                table: "People",
                type: "integer",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "University",
                table: "People",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true
            );

            migrationBuilder.CreateTable(
                name: "ProfessionalAvailabilities",
                columns: table => new
                {
                    ProfessionalId = table.Column<int>(type: "integer", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    TimeSlot = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ProfessionalAvailabilities",
                        x => new
                        {
                            x.ProfessionalId,
                            x.DayOfWeek,
                            x.TimeSlot,
                        }
                    );
                    table.ForeignKey(
                        name: "FK_ProfessionalAvailabilities_People_ProfessionalId",
                        column: x => x.ProfessionalId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "ProfessionalPatientGroups",
                columns: table => new
                {
                    ProfessionalId = table.Column<int>(type: "integer", nullable: false),
                    PatientGroup = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ProfessionalPatientGroups",
                        x => new { x.ProfessionalId, x.PatientGroup }
                    );
                    table.ForeignKey(
                        name: "FK_ProfessionalPatientGroups_People_ProfessionalId",
                        column: x => x.ProfessionalId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "ProfessionalTrainings",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ProfessionalId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(
                        type: "character varying(200)",
                        maxLength: 200,
                        nullable: false
                    ),
                    Institution = table.Column<string>(
                        type: "character varying(200)",
                        maxLength: 200,
                        nullable: true
                    ),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(
                        type: "character varying(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionalTrainings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfessionalTrainings_People_ProfessionalId",
                        column: x => x.ProfessionalId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalTrainings_ProfessionalId",
                table: "ProfessionalTrainings",
                column: "ProfessionalId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ProfessionalAvailabilities");

            migrationBuilder.DropTable(name: "ProfessionalPatientGroups");

            migrationBuilder.DropTable(name: "ProfessionalTrainings");

            migrationBuilder.DropColumn(name: "DegreeTitle", table: "People");

            migrationBuilder.DropColumn(name: "GraduationYear", table: "People");

            migrationBuilder.DropColumn(name: "University", table: "People");
        }
    }
}
