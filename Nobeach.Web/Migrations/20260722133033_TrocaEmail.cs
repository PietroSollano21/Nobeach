using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nobeach.Web.Migrations
{
    /// <inheritdoc />
    public partial class TrocaEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiracaoTrocaEmail",
                table: "Usuarios",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NovoEmail",
                table: "Usuarios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TokenTrocaEmail",
                table: "Usuarios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiracaoTrocaEmail",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "NovoEmail",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "TokenTrocaEmail",
                table: "Usuarios");
        }
    }
}
