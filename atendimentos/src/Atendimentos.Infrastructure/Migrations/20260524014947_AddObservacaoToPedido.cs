using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atendimentos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddObservacaoToPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OBSERVACAO",
                table: "PEDIDOS",
                type: "NVARCHAR2(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OBSERVACAO",
                table: "PEDIDOS");
        }
    }
}
