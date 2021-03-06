using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using WorldCities.Data;
using WorldCities.Data.Models;

namespace WorldCities
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{

			services.AddControllersWithViews()
				.AddJsonOptions(options =>
				{
					// set this option to TRUE to indent the JSON output
					options.JsonSerializerOptions.WriteIndented = true;
					// set this option to NULL to use PascalCase instead of camelCase (default)
					// options.JsonSerializerOptions.PropertyNamingPolicy = null;
				});

			// In production, the Angular files will be served from this directory
			services.AddSpaStaticFiles(configuration =>
	   {
		   configuration.RootPath = "ClientApp/dist";
	   });

			// Add EntityFramework support for SqlServer
			//services.AddEntityFrameworkSqlServer();

			// Add ApplicationDbContext.
			services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

			//Add ASP.NET Core Identity support
			services.AddDefaultIdentity<ApplicationUser>(options =>
			{
				options.SignIn.RequireConfirmedAccount = true;
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireUppercase = true;
				options.Password.RequireNonAlphanumeric = true;
				options.Password.RequiredLength = 8;
			})
			.AddRoles<IdentityRole>()
			.AddEntityFrameworkStores<ApplicationDbContext>();

			services.AddIdentityServer().AddApiAuthorization<ApplicationUser, ApplicationDbContext>();
			services.AddAuthentication().AddIdentityServerJwt();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if(env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();

			//app.UseStaticFiles();

			app.UseStaticFiles(new StaticFileOptions()
			{
				OnPrepareResponse = (context) =>
				{
					// Desabilita o cache para todos os arquivos est?ticos.
					//context.Context.Response.Headers["Cache-Control"] = "no-cache, no-store";
					//context.Context.Response.Headers["Pragma"] = "no-cache";
					//context.Context.Response.Headers["Expires"] = "-1";

					// Retrieve as configura??es do cache de arquivo appsettings.json
					context.Context.Response.Headers["Cache-Control"] = Configuration["StaticFiles:Headers:Cache-Control"];
					context.Context.Response.Headers["Pragma"] = Configuration["StaticFiles:Headers:Pragma"];
					context.Context.Response.Headers["Expires"] = Configuration["StaticFiles:Headers:Expires"];
				}
			});

			if(!env.IsDevelopment())
			{
				app.UseSpaStaticFiles();
			}

			app.UseRouting();

			app.UseAuthentication();
			app.UseIdentityServer();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller}/{action=Index}/{id?}");
			});

			app.UseSpa(spa =>
			{
				// To learn more about options for serving an Angular SPA from ASP.NET Core,
				// see https://go.microsoft.com/fwlink/?linkid=864501

				spa.Options.SourcePath = "ClientApp";
				// Adi??o de TimeOut para evitar erro carregamento, fonte: https://stackoverflow.com/questions/60189930/timeoutexception-the-angular-cli-process-did-not-start-listening-for-requests-w
				spa.Options.StartupTimeout = new TimeSpan(0, 5, 0);
				if(env.IsDevelopment())
				{
					spa.UseAngularCliServer(npmScript: "start");
				}
			});
		}
	}
}
