using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterIndexPurchaseOrderItemOnProductIdPurchaseOrderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrderItems_ProductId",
                table: "PurchaseOrderItems");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_ProductId_PurchaseOrderId",
                table: "PurchaseOrderItems",
                columns: new[] { "ProductId", "PurchaseOrderId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrderItems_ProductId_PurchaseOrderId",
                table: "PurchaseOrderItems");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_ProductId",
                table: "PurchaseOrderItems",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
