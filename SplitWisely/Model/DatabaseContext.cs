using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWisely.Model
{
    class DatabaseContext : DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<Expense> Expense { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<Picture> Picture { get; set; }
        public DbSet<Balance_User> Balance_User { get; set; }
        public DbSet<Debt_Expense> Debt_Expense { get; set; }
        public DbSet<Debt_Group> Debt_Group { get; set; }
        public DbSet<Expense_Share> Expense_Share { get; set; }
        public DbSet<Group_Members> Group_Members { get; set; }
        public DbSet<AmountSplit> AmountSplit { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Currency> Currency { get; set; }
        public DbSet<ExpenseType> ExpenseType { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename=Database.db");
        }
    }
}
