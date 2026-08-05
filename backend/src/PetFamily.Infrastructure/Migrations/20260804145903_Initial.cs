using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFamily.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    species = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    breed = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    health_info = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    current_address = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    weight = table.Column<int>(type: "integer", maxLength: 50, nullable: false),
                    height = table.Column<int>(type: "integer", maxLength: 50, nullable: false),
                    owner_phone_number = table.Column<int>(type: "integer", maxLength: 11, nullable: false),
                    is_neutered = table.Column<bool>(type: "boolean", nullable: false),
                    birth_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_vaccinated = table.Column<bool>(type: "boolean", nullable: false),
                    help_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    donation_details = table.Column<string>(type: "jsonb", nullable: true),
                    photos = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pets", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pets");
        }
    }
}
