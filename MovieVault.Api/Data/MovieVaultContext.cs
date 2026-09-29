using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Domain.Models;

namespace MovieVault.Api.Data;

public class MovieVaultContext : DbContext
{
    public MovieVaultContext(DbContextOptions<MovieVaultContext> options)
        : base(options)
    {
    }

    public DbSet<Director> Directors { get; set; }
    public DbSet<ProductionCompany> ProductionCompanies { get; set; }
    public DbSet<Studio> Studios { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Cinema> Cinemas { get; set; }
    public DbSet<Screen> Screens { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<ProductionRecord> ProductionRecords { get; set; }
    public DbSet<MovieActor> MovieActors { get; set; }
    public DbSet<MovieLocation> MovieLocations { get; set; }
    public DbSet<Screening> Screenings { get; set; }
    public DbSet<TicketSale> TicketSales { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>()
            .HasOne(m => m.Director)
            .WithMany(d => d.Movies)
            .HasForeignKey(m => m.DirectorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Movie>()
            .HasOne(m => m.ProductionCompany)
            .WithMany(pc => pc.Movies)
            .HasForeignKey(m => m.ProductionCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Studio>()
            .HasOne(s => s.ProductionCompany)
            .WithMany(pc => pc.Studios)
            .HasForeignKey(s => s.ProductionCompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Movie>()
            .HasOne(m => m.ProductionRecord)
            .WithOne(pr => pr.Movie)
            .HasForeignKey<ProductionRecord>(pr => pr.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MovieActor>()
            .HasKey(ma => new { ma.MovieId, ma.ActorId });

        modelBuilder.Entity<MovieActor>()
            .HasOne(ma => ma.Movie)
            .WithMany(m => m.MovieActors)
            .HasForeignKey(ma => ma.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MovieActor>()
            .HasOne(ma => ma.Actor)
            .WithMany(a => a.MovieActors)
            .HasForeignKey(ma => ma.ActorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MovieLocation>()
            .HasKey(ml => new { ml.MovieId, ml.LocationId });

        modelBuilder.Entity<MovieLocation>()
            .HasOne(ml => ml.Movie)
            .WithMany(m => m.MovieLocations)
            .HasForeignKey(ml => ml.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MovieLocation>()
            .HasOne(ml => ml.Location)
            .WithMany(l => l.MovieLocations)
            .HasForeignKey(ml => ml.LocationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Screen>()
            .HasOne(s => s.Cinema)
            .WithMany(c => c.Screens)
            .HasForeignKey(s => s.CinemaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Screen>()
            .HasIndex(s => new { s.CinemaId, s.ScreenNumberOrName })
            .IsUnique();

        modelBuilder.Entity<Screening>()
            .HasOne(s => s.Movie)
            .WithMany(m => m.Screenings)
            .HasForeignKey(s => s.MovieId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Screening>()
            .HasOne(s => s.Screen)
            .WithMany(sc => sc.Screenings)
            .HasForeignKey(s => s.ScreenId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TicketSale>()
            .HasOne(ts => ts.Screening)
            .WithMany(s => s.TicketSales)
            .HasForeignKey(ts => ts.ScreeningId)
            .OnDelete(DeleteBehavior.Restrict);

        SeedData.Seed(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}
