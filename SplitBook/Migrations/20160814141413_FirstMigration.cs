using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SplitBook.Migrations
{
    public partial class FirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    currency_code = table.Column<string>(nullable: false),
                    unit = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.currency_code);
                });

            migrationBuilder.CreateTable(
                name: "Expense",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    cost = table.Column<string>(nullable: true),
                    created_at = table.Column<string>(nullable: true),
                    created_by_user_id = table.Column<int>(nullable: false),
                    creation_method = table.Column<string>(nullable: true),
                    currency_code = table.Column<string>(nullable: true),
                    date = table.Column<string>(nullable: true),
                    deleted_at = table.Column<string>(nullable: true),
                    deleted_by_user_id = table.Column<int>(nullable: false),
                    description = table.Column<string>(nullable: true),
                    details = table.Column<string>(nullable: true),
                    group_id = table.Column<int>(nullable: false),
                    payment = table.Column<bool>(nullable: false),
                    transaction_confirmed = table.Column<bool>(nullable: false),
                    updated_at = table.Column<string>(nullable: true),
                    updated_by_user_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expense", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    group_type = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    simplify_by_default = table.Column<bool>(nullable: false),
                    updated_at = table.Column<string>(nullable: true),
                    whiteboard = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    added_as_friend = table.Column<bool>(nullable: false),
                    added_to_group = table.Column<bool>(nullable: false),
                    announcements = table.Column<bool>(nullable: false),
                    bills = table.Column<bool>(nullable: false),
                    expense_added = table.Column<bool>(nullable: false),
                    expense_updated = table.Column<bool>(nullable: false),
                    monthly_summary = table.Column<bool>(nullable: false),
                    payments = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    country_code = table.Column<string>(nullable: true),
                    default_currency = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: true),
                    first_name = table.Column<string>(nullable: true),
                    last_name = table.Column<string>(nullable: true),
                    updated_at = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Debt_Expense",
                columns: table => new
                {
                    expense_id = table.Column<int>(nullable: false),
                    from = table.Column<int>(nullable: false),
                    to = table.Column<int>(nullable: false),
                    amount = table.Column<string>(nullable: true),
                    currency_code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debt_Expense", x => new { x.expense_id, x.from, x.to });
                    table.ForeignKey(
                        name: "FK_Debt_Expense_Expense_expense_id",
                        column: x => x.expense_id,
                        principalTable: "Expense",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Expense_Share",
                columns: table => new
                {
                    expense_id = table.Column<int>(nullable: false),
                    user_id = table.Column<int>(nullable: false),
                    net_balance = table.Column<string>(nullable: true),
                    owed_share = table.Column<string>(nullable: true),
                    paid_share = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expense_Share", x => new { x.expense_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_Expense_Share_Expense_expense_id",
                        column: x => x.expense_id,
                        principalTable: "Expense",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Receipt",
                columns: table => new
                {
                    expense_id = table.Column<int>(nullable: false),
                    large = table.Column<string>(nullable: true),
                    original = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipt", x => x.expense_id);
                    table.ForeignKey(
                        name: "FK_Receipt_Expense_expense_id",
                        column: x => x.expense_id,
                        principalTable: "Expense",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Group_Members",
                columns: table => new
                {
                    group_id = table.Column<int>(nullable: false),
                    user_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group_Members", x => new { x.group_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_Group_Members_Group_group_id",
                        column: x => x.group_id,
                        principalTable: "Group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Balance_User",
                columns: table => new
                {
                    user_id = table.Column<int>(nullable: false),
                    amount = table.Column<string>(nullable: true),
                    currency_code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Balance_User", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_Balance_User_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Debt_Group",
                columns: table => new
                {
                    group_id = table.Column<int>(nullable: false),
                    from = table.Column<int>(nullable: false),
                    to = table.Column<int>(nullable: false),
                    amount = table.Column<string>(nullable: true),
                    currency_code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debt_Group", x => new { x.group_id, x.from, x.to });
                    table.ForeignKey(
                        name: "FK_Debt_Group_User_from",
                        column: x => x.from,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Debt_Group_Group_group_id",
                        column: x => x.group_id,
                        principalTable: "Group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Debt_Group_User_to",
                        column: x => x.to,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Picture",
                columns: table => new
                {
                    user_id = table.Column<int>(nullable: false),
                    large = table.Column<string>(nullable: true),
                    medium = table.Column<string>(nullable: true),
                    original = table.Column<string>(nullable: true),
                    small = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Picture", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_Picture_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Balance_User_user_id",
                table: "Balance_User",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Debt_Expense_expense_id",
                table: "Debt_Expense",
                column: "expense_id");

            migrationBuilder.CreateIndex(
                name: "IX_Debt_Group_from",
                table: "Debt_Group",
                column: "from");

            migrationBuilder.CreateIndex(
                name: "IX_Debt_Group_group_id",
                table: "Debt_Group",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_Debt_Group_to",
                table: "Debt_Group",
                column: "to");

            migrationBuilder.CreateIndex(
                name: "IX_Expense_Share_expense_id",
                table: "Expense_Share",
                column: "expense_id");

            migrationBuilder.CreateIndex(
                name: "IX_Group_Members_group_id",
                table: "Group_Members",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_Picture_user_id",
                table: "Picture",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receipt_expense_id",
                table: "Receipt",
                column: "expense_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Balance_User");

            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropTable(
                name: "Debt_Expense");

            migrationBuilder.DropTable(
                name: "Debt_Group");

            migrationBuilder.DropTable(
                name: "Expense_Share");

            migrationBuilder.DropTable(
                name: "Group_Members");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Picture");

            migrationBuilder.DropTable(
                name: "Receipt");

            migrationBuilder.DropTable(
                name: "Group");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Expense");
        }
    }
}
