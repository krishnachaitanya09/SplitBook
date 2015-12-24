using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace SplitWisely.Migrations
{
    public partial class MyFirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AmountSplit",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    typeString = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmountSplit", x => x.id);
                });
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.id);
                });
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
                name: "ExpenseType",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    typeString = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseType", x => x.id);
                });
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
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
                name: "Comment",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    comment_type = table.Column<string>(nullable: true),
                    content = table.Column<string>(nullable: true),
                    created_at = table.Column<string>(nullable: true),
                    deleted_at = table.Column<string>(nullable: true),
                    relation_id = table.Column<int>(nullable: false),
                    relation_type = table.Column<string>(nullable: true),
                    user_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.id);
                    table.ForeignKey(
                        name: "FK_Comment_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "Expense",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
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
                    table.ForeignKey(
                        name: "FK_Expense_User_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Expense_User_deleted_by_user_id",
                        column: x => x.deleted_by_user_id,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Expense_User_updated_by_user_id",
                        column: x => x.updated_by_user_id,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Userid = table.Column<int>(nullable: true),
                    group_type = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    simplify_by_default = table.Column<bool>(nullable: false),
                    updated_at = table.Column<string>(nullable: true),
                    whiteboard = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.id);
                    table.ForeignKey(
                        name: "FK_Group_User_Userid",
                        column: x => x.Userid,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
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
            migrationBuilder.CreateTable(
                name: "Debt_Expense",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Expenseid = table.Column<int>(nullable: true),
                    amount = table.Column<string>(nullable: true),
                    currency_code = table.Column<string>(nullable: true),
                    expense_id = table.Column<int>(nullable: false),
                    @from = table.Column<int>(name: "from", nullable: false),
                    to = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debt_Expense", x => x.id);
                    table.ForeignKey(
                        name: "FK_Debt_Expense_Expense_Expenseid",
                        column: x => x.Expenseid,
                        principalTable: "Expense",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Debt_Expense_Debt_Expense_expense_id",
                        column: x => x.expense_id,
                        principalTable: "Debt_Expense",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Debt_Expense_User_from",
                        column: x => x.@from,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Debt_Expense_User_to",
                        column: x => x.to,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "Expense_Share",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    expense_id = table.Column<int>(nullable: false),
                    net_balance = table.Column<string>(nullable: true),
                    owed_share = table.Column<string>(nullable: true),
                    paid_share = table.Column<string>(nullable: true),
                    user_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expense_Share", x => x.id);
                    table.ForeignKey(
                        name: "FK_Expense_Share_Expense_expense_id",
                        column: x => x.expense_id,
                        principalTable: "Expense",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Expense_Share_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "Debt_Group",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    amount = table.Column<string>(nullable: true),
                    currency_code = table.Column<string>(nullable: true),
                    @from = table.Column<int>(name: "from", nullable: false),
                    group_id = table.Column<int>(nullable: false),
                    to = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debt_Group", x => x.id);
                    table.ForeignKey(
                        name: "FK_Debt_Group_User_from",
                        column: x => x.@from,
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
                name: "Group_Members",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    group_id = table.Column<int>(nullable: false),
                    user_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group_Members", x => x.id);
                    table.ForeignKey(
                        name: "FK_Group_Members_Group_group_id",
                        column: x => x.group_id,
                        principalTable: "Group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Group_Members_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("AmountSplit");
            migrationBuilder.DropTable("Balance_User");
            migrationBuilder.DropTable("Category");
            migrationBuilder.DropTable("Comment");
            migrationBuilder.DropTable("Currency");
            migrationBuilder.DropTable("Debt_Expense");
            migrationBuilder.DropTable("Debt_Group");
            migrationBuilder.DropTable("Expense_Share");
            migrationBuilder.DropTable("ExpenseType");
            migrationBuilder.DropTable("Group_Members");
            migrationBuilder.DropTable("Picture");
            migrationBuilder.DropTable("Expense");
            migrationBuilder.DropTable("Group");
            migrationBuilder.DropTable("User");
        }
    }
}
