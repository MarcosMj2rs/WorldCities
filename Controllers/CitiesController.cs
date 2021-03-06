using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WorldCities.Data;
using WorldCities.Data.Models;

namespace WorldCities.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CitiesController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public CitiesController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<ApiResult<CityDTO>>> GetCities(
		int pageIndex = 0,
		int pageSize = 10,
		string sortColumn = null,
		string sortOrder = null,
		string filterColumn = null,
		string filterQuery = null)
		{
			#region[[TESTE FILTRO]]
			// Criando o filtro...
			//var cities = _context.Cities;

			//if(!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterQuery) &&
			//	!string.IsNullOrWhiteSpace(filterColumn) && !string.IsNullOrWhiteSpace(filterQuery))
			//{
			//	var citiesLocal = cities.Where(c => c.Name.Contains(filterQuery));
			//}
			#endregion

			return await ApiResult<CityDTO>.CreateAsync(
				_context.Cities
					.Select(c => new CityDTO()
					{
						Id = c.Id,
						Name = c.Name,
						Lat = c.Lat,
						Lon = c.Lon,
						CountryId = c.Country.Id,
						CountryName = c.Country.Name
					}),
				pageIndex,
				pageSize,
				sortColumn,
				sortOrder,
				filterColumn,
				filterQuery);
		}
		// GET: api/Cities/5
		[HttpGet("{id}")]
		public async Task<ActionResult<City>> GetCity(int id)
		{
			var city = await _context.Cities.FindAsync(id);

			if(city == null)
			{
				return NotFound();
			}

			return city;
		}

		// PUT: api/Cities/5
		// To protect from overposting attacks, enable the specific properties you want to bind to, for
		// more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
		[HttpPut("{id}")]
		public async Task<IActionResult> PutCity(int id, City city)
		{
			if(id != city.Id)
			{
				return BadRequest();
			}

			_context.Entry(city).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch(DbUpdateConcurrencyException)
			{
				if(!CityExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		// POST: api/Cities
		// To protect from overposting attacks, enable the specific properties you want to bind to, for
		// more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
		[HttpPost]
		public async Task<ActionResult<City>> PostCity(City city)
		{
			_context.Cities.Add(city);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetCity", new { id = city.Id }, city);
		}

		// DELETE: api/Cities/5
		[HttpDelete("{id}")]
		public async Task<ActionResult<City>> DeleteCity(int id)
		{
			var city = await _context.Cities.FindAsync(id);
			if(city == null)
			{
				return NotFound();
			}

			_context.Cities.Remove(city);
			await _context.SaveChangesAsync();

			return city;
		}

		private bool CityExists(int id)
		{
			return _context.Cities.Any(e => e.Id == id);
		}

		[HttpPost]
		[Route("IsDupeCity")]
		public bool IsDupeCity(City city)
		{
			return _context.Cities.Any(
						e => e.Name == city.Name
						&& e.Lat == city.Lat
						&& e.Lon == city.Lon
						&& e.CountryId == city.CountryId
						&& e.Id != city.Id
					);
		}

	}
}
