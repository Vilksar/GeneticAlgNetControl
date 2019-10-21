using GeneticAlgNetControl.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Data
{
    /// <summary>
    /// Represents the database context of the application.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Gets or sets the database table containing the algorithm data (nodes and edges).
        /// </summary>
        public DbSet<AlgorithmData> AlgorithmData { get; set; }

        /// <summary>
        /// Gets or sets the database table containing the algorithm parameters.
        /// </summary>
        public DbSet<AlgorithmParameters> AlgorithmParameters { get; set; }

        /// <summary>
        /// Gets or sets the database table containing the algorithm runs.
        /// </summary>
        public DbSet<AlgorithmRun> AlgorithmRuns { get; set; }

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
            modelBuilder.Entity<AlgorithmData>()
                .Property(ad => ad.TargetNodes)
                .HasConversion(item => JsonSerializer.Serialize(item, null), item => JsonSerializer.Deserialize<List<string>>(item, null));
            modelBuilder.Entity<AlgorithmData>()
                .Property(ad => ad.Nodes)
                .HasConversion(item => JsonSerializer.Serialize(item, null), item => JsonSerializer.Deserialize<List<string>>(item, null));
            modelBuilder.Entity<AlgorithmData>()
                .Property(ad => ad.DrugTargetNodes)
                .HasConversion(item => JsonSerializer.Serialize(item, null), item => JsonSerializer.Deserialize<List<string>>(item, null));
            modelBuilder.Entity<AlgorithmData>()
                .Property(ad => ad.Edges)
                .HasConversion(item => JsonSerializer.Serialize(item, null), item => JsonSerializer.Deserialize<List<(string SourceNode, string TargetNode)>>(item, null));
            modelBuilder.Entity<AlgorithmRun>()
                .Property(ar => ar.DateTimeList)
                .HasConversion(item => JsonSerializer.Serialize(item, null), item => JsonSerializer.Deserialize<List<(DateTime StartTime, DateTime? EndTime)>>(item, null));
            // Configure the relationships between the tables.
            modelBuilder.Entity<AlgorithmRun>(algorithmRun =>
            {
                algorithmRun.HasOne(ar => ar.AlgorithmData)
                    .WithOne(ad => ad.AlgorithmRun)
                    .HasForeignKey<AlgorithmRun>(ar => ar.AlgorithmDataId)
                    .IsRequired();
                algorithmRun.HasOne(ar => ar.AlgorithmParameters)
                    .WithOne(ap => ap.AlgorithmRun)
                    .HasForeignKey<AlgorithmRun>(ar => ar.AlgorithmParametersId)
                    .IsRequired();
            });
        }
    }
}
