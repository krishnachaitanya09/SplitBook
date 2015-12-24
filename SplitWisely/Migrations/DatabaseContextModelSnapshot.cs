using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using SplitWisely.Model;

namespace SplitWisely.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348");

            modelBuilder.Entity("SplitWisely.Model.AmountSplit", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("typeString");

                    b.HasKey("id");
                });

            modelBuilder.Entity("SplitWisely.Model.Balance_User", b =>
                {
                    b.Property<int>("user_id");

                    b.Property<string>("amount");

                    b.Property<string>("currency_code");

                    b.HasKey("user_id");
                });

            modelBuilder.Entity("SplitWisely.Model.Category", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("name");

                    b.HasKey("id");
                });

            modelBuilder.Entity("SplitWisely.Model.Comment", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("comment_type");

                    b.Property<string>("content");

                    b.Property<string>("created_at");

                    b.Property<string>("deleted_at");

                    b.Property<int>("relation_id");

                    b.Property<string>("relation_type");

                    b.Property<int>("user_id");

                    b.HasKey("id");
                });

            modelBuilder.Entity("SplitWisely.Model.Currency", b =>
                {
                    b.Property<string>("currency_code");

                    b.Property<string>("unit");

                    b.HasKey("currency_code");
                });

            modelBuilder.Entity("SplitWisely.Model.Debt_Expense", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("Expenseid");

                    b.Property<string>("amount");

                    b.Property<string>("currency_code");

                    b.Property<int>("expense_id");

                    b.Property<int>("from");

                    b.Property<int>("to");

                    b.HasKey("id");
                });

            modelBuilder.Entity("SplitWisely.Model.Debt_Group", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("amount");

                    b.Property<string>("currency_code");

                    b.Property<int>("from");

                    b.Property<int>("group_id");

                    b.Property<int>("to");

                    b.HasKey("id");
                });

            modelBuilder.Entity("SplitWisely.Model.Expense", b =>
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
                });

            modelBuilder.Entity("SplitWisely.Model.Expense_Share", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("expense_id");

                    b.Property<string>("net_balance");

                    b.Property<string>("owed_share");

                    b.Property<string>("paid_share");

                    b.Property<int>("user_id");

                    b.HasKey("id");
                });

            modelBuilder.Entity("SplitWisely.Model.ExpenseType", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("typeString");

                    b.HasKey("id");
                });

            modelBuilder.Entity("SplitWisely.Model.Group", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("Userid");

                    b.Property<string>("group_type");

                    b.Property<string>("name");

                    b.Property<bool>("simplify_by_default");

                    b.Property<string>("updated_at");

                    b.Property<string>("whiteboard");

                    b.HasKey("id");
                });

            modelBuilder.Entity("SplitWisely.Model.Group_Members", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("group_id");

                    b.Property<int>("user_id");

                    b.HasKey("id");
                });

            modelBuilder.Entity("SplitWisely.Model.Picture", b =>
                {
                    b.Property<int>("user_id");

                    b.Property<string>("large");

                    b.Property<string>("medium");

                    b.Property<string>("original");

                    b.Property<string>("small");

                    b.HasKey("user_id");
                });

            modelBuilder.Entity("SplitWisely.Model.User", b =>
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
                });

            modelBuilder.Entity("SplitWisely.Model.Balance_User", b =>
                {
                    b.HasOne("SplitWisely.Model.User")
                        .WithMany()
                        .HasForeignKey("user_id");
                });

            modelBuilder.Entity("SplitWisely.Model.Comment", b =>
                {
                    b.HasOne("SplitWisely.Model.User")
                        .WithMany()
                        .HasForeignKey("user_id");
                });

            modelBuilder.Entity("SplitWisely.Model.Debt_Expense", b =>
                {
                    b.HasOne("SplitWisely.Model.Expense")
                        .WithMany()
                        .HasForeignKey("Expenseid");

                    b.HasOne("SplitWisely.Model.Debt_Expense")
                        .WithMany()
                        .HasForeignKey("expense_id");

                    b.HasOne("SplitWisely.Model.User")
                        .WithMany()
                        .HasForeignKey("from");

                    b.HasOne("SplitWisely.Model.User")
                        .WithMany()
                        .HasForeignKey("to");
                });

            modelBuilder.Entity("SplitWisely.Model.Debt_Group", b =>
                {
                    b.HasOne("SplitWisely.Model.User")
                        .WithMany()
                        .HasForeignKey("from");

                    b.HasOne("SplitWisely.Model.Group")
                        .WithMany()
                        .HasForeignKey("group_id");

                    b.HasOne("SplitWisely.Model.User")
                        .WithMany()
                        .HasForeignKey("to");
                });

            modelBuilder.Entity("SplitWisely.Model.Expense", b =>
                {
                    b.HasOne("SplitWisely.Model.User")
                        .WithMany()
                        .HasForeignKey("created_by_user_id");

                    b.HasOne("SplitWisely.Model.User")
                        .WithMany()
                        .HasForeignKey("deleted_by_user_id");

                    b.HasOne("SplitWisely.Model.User")
                        .WithMany()
                        .HasForeignKey("updated_by_user_id");
                });

            modelBuilder.Entity("SplitWisely.Model.Expense_Share", b =>
                {
                    b.HasOne("SplitWisely.Model.Expense")
                        .WithMany()
                        .HasForeignKey("expense_id");

                    b.HasOne("SplitWisely.Model.User")
                        .WithMany()
                        .HasForeignKey("user_id");
                });

            modelBuilder.Entity("SplitWisely.Model.Group", b =>
                {
                    b.HasOne("SplitWisely.Model.User")
                        .WithMany()
                        .HasForeignKey("Userid");
                });

            modelBuilder.Entity("SplitWisely.Model.Group_Members", b =>
                {
                    b.HasOne("SplitWisely.Model.Group")
                        .WithMany()
                        .HasForeignKey("group_id");

                    b.HasOne("SplitWisely.Model.User")
                        .WithMany()
                        .HasForeignKey("user_id");
                });

            modelBuilder.Entity("SplitWisely.Model.Picture", b =>
                {
                    b.HasOne("SplitWisely.Model.User")
                        .WithOne()
                        .HasForeignKey("SplitWisely.Model.Picture", "user_id");
                });
        }
    }
}
