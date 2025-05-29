using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kutuphane.Migrations
{
    /// <inheritdoc />
    public partial class OgrenciSilmeFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OduncKitaplar_Kitaplar_KitapId",
                table: "OduncKitaplar");

            migrationBuilder.DropForeignKey(
                name: "FK_OduncKitaplar_Ogrenciler_OgrenciId",
                table: "OduncKitaplar");

            migrationBuilder.AddForeignKey(
                name: "FK_OduncKitaplar_Kitaplar_KitapId",
                table: "OduncKitaplar",
                column: "KitapId",
                principalTable: "Kitaplar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OduncKitaplar_Ogrenciler_OgrenciId",
                table: "OduncKitaplar",
                column: "OgrenciId",
                principalTable: "Ogrenciler",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OduncKitaplar_Kitaplar_KitapId",
                table: "OduncKitaplar");

            migrationBuilder.DropForeignKey(
                name: "FK_OduncKitaplar_Ogrenciler_OgrenciId",
                table: "OduncKitaplar");

            migrationBuilder.AddForeignKey(
                name: "FK_OduncKitaplar_Kitaplar_KitapId",
                table: "OduncKitaplar",
                column: "KitapId",
                principalTable: "Kitaplar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OduncKitaplar_Ogrenciler_OgrenciId",
                table: "OduncKitaplar",
                column: "OgrenciId",
                principalTable: "Ogrenciler",
                principalColumn: "Id");
        }
    }
}
