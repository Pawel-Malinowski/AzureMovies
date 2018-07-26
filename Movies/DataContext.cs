using Microsoft.EntityFrameworkCore;
using Movies.Models;

namespace Movies
{
    public class DataContext : DbContext
    {
        public DbSet<Actor> Actors { get; set; }

        public DbSet<MovieRole> MovieRoles { get; set; }

        public DbSet<Movie> Movies { get; set; }
        
        //public DataContext(DbContextOptions options) : base(options)
        //{
        //}

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        { }
        //public override int SaveChanges()
        //{
        //    IEnumerable<EntityEntry> changedEntities = ChangeTracker
        //        .Entries()
        //        .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

        //    var errors = new List<ValidationResult>();

        //    foreach (var e in changedEntities)
        //    {
        //        var vc = new ValidationContext(e.Entity, null, null);
        //        Validator.TryValidateObject(e.Entity, vc, errors, validateAllProperties: true);
        //    }

        //    return base.SaveChanges();
        //}

        //public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        //{
        //    return base.SaveChangesAsync(cancellationToken);
        //}

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
