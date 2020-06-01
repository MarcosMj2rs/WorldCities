using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorldCities.Data.Models
{
	public class City
	{
		#region[[CONSTRUCTOR]]
		public City()
		{

		}
		#endregion

		#region[[PROPERTIES]]
		/// <summary>
		/// Id único e chave primária de City
		/// </summary>
		[Key]
		[Required]
		public int Id { get; set; }

		/// <summary>
		/// Nome da cidade (formato UTF8)
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Nome da cidade (formato ASCII)
		/// </summary>
		public string Name_ASCII { get; set; }

		/// <summary>
		/// Latitude da cidade
		/// </summary>
		[Column(TypeName = "decimal(7,4)")]
		public decimal Lat { get; set; }

		/// <summary>
		/// Longitude da cidade
		/// </summary>
		[Column(TypeName = "decimal(7,4)")]
		public decimal Lon { get; set; }

		/// <summary>
		/// Id do pais (chave estrangeira)
		/// </summary>
		[ForeignKey("Country")]
		public int CountryId { get; set; }
		#endregion

		#region[[NAVIGATION PROPERTIES]]
		/// <summary>
		/// A relação de pais com cidade(s)
		/// </summary>
		public virtual Country Country { get; set; }
		#endregion
	}
}
