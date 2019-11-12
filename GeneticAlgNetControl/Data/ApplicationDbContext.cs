using GeneticAlgNetControl.Data.Models;
using GeneticAlgNetControl.Helpers.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json;

namespace GeneticAlgNetControl.Data
{
    /// <summary>
    /// Represents the database context of the application.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Gets or sets the database table containing the algorithm runs.
        /// </summary>
        public DbSet<Algorithm> Algorithms { get; set; }

        /// <summary>
        /// Initializes a new instance of the database context.
        /// </summary>
        /// <param name="options">Represents the options for the database context.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        /// <summary>
        /// Configures code-first the database entities and relationships between them.
        /// </summary>
        /// <param name="modelBuilder">Represents the model builder that will be in charge of building the database.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the mapping of more complex types to database fields.
            modelBuilder.Entity<Algorithm>()
                .Property(ad => ad.Nodes)
                .HasConversion(item => JsonSerializer.Serialize(item, null), item => JsonSerializer.Deserialize<List<string>>(item, null));
            modelBuilder.Entity<Algorithm>()
                .Property(ad => ad.TargetNodes)
                .HasConversion(item => JsonSerializer.Serialize(item, null), item => JsonSerializer.Deserialize<List<string>>(item, null));
            modelBuilder.Entity<Algorithm>()
                .Property(ad => ad.PreferredNodes)
                .HasConversion(item => JsonSerializer.Serialize(item, null), item => JsonSerializer.Deserialize<List<string>>(item, null));
            modelBuilder.Entity<Algorithm>()
                .Property(ad => ad.Edges)
                .HasConversion(item => JsonSerializer.Serialize(item, null), item => JsonSerializer.Deserialize<List<Edge>>(item, null));
            modelBuilder.Entity<Algorithm>()
                .Property(ar => ar.DateTimePeriods)
                .HasConversion(item => JsonSerializer.Serialize(item, null), item => JsonSerializer.Deserialize<List<DateTimePeriod>>(item, null));
            modelBuilder.Entity<Algorithm>()
                .Property(ad => ad.Parameters)
                .HasConversion(item => JsonSerializer.Serialize(item, null), item => JsonSerializer.Deserialize<Parameters>(item, null));
            modelBuilder.Entity<Algorithm>()
                .Property(ad => ad.Population)
                .HasConversion(item => JsonSerializer.Serialize(item, null), item => JsonSerializer.Deserialize<Population>(item, null));
        }
    }
}
