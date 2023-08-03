using MdNotesServer.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace MdNotes.WebApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;   
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<NotesContext>(options =>
            {
                options.UseSqlite();
            });

            services.AddControllers();
            
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("MD Notes API", new OpenApiInfo 
                { 
                    Title = "MD NOtes API",
                    Version = "V1",
                    Description = "API for MD Notes"
                });

                var xmlFile = Assembly.GetExecutingAssembly()
                .GetName()
                .Name + ".xml";

                var path = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(path);
            });

        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.DocumentTitle = "MD Notes API";
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseEndpoints(c => c.MapControllers());
        }
    }
}
