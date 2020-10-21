using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WorldCities.Data.Models;

namespace WorldCities.Data
{
	public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
	{
		#region[[CONSTRUCTOR]]
		public ApplicationDbContext(
			DbContextOptions options,
			IOptions<OperationalStoreOptions> operationalStoreOptions)
			: base(options, operationalStoreOptions)
		{
		
		}
		#endregion

		#region[[METHODS]]
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Mapear nomes de entidades para nomes de tabelas de banco de dados
			modelBuilder.Entity<City>().ToTable("Cities");
			modelBuilder.Entity<Country>().ToTable("Countries");
		}
		#endregion

		#region[[PROPERTIES]]
		public DbSet<City> Cities { get; set; }
		public DbSet<Country> Countries { get; set; }
		#endregion
	}
}
