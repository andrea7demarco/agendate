using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApi.shared.persistence.migrations
{
    /// <inheritdoc />
    public partial class AddHealthInsurances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalHealthInsurance_HealthInsurance_HealthInsurance~",
                table: "ProfessionalHealthInsurance"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalHealthInsurance_People_ProfessionalId",
                table: "ProfessionalHealthInsurance"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalSpecialties_Specialties_SpecialtyId",
                table: "ProfessionalSpecialties"
            );

            migrationBuilder.DropTable(name: "HealthInsurance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProfessionalHealthInsurance",
                table: "ProfessionalHealthInsurance"
            );

            migrationBuilder.DropIndex(
                name: "IX_ProfessionalHealthInsurance_ProfessionalId",
                table: "ProfessionalHealthInsurance"
            );

            migrationBuilder.DropColumn(name: "Id", table: "ProfessionalHealthInsurance");

            migrationBuilder.RenameTable(
                name: "ProfessionalHealthInsurance",
                newName: "ProfessionalHealthInsurances"
            );

            migrationBuilder.RenameIndex(
                name: "IX_ProfessionalHealthInsurance_HealthInsuranceId",
                table: "ProfessionalHealthInsurances",
                newName: "IX_ProfessionalHealthInsurances_HealthInsuranceId"
            );

            migrationBuilder.AddColumn<bool>(
                name: "HasCud",
                table: "People",
                type: "boolean",
                nullable: true
            );

            migrationBuilder.AddColumn<bool>(
                name: "AcceptsReimbursement",
                table: "ProfessionalHealthInsurances",
                type: "boolean",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<bool>(
                name: "RequiresAuthorization",
                table: "ProfessionalHealthInsurances",
                type: "boolean",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProfessionalHealthInsurances",
                table: "ProfessionalHealthInsurances",
                columns: new[] { "ProfessionalId", "HealthInsuranceId" }
            );

            migrationBuilder.CreateTable(
                name: "HealthInsurances",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    Name = table.Column<string>(
                        type: "character varying(150)",
                        maxLength: 150,
                        nullable: false
                    ),
                    Acronym = table.Column<string>(
                        type: "character varying(30)",
                        maxLength: 30,
                        nullable: false
                    ),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthInsurances", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "ProfessionalLocations",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ProfessionalId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(
                        type: "character varying(150)",
                        maxLength: 150,
                        nullable: false
                    ),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Street = table.Column<string>(
                        type: "character varying(150)",
                        maxLength: 150,
                        nullable: false
                    ),
                    StreetNumber = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    Floor = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: true
                    ),
                    Office = table.Column<string>(
                        type: "character varying(30)",
                        maxLength: 30,
                        nullable: true
                    ),
                    City = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    Province = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    PostalCode = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: true
                    ),
                    Latitude = table.Column<decimal>(
                        type: "numeric(9,6)",
                        precision: 9,
                        scale: 6,
                        nullable: false
                    ),
                    Longitude = table.Column<decimal>(
                        type: "numeric(9,6)",
                        precision: 9,
                        scale: 6,
                        nullable: false
                    ),
                    GooglePlaceId = table.Column<string>(
                        type: "character varying(250)",
                        maxLength: 250,
                        nullable: true
                    ),
                    Instructions = table.Column<string>(
                        type: "character varying(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionalLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfessionalLocations_People_ProfessionalId",
                        column: x => x.ProfessionalId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "PatientHealthInsurances",
                columns: table => new
                {
                    PatientId = table.Column<int>(type: "integer", nullable: false),
                    HealthInsuranceId = table.Column<int>(type: "integer", nullable: false),
                    AffiliateNumber = table.Column<string>(
                        type: "character varying(80)",
                        maxLength: 80,
                        nullable: true
                    ),
                    PlanName = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_PatientHealthInsurances",
                        x => new { x.PatientId, x.HealthInsuranceId }
                    );
                    table.ForeignKey(
                        name: "FK_PatientHealthInsurances_HealthInsurances_HealthInsuranceId",
                        column: x => x.HealthInsuranceId,
                        principalTable: "HealthInsurances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_PatientHealthInsurances_People_PatientId",
                        column: x => x.PatientId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_HealthInsurances_Acronym",
                table: "HealthInsurances",
                column: "Acronym"
            );

            migrationBuilder.CreateIndex(
                name: "IX_HealthInsurances_Name",
                table: "HealthInsurances",
                column: "Name",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_PatientHealthInsurances_HealthInsuranceId",
                table: "PatientHealthInsurances",
                column: "HealthInsuranceId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalLocations_ProfessionalId",
                table: "ProfessionalLocations",
                column: "ProfessionalId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalHealthInsurances_HealthInsurances_HealthInsuran~",
                table: "ProfessionalHealthInsurances",
                column: "HealthInsuranceId",
                principalTable: "HealthInsurances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalHealthInsurances_People_ProfessionalId",
                table: "ProfessionalHealthInsurances",
                column: "ProfessionalId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalSpecialties_Specialties_SpecialtyId",
                table: "ProfessionalSpecialties",
                column: "SpecialtyId",
                principalTable: "Specialties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalHealthInsurances_HealthInsurances_HealthInsuran~",
                table: "ProfessionalHealthInsurances"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalHealthInsurances_People_ProfessionalId",
                table: "ProfessionalHealthInsurances"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalSpecialties_Specialties_SpecialtyId",
                table: "ProfessionalSpecialties"
            );

            migrationBuilder.DropTable(name: "PatientHealthInsurances");

            migrationBuilder.DropTable(name: "ProfessionalLocations");

            migrationBuilder.DropTable(name: "HealthInsurances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProfessionalHealthInsurances",
                table: "ProfessionalHealthInsurances"
            );

            migrationBuilder.DropColumn(name: "HasCud", table: "People");

            migrationBuilder.DropColumn(
                name: "AcceptsReimbursement",
                table: "ProfessionalHealthInsurances"
            );

            migrationBuilder.DropColumn(
                name: "RequiresAuthorization",
                table: "ProfessionalHealthInsurances"
            );

            migrationBuilder.RenameTable(
                name: "ProfessionalHealthInsurances",
                newName: "ProfessionalHealthInsurance"
            );

            migrationBuilder.RenameIndex(
                name: "IX_ProfessionalHealthInsurances_HealthInsuranceId",
                table: "ProfessionalHealthInsurance",
                newName: "IX_ProfessionalHealthInsurance_HealthInsuranceId"
            );

            migrationBuilder
                .AddColumn<int>(
                    name: "Id",
                    table: "ProfessionalHealthInsurance",
                    type: "integer",
                    nullable: false,
                    defaultValue: 0
                )
                .Annotation(
                    "Npgsql:ValueGenerationStrategy",
                    NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                );

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProfessionalHealthInsurance",
                table: "ProfessionalHealthInsurance",
                column: "Id"
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

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalHealthInsurance_ProfessionalId",
                table: "ProfessionalHealthInsurance",
                column: "ProfessionalId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalHealthInsurance_HealthInsurance_HealthInsurance~",
                table: "ProfessionalHealthInsurance",
                column: "HealthInsuranceId",
                principalTable: "HealthInsurance",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalHealthInsurance_People_ProfessionalId",
                table: "ProfessionalHealthInsurance",
                column: "ProfessionalId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalSpecialties_Specialties_SpecialtyId",
                table: "ProfessionalSpecialties",
                column: "SpecialtyId",
                principalTable: "Specialties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
