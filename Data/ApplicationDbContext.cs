using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldCities.Data.Models;

namespace WorldCities.Data
{
	public class ApplicationDbContext : DbContext
	{
		#region[[CONSTRUCTOR]]
		public ApplicationDbContext() : base() { }

		public ApplicationDbContext(DbContextOptions options) : base(options) { }
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
