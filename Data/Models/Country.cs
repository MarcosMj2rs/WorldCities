using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WorldCities.Data.Models
{
	public class Country
	{
		#region[[CONSTRUCTOR]]
		public Country() { }
		#endregion

		#region[[PROPERTIES]]
		/// <summary>
		/// Id único e chave primária de Country
		/// </summary>
		[Key]
		[Required]
		public int Id { get; set; }

		/// <summary>
		/// Nome do país (formato UTF8)
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Código do país (ISO 3166-1 formato ALPHA-2)
		/// </summary>
		[JsonPropertyName("iso2")]
		public string ISO2 { get; set; }

		/// <summary>
		/// Código do país (ISO 3166-1 formato ALPHA-3)
		/// </summary>
		[JsonPropertyName("iso3")]
		public string ISO3 { get; set; }
		#endregion

		#region[[NAVIGATION PROPERTIES]]
		/// <summary>
		/// Lista contendo todas as Cidades relacionadas com país
		/// </summary>
		public virtual List<City> Cities { get; set; }
		#endregion
	}
}
