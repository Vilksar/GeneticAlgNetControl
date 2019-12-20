using GeneticAlgNetControl.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GeneticAlgNetControl.Data
{
    /// <summary>
    /// Represents the database context of the application.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Gets or sets the database table containing the analyses.
        /// </summary>
        public DbSet<Analysis> Analyses { get; set; }

        /// <summary>
        /// Initializes a new instance of the database context.
        /// </summary>
        /// <param name="options">Represents the options for the database context.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        /// <summary>
        /// Configures code-first the database entities and relationships between them.
        /// </summary>
        /// <param name="modelBuilder">Represents the model builder that will be in charge of building the database.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder) { }
    }
}
