using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kutuphane.Migrations
{
    /// <inheritdoc />
    public partial class AddSilinenOgrencilerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SilinenOgrenciler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OgrenciAdi = table.Column<string>(type: "TEXT", nullable: false),
                    OgrenciSoyadi = table.Column<string>(type: "TEXT", nullable: false),
                    OkulNumarasi = table.Column<string>(type: "TEXT", nullable: false),
                    SinifAdi = table.Column<string>(type: "TEXT", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SilenKullanici = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SilinenOgrenciler", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SilinenOgrenciler");
        }
    }
}
