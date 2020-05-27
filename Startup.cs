using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

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
			services.AddControllersWithViews();
			// In production, the Angular files will be served from this directory
			services.AddSpaStaticFiles(configuration =>
			{
				configuration.RootPath = "ClientApp/dist";
			});
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
					// Desabilita o cache para todos os arquivos estáticos.
					//context.Context.Response.Headers["Cache-Control"] = "no-cache, no-store";
					//context.Context.Response.Headers["Pragma"] = "no-cache";
					//context.Context.Response.Headers["Expires"] = "-1";

					// Retrieve as configurações do cache de arquivo appsettings.json
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
				// Adição de TimeOut para evitar erro carregamento, fonte: https://stackoverflow.com/questions/60189930/timeoutexception-the-angular-cli-process-did-not-start-listening-for-requests-w
				spa.Options.StartupTimeout = new TimeSpan(0, 5, 0);
				if(env.IsDevelopment())
				{
					spa.UseAngularCliServer(npmScript: "start");
				}
			});
		}
	}
}
