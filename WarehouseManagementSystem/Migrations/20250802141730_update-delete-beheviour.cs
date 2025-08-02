using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class updatedeletebeheviour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_balances_Resources_ResourceId",
                table: "balances");

            migrationBuilder.DropForeignKey(
                name: "FK_balances_UnitsOfMeasurement_UnitOfMeasurementId",
                table: "balances");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptItems_Receipts_ReceiptId",
                table: "ReceiptItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptItems_Resources_ResourceId",
                table: "ReceiptItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptItems_UnitsOfMeasurement_UnitId",
                table: "ReceiptItems");

            migrationBuilder.AddForeignKey(
                name: "FK_balances_Resources_ResourceId",
                table: "balances",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_balances_UnitsOfMeasurement_UnitOfMeasurementId",
                table: "balances",
                column: "UnitOfMeasurementId",
                principalTable: "UnitsOfMeasurement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptItems_Receipts_ReceiptId",
                table: "ReceiptItems",
                column: "ReceiptId",
                principalTable: "Receipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptItems_Resources_ResourceId",
                table: "ReceiptItems",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptItems_UnitsOfMeasurement_UnitId",
                table: "ReceiptItems",
                column: "UnitId",
                principalTable: "UnitsOfMeasurement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_balances_Resources_ResourceId",
                table: "balances");

            migrationBuilder.DropForeignKey(
                name: "FK_balances_UnitsOfMeasurement_UnitOfMeasurementId",
                table: "balances");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptItems_Receipts_ReceiptId",
                table: "ReceiptItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptItems_Resources_ResourceId",
                table: "ReceiptItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptItems_UnitsOfMeasurement_UnitId",
                table: "ReceiptItems");

            migrationBuilder.AddForeignKey(
                name: "FK_balances_Resources_ResourceId",
                table: "balances",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_balances_UnitsOfMeasurement_UnitOfMeasurementId",
                table: "balances",
                column: "UnitOfMeasurementId",
                principalTable: "UnitsOfMeasurement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptItems_Receipts_ReceiptId",
                table: "ReceiptItems",
                column: "ReceiptId",
                principalTable: "Receipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptItems_Resources_ResourceId",
                table: "ReceiptItems",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptItems_UnitsOfMeasurement_UnitId",
                table: "ReceiptItems",
                column: "UnitId",
                principalTable: "UnitsOfMeasurement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
