using System.Reflection;
using WebbySoftware.DBOperations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Microsoft.OpenApi.Models;

namespace WebbySoftware
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			// Add services to the container.
			builder.Services.AddControllersWithViews();

			builder.Services.AddEndpointsApiExplorer();

			string apiVersion = "0.8";
			string swaggerVersion = "0.8";

			builder.Services.AddSwaggerGen(c => {

				c.SwaggerDoc(apiVersion, new OpenApiInfo { Title = "Webbysoft", Version = swaggerVersion });
			});

			AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

			builder.Services.AddSingleton(builder.Configuration);
			builder.Services.AddScoped<IWebbySoftDBContext, WebbySoftDbContext>();
			builder.Services.AddDbContext<WebbySoftDbContext>(options => options.UseNpgsql(new WebbySoftConnStr(builder.Configuration).GetConnectionString()));


			builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

			var app = builder.Build();

			using (var scope = app.Services.CreateScope()){

				var services = scope.ServiceProvider;
				//WebbySoftDBSeed.Initialize(services);
			}
			

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Azure"))
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days.
				app.UseHsts();
			}

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint("/swagger/0.8/swagger.json", "Webbysoft API");
				});
			}

			app.UseAuthentication();

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "searchedTag",
					pattern: "Development/{action}/{searchedTag?}",
					defaults: new { controller = "Home" },
					constraints: new { action = "GameDevelopment|WebDevelopment|MobileAppDevelopment|DesktopDevelopment"}
				);

				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}"
				);

				// Other endpoints configuration
			});

			app.Run();

		}
	}
}