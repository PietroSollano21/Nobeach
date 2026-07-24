using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nobeach.Web.Migrations
{
    /// <inheritdoc />
    public partial class CorrigirTokenConfirmacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TokenConfirmacaoEmail",
                table: "Usuarios",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "TokenConfirmacaoEmail",
                keyValue: null,
                column: "TokenConfirmacaoEmail",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "TokenConfirmacaoEmail",
                table: "Usuarios",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
