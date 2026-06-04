using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApi.shared.persistence.migrations
{
    /// <inheritdoc />
    public partial class AddRecursiveSpecialties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Biography",
                table: "People",
                type: "text",
                nullable: true
            );

            migrationBuilder.CreateTable(
                name: "HealthInsurance",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ShortName = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthInsurance", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Specialties",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    Name = table.Column<string>(
                        type: "character varying(120)",
                        maxLength: 120,
                        nullable: false
                    ),
                    ParentSpecialtyId = table.Column<int>(type: "integer", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Specialties_Specialties_ParentSpecialtyId",
                        column: x => x.ParentSpecialtyId,
                        principalTable: "Specialties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "ProfessionalHealthInsurance",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ProfessionalId = table.Column<int>(type: "integer", nullable: false),
                    HealthInsuranceId = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionalHealthInsurance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfessionalHealthInsurance_HealthInsurance_HealthInsurance~",
                        column: x => x.HealthInsuranceId,
                        principalTable: "HealthInsurance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_ProfessionalHealthInsurance_People_ProfessionalId",
                        column: x => x.ProfessionalId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "ProfessionalSpecialties",
                columns: table => new
                {
                    ProfessionalId = table.Column<int>(type: "integer", nullable: false),
                    SpecialtyId = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ProfessionalSpecialties",
                        x => new { x.ProfessionalId, x.SpecialtyId }
                    );
                    table.ForeignKey(
                        name: "FK_ProfessionalSpecialties_People_ProfessionalId",
                        column: x => x.ProfessionalId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_ProfessionalSpecialties_Specialties_SpecialtyId",
                        column: x => x.SpecialtyId,
                        principalTable: "Specialties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalHealthInsurance_HealthInsuranceId",
                table: "ProfessionalHealthInsurance",
                column: "HealthInsuranceId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalHealthInsurance_ProfessionalId",
                table: "ProfessionalHealthInsurance",
                column: "ProfessionalId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalSpecialties_SpecialtyId",
                table: "ProfessionalSpecialties",
                column: "SpecialtyId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Specialties_ParentSpecialtyId",
                table: "Specialties",
                column: "ParentSpecialtyId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ProfessionalHealthInsurance");

            migrationBuilder.DropTable(name: "ProfessionalSpecialties");

            migrationBuilder.DropTable(name: "HealthInsurance");

            migrationBuilder.DropTable(name: "Specialties");

            migrationBuilder.DropColumn(name: "Biography", table: "People");
        }
    }
}
