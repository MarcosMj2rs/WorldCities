using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using WorldCities.Data;
using WorldCities.Data.Models;

namespace WorldCities.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SeedController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly IWebHostEnvironment _env;

		public SeedController(ApplicationDbContext context, IWebHostEnvironment env)
		{
			_context = context;
			_env = env;
		}

		[HttpGet]
		public async Task<ActionResult> Import()
		{
			var path = Path.Combine(_env.ContentRootPath, string.Format("Data/Source/worldcities.xlsx"));

			using(var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				using(var ep = new ExcelPackage(stream))
				{

					//obtém a primeira planilha
					var ws = ep.Workbook.Worksheets[0];

					//inicializa os contadores de registros
					var nCountries = 0;
					var nCities = 0;

					#region[[IMPORTAR TODOS PAISES]]
					//cria uma lista contendo todos os países já existentes no banco de dados (ele ficará vazio na primeira execução).
					var lstCountries = _context.Countries.ToList();

					//itera por todas as linhas, pulando a primeira
					for(int nRow = 2; nRow <= ws.Dimension.End.Row; nRow++)
					{
						var row = ws.Cells[nRow, 1, nRow, ws.Dimension.End.Column];
						var name = row[nRow, 5].GetValue<string>();

						//Já criamos um país com aquele nome?
						if(lstCountries.Where(c => string.Compare(c.Name, name, true) == 0).Count() == 0)
						{
							//crie a entidade Country e preenchê-la com dados do xlsx
							var country = new Country
							{
								Name = name,
								ISO2 = row[nRow, 6].GetValue<string>(),
								ISO3 = row[nRow, 7].GetValue<string>(),
							};

							//salvar na base de dados
							_context.Countries.Add(country);
							await _context.SaveChangesAsync();

							//armazena o país para recuperar seu Id posteriormente
							lstCountries.Add(country);
							nCountries++;
						}
					}
					#endregion

					#region[[IMPORTAR TODAS CIDADES]]
					//itera por todas as linhas, pulando a primeira
					for(int nRow = 2; nRow < ws.Dimension.End.Row; nRow++)
					{
						var row = ws.Cells[nRow, 1, nRow, ws.Dimension.End.Column];

						//cria a entidade City e preenche-a com dados xlsx
						var city = new City
						{
							Name = row[nRow, 1].GetValue<string>(),
							Name_ASCII = row[nRow, 2].GetValue<string>(),
							Lat = row[nRow, 3].GetValue<decimal>(),
							Lon = row[nRow, 4].GetValue<decimal>()
						};

						//recupera o CountryId
						var countryName = row[nRow, 5].GetValue<string>();
						var country = lstCountries.Where(c => string.Compare(c.Name, countryName, true) == 0).FirstOrDefault();
						city.CountryId = country.Id;

						//salvar cidade na base de dados
						_context.Cities.Add(city);
						await _context.SaveChangesAsync();
						nCities++;

					}
					#endregion

					return new JsonResult(new { Cities = nCities, Countries = nCountries });
				}
			}
		}
	}
}
