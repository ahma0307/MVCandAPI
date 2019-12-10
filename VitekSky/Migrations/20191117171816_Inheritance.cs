using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VitekSky.Migrations
{
    public partial class Inheritance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_Customer_CustomerID",
                table: "Subscription");

            migrationBuilder.DropIndex(name: "IX_Subscription_CustomerID", table: "Subscription");

            migrationBuilder.RenameTable(name: "ProductGuide", newName: "Person");
            migrationBuilder.AddColumn<DateTime>(name: "SubscriptionDate", table: "Person", nullable: true);
            migrationBuilder.AddColumn<string>(name: "Discriminator", table: "Person", nullable: false, maxLength: 128, defaultValue: "ProductGuide");
            migrationBuilder.AlterColumn<DateTime>(name: "HireDate", table: "Person", nullable: true);
            migrationBuilder.AddColumn<int>(name: "OldId", table: "Person", nullable: true);

            // Copy existing Student data into new Person table.
            migrationBuilder.Sql("INSERT INTO dbo.Person (LastName, FirstName, HireDate, SubscriptionDate, Discriminator, OldId) SELECT LastName, FirstName, null AS HireDate, SubscriptionDate, 'Customer' AS Discriminator, ID AS OldId FROM dbo.Customer");
            // Fix up existing relationships to match new PK's.
            migrationBuilder.Sql("UPDATE dbo.Subscription SET CustomerId = (SELECT ID FROM dbo.Person WHERE OldId = Subscription.CustomerId AND Discriminator = 'Customer')");

            // Remove temporary key
            migrationBuilder.DropColumn(name: "OldID", table: "Person");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.CreateIndex(
                 name: "IX_Subscription_CustomerID",
                 table: "Subscription",
                 column: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_Person_CustomerID",
                table: "Subscription",
                column: "CustomerID",
                principalTable: "Person",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CountryAssignment_Person_ProductGuideID",
                table: "CountryAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_Market_Person_ProductGuideID",
                table: "Market");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductAssignment_Person_ProductGuideID",
                table: "ProductAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_Person_CustomerID",
                table: "Subscription");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Person",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "SubscriptionDate",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Person");

            migrationBuilder.RenameTable(
                name: "Person",
                newName: "ProductGuide");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HireDate",
                table: "ProductGuide",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductGuide",
                table: "ProductGuide",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    SubscriptionDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.ID);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_CountryAssignment_ProductGuide_ProductGuideID",
                table: "CountryAssignment",
                column: "ProductGuideID",
                principalTable: "ProductGuide",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Market_ProductGuide_ProductGuideID",
                table: "Market",
                column: "ProductGuideID",
                principalTable: "ProductGuide",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAssignment_ProductGuide_ProductGuideID",
                table: "ProductAssignment",
                column: "ProductGuideID",
                principalTable: "ProductGuide",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_Customer_CustomerID",
                table: "Subscription",
                column: "CustomerID",
                principalTable: "Customer",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
