using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kutuphane.Migrations
{
    /// <inheritdoc />
    public partial class AddYenilikYapildi2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "kitapOduncİslemleris");

            migrationBuilder.AddColumn<bool>(
                name: "Aktif",
                table: "Siniflar",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Aktif",
                table: "Ogrenciler",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Aktif",
                table: "Kitaplar",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Aktif",
                table: "Kategoriler",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KullaniciAdi = table.Column<string>(type: "TEXT", nullable: false),
                    Sifre = table.Column<string>(type: "TEXT", nullable: false),
                    Rol = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "Aktif",
                table: "Siniflar");

            migrationBuilder.DropColumn(
                name: "Aktif",
                table: "Ogrenciler");

            migrationBuilder.DropColumn(
                name: "Aktif",
                table: "Kitaplar");

            migrationBuilder.DropColumn(
                name: "Aktif",
                table: "Kategoriler");

            migrationBuilder.CreateTable(
                name: "kitapOduncİslemleris",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OduncKitapId = table.Column<int>(type: "INTEGER", nullable: false),
                    AlinmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GercekTeslimTarihi = table.Column<DateTime>(type: "TEXT", nullable: true),
                    KitapAdi = table.Column<string>(type: "TEXT", nullable: false),
                    OgrenciAdi = table.Column<string>(type: "TEXT", nullable: false),
                    TeslimTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kitapOduncİslemleris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_kitapOduncİslemleris_OduncKitaplar_OduncKitapId",
                        column: x => x.OduncKitapId,
                        principalTable: "OduncKitaplar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_kitapOduncİslemleris_OduncKitapId",
                table: "kitapOduncİslemleris",
                column: "OduncKitapId");
        }
    }
}
