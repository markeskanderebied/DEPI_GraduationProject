using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEPI_GraduationProject.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiProductStockRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdhesiveUsage");

            migrationBuilder.DropTable(
                name: "SalesDetails");

            migrationBuilder.RenameColumn(
                name: "SaleDate",
                table: "Sales",
                newName: "sale_date");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "Sales",
                newName: "location_id");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "Sales",
                newName: "client_id");

            migrationBuilder.RenameIndex(
                name: "IX_Sales_LocationId",
                table: "Sales",
                newName: "IX_Sales_location_id");

            migrationBuilder.RenameIndex(
                name: "IX_Sales_ClientId",
                table: "Sales",
                newName: "IX_Sales_client_id");

            migrationBuilder.RenameColumn(
                name: "CarNumber",
                table: "Clients",
                newName: "car_number");

            migrationBuilder.CreateTable(
                name: "SaleDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sale_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleDetail_Product",
                        column: x => x.product_id,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleDetail_Sale",
                        column: x => x.sale_id,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockRequests",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromLocationId = table.Column<int>(type: "int", nullable: false),
                    ToLocationId = table.Column<int>(type: "int", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockRequests", x => x.RequestId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleDetails_product_id",
                table: "SaleDetails",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_SaleDetails_sale_id",
                table: "SaleDetails",
                column: "sale_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleDetails");

            migrationBuilder.DropTable(
                name: "StockRequests");

            migrationBuilder.RenameColumn(
                name: "sale_date",
                table: "Sales",
                newName: "SaleDate");

            migrationBuilder.RenameColumn(
                name: "location_id",
                table: "Sales",
                newName: "LocationId");

            migrationBuilder.RenameColumn(
                name: "client_id",
                table: "Sales",
                newName: "ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_Sales_location_id",
                table: "Sales",
                newName: "IX_Sales_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Sales_client_id",
                table: "Sales",
                newName: "IX_Sales_ClientId");

            migrationBuilder.RenameColumn(
                name: "car_number",
                table: "Clients",
                newName: "CarNumber");

            migrationBuilder.CreateTable(
                name: "AdhesiveUsage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    SalesId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    SaleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdhesiveUsage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdhesiveUsage_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdhesiveUsage_Sales_SalesId",
                        column: x => x.SalesId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    SaleId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleDetail_Product",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleDetail_Sale",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdhesiveUsage_ProductId",
                table: "AdhesiveUsage",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AdhesiveUsage_SalesId",
                table: "AdhesiveUsage",
                column: "SalesId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesDetails_ProductId",
                table: "SalesDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesDetails_SaleId",
                table: "SalesDetails",
                column: "SaleId");
        }
    }
}
