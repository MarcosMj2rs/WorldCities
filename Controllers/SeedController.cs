using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WorldCities.Data;
using WorldCities.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace WorldCities.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class SeedController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IWebHostEnvironment _env;

		public SeedController(ApplicationDbContext context,
							  RoleManager<IdentityRole> roleManager,
							  UserManager<ApplicationUser> userManager,
							  IWebHostEnvironment env
							)
		{
			_context = context;
			_roleManager = roleManager;
			_userManager = userManager;
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

		[HttpGet]
		public async Task<ActionResult> CreateDefaultUsers()
		{
			// setup the default role names
			string role_RegisteredUser = "RegisteredUser";
			string role_Administrator = "Administrator";

			// create the default roles (if they doesn't exist yet)
			if(await _roleManager.FindByNameAsync(role_RegisteredUser) == null)
				await _roleManager.CreateAsync(new IdentityRole(role_RegisteredUser));

			if(await _roleManager.FindByNameAsync(role_Administrator) == null)
				await _roleManager.CreateAsync(new IdentityRole(role_Administrator));

			// create a list to track the newly added users
			var addedUserList = new List<ApplicationUser>();

			// check if the admin user already exist
			var email_Admin = "admin@email.com";
			if(await _userManager.FindByNameAsync(email_Admin) == null)
			{
				// create a new admin ApplicationUser account
				var user_Admin = new ApplicationUser()
				{
					SecurityStamp = Guid.NewGuid().ToString(),
					UserName = email_Admin,
					Email = email_Admin,
				};

				// insert the admin user into the DB
				await _userManager.CreateAsync(user_Admin, "MySecr3t$");

				// assign the "RegisteredUser" and "Administrator" roles
				await _userManager.AddToRoleAsync(user_Admin, role_RegisteredUser);
				await _userManager.AddToRoleAsync(user_Admin, role_Administrator);

				// confirm the e-mail and remove lockout
				user_Admin.EmailConfirmed = true;
				user_Admin.LockoutEnabled = false;

				// add the admin user to the added users list
				addedUserList.Add(user_Admin);
			}

			// check if the standard user already exist
			var email_User = "user@email.com";
			if(await _userManager.FindByNameAsync(email_User) == null)
			{
				// create a new standard ApplicationUser account
				var user_User = new ApplicationUser()
				{
					SecurityStamp = Guid.NewGuid().ToString(),
					UserName = email_User,
					Email = email_User
				};

				// insert the standard user into the DB
				await _userManager.CreateAsync(user_User, "MySecr3t$");

				// assign the "RegisteredUser" role
				await _userManager.AddToRoleAsync(user_User, role_RegisteredUser);

				// confirm the e-mail and remove lockout
				user_User.EmailConfirmed = true;
				user_User.LockoutEnabled = false;

				// add the standard user to the added users list
				addedUserList.Add(user_User);
			}

			// if we added at least one user, persist the changes into the DB
			if(addedUserList.Count > 0)
				await _context.SaveChangesAsync();

			return new JsonResult(new
			{
				Count = addedUserList.Count,
				Users = addedUserList
			});
		}
	}
}
