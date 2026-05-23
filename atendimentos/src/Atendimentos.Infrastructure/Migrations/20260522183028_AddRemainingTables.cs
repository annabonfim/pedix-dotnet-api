using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atendimentos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRemainingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_COMANDAS_CLIENTES_ClienteId",
                table: "COMANDAS");

            migrationBuilder.DropForeignKey(
                name: "FK_COMANDAS_GARCONS_GarcomId",
                table: "COMANDAS");

            migrationBuilder.DropForeignKey(
                name: "FK_COMANDAS_MESAS_MesaId",
                table: "COMANDAS");

            migrationBuilder.DropIndex(
                name: "IX_MESAS_Numero",
                table: "MESAS");

            migrationBuilder.DropIndex(
                name: "IX_COMANDAS_ClienteId",
                table: "COMANDAS");

            migrationBuilder.DropIndex(
                name: "IX_COMANDAS_GarcomId",
                table: "COMANDAS");

            migrationBuilder.DropIndex(
                name: "IX_COMANDAS_MesaId",
                table: "COMANDAS");

            migrationBuilder.DropColumn(
                name: "CPF",
                table: "CLIENTES");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "MESAS",
                newName: "STATUS");

            migrationBuilder.RenameColumn(
                name: "Numero",
                table: "MESAS",
                newName: "NUMERO");

            migrationBuilder.RenameColumn(
                name: "Capacidade",
                table: "MESAS",
                newName: "CAPACIDADE");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "MESAS",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Telefone",
                table: "GARCONS",
                newName: "TELEFONE");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "GARCONS",
                newName: "NOME");

            migrationBuilder.RenameColumn(
                name: "Matricula",
                table: "GARCONS",
                newName: "MATRICULA");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "GARCONS",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "ValorTotal",
                table: "COMANDAS",
                newName: "VALORTOTAL");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "COMANDAS",
                newName: "STATUS");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "COMANDAS",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Telefone",
                table: "CLIENTES",
                newName: "TELEFONE");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "CLIENTES",
                newName: "NOME");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CLIENTES",
                newName: "ID");

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "MESAS",
                type: "RAW(2000)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "RAW(8)",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<string>(
                name: "QrCode",
                table: "MESAS",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Localizacao",
                table: "MESAS",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(80)",
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CAPACIDADE",
                table: "MESAS",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NOME",
                table: "GARCONS",
                type: "NVARCHAR2(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "MATRICULA",
                table: "GARCONS",
                type: "NVARCHAR2(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "NOME",
                table: "CLIENTES",
                type: "NVARCHAR2(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(120)",
                oldMaxLength: 120);

            migrationBuilder.CreateTable(
                name: "PAGAMENTOS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PEDIDOID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    VALOR = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false),
                    METODOPAGAMENTO = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    STATUS = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DATAPAGAMENTO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAGAMENTOS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PEDIDO_ITENS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PEDIDOID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ITEMCARDAPIOID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    QUANTIDADE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PRECOMOMENTO = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false),
                    SUBTOTAL = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PEDIDO_ITENS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PEDIDOS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CLIENTEID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    GARCOMID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    MESAID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DATAPEDIDO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    VALORTOTAL = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false),
                    STATUS = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PEDIDOS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "USUARIOS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NOME = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    SENHAHASH = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    TELEFONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    CPF = table.Column<string>(type: "NVARCHAR2(14)", maxLength: 14, nullable: true),
                    MATRICULA = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    ADMINKEY = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    ROLE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Ativo = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIOS", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PAGAMENTOS");

            migrationBuilder.DropTable(
                name: "PEDIDO_ITENS");

            migrationBuilder.DropTable(
                name: "PEDIDOS");

            migrationBuilder.DropTable(
                name: "USUARIOS");

            migrationBuilder.RenameColumn(
                name: "STATUS",
                table: "MESAS",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "NUMERO",
                table: "MESAS",
                newName: "Numero");

            migrationBuilder.RenameColumn(
                name: "CAPACIDADE",
                table: "MESAS",
                newName: "Capacidade");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "MESAS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "TELEFONE",
                table: "GARCONS",
                newName: "Telefone");

            migrationBuilder.RenameColumn(
                name: "NOME",
                table: "GARCONS",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "MATRICULA",
                table: "GARCONS",
                newName: "Matricula");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "GARCONS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "VALORTOTAL",
                table: "COMANDAS",
                newName: "ValorTotal");

            migrationBuilder.RenameColumn(
                name: "STATUS",
                table: "COMANDAS",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "COMANDAS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "TELEFONE",
                table: "CLIENTES",
                newName: "Telefone");

            migrationBuilder.RenameColumn(
                name: "NOME",
                table: "CLIENTES",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "CLIENTES",
                newName: "Id");

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "MESAS",
                type: "RAW(8)",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "RAW(2000)");

            migrationBuilder.AlterColumn<string>(
                name: "QrCode",
                table: "MESAS",
                type: "NVARCHAR2(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Localizacao",
                table: "MESAS",
                type: "NVARCHAR2(80)",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Capacidade",
                table: "MESAS",
                type: "NUMBER(10)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "GARCONS",
                type: "NVARCHAR2(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Matricula",
                table: "GARCONS",
                type: "NVARCHAR2(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "CLIENTES",
                type: "NVARCHAR2(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(150)",
                oldMaxLength: 150);

            migrationBuilder.AddColumn<string>(
                name: "CPF",
                table: "CLIENTES",
                type: "NVARCHAR2(14)",
                maxLength: 14,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_MESAS_Numero",
                table: "MESAS",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_COMANDAS_ClienteId",
                table: "COMANDAS",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_COMANDAS_GarcomId",
                table: "COMANDAS",
                column: "GarcomId");

            migrationBuilder.CreateIndex(
                name: "IX_COMANDAS_MesaId",
                table: "COMANDAS",
                column: "MesaId");

            migrationBuilder.AddForeignKey(
                name: "FK_COMANDAS_CLIENTES_ClienteId",
                table: "COMANDAS",
                column: "ClienteId",
                principalTable: "CLIENTES",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_COMANDAS_GARCONS_GarcomId",
                table: "COMANDAS",
                column: "GarcomId",
                principalTable: "GARCONS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_COMANDAS_MESAS_MesaId",
                table: "COMANDAS",
                column: "MesaId",
                principalTable: "MESAS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
