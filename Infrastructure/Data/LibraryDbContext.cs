using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Member>()
            .HasIndex(m => m.Email)
            .IsUnique();

            modelBuilder.Entity<Book>()
                .HasMany(B => B.BorrowRecords)
                .WithOne(Br => Br.Book)
                .HasForeignKey(Br => Br.BookId);

            modelBuilder.Entity<Member>()
                .HasMany(M => M.BorrowRecords)
                .WithOne(Br => Br.Member)
                .HasForeignKey(Br => Br.MemberId);
        }
    }
}
