using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class LibraryDbContext : IdentityDbContext<Member>
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }
        public DbSet<ConfirmationToken> ConfirmationToken { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
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


            modelBuilder.Entity<BorrowRecord>()
            .Property(b => b.Status)
            .HasConversion<string>();
        }
    }
}
