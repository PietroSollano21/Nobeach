using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nobeach.Web.Migrations
{
    /// <inheritdoc />
    public partial class RecuperacaoSenha : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiracaoRecuperacaoSenha",
                table: "Usuarios",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenRecuperacaoSenha",
                table: "Usuarios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiracaoRecuperacaoSenha",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "TokenRecuperacaoSenha",
                table: "Usuarios");
        }
    }
}
