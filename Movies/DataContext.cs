using Microsoft.EntityFrameworkCore;
using Movies.Models;

namespace Movies
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MovieRole>()
                .HasKey(pc => new { pc.MovieId, pc.ActorId });

            modelBuilder.Entity<MovieRole>()
                .HasOne(x => x.Movie)
                .WithMany(x => x.MovieRoles)
                .HasForeignKey(x => x.MovieId);

            modelBuilder.Entity<MovieRole>()
                .HasOne(x => x.Actor)
                .WithMany(x => x.MovieRoles)
                .HasForeignKey(x => x.ActorId);
        }
    }
}
