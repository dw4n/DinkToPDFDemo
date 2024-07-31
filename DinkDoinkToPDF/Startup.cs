using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.OpenApi.Models;

public class Startup
{
	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
	}

	public IConfiguration Configuration { get; }

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddControllers();
		services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

		// Register the Swagger generator, defining one or more Swagger documents
		services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo { Title = "HTML to PDF API", Version = "v1" });
		});
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}

		app.UseHttpsRedirection();
		app.UseRouting();
		app.UseAuthorization();

		// Enable middleware to serve generated Swagger as a JSON endpoint
		app.UseSwagger();

		// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
		// specifying the Swagger JSON endpoint.
		app.UseSwaggerUI(c =>
		{
			c.SwaggerEndpoint("/swagger/v1/swagger.json", "HTML to PDF API v1");
			c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
		});

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllers();
		});
	}
}
