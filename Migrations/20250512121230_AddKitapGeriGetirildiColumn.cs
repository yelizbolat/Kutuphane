using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kutuphane.Migrations
{
    /// <inheritdoc />
    public partial class AddKitapGeriGetirildiColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "KitapGeriGetirildi",
                table: "Ogrenciler",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "KitapOduncTarihi",
                table: "Ogrenciler",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OduncTarihi",
                table: "Kitaplar",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OgrenciId",
                table: "Kitaplar",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KitapGeriGetirildi",
                table: "Ogrenciler");

            migrationBuilder.DropColumn(
                name: "KitapOduncTarihi",
                table: "Ogrenciler");

            migrationBuilder.DropColumn(
                name: "OduncTarihi",
                table: "Kitaplar");

            migrationBuilder.DropColumn(
                name: "OgrenciId",
                table: "Kitaplar");
        }
    }
}
