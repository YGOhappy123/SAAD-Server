using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace milktea_server.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameVi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Toppings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    NameVi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescriptionVi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Toppings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Admins_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Admins_Admins_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Admins",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Milkteas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PriceS = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    PriceM = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    PriceL = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    NameVi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescriptionVi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Milkteas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Milkteas_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    ProcessingStaffId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Admins_ProcessingStaffId",
                        column: x => x.ProcessingStaffId,
                        principalTable: "Admins",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    MilkteaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CartItems_Milkteas_MilkteaId",
                        column: x => x.MilkteaId,
                        principalTable: "Milkteas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    MilkteaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Milkteas_MilkteaId",
                        column: x => x.MilkteaId,
                        principalTable: "Milkteas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CartItemToppings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ToppingId = table.Column<int>(type: "int", nullable: true),
                    CartItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItemToppings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItemToppings_CartItems_CartItemId",
                        column: x => x.CartItemId,
                        principalTable: "CartItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CartItemToppings_Toppings_ToppingId",
                        column: x => x.ToppingId,
                        principalTable: "Toppings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderItemToppings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ToppingId = table.Column<int>(type: "int", nullable: true),
                    OrderItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemToppings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemToppings_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderItemToppings_Toppings_ToppingId",
                        column: x => x.ToppingId,
                        principalTable: "Toppings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admins_AccountId",
                table: "Admins",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Admins_CreatedById",
                table: "Admins",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CustomerId",
                table: "CartItems",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_MilkteaId",
                table: "CartItems",
                column: "MilkteaId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItemToppings_CartItemId",
                table: "CartItemToppings",
                column: "CartItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItemToppings_ToppingId",
                table: "CartItemToppings",
                column: "ToppingId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_AccountId",
                table: "Customers",
                column: "AccountId",
                unique: true,
                filter: "[AccountId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Milkteas_CategoryId",
                table: "Milkteas",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_MilkteaId",
                table: "OrderItems",
                column: "MilkteaId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemToppings_OrderItemId",
                table: "OrderItemToppings",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemToppings_ToppingId",
                table: "OrderItemToppings",
                column: "ToppingId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProcessingStaffId",
                table: "Orders",
                column: "ProcessingStaffId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItemToppings");

            migrationBuilder.DropTable(
                name: "OrderItemToppings");

            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Toppings");

            migrationBuilder.DropTable(
                name: "Milkteas");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
