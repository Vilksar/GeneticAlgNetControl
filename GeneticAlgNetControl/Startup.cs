using GeneticAlgNetControl.Data;
using GeneticAlgNetControl.Helpers.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;

namespace GeneticAlgNetControl
{
    /// <summary>
    /// Represents the actions to perform by the application at startup.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Gets the configuration options for the application.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Initializes a new instance of the application startup.
        /// </summary>
        /// <param name="configuration">Represents the application configuration options.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configures the services at the application startup.
        /// </summary>
        /// <remarks>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </remarks>
        /// <param name="services">Represents the service collection to be configured.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add the database context and connection.
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("SQLiteConnection").Replace("{ApplicationDirectory}", AppDomain.CurrentDomain.BaseDirectory));
            });
            // Add the algorithm run service.
            services.AddHostedService<AnalysisRunWebHostedService>();
            // Add Razor pages.
            services.AddRazorPages();
        }

        /// <summary>
        /// Configures the application options at startup.
        /// </summary>
        /// <remarks>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </remarks>
        /// <param name="app">Represents the application builder.</param>
        /// <param name="env">Represents the hosting environment of the application.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Check the environment in which it is running.
            if (env.IsDevelopment())
            {
                // Display more details about the errors.
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Redirect to a generic "Error" page.
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            // Parameters for the application.
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            // Use Razor pages.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
            // Check if we are in production.
            if (env.IsProduction())
            {
                // Open a browser to the default address for the localhost, on either the default or a specific port.
                Process.Start(new ProcessStartInfo
                {
                    FileName = Configuration["Urls"],
                    UseShellExecute = true
                });
            }
        }
    }
}
