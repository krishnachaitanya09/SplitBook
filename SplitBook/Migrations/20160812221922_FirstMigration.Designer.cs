using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using SplitBook.Model;

namespace SplitBook.Migrations
{
    [DbContext(typeof(SplitBookContext))]
    [Migration("20160812221922_FirstMigration")]
    partial class FirstMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("SplitBook.Model.Balance_User", b =>
                {
                    b.Property<int>("user_id");

                    b.Property<string>("amount");

                    b.Property<string>("currency_code");

                    b.HasKey("user_id");

                    b.HasIndex("user_id");

                    b.ToTable("Balance_User");
                });

            modelBuilder.Entity("SplitBook.Model.Currency", b =>
                {
                    b.Property<string>("currency_code");

                    b.Property<string>("unit");

                    b.HasKey("currency_code");

                    b.ToTable("Currency");
                });

            modelBuilder.Entity("SplitBook.Model.Debt_Expense", b =>
                {
                    b.Property<int>("expense_id");

                    b.Property<int>("from");

                    b.Property<int>("to");

                    b.Property<string>("amount");

                    b.Property<string>("currency_code");

                    b.HasKey("expense_id", "from", "to");

                    b.HasIndex("expense_id");

                    b.ToTable("Debt_Expense");
                });

            modelBuilder.Entity("SplitBook.Model.Debt_Group", b =>
                {
                    b.Property<int>("group_id");

                    b.Property<int>("from");

                    b.Property<int>("to");

                    b.Property<string>("amount");

                    b.Property<string>("currency_code");

                    b.HasKey("group_id", "from", "to");

                    b.HasIndex("from");

                    b.HasIndex("group_id");

                    b.HasIndex("to");

                    b.ToTable("Debt_Group");
                });

            modelBuilder.Entity("SplitBook.Model.Expense", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("cost");

                    b.Property<string>("created_at");

                    b.Property<int>("created_by_user_id");

                    b.Property<string>("creation_method");

                    b.Property<string>("currency_code");

                    b.Property<string>("date");

                    b.Property<string>("deleted_at");

                    b.Property<int>("deleted_by_user_id");

                    b.Property<string>("description");

                    b.Property<string>("details");

                    b.Property<int>("group_id");

                    b.Property<bool>("payment");

                    b.Property<bool>("transaction_confirmed");

                    b.Property<string>("updated_at");

                    b.Property<int>("updated_by_user_id");

                    b.HasKey("id");

                    b.ToTable("Expense");
                });

            modelBuilder.Entity("SplitBook.Model.Expense_Share", b =>
                {
                    b.Property<int>("expense_id");

                    b.Property<int>("user_id");

                    b.Property<string>("net_balance");

                    b.Property<string>("owed_share");

                    b.Property<string>("paid_share");

                    b.HasKey("expense_id", "user_id");

                    b.HasIndex("expense_id");

                    b.ToTable("Expense_Share");
                });

            modelBuilder.Entity("SplitBook.Model.Group", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("group_type");

                    b.Property<string>("name");

                    b.Property<bool>("simplify_by_default");

                    b.Property<string>("updated_at");

                    b.Property<string>("whiteboard");

                    b.HasKey("id");

                    b.ToTable("Group");
                });

            modelBuilder.Entity("SplitBook.Model.Group_Members", b =>
                {
                    b.Property<int>("group_id");

                    b.Property<int>("user_id");

                    b.HasKey("group_id", "user_id");

                    b.HasIndex("group_id");

                    b.ToTable("Group_Members");
                });

            modelBuilder.Entity("SplitBook.Model.Notifications", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("added_as_friend");

                    b.Property<bool>("added_to_group");

                    b.Property<bool>("announcements");

                    b.Property<bool>("bills");

                    b.Property<bool>("expense_added");

                    b.Property<bool>("expense_updated");

                    b.Property<bool>("monthly_summary");

                    b.Property<bool>("payments");

                    b.HasKey("id");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("SplitBook.Model.Picture", b =>
                {
                    b.Property<int>("user_id");

                    b.Property<string>("large");

                    b.Property<string>("medium");

                    b.Property<string>("original");

                    b.Property<string>("small");

                    b.HasKey("user_id");

                    b.HasIndex("user_id")
                        .IsUnique();

                    b.ToTable("Picture");
                });

            modelBuilder.Entity("SplitBook.Model.User", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("country_code");

                    b.Property<string>("default_currency");

                    b.Property<string>("email");

                    b.Property<string>("first_name");

                    b.Property<string>("last_name");

                    b.Property<string>("updated_at");

                    b.HasKey("id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("SplitBook.Model.Balance_User", b =>
                {
                    b.HasOne("SplitBook.Model.User", "User")
                        .WithMany("balance")
                        .HasForeignKey("user_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SplitBook.Model.Debt_Expense", b =>
                {
                    b.HasOne("SplitBook.Model.Expense", "expense")
                        .WithMany("repayments")
                        .HasForeignKey("expense_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SplitBook.Model.Debt_Group", b =>
                {
                    b.HasOne("SplitBook.Model.User", "fromUser")
                        .WithMany()
                        .HasForeignKey("from")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SplitBook.Model.Group", "group")
                        .WithMany("simplified_debts")
                        .HasForeignKey("group_id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SplitBook.Model.User", "toUser")
                        .WithMany()
                        .HasForeignKey("to")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SplitBook.Model.Expense_Share", b =>
                {
                    b.HasOne("SplitBook.Model.Expense", "expense")
                        .WithMany("users")
                        .HasForeignKey("expense_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SplitBook.Model.Group_Members", b =>
                {
                    b.HasOne("SplitBook.Model.Group", "group")
                        .WithMany("group_members")
                        .HasForeignKey("group_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SplitBook.Model.Picture", b =>
                {
                    b.HasOne("SplitBook.Model.User", "User")
                        .WithOne("picture")
                        .HasForeignKey("SplitBook.Model.Picture", "user_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
