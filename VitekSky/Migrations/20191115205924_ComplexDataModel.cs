using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VitekSky.Migrations
{
    public partial class ComplexDataModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<int>(
            //    name: "MarketID",
            //    table: "Product",
            //    nullable: false,
            //    defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ProductGuide",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    HireDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductGuide", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CountryAssignment",
                columns: table => new
                {
                    ProductGuideID = table.Column<int>(nullable: false),
                    Location = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryAssignment", x => x.ProductGuideID);
                    table.ForeignKey(
                        name: "FK_CountryAssignment_ProductGuide_ProductGuideID",
                        column: x => x.ProductGuideID,
                        principalTable: "ProductGuide",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Market",
                columns: table => new
                {
                    MarketID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    Budget = table.Column<decimal>(type: "money", nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    ProductGuideID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Market", x => x.MarketID);
                    table.ForeignKey(
                        name: "FK_Market_ProductGuide_ProductGuideID",
                        column: x => x.ProductGuideID,
                        principalTable: "ProductGuide",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql("INSERT INTO dbo.Market (Name, Budget, StartDate) VALUES ('Temp', 0.00, GETDATE())");
            // Default value for FK points to department created above, with
            // defaultValue changed to 1 in following AddColumn statement.

            migrationBuilder.AddColumn<int>(
                name: "MarketID",
                table: "Product",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "ProductAssignment",
                columns: table => new
                {
                    ProductGuideID = table.Column<int>(nullable: false),
                    ProductID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAssignment", x => new { x.ProductID, x.ProductGuideID });
                    table.ForeignKey(
                        name: "FK_ProductAssignment_ProductGuide_ProductGuideID",
                        column: x => x.ProductGuideID,
                        principalTable: "ProductGuide",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductAssignment_Product_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_MarketID",
                table: "Product",
                column: "MarketID");

            migrationBuilder.CreateIndex(
                name: "IX_Market_ProductGuideID",
                table: "Market",
                column: "ProductGuideID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAssignment_ProductGuideID",
                table: "ProductAssignment",
                column: "ProductGuideID");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Market_MarketID",
                table: "Product",
                column: "MarketID",
                principalTable: "Market",
                principalColumn: "MarketID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Market_MarketID",
                table: "Product");

            migrationBuilder.DropTable(
                name: "CountryAssignment");

            migrationBuilder.DropTable(
                name: "Market");

            migrationBuilder.DropTable(
                name: "ProductAssignment");

            migrationBuilder.DropTable(
                name: "ProductGuide");

            migrationBuilder.DropIndex(
                name: "IX_Product_MarketID",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "MarketID",
                table: "Product");
        }
    }
}
