using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBook.Model
{
    class SplitBookContext : DbContext
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
        //public DbSet<Category> Category { get; set; }
        //public DbSet<Comment> Comment { get; set; }
        public DbSet<Currency> Currency { get; set; }
        public DbSet<Notifications> Notifications { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=SplitBook.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().Ignore(e => e.name).Ignore(e => e.PictureUrl);
            modelBuilder.Entity<Expense>().Ignore(e => e.receipt).Ignore(e => e.category)
                .Ignore(e => e.specificUserId).Ignore(e => e.displayType);
            modelBuilder.Entity<Picture>().HasKey(e => e.user_id);
            modelBuilder.Entity<Balance_User>().HasKey(e => e.user_id);
            modelBuilder.Entity<Debt_Group>().Ignore(e => e.ownerId).HasKey(e => new { e.group_id, e.from, e.to });
            modelBuilder.Entity<Debt_Expense>().HasKey(e => new { e.expense_id, e.from, e.to });
            modelBuilder.Entity<Expense_Share>().Ignore(e => e.currency).Ignore(e => e.percentage).Ignore(e => e.hasPaid).Ignore(e => e.share).HasKey(e => new { e.expense_id, e.user_id });
            modelBuilder.Entity<Currency>().HasKey(e => e.currency_code);
            modelBuilder.Entity<Group_Members>().HasKey(e => new { e.group_id, e.user_id });
        }
    }
}
